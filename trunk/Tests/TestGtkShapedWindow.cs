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
		window.GdkWindow.Clear();
		Gtk.Fixed fixedBox = new Gtk.Fixed();
		window.Add(fixedBox);
		Gtk.Label label = new Gtk.Label("Hello World");
		fixedBox.Put(label, 130, 100);
		window.ShowAll();

		Gtk.Application.Run();
	}
}
