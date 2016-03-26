namespace Layered {

	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Security.Permissions;
	using System.Windows.Forms;




	/// <summary><see cref="Layered.Theme"/> represents a visual appearance of <see cref="Layered.Window"/> and <see cref="Layered.Internal.WindowSide"/></summary>
	/// <seealso cref="Layered.Surface" />
	public class Theme : Surface {

		internal readonly Padding Margin;
		internal readonly Metrics Metrics;




		/// <summary>Initializes a new instance of the <see cref="Theme"/> class.</summary>
		/// <param name="template"><see cref="System.Drawing.Bitmap"/>, that will be used as visual appearance of <see cref="Layered.Window"/>.</param>
		/// <param name="innerRectangle"><see cref="System.Drawing.Rectangle"/>, from which <see cref="Layered.Metrics"/> can be created.</param>
		/// <param name="margin">Default for this <see cref="Layered.Theme"/> margin (<see cref="System.Windows.Forms.Padding"/>)</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Theme(Bitmap template, Rectangle innerRectangle, Padding margin)
			: this(template, new Metrics(template.Size, innerRectangle), margin) {
			
		}




		/// <summary>Initializes a new instance of the <see cref="Theme"/> class.</summary>
		/// <param name="template"><see cref="System.Drawing.Bitmap"/>, that will be used as visual appearance of <see cref="Layered.Window"/>.</param>
		/// <param name="metrics"><see cref="Layered.Metrics"/>, that contains information about each side and corner in <see cref="System.Drawing.Bitmap"/></param>
		/// <param name="margin">Default for this <see cref="Layered.Theme"/> margin (<see cref="System.Windows.Forms.Padding"/>)</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Theme(Bitmap template, Metrics metrics, Padding margin) {

			if (template.PixelFormat != PixelFormat.Format32bppArgb) {
				throw new Exception("Pixel format of provided bitmap must be PixelFormat.Format32bppArgb");
			}

			// Margin and metrics
			Margin = margin;
			Metrics = metrics;

			// Device context and surface
			CreateDeviceContext();
			CreateSurface(template);
			SelectSurface();

		}



	}




}
