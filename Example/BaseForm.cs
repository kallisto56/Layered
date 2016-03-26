namespace Example {

	using System;
	using System.Drawing;
	using System.Windows.Forms;




	public sealed class BaseForm : Form {

		private readonly Layered.Window _layeredWindow;




		public BaseForm() {
			
			StartPosition = FormStartPosition.CenterScreen;
			MinimumSize = new Size(150, 150);
			MaximumSize = new Size(900, 900);
			ClientSize = new Size(350, 350);

			_layeredWindow = new Layered.Window(this, Program.DefaultTheme, new Size(900, 900));

		}




		protected override void WndProc(ref Message m) {
			switch (m.Msg) {

				// Yes, Layered.Window subscribes on Form.SizeChanged and Form.LocationChanged events, but unfortunately,
				// this is not enought. Form.SizeChanged good for resizing, Form.LocationChanged good for area-snap,
				// WINDOWPOSCHANGED works, when visibility or z-order changed.
				case 0x0047: // (int)Native.WindowMessages.WM_WINDOWPOSCHANGED:
					_layeredWindow.Update(this, EventArgs.Empty);
					base.WndProc(ref m);
					break;

				default:
					base.WndProc(ref m);
					break;
			}
		}




		protected override void Dispose(bool disposing) {
			_layeredWindow.Dispose();
			base.Dispose(disposing);
		}




	}




}
