# Makefile for Niry Sharp
# Author: Matteo Bertozzi <theo.bertozzi@gmail.com>
# ===================================================

CC = gmcs
CFLAGS = 

PKG_GTK = -pkg:gtk-sharp-2.0
PKG_CAIRO = -r:Mono.Cairo.dll
PKG_SQLITE = -r:System.Data.dll -r:Mono.Data.SqliteClient.dll
PKGS = $(PKG_GTK) $(PKG_SQLITE)

# ============================= Niry Sharp Infos ====
NAME = Niry Sharp
VERSION = 1.0.0
COMPILER = `$(CC) --version`
# ===================================================

SRC_BASE = 		AssemblyInfo.cs	\
				Delegates.cs

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

SRC = $(SRC_BASE) $(SRC_GUI) $(SRC_DATABASE) $(SRC_NETWORK) $(SRC_UTILS)

NirySharp: NirySharpInfo $(SRC) $(SRC_GUI_GTK_210)
	@echo
	@echo "$(NAME)"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:library $(PKGS) $(SRC) -out:niry-sharp.dll
	@echo "======================================================="
	@echo

NirySharpGtk210: NirySharpInfo $(SRC)
	@echo
	@echo "$(NAME) [Need Gtk 2.10]"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:library $(PKGS) $(PKG_CAIRO) $(SRC_GUI_GTK_210) $(SRC) -out:niry-sharp.dll
	@echo "======================================================="
	@echo

NirySharpInfo: Info.cs.in AssemblyInfo.cs.in
	@echo
	@echo "Name: $(NAME)"
	@echo "Version: $(VERSION)"
	@echo "Compiler: $(COMPILER)"
	@echo "======================================================="
	sed -e "s|\@name\@|${NAME}|" 				\
		-e "s|\@version\@|${VERSION}|" 			\
	    -e "s|\@compiler\@|${COMPILER}|" 		\
		 Info.cs.in > Info.cs
	@echo "======================================================="
	@echo
	sed -e "s|\@version\@|${VERSION}|" AssemblyInfo.cs.in > AssemblyInfo.cs
	@echo

clean:
	rm -r niry-sharp.dll Info.cs AssemblyInfo.cs
