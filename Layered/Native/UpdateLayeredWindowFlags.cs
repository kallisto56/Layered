namespace Layered.Native {

	using System;

	[Flags]
	internal enum UpdateLayeredWindowFlags {

		/// <summary>Use pblend as the blend function. If the display mode is 256 colors or less, the effect of this value is the same as the effect of ULW_OPAQUE.</summary>
		ULW_ALPHA = 0x00000002,

		/// <summary>Use crKey as the transparency color.</summary>
		ULW_COLORKEY = 0x00000001,

		/// <summary>Draw an opaque layered window.</summary>
		ULW_OPAQUE = 0x00000004
	}

}
