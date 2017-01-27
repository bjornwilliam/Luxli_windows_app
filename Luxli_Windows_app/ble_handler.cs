using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


using Windows.Devices.Enumeration;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;



namespace Luxli_Windows_app
{
	class ble_handler
	{
		private const string luxli_ble_name = "Rift Labs Kick";
		public static readonly string ServiceUUID = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
		public static readonly string txCharacteristicUUID = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
		public static readonly string rxCharacteristicUUID = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";

		private DeviceWatcher BLE_deviceWatcher = null; // Used to find BLE devices
		private TypedEventHandler<DeviceWatcher, DeviceInformation> BLE_DeviceWatcher_EventHandler_DeviceAdded = null;
		private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> BLE_DeviceWatcher_EventHandler_DeviceUpdated = null;
		private TypedEventHandler<DeviceWatcher, DeviceInformation> BLE_DeviceWatcher_EventHandler_DeviceRemoved = null;


		private BluetoothLEDevice luxli_ble_device = null;
		public List<luxli_ble_device> connected_Luxlis = new List<luxli_ble_device>();
		public List<DeviceInformation> discovered_luxlis = new List<DeviceInformation>();

		
				
		public ble_handler()
		{
			StartBleWatcher();
		}

		private void StartBleWatcher()
		{
			string selector = "System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\"";
			// Kind is specified in the selector info
			BLE_deviceWatcher = DeviceInformation.CreateWatcher(
					selector,
					null, // don't request additional properties for this sample
					Windows.Devices.Enumeration.DeviceInformationKind.AssociationEndpoint);

			// Hook up handlers to the device watcher which will handle updates / and new discoveries

			BLE_DeviceWatcher_EventHandler_DeviceAdded = new TypedEventHandler<DeviceWatcher, DeviceInformation>(async(watcher, deviceInfo) =>
			{
				// DeviceInformation contains :
				//
				if (deviceInfo.Name == luxli_ble_name)
				{
					if (!discovered_luxlis.Contains(deviceInfo)) {
						discovered_luxlis.Add(deviceInfo);
					}
					//if its a luxli device, unpair, pair and connect 					

					DeviceUnpairingResult unpair_result =  await deviceInfo.Pairing.UnpairAsync();
					DevicePairingResult pair_result = await deviceInfo.Pairing.PairAsync();
					//connectToLuxli(deviceInfo);
					if ( (pair_result.Status.Equals(DevicePairingResultStatus.Paired) || pair_result.Status.Equals(DevicePairingResultStatus.AlreadyPaired))
						 ) // && deviceInfo.IsEnabled )
					{
						connectToLuxli(deviceInfo);
					}
				}
			});
			BLE_deviceWatcher.Added += BLE_DeviceWatcher_EventHandler_DeviceAdded;

			BLE_DeviceWatcher_EventHandler_DeviceUpdated = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>((watcher, deviceInfoUpdate) =>
			{
				int index = discovered_luxlis.FindIndex(f => f.Id == deviceInfoUpdate.Id);
				if (index >= 0)
				{
					discovered_luxlis[index].Update(deviceInfoUpdate);
				}
				else
				{
					int notSupposedtohappen = 5;
				}
				//foreach (DeviceInformation luxli_deviceInfo in discovered_luxlis)
				//{
				//	if (luxli_deviceInfo.Id == deviceInfoUpdate.Id)
				//	{
				//		luxli_deviceInfo.Update(deviceInfoUpdate);
				//	}
				//}
			});
			BLE_deviceWatcher.Updated += BLE_DeviceWatcher_EventHandler_DeviceUpdated;

			BLE_deviceWatcher.Start();
		}
		
		private async void connectToLuxli(DeviceInformation deviceInfo)
		{
			try
			{

				//GattDeviceService x = await GattDeviceService.FromIdAsync(deviceInfo.Id);
				luxli_ble_device = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
				luxli_ble_device = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
			}
			catch (Exception ex) when ((uint)ex.HResult == 0x8000000E)
			{

			}
			catch (Exception ex) when ((uint)ex.HResult == 0x800710df)
			{
				// ERROR_DEVICE_NOT_AVAILABLE because the Bluetooth radio is not on.
			}
			if (luxli_ble_device != null)
			{
				// BT_Code: GattServices returns a list of all the supported services of the device.
				// If the services supported by the device are expected to change
				// during BT usage, subscribe to the GattServicesChanged event.
				foreach (var service in luxli_ble_device.GattServices)
				{
					if (service.Uuid.ToString() == ServiceUUID)
					{
						foreach (var characteristic in service.GetAllCharacteristics())
						{
							if (characteristic.Uuid.ToString() == txCharacteristicUUID)
							{
								connected_Luxlis.Add(new luxli_ble_device(deviceInfo, characteristic));
							}
						}
					}
				}
			}
		}

		public async void send_ble_luxli_packet(MasterPacket packet)
		{
			var headerBytes = new byte[] {// data package has room for 11 bytes
						(byte)'R',
						(byte)'L',
						0x00, // group
						0x00, // address 1
						0x00, // address 2
						0x00, // address 3
						0x00, // length 1
						(byte)(packet.Data.Length +1),
						(byte)packet.CommandData
					};
			var allBytes = new List<byte>(headerBytes);

			if (packet.Data.Length > 0)
				allBytes.AddRange(packet.Data);

			var data = allBytes.ToArray();
			var completePacket = data.AsBuffer();

			//IEnumerable<byte> completePacket = (headerBytes.Concat(packet.Data));
			
			foreach (var luxli in connected_Luxlis.Where(k => k.ble_transmit != null)) {
				GattCommunicationStatus status = await luxli.ble_transmit.WriteValueAsync(completePacket);		
			};
		}
	}
		// private pair, unpair, connect, disconnect
}
