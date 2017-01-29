using System;

namespace Luxli_Windows_app
{
	/// <summary>
	/// Commands that can be sent from the master to a slave device.
	/// </summary>
	public enum MasterCommand : byte
	{
		/// <summary>Sets the current color, using raw RGB data. <b>Parameters:</b> 3 bytes: <c>red</c>, <c>green></c> and <c>blue</c>. For each color a value between <c>0</c> and <c>255</c>.
		/// This color format is device dependent and should only be used for testing purposes.</summary>
		SetRawRGB = 0x01,
		/// <summary>
		/// Sets the current color, using the Lab color space. <b>Parameters</b>: 3 bytes: <c>L</c>, <c>a</c> and <c>b</c>. For each byte a value between <c>0</c> and <c>255</c>. 
		/// This format is compact and device independent, but requires color domain transformation on the slave. Color domain transformation is computational intensive and may require a few milliseconds to perform.
		/// </summary>
		SetLabColor = 0x02,
		/// <summary>
		/// Sets the current color, using the CIEXYZ color space. <b>Parameters</b>: 12 bytes consisting of 3 x 32 bit (decimal32) floating point values, <c>X</c>, <c>Y</c> and <c>Z</c>. 
		/// This color format is device independent and is the internal color representation of the slave.
		/// </summary>
		SetCIEXYZ = 0x03,
		/// <summary>
		/// Sets the exposure value (deprecated). <b>Parameters</b>: A 32 bit value (4 bytes).
		/// 0 is full intensity. Each full Int is one stop down. Eg. 3 is three stops down. 1.5 is one and a half stop down.
		/// This is a int-converted float (ev * 100 000). So 2.8 stops down from full intensity is sent as 280 000. 1.5 stops is sent as 150 000.
		/// Any values beyond the lowest value the device can emit is interpreted as off. The Kick has a 5 stop range, so any value larger than 500000 will turn off the light.
		/// </summary>
		SetEV = 0x04,
		/// <summary>
		/// Sets the color temperature in Kelvin. <b>Parameters</b>: A 16 bit value (2 bytes). Typical values are between <c>2500</c> and <c>10000</c>.
		/// </summary>
		SetCCT = 0x05,
		/// <summary>
		/// Sets the exposure value. This is the preferred version. <b>Parameters</b>: An 8 bit value.
		/// Brightness from full (<c>255</c>) to off (<c>0</c>) on a linear scale (<c>127</c> is half power, <c>63</c> is quarter power etc.).
		/// </summary>
		SetEV2 = 0x06,
		/// <summary>
		/// Sets the current luminance. <b>Parameters</b>: 8 bytes consisting of 8 x 32 bit (decimal32) floating point values, <c>x</c>, <c>y</c>.
		/// The color space is CIE xyY. Only the x,y color coordinates are sent. This color format is device independent.
		/// </summary>
		SetChroma = 0x07,
		/// <summary>
		/// Sets the PWM clock speed (refresh). <b>Parameters</c>: 1 byte. Use any value between <c>1</c> and <c>50</c>. Default is <c>25</c>.
		/// Low values improves flicker but affects color linearity. Refresh = <c>25000/PR+1</c>. Examples: PR 49 = 500Hz, PR 24 = 1kHz, 4 = 5kHz.
		/// </summary>
		SetPR = 0x08,
		/// <summary>
		/// Sets refresh rate directly in hz. <b>Parameters</b>: A 16 bit value (2 bytes). Minimum value is <c>10</c>, maximum value is <c>5000</c>.
		/// Not every single frequency can be generated. The PWM engine will use the closest match.
		/// </summary>
		SetRefreshRate = 0x09,
		/// <summary>
		/// Alters the behavior of the device's physical buttons. <b>Parameters</b>: 1 byte. A value of <c>0</c> (zero) is default.
		/// <para>
		/// <c>0</c> = Default(Brightness buttons control brightness, CCT buttons controls color temperature)<br/>
		/// <c>1</c> = Refresh Rate (Brightness buttons control brightness, CCT buttons controls refresh rate)<br/>
		/// <c>2</c> = Demo Mode (Each button triggers a different light effect)<br/>
		/// </para>
		/// This value is not stored across sessions. Each time the device is turned on, the alternate button functions reverts to default.
		/// </summary>
		SetAlternateButtonFunction = 0x0A,
		/// <summary>
		/// Starts a preset light effect. <b>Parameters</b>: The parameters depend on the desired effect.
		/// <para>
		/// Strobe. 10 bytes:
		/// <b>TODO:</b> Implement effect classes (but isn't it already?)
		/// </summary>
		RunPresetEffect = 0x10,
		/// <summary>
		/// Stops an ongoing light effect. <b>Parameters</b>: Effect UID (16bit, two bytes)
		/// </summary>
		StopEffect = 0x11,
		/// <summary>
		/// Loads custom effect into the KickLight. <b>Parameters</b>: Effect UID (16bit, two bytes) plus data. 
		/// This will typically transmit a relatively large amount of data describing a light effect. The slave will store the effect for future execution. Probably requires an ACK from the slave.
		/// Data format TBD.
		/// </summary>
		LoadEffect = 0x12,
		/// <summary>
		///  Set up to 5 leds, 16 bit per Led , R G B WW CW 
		/// </summary>
		SetLedsDirect = 0x14,
		/// <summary>
		/// Search for available slave units. Typically broadcast when the master does not have any slaves registered. No data is sent with this command. Slaves will respond with their UID.
		/// </summary>
		/// 
		Hello = 0x80,
		/// <summary>
		/// Query slave name. <b>Parameters</b>: Ask for the user name and ID color of the slave unit. After receiving this command, the slave will wait 1 ms before sending back data. No data is sent with this command.
		/// </summary>
		GetName = 0x81,
		/// <summary>
		/// Assign a user defined name and a ID color to the slave. <b>Parameters</b>: The first 3 bytes are RGB values for the ID color <red>, <green> and <blue>, followed by the user defined name (16 ascii characters).
		/// The slave unit will store its assigned user name and ID color in non-volatile memory.
		/// The slave is the primary storage for names and IDs. The master acts as a cache, and til update itself if the data in the slave changes.
		/// Do not send less than 16 ascii characters for the name. Pad with <c>NUL</c> (0x00) if name is shorter.
		/// </summary>
		SetName = 0x82,
		/// <summary>
		/// Query slave status. <b>Parameters</b>: Ask for status from the slave. After receiving this command, the slave will wait 1 ms before sending back data. No data is sent with this command.
		/// See slave command <c>0x83</c> for return values.
		/// </summary>
		GetStatus = 0x83,
		/// <summary>
		/// Set current time. <b>Parameters</b>: The master can use either network time or is own RTC for synchronizing. If there is no network time available the master can set the slave Real Time Clock with this command.
		/// Data format TBD
		/// </summary>
		SetClock = 0x84,
		/// <summary>
		/// Query custom effects.  Ask the slave to list custom effects and the remaining space in non-volatile storage. After receiving this command, the slave will wait 1 ms before sending back data.
		/// <b>Parameters</b>: No data is sent with this command.
		/// </summary>
		GetCustomEffects = 0x85,
		/// <summary>
		/// Light the slave for a given period of time. <b>Parameters</b>: 2 bytes (16bit) duration in ms.
		/// Upon receiving this command, the slave will emit a full power (EV=0) light pulse for <duration> milliseconds. The color of the light will be the user defined ID color for the slave unit.
		/// </summary>
		Signal = 0x86,
		/// <summary>
		/// Ask for the hardware version, firmware version and serial number of the slave. After receiving this command, the slave will wait 1 ms before sending back data.
		/// <b>Parameters</b>: No data is sent with this command.
		/// </summary>
		GetVersion = 0x87,
		/// <summary>
		/// Indicates to the slave that the master is disconnecting and the slave should change mode to standalone.
		/// <b>Parameters</b>: No data is sent with this command.
		/// </summary>
		MasterDisconnect = 0x88,
		/// <summary>
		/// Ask for EV2, Chroma, ID color, firmware version, hardware version, and user name of the slave unit. After receiving this command, the slave will wait 1 ms before sending back data.
		/// <b>Parameters</b>: No data is sent with this command.
		/// </summary>
		GetSummary = 0x91,
		/// <summary>
		/// Ask for debug info.
		/// <b>Parameters</b>: No data is sent with this command.
		/// </summary>
		GetSessionDebugInfo = 0x92,
		/// <summary>
		/// Set the SSID for the alternative AP.
		/// <b>Parameters</b>: A string of max 31 ASCII characters. 	
		/// </summary>
		SetAlternativeNetworkSSID = 0x94,
		/// <summary>
		/// Set the password for the alternative AP. <b>Parameters</b>: Password, max 64 characters.
		/// </summary>
		SetAlternativeNetworkPassword = 0x95,
		/// <summary>
		/// Query alternative SSID.	After receiving this command, the slave will wait 1 ms before sending back data. Slave will return the stored alternative net SSID, max 32 characters (31 characters plus terminating null).
		/// <b>Parameters</b>: No data is sent with this command.
		/// </summary>
		GetAlternativeNetworkSSID = 0x96,
		/// <summary>
		/// Sets the authentication method for the alternative network AP.
		/// <b>Parameters</b>: 1 byte, indicating authentication type. <c>0</c> = none, <c>1</c> = WEP, <c>2</c> = WPA
		/// </summary>
		SetAlternativeNetworkAuthentication = 0x97,
		/// <summary>
		/// Ask the device to enter sleep mode.
		/// <b>Parameters</b>: No data is sent with this command. 	
		/// </summary>
		Sleep = 0x98,
		/// <summary>
		/// Ask the device about its capabilities. After receiving this command, the slave will wait 1 ms before sending back data.
		/// <b>Parameters</b>: No data is sent with this command. 
		/// </summary>
		GetCapabilities = 0x99,
		/// <summary>
		/// Placeholder for an unknown command.
		/// </summary>
		UNKNOWN = 0xFF
	}
}

