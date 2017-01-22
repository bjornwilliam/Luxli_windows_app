//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Luxli_Windows_app
{
	public partial class MainPage : Page
	{
		public const string FEATURE_NAME = "Bluetooth Low Energy Client C# Sample";

		List<Scenario> scenarios = new List<Scenario>
				{
						new Scenario() { Title="Control Leds", ClassType=typeof(Function1_ControlLEDS) },
						new Scenario() { Title="Kelvin Control", ClassType=typeof(Function2_Kelvin) },
				};

		public string SelectedBleDeviceId;
		public string SelectedBleDeviceName = "No device selected";
	}

	public class Scenario
	{
		public string Title { get; set; }
		public Type ClassType { get; set; }
	}
}
