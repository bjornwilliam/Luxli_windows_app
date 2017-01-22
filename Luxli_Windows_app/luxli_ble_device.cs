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
	class luxli_ble_device
	{

		public DeviceInformation luxli_deviceInfo;
		public GattCharacteristic ble_transmit;
		//public GattCharacteristic ble_receive;

		

		public luxli_ble_device(DeviceInformation deviceInfo, GattCharacteristic characteristic)
		{
			this.luxli_deviceInfo = deviceInfo;
			this.ble_transmit			= characteristic;		
		}

	}
}
