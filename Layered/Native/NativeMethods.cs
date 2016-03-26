namespace Layered.Native {

	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	internal static class NativeMethods {




		/// <summary>
		/// The CreateCompatibleDC function creates a memory device context (DC) compatible with the specified device.
		/// </summary>
		/// <param name="hDc">A handle to an existing DC. If this handle is NULL, the function creates a memory DC compatible with the application's current screen.</param>
		/// <returns>If the function succeeds, the return value is the handle to a memory DC. If the function fails, the return value is NULL.</returns>
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr CreateCompatibleDC(IntPtr hDc);




		/// <summary>
		/// The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen. You can use the returned handle in subsequent GDI functions to draw in the DC. The device context is an opaque data structure, whose values are used internally by GDI.
		/// </summary>
		/// <param name="hWnd">A handle to the window whose DC is to be retrieved. If this value is NULL, GetDC retrieves the DC for the entire screen.</param>
		/// <returns>If the function succeeds, the return value is a handle to the DC for the specified window's client area. If the function fails, the return value is NULL.</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr GetDC(IntPtr hWnd);




		/// <summary>
		/// The SelectObject function selects an object into the specified device context(DC). The new object replaces the previous object of the same type.
		/// </summary>
		/// <param name="hDc">A handle to the DC.</param>
		/// <param name="hObject">A handle to the object to be selected.</param>
		/// <returns>If the selected object is not a region and the function succeeds, the return value is a handle to the object being replaced. If the selected object is a region and the function succeeds, the return value is one of the following values: <para>SIMPLEREGION: Region consists of a single rectangle.</para><para>COMPLEXREGION: Region consists of more than one rectangle.</para><para>NULLREGION: Region is empty.</para></returns>
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SelectObject(IntPtr hDc, IntPtr hObject);




		/// <summary>
		/// The ReleaseDC function releases a device context (DC), freeing it for use by other applications. The effect of the ReleaseDC function depends on the type of DC. It frees only common and window DCs. It has no effect on class or private DCs.
		/// </summary>
		/// <param name="hWnd">A handle to the window whose DC is to be released.</param>
		/// <param name="hDc">A handle to the DC to be released.</param>
		/// <returns>The return value indicates whether the DC was released. If the DC was released, the return value is 1. If the DC was not released, the return value is zero.</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);




		/// <summary>
		/// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object. After the object is deleted, the specified handle is no longer valid.
		/// </summary>
		/// <param name="hObject">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
		/// <returns>If the function succeeds, the return value is nonzero. If the specified handle is not valid or is currently selected into a DC, the return value is zero.</returns>
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteObject(IntPtr hObject);




		/// <summary>
		/// The DeleteDC function deletes the specified device context (DC).
		/// </summary>
		/// <param name="hdc">A handle to the device context.</param>
		/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DeleteDC(IntPtr hdc);




		/// <summary>
		/// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
		/// </summary>
		/// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs..</param>
		/// <param name="nIndex">The zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer. To set any other value, specify one of the following values: GWL_EXSTYLE, GWL_HINSTANCE, GWL_ID, GWL_STYLE, GWL_USERDATA, GWL_WNDPROC </param>
		/// <param name="dwNewLong">The replacement value.</param>
		/// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer. 
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError. </returns>
		[DllImport("user32.dll")]
		internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);




		/// <summary>
		/// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
		/// </summary>
		/// <param name="hWnd">A handle to the window.</param>
		/// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order. This parameter must be a window handle or one of the following values: <para>HWND_BOTTOM: Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.</para><para>HWND_NOTOPMOST: Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.</para><para>HWND_TOP: Places the window at the top of the Z order.</para><para>HWND_TOPMOST: Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.</para></param>
		/// <param name="x">The new position of the left side of the window, in client coordinates.</param>
		/// <param name="y">The new position of the top of the window, in client coordinates.</param>
		/// <param name="cx">The new width of the window, in pixels.</param>
		/// <param name="cy">The new height of the window, in pixels.</param>
		/// <param name="uFlags">The window sizing and positioning flags. This parameter can be a combination of the values specified in SetWindowPosFlags enumerator.</param>
		/// <returns></returns>
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);




		/// <summary>
		/// The BitBlt function performs a bit-block transfer of the color data corresponding to a rectangle of pixels from the specified source device context into a destination device context.
		/// </summary>
		/// <param name="hdc">A handle to the destination device context.</param>
		/// <param name="nXDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
		/// <param name="nYDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
		/// <param name="nWidth">The width, in logical units, of the source and destination rectangles.</param>
		/// <param name="nHeight">The height, in logical units, of the source and the destination rectangles.</param>
		/// <param name="hdcSrc">A handle to the source device context.</param>
		/// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
		/// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
		/// <param name="dwRop">A raster-operation code. These codes define how the color data for the source rectangle is to be combined with the color data for the destination rectangle to achieve the final color.</param>
		/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call <code>GetLastError</code>.</returns>
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
			int nXSrc, int nYSrc, CopyPixelOperation dwRop);




		/// <summary>
		/// The StretchBlt function copies a bitmap from a source rectangle into a destination rectangle, stretching or compressing the bitmap to fit the dimensions of the destination rectangle, if necessary. The system stretches or compresses the bitmap according to the stretching mode currently set in the destination device context.
		/// </summary>
		/// <param name="hdcDest">A handle to the destination device context.</param>
		/// <param name="nXOriginDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
		/// <param name="nYOriginDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
		/// <param name="nWidthDest">The width, in logical units, of the destination rectangle.</param>
		/// <param name="nHeightDest">The height, in logical units, of the destination rectangle.</param>
		/// <param name="hdcSrc">A handle to the source device context.</param>
		/// <param name="nXOriginSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
		/// <param name="nYOriginSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
		/// <param name="nWidthSrc">The width, in logical units, of the source rectangle.</param>
		/// <param name="nHeightSrc">The height, in logical units, of the source rectangle.</param>
		/// <param name="dwRop">The raster operation to be performed. Raster operation codes define how the system combines colors in output operations that involve a brush, a source bitmap, and a destination bitmap.</param>
		/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
		[DllImport("gdi32.dll")]
		internal static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
			int nWidthDest, int nHeightDest,
			IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
			CopyPixelOperation dwRop);




		/// <summary>
		/// Updates the position, size, shape, content, and translucency of a layered window.
		/// </summary>
		/// <param name="hwnd">A handle to a layered window. A layered window is created by specifying WS_EX_LAYERED when creating the window with the CreateWindowEx function. Windows 8: The WS_EX_LAYERED style is supported for top-level windows and child windows. Previous Windows versions support WS_EX_LAYERED only for top-level windows.</param>
		/// <param name="hdcDst">A handle to a DC for the screen. This handle is obtained by specifying NULL when calling the function. It is used for palette color matching when the window contents are updated. If hdcDst isNULL, the default palette will be used. If hdcSrc is NULL, hdcDst must be NULL.</param>
		/// <param name="pptDst">A pointer to a structure that specifies the new screen position of the layered window. If the current position is not changing, pptDst can be NULL.</param>
		/// <param name="psize">A pointer to a structure that specifies the new size of the layered window. If the size of the window is not changing, psize can be NULL. If hdcSrc is NULL, psize must be NULL.</param>
		/// <param name="hdcSrc">A handle to a DC for the surface that defines the layered window. This handle can be obtained by calling the CreateCompatibleDC function. If the shape and visual context of the window are not changing, hdcSrc can be NULL.</param>
		/// <param name="pptSrc">A pointer to a structure that specifies the location of the layer in the device context. If hdcSrc is NULL, pptSrc should be NULL.</param>
		/// <param name="crKey">A structure that specifies the color key to be used when composing the layered window. To generate a COLORREF, use the RGB macro.</param>
		/// <param name="pblend">A pointer to a structure that specifies the transparency value to be used when composing the layered window.</param>
		/// <param name="dwFlags">This parameter can be one of the following values.<para>ULW_ALPHA: Use pblend as the blend function. If the display mode is 256 colors or less, the effect of this value is the same as the effect of ULW_OPAQUE.</para><para>ULW_COLORKEY: Use crKey as the transparency color.</para><para>ULW_OPAQUE: Draw an opaque layered window.</para></param>
		/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst,
			ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc,
			Int32 crKey, ref BlendFunction pblend, UpdateLayeredWindowFlags dwFlags);




	}

}
