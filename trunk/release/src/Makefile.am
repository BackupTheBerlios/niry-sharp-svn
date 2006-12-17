nirydir=$(pkglibdir)
niry_SCRIPTS = niry-sharp.dll
EXTRA_DIST = $(niry_sources) $(niry_sources_in)
CLEANFILES = niry-sharp.dll
DISTCLEANFILES = Info.cs AssemblyInfo.cs Makefile.in

SRC_BASE = 		Delegates.cs

SRC_GUI_GTK_28 = 	GUI/Gtk2/ExtMenuItem.cs	\
					GUI/Gtk2/ImageUtils.cs		\
					GUI/Gtk2/LoginDialog.cs		\
					GUI/Gtk2/PopupMenu.cs		\
					GUI/Gtk2/ProxySettings.cs	\
					GUI/Gtk2/ScrollBox.cs		\
					GUI/Gtk2/ShapedWindow.cs	\
					GUI/Gtk2/TimeUtils.cs		\
					GUI/Gtk2/WindowUtils.cs

SRC_GUI_GTK_210 =	GUI/Gtk2/Spinner.cs			\
					GUI/Gtk2/SplashScreen.cs

SRC_GUI = $(SRC_GUI_GTK_28)

SRC_DATABASE =	Database/SQLite.cs

SRC_NETWORK =	Network/FtpClient.cs		\
				Network/P2P/P2PManager.cs	\
				Network/P2P/PeerEvent.cs	\
				Network/P2P/PeerSocket.cs	\
				Network/SocketReader.cs		\
				Network/UdpServer.cs		\
				Network/Util.cs				\
				Network/abstract/Server.cs

SRC_UTILS = 	Utils/CryptoUtils.cs		\
				Utils/Debug.cs				\
				Utils/FileUtils.cs			\
				Utils/TextUtils.cs			\
				Utils/UrlExtractor.cs		\
				Utils/UrlUtils.cs

niry_sources_in = AssemblyInfo.cs.in Info.cs.in
niry_generated_sources = $(niry_sources_in:.in=)
niry_sources = $(SRC_BASE) $(SRC_GUI) $(SRC_DATABASE) $(SRC_NETWORK) $(SRC_UTILS)

niry_build_sources = $(addprefix $(srcdir)/, $(niry_sources))
niry_build_sources += $(niry_generated_sources)

PKG_SQLITE = -r:System.Data.dll -r:Mono.Data.SqliteClient.dll
PKGS = $(GTKSHARP_LIBS) $(PKG_SQLITE)

niry-sharp.dll: $(niry_build_sources)
	$(MCS) $(PKGS) -target:library -out:$@ $(niry_build_sources)
