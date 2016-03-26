namespace Layered {

	using System;
	using System.Drawing;
	using System.Security.Permissions;
	using System.Windows.Forms;

	using Internal;
	using Native;




	/// <summary>Represents a group of layered windows, that stands after target <see cref="System.Windows.Forms.Form"/>.</summary>
	/// <seealso cref="System.IDisposable" />
	public sealed class Window : IDisposable {

		/// <summary>Returns true, if update of layered windows suspended. To suspend or resume update, use <see cref="Window.Suspend()"/> and <see cref="Window.Resume(bool)"/></summary>
		public bool IsSuspended { get; private set; }




		/// <summary>Gets or sets the opacity of layered windows. Setting value will provoke full redraw.</summary>
		public int Opacity {
			get { return _opacity; }
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set {
				_opacity = value;
				_fullRedrawing = true;
				if (!IsSuspended) Update(null, EventArgs.Empty);
			}
		}




		/// <summary>Gets or sets <see cref="System.Windows.Forms.Padding"/> for current <see cref="Layered.Window"/>. Setting value will not provoke full redraw.</summary>
		public Padding Margin {
			get { return _margin; }
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set {
				_margin = value;
				if (!IsSuspended) Update(null, EventArgs.Empty);
			}
		}




		/// <summary>Gets or sets <see cref="Layered.Theme"/>. Setting value will provoke full redraw.</summary>
		public Theme Theme {
			get { return _theme; }
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set {
				_theme = value;
				_fullRedrawing = true;
				if (_theme != null && !IsSuspended) Update(null, EventArgs.Empty);
			}
		}




		/// <summary>Gets or sets <see cref="System.Windows.Forms.Form"/>. Setting value will not provoke full redraw.</summary>
		public Form TargetForm {
			get { return _targetForm; }
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			set {

				// Unsubscribing
				if (_targetForm != null && !_targetForm.Disposing) {
					_targetForm.SizeChanged -= Update;
					_targetForm.LocationChanged -= Update;
				}

				_targetForm = value;
				if (_targetForm == null) return;

				// Subscribing on specific events, in order to keep window sides up to date
				_targetForm.SizeChanged += Update;
				_targetForm.LocationChanged += Update;

				// Updating, if
				if (!IsSuspended) Update(null, EventArgs.Empty);

			}
		}




		internal IntPtr DesktopDeviceContext;
		internal BlendFunction BlendFunction = BlendFunction.Default;
		internal Point SurfaceLocation = Point.Empty;

		private readonly WindowSide _left, _top, _right, _bottom;
		private Size _previousSize = Size.Empty;

		private bool _isDisposed;
		private bool _fullRedrawing = true;
		private Padding _margin;
		private Theme _theme;
		private Form _targetForm;
		private int _opacity = 255;




		/// <summary>Initializes a new instance of the <see cref="Window"/> class.</summary>
		/// <param name="targetForm"><see cref="System.Windows.Forms.Form"/>, that will be a target for current <see cref="Layered.Window"/>. When <see cref="System.Windows.Forms.Form"/> changes, <see cref="Layered.Window"/> updates.</param>
		/// <param name="theme"><see cref="Layered.Theme"/>, that will be used while UpdateLayeredWindow.</param>
		/// <param name="maximumSize">Maximum <see cref="System.Drawing.Size"/> of target <see cref="System.Windows.Forms.Form"/>.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Window(Form targetForm, Theme theme, Size maximumSize) : this(targetForm, theme, maximumSize, Padding.Empty) {
			
		}




		/// <summary>Initializes a new instance of the <see cref="Window"/> class.</summary>
		/// <param name="targetForm"><see cref="System.Windows.Forms.Form"/>, that will be a target for current <see cref="Layered.Window"/>. When <see cref="System.Windows.Forms.Form"/> changes, <see cref="Layered.Window"/> updates.</param>
		/// <param name="theme"><see cref="Layered.Theme"/>, that will be used while UpdateLayeredWindow.</param>
		/// <param name="maximumSize">Maximum <see cref="System.Drawing.Size"/> of target <see cref="System.Windows.Forms.Form"/>.</param>
		/// <param name="margin"><see cref="System.Windows.Forms.Padding"/> from each side of target <see cref="System.Windows.Forms.Form"/>.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Window(Form targetForm, Theme theme, Size maximumSize, Padding margin) {

			// ...
			Suspend();

			// Properties first
			TargetForm = targetForm;
			Theme = theme;
			Margin = margin;

			// Throw an exception, if maximum size is less than bitmap
			if (maximumSize.Width < theme.Width || maximumSize.Height < theme.Height) {
				throw new Exception($"Maximum size of layered windows must be greater than bitmap size. MaximumSize: {maximumSize}; Bitmap.Size: Width={theme.Width}, Height={theme.Height};");
            }

			// Think about Window class as a single window, it's definetly larger than target form.
			maximumSize = new Size(
				maximumSize.Width + theme.Metrics.Left.Width + theme.Metrics.Right.Width + Math.Abs(margin.Horizontal),
				maximumSize.Height + theme.Metrics.Top.Height + theme.Metrics.Bottom.Height + Math.Abs(margin.Vertical)
			);

			// Creating each side with specific surface sizes depending on how they will be used
			_left = new WindowSide(this, new Size(theme.Metrics.Left.Width, maximumSize.Height));
			_right = new WindowSide(this, new Size(theme.Metrics.Right.Width, maximumSize.Height));

			// Horizontal sides
			_top = new WindowSide(this, new Size(maximumSize.Width, theme.Metrics.Top.Height));
			_bottom = new WindowSide(this, new Size(maximumSize.Width, theme.Metrics.Bottom.Height));

			// ...
			Resume(false);

		}




		/// <summary>Suspends update of layered windows.</summary>
		public void Suspend() {
			IsSuspended = true;
		}




		/// <summary>Resumes update of layered windows.</summary>
		/// <param name="performLayout">if set to <c>true</c>, <see cref="Layered.Window.Update(object, EventArgs)"/> will be invoked.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Resume(bool performLayout = true) {

			// Throwing an exception, if one of the important variables is not set
			if (_targetForm == null) throw new Exception("TargetForm not specified.");
			if (_theme == null) throw new Exception("Theme not specified.");

			// ...
			IsSuspended = false;

			// Calling method, to update window sides, if
			if (performLayout) Update(null, EventArgs.Empty);

		}




		/// <summary>Provokes instance to fully redraw each of the <see cref="Layered.Internal.WindowSide"/> classes.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Invalidate() {
			_fullRedrawing = true;
			if (!IsSuspended) Update(null, EventArgs.Empty);
		}




		/// <summary>Provokes instance to fully redraw each of the <see cref="Layered.Internal.WindowSide"/> classes.</summary>
		/// <param name="sender">Object, that invoked the event, that fired the event handler.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Invalidate(object sender, EventArgs e) {
			_fullRedrawing = true;
			if (!IsSuspended) Update(null, EventArgs.Empty);
		}




		/// <summary>Updates layered windows.</summary>
		/// <param name="sender">Object, that invoked the event, that fired the event handler.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Update(object sender, EventArgs e) {

			// Do nothing, if layout suspended
			if (IsSuspended) return;

			// By design, this method will be called very frequently. So, instead of checking
			// each important variable here, we're doing it in Resume() method.

			// Calculating base location and size
			var x = _targetForm.Left - _theme.Metrics.Left.Width - (_theme.Margin.Left + _margin.Left);
			var y = _targetForm.Top - _theme.Metrics.Top.Height - (_theme.Margin.Top + _margin.Top);
			var cx = _targetForm.Width + _theme.Metrics.Left.Width + _theme.Metrics.Right.Width + _theme.Margin.Horizontal + _margin.Horizontal;
			var cy = _targetForm.Height + _theme.Metrics.Top.Height + _theme.Metrics.Bottom.Height + _theme.Margin.Vertical + _margin.Vertical;

			// We're setting up location for each side here, because in one, or in both cases
			// we will need new information about, where to put window sides.
			_left.Location = new Point(x, y);
			_top.Location = new Point(x, y);
			_right.Location = new Point(x + cx - _theme.Metrics.Right.Width, y);
			_bottom.Location = new Point(x, y + cy - _theme.Metrics.Bottom.Height);

			// Determining behaviour.
			// Sometimes, we need to change position, sometimes, we need to change size,
			// and sometimes, we need to change size, and visibility-state. In last case,
			// we will call both native methods, UpdateLayeredWindow and SetWindowPos.
			// There is also, one state (last condition - partial change), when both native
			// methods are called, - when only vertical or horizontal sides are redrawned,
			// in this case we're callling SetWindowPos after UpdateLayeredWindow.
			var changeVisibility = _targetForm.Visible != _left.Visible;
			var updateLayeredWindows = cx != _previousSize.Width || cy != _previousSize.Height;
			var partialChange = (cx == _previousSize.Width && cy != _previousSize.Height) || (cx != _previousSize.Width && cy == _previousSize.Height);

			// Drawing only if required, and when target form visible (or will be)
			if ((updateLayeredWindows || _fullRedrawing) && _targetForm.Visible) {

				// First, calling specific method, according to each side. After, setting up new size.
				// In this case, each mechanism can determine difference and draw only necessary parts,
				// if full redraw not required.

				// BlendFunction stored in thi sclass, but used by WindowSide classes. Reason for that is
				// simple, - one structure instead of four and we can change opacity for all sides at once.
				BlendFunction.SourceConstantAlpha = (byte)_opacity;

				// Retrieving desktop DC
				DesktopDeviceContext = NativeMethods.GetDC(IntPtr.Zero);

				// At the end of each side, NativeMethods.UpdateLayeredWindow will be called
				_left.PaintLeft(_theme.Metrics.Left.Width, cy, _fullRedrawing);
				_top.PaintTop(cx, _theme.Metrics.Top.Height, _fullRedrawing);
				_right.PaintRight(_theme.Metrics.Right.Width, cy, _fullRedrawing);
				_bottom.PaintBottom(cx, _theme.Metrics.Bottom.Height, _fullRedrawing);

				// Since we don't need desktop dc elsewere, releasing it
				NativeMethods.ReleaseDC(IntPtr.Zero, DesktopDeviceContext);

				// Since we're done, setting value to false
				_fullRedrawing = false;

			}

			// Change location and visibility
			if (changeVisibility || !updateLayeredWindows || partialChange) {

				// Common for each side flags. Without changing focus, without changing size.
				var swpFlags = SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSIZE
					| (_targetForm.Visible ? SetWindowPosFlags.SWP_SHOWWINDOW : SetWindowPosFlags.SWP_HIDEWINDOW);

				// Nothing special, just changing position of each side according to base location, but ignoring size
				NativeMethods.SetWindowPos(_left.Handle, _targetForm.Handle, _left.X, _left.Y, 0, 0, swpFlags);
				NativeMethods.SetWindowPos(_top.Handle, _left.Handle, _top.X, _top.Y, 0, 0, swpFlags);
				NativeMethods.SetWindowPos(_right.Handle, _top.Handle, _right.X, _right.Y, 0, 0, swpFlags);
				NativeMethods.SetWindowPos(_bottom.Handle, _right.Handle, _bottom.X, _bottom.Y, 0, 0, swpFlags);

			}

			// ...
			_previousSize = new Size(cx, cy);

		}




		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		/// <param name="supressFinalize">When set to true, call for finalizer for specified object won't be called.</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		private void Dispose(bool supressFinalize) {

			// ...
			if (_isDisposed) return;
			_isDisposed = true;

			// ...
			_left.Dispose();
			_top.Dispose();
			_right.Dispose();
			_bottom.Dispose();

			// ...
			_theme = null;
			_targetForm = null;

			if (supressFinalize) GC.SuppressFinalize(this);

		}




		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Dispose() {
			IsSuspended = true;
			Dispose(true);
		}




		/// <summary>Finalizes an instance of the <see cref="Window"/> class.</summary>
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		~Window() {
			IsSuspended = true;
			Dispose(false);
		}




	}




}
