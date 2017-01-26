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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Luxli_Windows_app
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Function2_Kelvin : Page
	{


		private bool sendingState;

		private ble_handler _ble_handler;

		public Int32 Interval_s { get; set; }
		private int start_value;
		private int stop_value;
		private int increment_size;
		private int brightness;

		public Function2_Kelvin()
		{
			this.InitializeComponent();
			this.start_value = 3000;
			this.stop_value = 10000;
			this.Interval_s = 3;
			this.increment_size = 200;
			this.brightness = 10;
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
				sendingState = true;
				startStopButton.Content = "Stop";
				sendKelvin(Interval_s, start_value, stop_value, increment_size);
			}
			else
			{
				sendingState = false;
				startStopButton.Content = "Start";
			}
		}

		private async void sendKelvin(Int32 interval, Int32 start_value, Int32 stop_value, Int32 increment_size)
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
	}
}
