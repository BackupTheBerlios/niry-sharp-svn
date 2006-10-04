# Makefile for Niry Sharp
# =================================================================================
CC = gmcs
CFLAGS = 

# ===================================== Packages ===========================
PKG_GTK = -pkg:gtk-sharp-2.0
PKGS = $(PKG_GTK)
# ==========================================================================

# ===================================== NyIV Info ======================
NAME = Niry Sharp
VERSION = 1.0.0
COMPILER = `$(CC) --version`
# ==========================================================================

BASE_SRC = Base/Delegate.cs

GUI_GTK2_SRC = 	GUI/Gtk2/ExtMenuItem.cs		\
				GUI/Gtk2/ImageUtils.cs		\
				GUI/Gtk2/LoginDialog.cs		\
				GUI/Gtk2/PopupMenu.cs		\
				GUI/Gtk2/ProxySettings.cs	\
				GUI/Gtk2/ScrollBox.cs		\
				GUI/Gtk2/ShapedWindow.cs	\
				GUI/Gtk2/TimeUtils.cs		\
				GUI/Gtk2/WindowUtils.cs

NETWORK_SRC = 	Network/FtpClient.cs			\
				Network/P2P/P2PManager.cs		\
				Network/P2P/PeerEvent.cs		\
				Network/P2P/PeerSocket.cs		\
				Network/SocketReader.cs			\
				Network/TcpServer.cs			\
				Network/UdpServer.cs			\
				Network/Util.cs					\
				Network/abstract/Connection.cs	\
				Network/abstract/Server.cs

UTILS_SRC =	Utils/CryptoUtils.cs	\
			Utils/Debug.cs			\
			Utils/FileUtils.cs		\
			Utils/TextUtils.cs		\
			Utils/UrlUtils.cs

SRC = $(BASE_SRC) $(GUI_GTK2_SRC) $(NETWORK_SRC) $(UTILS_SRC) Info.cs

NirySharp: NirySharpInfo $(SRC)
	@echo
	@echo "$(NAME) Plugin (DLL)"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:library $(PKGS) $(SRC) -out:niry-sharp.dll
	@echo "======================================================="
	@echo

NirySharpInfo: Info.cs.in
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

clean:
	rm -r niry-sharp.dll Info.cs

