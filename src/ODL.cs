using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odl;

public static class ODL
{
	public static ConsoleLogger ConsoleLogger = new ConsoleLogger();

	/// <summary>
	/// A simple logger with a WriteLine method.
	/// </summary>
	public static ILogger Logger;

	private static Platform? _platform;
	/// <summary>
	/// The current OS.
	/// </summary>
	public static Platform Platform
	{
		get
		{
			if (_platform != null) return (Platform) _platform;
			if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) _platform = Platform.Windows;
			else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux)) _platform = Platform.Linux;
			else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX)) _platform = Platform.MacOS;
			else _platform = Platform.Unknown;
			return (Platform) _platform;
		}
	}

	public static bool OnWindows => Platform == Platform.Windows;
	public static bool OnLinux => Platform == Platform.Linux;
	public static bool OnMacOS => Platform == Platform.MacOS;

	public static ImageResolver ImageResolver = new ImageResolver();
	public static AudioResolver AudioResolver = new AudioResolver();
	public static FontResolver FontResolver = new FontResolver();
}

public enum Platform
{
	Unknown,
	Windows,
	Linux,
	MacOS,
	IOS,
	Android
}