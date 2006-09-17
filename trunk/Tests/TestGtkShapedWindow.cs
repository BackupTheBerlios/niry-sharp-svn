using Gtk;

using System;
using System.Net;
using System.Net.Sockets;

using Niry;
using Niry.GUI.Gtk2;

public class TestGtkShapedWindow {
	public static void Main() {
		Gtk.Application.Init();

		ShapedWindow window = new ShapedWindow(new Gdk.Pixbuf("window.xpm"));

		Gtk.Fixed fixedBox = new Gtk.Fixed();
		window.Add(fixedBox);

		fixedBox.Put(new Gtk.Label("Ciao a Tutti"), 50, 50);

		window.ShowAll();

		Gtk.Application.Run();
	}
}
