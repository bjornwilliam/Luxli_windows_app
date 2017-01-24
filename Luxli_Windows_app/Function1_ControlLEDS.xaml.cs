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
	public sealed partial class Function1_ControlLEDS : Page
	{

		public MainPage rootPage = MainPage.Current;

		private ble_handler _ble_handler;

		private UInt16 R_SliderVal = 0;
		private UInt16 G_SliderVal = 0;
		private UInt16 B_SliderVal = 0;
		private UInt16 WW_SliderVal = 0;
		private UInt16 CW_SliderVal = 0;

		public Function1_ControlLEDS()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_ble_handler = e.Parameter as ble_handler;

		}
		private void redLedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			this.R_SliderVal = (UInt16)e.NewValue;
			var led_data = new byte[]
			{
				(byte)R_SliderVal,
				(byte)G_SliderVal,
				(byte)B_SliderVal
			};
			var packet = new MasterPacket(MasterCommand.SetRawRGB, led_data);

			_ble_handler.send_ble_luxli_packet(packet);
		}

		private void greenLedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{

		}

		private void blueLedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{

		}

		private void wwLedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{

		}

		private void cwLedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{

		}

	}
}
