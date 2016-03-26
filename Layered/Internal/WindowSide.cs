namespace Layered.Internal {

	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;
	using System.Security.Permissions;
	using System.Windows.Forms;

	using Native;




	/// <summary>The layered window, that represents one of the side of <see cref="Layered.Window"/>.</summary>
	/// <seealso cref="System.Windows.Forms.NativeWindow" />
	/// <seealso cref="System.IDisposable" />
	internal sealed class WindowSide : NativeWindow, IDisposable {

		/// <summary><see cref="Layered.Window"/> class, that owns this instance</summary>
		public readonly Window Owner;

		/// <summary>Device context for this instance</summary>
		public readonly Surface Surface;

		/// <summary>Gets or sets location of <see cref="Layered.Internal.WindowSide"/></summary>
		public Point Location = Point.Empty;

		/// <summary>Gets or sets size of <see cref="Layered.Internal.WindowSide"/></summary>
		public Size Size = Size.Empty;

		/// <summary>Gets x-coordinate of <see cref="Layered.Internal.WindowSide"/></summary>
		internal int X => Location.X;

		/// <summary>Gets y-coordinate of <see cref="Layered.Internal.WindowSide"/></summary>
		internal int Y => Location.Y;

		/// <summary>Gets or sets state, that determines, whether this form is visible.</summary>
		internal bool Visible;




		/// <summary>Initializes a new instance of the <see cref="WindowSide"/> class.</summary>
		/// <param name="owner">Instance of <see cref="Layered.Window"/>.</param>
		/// <param name="surfaceSize">Maximum <see cref="System.Drawing.Size"/> of surface.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public WindowSide(Window owner, Size surfaceSize) {

			// ...
			Owner = owner;

			// Creating surface
			Surface = new Surface();
			Surface.CreateDeviceContext();
			Surface.CreateSurface(surfaceSize.Width, surfaceSize.Height, PixelFormat.Format32bppArgb);
			Surface.SelectSurface();

			// ...
			var createParams = new CreateParams {
				ExStyle = (int)(ExtendedWindowStyles.WS_EX_LAYERED | ExtendedWindowStyles.WS_EX_TRANSPARENT | ExtendedWindowStyles.WS_EX_TOOLWINDOW)
			};

			// Creating handle
			CreateHandle(createParams);

			// Clearing base style, since we need plane rectangle without anything
			NativeMethods.SetWindowLong(Handle, (int)WindowLongFlags.GWL_STYLE, 0);

		}




		/// <summary>Method for updating <see cref="Layered.Internal.WindowSide"/>, that stands at left side of target <see cref="System.Windows.Forms.Form"/>.</summary>
		/// <param name="cx">The new width of <see cref="Layered.Internal.WindowSide"/>.</param>
		/// <param name="cy">The new height of <see cref="Layered.Internal.WindowSide"/>.</param>
		/// <param name="fullRedrawing">When set to true, <see cref="Layered.Surface"/> will be redrawn fully, otherwise, only changed area.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		internal void PaintLeft(int cx, int cy, bool fullRedrawing = false) {

			// ...
			if (cy == Size.Height && !fullRedrawing) return;

			// Position and size of side-fragment
			int offset, height;

			// ...
			if (fullRedrawing) {
				
				// Drawing corner
				BitBlt(Owner.Theme.Metrics.Vertical.TopLeft, new Rectangle(Point.Empty, Owner.Theme.Metrics.Vertical.TopLeft.Size));

				// In this case, left-side will start right after top-left corner, and end right before bottom-left corner
				offset = Owner.Theme.Metrics.Vertical.TopLeft.Height;
				height = cy - Owner.Theme.Metrics.Vertical.TopLeft.Height - Owner.Theme.Metrics.Vertical.BottomLeft.Height;

			} else {

				// In this case, left-side will start right before bottom-left corner
				offset = Size.Height - Owner.Theme.Metrics.Vertical.BottomLeft.Height;
				height = Math.Abs(cy - Size.Height);

			}

			// Drawing side, if
			if (height != 0) StretchBlt(Owner.Theme.Metrics.Left, new Rectangle(0, offset, Owner.Theme.Metrics.Left.Width, height));

			// The only corner, that we need to draw always
			BitBlt(Owner.Theme.Metrics.Vertical.BottomLeft,
				new Rectangle(new Point(0, cy - Owner.Theme.Metrics.Vertical.BottomLeft.Height), Owner.Theme.Metrics.Vertical.BottomLeft.Size));

			// Updating size
			Size = new Size(cx, cy);

			// Updating position, size and content of layered window.
			NativeMethods.UpdateLayeredWindow(Handle, Owner.DesktopDeviceContext, ref Location, ref Size,
				Surface.DeviceContext, ref Owner.SurfaceLocation, 0, ref Owner.BlendFunction, UpdateLayeredWindowFlags.ULW_ALPHA);

		}




		/// <summary>Method for updating <see cref="Layered.Internal.WindowSide"/>, that stands at right side of target <see cref="System.Windows.Forms.Form"/>.</summary>
		/// <param name="cx">The new width of <see cref="Layered.Internal.WindowSide"/>.</param>
		/// <param name="cy">The new height of <see cref="Layered.Internal.WindowSide"/>.</param>
		/// <param name="fullRedrawing">When set to true, <see cref="Layered.Surface"/> will be redrawn fully, otherwise, only changed area.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		internal void PaintRight(int cx, int cy, bool fullRedrawing = false) {

			// ...
			if (cy == Size.Height && !fullRedrawing) return;

			// Position and size of side-fragment
			int offset, height;

			// ...
			if (fullRedrawing) {

				// Drawing corner
				BitBlt(Owner.Theme.Metrics.Vertical.TopRight, new Rectangle(Point.Empty, Owner.Theme.Metrics.Vertical.TopRight.Size));

				// In this case, left-side will start right after top-left corner, and end right before bottom-left corner
				offset = Owner.Theme.Metrics.Vertical.TopRight.Height;
				height = cy - Owner.Theme.Metrics.Vertical.TopRight.Height - Owner.Theme.Metrics.Vertical.BottomRight.Height;

			} else {

				// In this case, left-side will start right before bottom-left corner
				offset = Size.Height - Owner.Theme.Metrics.Vertical.BottomRight.Height;
				height = Math.Abs(cy - Size.Height);

			}

			// Drawing side, if
			if (height != 0) StretchBlt(Owner.Theme.Metrics.Right, new Rectangle(0, offset, Owner.Theme.Metrics.Right.Width, height));

			// The only corner, that we need to draw always
			BitBlt(Owner.Theme.Metrics.Vertical.BottomRight,
				new Rectangle(new Point(0, cy - Owner.Theme.Metrics.Vertical.BottomRight.Height), Owner.Theme.Metrics.Vertical.BottomRight.Size));

			// Updating size
			Size = new Size(cx, cy);

			// Updating position, size and content of layered window.
			NativeMethods.UpdateLayeredWindow(Handle, Owner.DesktopDeviceContext, ref Location, ref Size,
				Surface.DeviceContext, ref Owner.SurfaceLocation, 0, ref Owner.BlendFunction, UpdateLayeredWindowFlags.ULW_ALPHA);

		}




		/// <summary>Method for updating <see cref="Layered.Internal.WindowSide"/>, that stands at top side of target <see cref="System.Windows.Forms.Form"/>.</summary>
		/// <param name="cx">The new width of <see cref="Layered.Internal.WindowSide"/>.</param>
		/// <param name="cy">The new height of <see cref="Layered.Internal.WindowSide"/>.</param>
		/// <param name="fullRedrawing">When set to true, <see cref="Layered.Surface"/> will be redrawn fully, otherwise, only changed area.</param>
		//[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		internal void PaintTop(int cx, int cy, bool fullRedrawing = false) {

			// ...
			if (cx == Size.Width && !fullRedrawing) return;

			// Position and size of side-fragment
			int offset, width;

			// ...
			if (fullRedrawing) {

				// Drawing corner
				BitBlt(Owner.Theme.Metrics.Horizontal.TopLeft, new Rectangle(Point.Empty, Owner.Theme.Metrics.Horizontal.TopLeft.Size));

				// In this case, left-side will start right after top-left corner, and end right before bottom-left corner
				offset = Owner.Theme.Metrics.Horizontal.TopLeft.Width;
				width = cx - Owner.Theme.Metrics.Horizontal.TopLeft.Width - Owner.Theme.Metrics.Horizontal.TopRight.Width;

			} else {

				// In this case, left-side will start right before bottom-left corner
				offset = Size.Width - Owner.Theme.Metrics.Horizontal.TopRight.Width;
				width = Math.Abs(cx - Size.Width);

			}

			// Drawing side, if
			if (width != 0) StretchBlt(Owner.Theme.Metrics.Top, new Rectangle(offset, 0, width, Owner.Theme.Metrics.Top.Height));

			// The only corner, that we need to draw always
			BitBlt(Owner.Theme.Metrics.Horizontal.TopRight,
				new Rectangle(new Point(cx - Owner.Theme.Metrics.Horizontal.TopRight.Width, 0), Owner.Theme.Metrics.Horizontal.TopRight.Size));

			// Updating size
			Size = new Size(cx, cy);

			// Updating position, size and content of layered window.
			NativeMethods.UpdateLayeredWindow(Handle, Owner.DesktopDeviceContext, ref Location, ref Size,
				Surface.DeviceContext, ref Owner.SurfaceLocation, 0, ref Owner.BlendFunction, UpdateLayeredWindowFlags.ULW_ALPHA);

		}




		/// <summary>Method for updating <see cref="Layered.Internal.WindowSide"/>, that stands at bottom side of target <see cref="System.Windows.Forms.Form"/>.</summary>
		/// <param name="cx">The new width of <see cref="Layered.Internal.WindowSide"/>.</param>
		/// <param name="cy">The new height of <see cref="Layered.Internal.WindowSide"/>.</param>
		/// <param name="fullRedrawing">When set to true, <see cref="Layered.Surface"/> will be redrawn fully, otherwise, only changed area.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		internal void PaintBottom(int cx, int cy, bool fullRedrawing = false) {

			// ...
			if (cx == Size.Width && !fullRedrawing) return;

			// Position and size of side-fragment
			int offset, width;

			// ...
			if (fullRedrawing) {

				// Drawing corner
				BitBlt(Owner.Theme.Metrics.Horizontal.BottomLeft, new Rectangle(Point.Empty, Owner.Theme.Metrics.Horizontal.BottomLeft.Size));

				// In this case, left-side will start right after top-left corner, and end right before bottom-left corner
				offset = Owner.Theme.Metrics.Horizontal.BottomLeft.Width;
				width = cx - Owner.Theme.Metrics.Horizontal.BottomLeft.Width - Owner.Theme.Metrics.Horizontal.BottomRight.Width;

			} else {

				// In this case, left-side will start right before bottom-left corner
				offset = Size.Width - Owner.Theme.Metrics.Horizontal.BottomRight.Width;
				width = Math.Abs(cx - Size.Width);

			}

			// Drawing side, if
			if (width != 0) StretchBlt(Owner.Theme.Metrics.Bottom, new Rectangle(offset, 0, width, Owner.Theme.Metrics.Bottom.Height));

			// The only corner, that we need to draw always
			BitBlt(Owner.Theme.Metrics.Horizontal.BottomRight,
				new Rectangle(new Point(cx - Owner.Theme.Metrics.Horizontal.BottomRight.Width, 0), Owner.Theme.Metrics.Horizontal.BottomRight.Size));

			// Updating size
			Size = new Size(cx, cy);

			// Updating position, size and content of layered window.
			NativeMethods.UpdateLayeredWindow(Handle, Owner.DesktopDeviceContext, ref Location, ref Size,
				Surface.DeviceContext, ref Owner.SurfaceLocation, 0, ref Owner.BlendFunction, UpdateLayeredWindowFlags.ULW_ALPHA);

		}




		/// <summary>Bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context. Depending on provided sizes, StretchBlt or BitBlt function will be called.</summary>
		/// <param name="src"><see cref="System.Drawing.Rectangle"/>, that will be copied from source device context.</param>
		/// <param name="dst"><see cref="System.Drawing.Rectangle"/>, that will be replaced in destination device context.</param>
		private void StretchBlt(Rectangle src, Rectangle dst) {

			if (dst.Width > src.Width || dst.Height > src.Height) {

				// Stretch
				NativeMethods.StretchBlt(Surface.DeviceContext, dst.X, dst.Y, dst.Width, dst.Height,
					Owner.Theme.DeviceContext, src.X, src.Y, src.Width, src.Height, CopyPixelOperation.SourceCopy);

			} else {

				// Copy
				NativeMethods.BitBlt(Surface.DeviceContext, dst.X, dst.Y, dst.Width, dst.Height,
					Owner.Theme.DeviceContext, src.X, src.Y, CopyPixelOperation.SourceCopy);

			}

		}




		/// <summary>Bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context.</summary>
		/// <param name="src"><see cref="System.Drawing.Rectangle"/>, that will be copied from source device context.</param>
		/// <param name="dst"><see cref="System.Drawing.Rectangle"/>, that will be replaced in destination device context.</param>
		private void BitBlt(Rectangle src, Rectangle dst) {
			NativeMethods.BitBlt(Surface.DeviceContext, dst.X, dst.Y, dst.Width, dst.Height,
				Owner.Theme.DeviceContext, src.X, src.Y, CopyPixelOperation.SourceCopy);

		}




		/// <summary>Function, that processes messages sent to a window.</summary>
		/// <param name="m">Windows message</param>
		protected override void WndProc(ref Message m) {

			switch (m.Msg) {
				case (int)WindowMessages.WM_WINDOWPOSCHANGING:

					// Retrieving structure from LParam
					var windowPos = (WindowPos) Marshal.PtrToStructure(m.LParam, typeof (WindowPos));

					// Visibility state
					if ((windowPos.flags & SetWindowPosFlags.SWP_SHOWWINDOW) != 0) {
						Visible = true;

					} else if ((windowPos.flags & SetWindowPosFlags.SWP_HIDEWINDOW) != 0) {
						Visible = false;

					}

					// Maximum size
					windowPos.cx = Math.Min(windowPos.cx, Surface.Width);
					windowPos.cy = Math.Min(windowPos.cy, Surface.Height);

					// Transfering structure
					Marshal.StructureToPtr(windowPos, m.LParam, true);

					// ...
					m.Result = IntPtr.Zero;
					break;

				default:
					base.WndProc(ref m);
					break;

			}
		}




		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Dispose() {

			// Disposing surface and destroing window handle
			Surface.Dispose();
			DestroyHandle();

		}




	}




}
