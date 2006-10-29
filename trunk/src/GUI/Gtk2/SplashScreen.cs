/* [ GUI/Gtk2/SplashScreen.cs ] - Niry Gtk2/Cairo Splash Screen
 * Author: Matteo Bertozzi
 * =============================================================================
 * Niry Sharp
 * Copyright (C) 2006 Matteo Bertozzi.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301 USA
 */

using Gtk;
using Cairo;
using System;

namespace Niry.GUI.Gtk2 {
	public delegate void SplashScreenDrawHandler (object o, SplashScreenDrawArgs args);

	public class SplashScreenDrawArgs : EventArgs {
		private Gdk.Rectangle allocation;
		private Gdk.Drawable drawable;
		private Context context;
		private bool retval;
        
        public SplashScreenDrawArgs (Context context, Gdk.Drawable drawable, Gdk.Rectangle allocation) {
			this.context = context;
			this.drawable = drawable;
			this.allocation = allocation;
			this.retval = true;
		}
        
		public Gdk.Rectangle Allocation {
			get { return(this.allocation); }
		}
        
		public Context Context {
			get { return(this.context); }
		}
        
		public Gdk.Drawable Drawable {
			get { return(this.drawable); }
		}
        
		public bool RetVal {
			get { return(this.retval); }
			set { this.retval = value; }
		}
	}

	/// Gtk2/Cairo Splash Screen
	public class SplashScreen : Gtk.Window, IDisposable {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event SplashScreenDrawHandler DrawScreen = null;

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private static Color defaultColor = new Color(0xff, 0xff, 0xff, 0.65);
        private Color textColor = defaultColor;
        private Color barFillColor = defaultColor;
        private Color barOutlineColor = defaultColor;
		private Gdk.Pixbuf pixbuf;
		private double progress;
		private string message;
        
		// ============================================
		// PUBLIC Constructors
		// ============================================
		public SplashScreen (string title, Gdk.Pixbuf pixbuf) : base(WindowType.Toplevel) {
			this.pixbuf = pixbuf;

			// Initialize Window Options
			Title = title;
			KeepAbove = true;
			Decorated = false;
			AllowGrow = false;
			Resizable = false;
			AppPaintable = true;
			WindowPosition = WindowPosition.Center;
			TypeHint = Gdk.WindowTypeHint.Splashscreen;
			SetSizeRequest(pixbuf.Width, pixbuf.Height);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Run Splash Screen
		public void Run() {
			ShowAll();
			PumpEventLoop();
		}

		/// Update Splash Screen Progress
		public void Update (string message, int current, int total) {
			this.message = message;
			this.progress = current / (double) total;
			this.QueueDraw();
			this.PumpEventLoop();
		}

		/// Destroy Splash Screen
		public override void Dispose() {
			Hide();
			Destroy();
			base.Dispose();
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		protected override void OnRealized() {
			base.OnRealized();
			GdkWindow.SetBackPixmap(null, false);
			QueueDraw();
		}

		protected override bool OnExposeEvent (Gdk.EventExpose evnt) {
			if (!IsRealized) return(false);

			Cairo.Context cr = Gdk.CairoHelper.Create(GdkWindow);
			bool retval = true;
            
			foreach(Gdk.Rectangle rect in evnt.Region.GetRectangles()) {
				cr.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
				cr.Clip();
				retval |= DrawSplash(cr);
			}
            
			((IDisposable)cr).Dispose();
			return(retval);
        }

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void PumpEventLoop() {
			while(Application.EventsPending()) {
				Application.RunIteration(false);
				System.Threading.Thread.Sleep(10);
			}
		}

		private bool DrawSplash (Cairo.Context cr) {
			if (DrawScreen != null) {
				SplashScreenDrawArgs args = new SplashScreenDrawArgs(cr, GdkWindow, Allocation);
				DrawScreen(this, args);
				return(args.RetVal);
			}

			int barHeight = 6;
			GdkWindow.DrawPixbuf(Style.LightGC(StateType.Normal), pixbuf, 0, 0, 
								 Allocation.X, Allocation.Y, 
								 pixbuf.Width, pixbuf.Height,
								 Gdk.RgbDither.None, 0, 0);
			cr.Antialias = Antialias.Default;
            cr.Color = textColor;

			if (message != null) {
				cr.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Normal);
				cr.SetFontSize(12);
				cr.MoveTo(20, Allocation.Height - barHeight - 26);
				cr.ShowText(message);
			}
            
			cr.Antialias = Antialias.None;
            
			cr.Color = barOutlineColor;
			cr.LineWidth = 1.0;
			cr.Rectangle(Allocation.X + 20, Allocation.Height - barHeight - 20,
						 Allocation.Width - 40, barHeight);
			cr.Stroke();

			cr.Color = barFillColor;
			cr.Rectangle(Allocation.X + 20 + 1, Allocation.Height - barHeight - 18,
						 (int)((double)(Allocation.Width - 43) * progress), barHeight - 3);
			cr.FillPreserve();

			return(true);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public Color TextColor {
			get { return(textColor); }
			set {
				this.textColor = value;
				this.QueueDraw();
			}
		}
        
		public Color BarOutlineColor {
			get { return(this.barOutlineColor); }
			set {
				this.barOutlineColor = value;
				this.QueueDraw();
			}
		}
        
		public Color BarFillColor {
			get { return(this.barFillColor); }
			set {
				this.barFillColor = value;
				this.QueueDraw();
			}
		}
	}
}
