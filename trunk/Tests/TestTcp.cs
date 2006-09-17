// [ Test/TestTcp.cs ]
// Author: Matteo Bertozzi
// gmcs TestTcp.cs -r:niry-sharp.dll

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;

using Niry;
using Niry.Network;

namespace Niry.Tests {
	public class TestTcp {
		public static void Main() {
			try {
				TcpServer.StartListening(7085);
				Console.WriteLine("Tcp Server Listening On 7085");

				Thread thread = new Thread(new ThreadStart(TcpWrite));
				thread.Start();
				thread.Join();
				Thread.Sleep(1000);
			} catch (Exception e) {
				Console.WriteLine("Exception: {0}", e.Message);
			} finally {
				TcpServer.StopListening();
				Console.WriteLine("Tcp Server Closed");
			}
		}

		public static void TcpWrite() {
			try {
				TcpClient client = new TcpClient("127.0.0.1", 7085);
				StreamWriter stream = new StreamWriter(client.GetStream());
				stream.Write("Prova");
				client.Close();
			} catch (Exception e) {
				Console.WriteLine("Tcp Write Error: {0}", e.Message);
			}
		}
	}
}
