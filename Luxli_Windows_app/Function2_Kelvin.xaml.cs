﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Luxli_Windows_app
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Function2_Kelvin : Page
	{
		CancellationTokenSource cts;

		private bool sendingState;

		private ble_handler _ble_handler;

		public Int32 Interval_s { get; set; }
		private int start_value;
		private int stop_value;
		private int increment_size;
		private int brightness;
		private System.Threading.Timer Timer_updateConnLuxlis;

		public Function2_Kelvin()
		{
			this.InitializeComponent();
			this.start_value		= 3000;		startValue_TextBox.Text			= start_value.ToString();
			this.stop_value			= 10000;	stopValue_TextBox.Text			= stop_value.ToString();
			this.Interval_s			=	3;			interval_s_TextBox.Text			= Interval_s.ToString();
			this.increment_size = 200;		incrementSize_TextBox.Text	= increment_size.ToString();
			this.brightness			= 10;			brightness_TextBox.Text			= brightness.ToString();
			this.Timer_updateConnLuxlis = new System.Threading.Timer(updateConnectedLuxlisTextbox, null, 10, 100);
		}

		private async void updateConnectedLuxlisTextbox(object state)
		{
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				if (_ble_handler != null)
				{
					connLuxlis_TextBox.Text = _ble_handler.connected_Luxlis.Count.ToString();
				}
			});
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_ble_handler = e.Parameter as ble_handler;
		}
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			// Stop sending kelvin to lamp
		}

		private void interval_s_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Interval_s = Convert.ToInt32(interval_s_TextBox.Text);
		}

		private void startStopButton_Click(object sender, RoutedEventArgs e)
		{
			if (sendingState == false)
			{
				cts = new CancellationTokenSource();
				sendingState = true;
				startStopButton.Content = "Stop";

				try
				{
					sendKelvin(Interval_s, start_value, stop_value, increment_size, cts.Token);
				}
				catch (OperationCanceledException)
				{

					UInt32 hie = 5;
				}
				
			}
			else
			{
				sendingState = false;
				startStopButton.Content = "Start";
				if (cts != null)
				{
					cts.Cancel();
				}

			}
		}

		private async void sendKelvin(Int32 interval, Int32 start_value, Int32 stop_value, Int32 increment_size, CancellationToken ct)
		{
			var brightness_value = new byte[]
			{
			(byte)(this.brightness)
			};
			var brightness_packet = new MasterPacket(MasterCommand.SetEV2, brightness_value);
			_ble_handler.send_ble_luxli_packet(brightness_packet);
			await System.Threading.Tasks.Task.Delay(50);

			for (var value = start_value; value <= stop_value; value += increment_size)
			{
				var kelvin_packet = new byte[]
				{
				(byte)(value>>8),
				(byte)(value)
				};
				var packet = new MasterPacket(MasterCommand.SetCCT, kelvin_packet);
				_ble_handler.send_ble_luxli_packet(packet);
				await System.Threading.Tasks.Task.Delay(interval * 1000);
			}
		}

		private void startValue_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			start_value = Convert.ToInt32(startValue_TextBox.Text);
		}

		private void stopValue_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			stop_value = Convert.ToInt32(stopValue_TextBox.Text);
		}

		private void incrementSize_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			increment_size = Convert.ToInt32(incrementSize_TextBox.Text);
		}

		private void brightness_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			brightness = Convert.ToInt32(brightness_TextBox.Text);
		}

		private void connLuxlis_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}
	}
}
