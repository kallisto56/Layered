namespace Layered.Native {

	internal enum WindowLongFlags {

		/// <summary><see cref="Layered.Native.WindowStyles"/>.</summary>
		GWL_STYLE = -16,

		/// <summary><see cref="Layered.Native.ExtendedWindowStyles"/>.</summary>
		GWL_EXSTYLE = -20,

		/// <summary>Application instance handle.</summary>
		GWLP_HINSTANCE = -6,

		/// <summary>Handle to the parent window.</summary>
		GWLP_HWNDPARENT = -8,

		/// <summary>Identifier of the child window. The window cannot be a top-level window.</summary>
		GWL_ID = -12,

		/// <summary>User data associated with the window. This data is intended for use by the application that created the window. Its value is initially zero.</summary>
		GWL_USERDATA = -21,

		/// <summary>Addres for the window procedure. You cannot change this attribute if the window does not belong to the same process as the calling thread.</summary>
		GWL_WNDPROC = -4,

		/// <summary>Extra information private to the application, such as handles or pointers.</summary>
		DWLP_USER = 0x8,

		/// <summary>The return value of a message processed in the dialog box procedure.</summary>
		DWLP_MSGRESULT = 0x0,

		/// <summary>The address of the dialog box procedure, or a handle representing the address of the dialog box procedure. You must use the CallWindowProc function to call the dialog box procedure.</summary>
		DWLP_DLGPROC = 0x4

	}

}
