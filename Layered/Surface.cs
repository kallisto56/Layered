namespace Layered {

	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;
	using System.Security.Permissions;

	using Native;




	/// <summary>Device context, compatible for work with layered windows.</summary>
	/// <seealso cref="System.IDisposable" />
	public class Surface : IDisposable {

		private bool _isDisposed;

		/// <summary>Gets the width of surface</summary>
		public int Width { get; private set; }

		/// <summary>Gets the height of surface</summary>
		public int Height { get; private set; }

		internal IntPtr Placeholder { get; private set; }
		internal IntPtr DeviceContext { get; private set; }
		internal IntPtr Object { get; private set; }

		/// <summary>Returns true, if device context created, otherwise false.</summary>
		public bool IsDeviceContextCreated => DeviceContext != IntPtr.Zero;

		/// <summary>Returns true, if object selected into device context, otherwise false.</summary>
		public bool IsObjectSelected => Placeholder != IntPtr.Zero;
		
		/// <summary>Returns true, if object (GDI Bitmap) created, otherwise false.</summary>
		public bool IsObjectCreated => Object != IntPtr.Zero;




		/// <summary>Creates the device context.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void CreateDeviceContext() {

			// Throw an exception, if device context already created
			if (IsDeviceContextCreated) throw new Exception("Attempt to create device context, when previous DC is not deleted");

			// Retrieving desktop DC in order to creating compatible DC
			var desktopDeviceContext = NativeMethods.GetDC(IntPtr.Zero);
			DeviceContext = NativeMethods.CreateCompatibleDC(desktopDeviceContext);

			// Releasing desktop DC, before checking anything
			NativeMethods.ReleaseDC(IntPtr.Zero, desktopDeviceContext);

			// Throw an exception, if NativeMethods.CreateCompatibleDC failed
			if (DeviceContext == IntPtr.Zero) throw new Exception("Layered.Internal.Surface -> NativeMethods.CreateCompatibleDC() failed.");

		}




		/// <summary>Deletes the device context.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void DeleteDeviceContext() {

			// Throw an exception, if device context not created
			if (!IsDeviceContextCreated) throw new Exception("Attempt to delete device context, when device context not created or already deleted.");

			// Throw an exception, if we have an object, selected inside device context
			if (IsObjectSelected) throw new Exception("Attempt to delete device context, that holding object inside.");

			// Throw an exception, if NativeMethods.DeleteDC failed
			if (!NativeMethods.DeleteDC(DeviceContext)) throw new Exception("Layered.Internal.Surface -> NativeMethods.DeleteDC() failed.");

			// Value of 'IsDeviceContextCreated' depends on value of 'DeviceContext'
			DeviceContext = IntPtr.Zero;

		}




		/// <summary>Creates the surface.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void CreateSurface(int width, int height, PixelFormat format) {

			// Throw an exception, if we already have one surface
			if (IsObjectCreated) throw new Exception("Attempt to create surface, without disposing from previously created surface.");

			// Throw an exception, if width or height equal or less than zero
			if (width <= 0 || height <= 0) {
				throw new Exception($"Attempt to create surafece with negative size (Width: {width}; Height:{height};)");
            }

			// Using System.Drawing.Bitmap to create GDI Bitmap
			using (var bitmap = new Bitmap(width, height, format)) {
				Object = bitmap.GetHbitmap(Color.FromArgb(0));
			}

			// ...
			Width = width;
			Height = height;

			// Throw an exception, if something went wrong in System.Drawing.Bitmap.GetHbitmap
			if (Object == IntPtr.Zero) throw new Exception("Layered.Internal.Surface -> System.Drawing.Bitmap.GetHbitmap() failed.");

		}




		/// <summary>Creates GDI Bitmap, that can be selected into device context.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected void CreateSurface(Bitmap bitmap) {

			// Throw an exception, if we already have one surface
			if (IsObjectCreated) throw new Exception("Attempt to create surface, without disposing from previously created surface.");

			// Using System.Drawing.Bitmap to create GDI Bitmap
			Object = bitmap.GetHbitmap(Color.FromArgb(0));

			// ...
			Width = bitmap.Width;
			Height = bitmap.Height;

			// Throw an exception, if something went wrong in System.Drawing.Bitmap.GetHbitmap
			if (Object == IntPtr.Zero) throw new Exception("Layered.Internal.Surface -> System.Drawing.Bitmap.GetHbitmap() failed.");

		}




		/// <summary>Deletes the surface (GDI Bitmap).</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void DeleteSurface() {

			// Throw an exception, if surface already deleted
			if (!IsObjectCreated) throw new Exception("Attempt to delete surface, when surface not created or already deleted");

			// Throw an exception, if surface selected
			if (IsObjectSelected) throw new Exception("Attempt to delete surface, selected into device context");

			// Throw an exception, if NativeMethods.DeleteObject failed
			if (!NativeMethods.DeleteObject(Object)) {
				throw new Exception("Layered.Internal.Surface -> NativeMethods.DeleteObject() failed.",
					new Win32Exception(Marshal.GetLastWin32Error()));
			}

			// ...
			Width = Height = 0;
			
			// Value of 'IsObjectCreated' depends on value of 'Object'
			Object = IntPtr.Zero;

		}




		/// <summary>Selects the surface (GDI Bitmap) into device context.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void SelectSurface() {

			// Throw an exception, if device context not created
			if (!IsDeviceContextCreated) throw new Exception("Attempt to select surface into device context, when device context not created.");

			// Throw an exception, if surface not created
			if (!IsObjectCreated) throw new Exception("Attempt to select surface into device context, when surface is not created.");

			// Throw and exception, if surface already selected
			if (IsObjectSelected) throw new Exception("Attempt to select surface into device context, when surface already selected.");

			// Selecting object, taking in return placeholder - monochrome one pixel wide and high surface
			Placeholder = NativeMethods.SelectObject(DeviceContext, Object);

			// Throw an exception, if NativeMethods.SelectObject failed
			if (Placeholder == IntPtr.Zero) throw new Exception("Layered.Internal.Surface -> NativeMethods.SelectObject() failed.");

		}




		/// <summary>Deselects the surface (GDI Bitmap) from device context.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void DeselectSurface() {

			// Throw an exception, if device context no created
			if (!IsDeviceContextCreated) throw new Exception("Attempt to deselect surface, when device context is not created.");

			// Throw an exception, if surface is not created
			if (!IsObjectCreated) throw new Exception("Attempt to deselect surface, when surface is not created.");

			// Throw an exception, if surface is not selected
			if (!IsObjectSelected) throw new Exception("Attempt to deselect surface, when surface is not selected into device context.");

			// Selecting placeholder, in order to take surface back
			Object = NativeMethods.SelectObject(DeviceContext, Placeholder);

			// Throw an exception, if NativeMethods.SelectObject failed
			if (Object == IntPtr.Zero) throw new Exception("Layered.Internal.Surface -> NativeMethods.SelectObject() failed.");

			// Value of 'IsObjectSelected' depends on value of 'Placeholder'
			Placeholder = IntPtr.Zero;

		}




		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected virtual void Dispose(bool disposing) {

			// ...
			if (_isDisposed) return;

			// Deselecting & deleting surface, if there is one
			if (IsObjectSelected) DeselectSurface();
			if (IsObjectCreated) DeleteSurface();

			// Last thing, deleting device context
			if (IsDeviceContextCreated) DeleteDeviceContext();

			// ...
			_isDisposed = true;

		}




		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}




		/// <summary>Finalizes an instance of the <see cref="Surface"/> class.</summary>
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		~Surface() {
			Dispose(true);
		}




	}




}
