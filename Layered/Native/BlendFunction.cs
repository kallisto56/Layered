namespace Layered.Native {

	using System;
	using System.Runtime.InteropServices;

	[Flags]
	internal enum AlphaFormatFlags : byte {

		/// <summary>When the BlendOp member in BLENDFUNCTION is AC_SRC_OVER, the source bitmap is placed over the destination bitmap based on the alpha values of the source pixels.</summary>
		AC_SRC_OVER = 0x0,

		/// <summary>This flag is set when the bitmap has an Alpha channel (that is, per-pixel alpha). Note that the APIs use premultiplied alpha, which means that the red, green and blue channel values in the bitmap must be premultiplied with the alpha channel value. For example, if the alpha channel value is x, the red, green and blue channels must be multiplied by x and divided by 0xff prior to the call.</summary>
		AC_SRC_ALPHA = 0x1

	}

	/// <summary>The BlendFunction structure controls blending by specifying the blending functions for source and destination bitmaps.</summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct BlendFunction {

		/// <summary>The source blend operation. Currently, the only source and destination blend operation that has been defined is AC_SRC_OVER. For details, see the following Remarks section.</summary>
		internal readonly byte BlendOperation;

		/// <summary>Accordint to MSDN documentation - Must be zero.</summary>
		internal readonly byte BlendFlags;

		/// <summary>Specifies an alpha transparency value to be used on the entire source bitmap. The SourceConstantAlpha value is combined with any per-pixel alpha values in the source bitmap. If you set SourceConstantAlpha to 0, it is assumed that your image is transparent. Set the SourceConstantAlpha value to 255 (opaque) when you only want to use per-pixel alpha values.</summary>
		internal byte SourceConstantAlpha;

		/// <summary>This member controls the way the source and destination bitmaps are interpreted. AlphaFormat has the following value.</summary>
		internal readonly byte AlphaFormat;

		/// <summary>The BlendFunction structure controls blending by specifying the blending functions for source and destination bitmaps.</summary>
		/// <param name="sourceConstantAlpha">Specifies an alpha transparency value to be used on the entire source bitmap. The SourceConstantAlpha value is combined with any per-pixel alpha values in the source bitmap. If you set SourceConstantAlpha to 0, it is assumed that your image is transparent. Set the SourceConstantAlpha value to 255 (opaque) when you only want to use per-pixel alpha values.</param>
		private BlendFunction(byte sourceConstantAlpha = 255) {
			BlendOperation = (byte)AlphaFormatFlags.AC_SRC_OVER;
			BlendFlags = byte.MinValue;
			SourceConstantAlpha = sourceConstantAlpha;
			AlphaFormat = (byte)AlphaFormatFlags.AC_SRC_ALPHA;
		}

		/// <summary>Static default value of BlendFunction, that used by NativeMethods.UpdateLayeredWindow</summary>
		public static readonly BlendFunction Default = new BlendFunction(255);

	}

}