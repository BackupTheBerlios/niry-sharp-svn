// [ Test/TestUdp.cs ]
// Author: Matteo Bertozzi
// gmcs TestUdp.cs -r:niry-sharp.dll

using System;
using System.Threading;

using Niry;
using Niry.Network;

namespace Niry.Tests {
	public class TestUdp {
		public static void Main() {
			try {
				UdpServer.StartListening(7085);
				Console.WriteLine("UDP Server Listening On 7085");

				Thread thread = new Thread(new ThreadStart(UdpWrite));
				thread.Start();
				thread.Join();
				Thread.Sleep(1000);
			} catch (Exception e) {
				Console.WriteLine("Exception: {0}", e.Message);
			} finally {
				UdpServer.StopListening();
				Console.WriteLine("UDP Server Closed");
			}
		}

		public static void UdpWrite() {
			try {
				Console.WriteLine("UDP Write");
				for (int i=0; i < 10; i++)
					UdpServer.Send("localhost", 7085, "Ciao " + i.ToString());
			} catch (Exception e) {
				Console.WriteLine("UDP Write Error: {0}", e.Message);
			}
		}
	}
}
