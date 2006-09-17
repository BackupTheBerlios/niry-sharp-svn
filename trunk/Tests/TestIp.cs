using System;
using System.Net;
using System.Net.Sockets;

public class TestIp {
	public static void Main() {
		Console.WriteLine("Any {0}", IPAddress.Any);
		Console.WriteLine("Broadcast {0}", IPAddress.Broadcast);
		Console.WriteLine("IPv6Any {0}", IPAddress.IPv6Any);
		Console.WriteLine("IPv6Loopback {0}", IPAddress.IPv6Loopback);
		Console.WriteLine("IPv6None {0}", IPAddress.IPv6None);
		Console.WriteLine("Loopback {0}", IPAddress.Loopback);
		Console.WriteLine();

		string hostname = Dns.GetHostName();
		Console.WriteLine("Local Machine's Host Name: " +  hostname);

		IPHostEntry ipEntry = Dns.GetHostEntry(hostname);
		foreach (IPAddress addr in ipEntry.AddressList) {
			Console.WriteLine (" - IP Address: {0} ", addr.ToString());
		}
	}
}
