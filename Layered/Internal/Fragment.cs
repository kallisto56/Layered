namespace Layered.Internal {

	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;




	/// <summary><see cref="Layered.Internal.Fragment"/> provides extended functionality for <see cref="System.Drawing.Bitmap"/>. Contains methods for trimming empty space, scaling, generating and applying gradient mask.</summary>
	/// <seealso cref="System.IDisposable" />
	internal sealed class Fragment : IDisposable {

		/// <summary><see cref="System.Drawing.Bitmap"/>, cloned from provided in constructor arguments.</summary>
		public Bitmap Bitmap;

		/// <summary><see cref="System.Drawing.Bitmap"/>, that contains gradient mask, that can be applied to cloned <see cref="System.Drawing.Bitmap"/></summary>
		public Bitmap Mask;

		/// <summary>Gets the width and height of <see cref='System.Drawing.Bitmap'/>, cloned in constructor.</summary>
		public Size Size => Bitmap.Size;

		/// <summary>Gets the width of <see cref='System.Drawing.Bitmap'/>, cloned in constructor.</summary>
		public int Width => Bitmap.Width;

		/// <summary>Gets the height of <see cref='System.Drawing.Bitmap'/>, cloned in constructor.</summary>
		public int Height => Bitmap.Height;




		/// <summary>Initializes a new instance of the <see cref="Layered.Internal.Fragment"/> class.</summary>
		/// <param name="target"><see cref="System.Drawing.Bitmap"/>, that will be cloned.</param>
		/// <param name="rectangle"><see cref="System.Drawing.Rectangle"/> of <see cref="System.Drawing.Bitmap"/>, that will be copied to new <see cref="System.Drawing.Bitmap"/>.</param>
		/// <param name="trimTargetBitmap">If set to true, cloned <see cref="System.Drawing.Bitmap"/> will be immediately trimmed.</param>
		public Fragment(Bitmap target, Rectangle rectangle, bool trimTargetBitmap = false) {

			Bitmap = target.Clone(rectangle, target.PixelFormat);
			if (trimTargetBitmap) Trim();

		}




		/// <summary>Trims empty-space of cloned <see cref="System.Drawing.Bitmap"/>.</summary>
		public void Trim() {

			// By design, provided template will have a lot of empty areas, so in order to economy memory
			// and keep up good performance while calling UpdateLayeredWindow, we're trimming that areas

			// Defining rectangle, that will be used for cloning original bitmap
			var rectangle = new Rectangle(Bitmap.Width, Bitmap.Height, 0, 0);

			// Searching for empty pixels on each side of bitmap
			for (var x = 0; x < Bitmap.Width; x++) {
				for (var y = 0; y < Bitmap.Height; y++) {

					// ...
					if (Bitmap.GetPixel(x, y).A == 0) continue;

					// ...
					rectangle.Y = Math.Min(rectangle.Y, y);
					rectangle.X = Math.Min(rectangle.X, x);
					rectangle.Width = Math.Max(rectangle.Width, x);
					rectangle.Height = Math.Max(rectangle.Height, y);

				}
			}

			// We're using rectangle as Padding (left, top, right, bottom), and
			// after calculating each side, we need to calculate actual rectangle,
			// or we will end up with strange results or worse - exception.
			rectangle.Height = rectangle.Height - rectangle.Y;
			rectangle.Width = rectangle.Width - rectangle.X;

			// Cloning, and replacing current bitmap with new one
			var result = Bitmap.Clone(rectangle, Bitmap.PixelFormat);
			Bitmap.Dispose();
			Bitmap = result;

		}




		/// <summary>Scales cloned <see cref="System.Drawing.Bitmap"/> vertically or horizontally.</summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public void Scale(int width, int height) {

			// Calculating size
			var size = new Size {
				Width = (width == 0) ? Bitmap.Width : width,
				Height = (height == 0) ? Bitmap.Height : height
			};

			// Creating bitmap with specified size, that will replace current bitmap
			var result = new Bitmap(size.Width, size.Height, Bitmap.PixelFormat);

			// Actually, we're not scaling, we're tiling bitmap.
			// Scaling, sometimes, with some bitmaps might create very visible artefacts
			using (var g = Graphics.FromImage(result)) {

				// Depending on which value was specified, applying different type of 'scaling'
				if (width != 0) {

					// Scaling width
					for (var x = 0; x < width; x += Bitmap.Width) {
						g.DrawImage(Bitmap, new Rectangle(new Point(x, 0), Bitmap.Size),
							new Rectangle(Point.Empty, Bitmap.Size), GraphicsUnit.Pixel);
					}

				} else {

					// Scaling height
					for (var y = 0; y < height; y += Bitmap.Height) {
						g.DrawImage(Bitmap, new Rectangle(new Point(0, y), Bitmap.Size),
							new Rectangle(Point.Empty, Bitmap.Size), GraphicsUnit.Pixel);
					}

				}
				
			}

		}




		/// <summary>Generates and applies gradient mask to cloned <see cref="System.Drawing.Bitmap"/>.</summary>
		/// <param name="type"><see cref="MaskType"/> specifies which type of mask to generate.</param>
		/// <param name="width">Width or height of gradient rectangle.</param>
		public void ApplyMask(MaskType type, int width = 20) {

			// First of all, generating mask
			GenerateMask(type, width);

			// Right after, applying generated mask to bitmap
			for (var x = 0; x < Bitmap.Width; x++) {
				for (var y = 0; y < Bitmap.Height; y++) {

					// Reading pixels from both, mask and target bitmaps
					var msk = Mask.GetPixel(x, y);
					var bmp = Bitmap.GetPixel(x, y);

					// Calculating alpha-value
					var alpha = (msk.A * 100) / 255;
					alpha = (bmp.A * alpha) / 100;
					alpha = Math.Min(255, Math.Max(0, alpha));

					// Setting pixel with same colors except alpha
					Bitmap.SetPixel(x, y, Color.FromArgb(alpha, bmp));

				}
			}

		}




		/// <summary>Generates gradient mask with specified type.</summary>
		/// <param name="type"><see cref="MaskType"/> specifies which type of mask to generate.</param>
		/// <param name="width">Width or height of gradient rectangle.</param>
		private void GenerateMask(MaskType type, int width = 20) {

			// Unfortunately, we have to specify different actions for each case
			switch (type) {
				
				// Top Left Corner
				case MaskType.VTopLeft:
					GenerateMask(126, 255, 0, width);
					break;
				case MaskType.HTopLeft:
					GenerateMask(126, 0, 255, width);
					break;

				// Top Right Corner
				case MaskType.VTopRight:
					GenerateMask(126, 255, 0, width);
					Mask.RotateFlip(RotateFlipType.RotateNoneFlipX);
					break;
				case MaskType.HTopRight:
					GenerateMask(126, 0, 255, width);
					Mask.RotateFlip(RotateFlipType.RotateNoneFlipX);
					break;

				// Bottom Left Corner
				case MaskType.VBottomLeft:
					GenerateMask(126, 255, 0, width);
					Mask.RotateFlip(RotateFlipType.RotateNoneFlipY);
					break;
				case MaskType.HBottomLeft:
					GenerateMask(126, 0, 255, width);
					Mask.RotateFlip(RotateFlipType.RotateNoneFlipY);
					break;

				// Bottom Right Corner
				case MaskType.VBottomRight:
					GenerateMask(126, 255, 0, width);
					Mask.RotateFlip(RotateFlipType.RotateNoneFlipXY);
					break;
				case MaskType.HBottomRight:
					GenerateMask(126, 0, 255, width);
					Mask.RotateFlip(RotateFlipType.RotateNoneFlipXY);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}

		}




		/// <summary>Generates standard gradient mask with specified alpha values for each of three colors.</summary>
		/// <param name="a">Alpha-channel value for first <see cref="System.Drawing.Color"/>.</param>
		/// <param name="b">Alpha-channel value for second <see cref="System.Drawing.Color"/>.</param>
		/// <param name="c">Alpha-channel value for third <see cref="System.Drawing.Color"/>.</param>
		/// <param name="width">Width or height of gradient rectangle.</param>
		private void GenerateMask(int a, int b, int c, int width = 20) {
			// Dispose from previously created mask, if
			Mask?.Dispose();

			// Creating new bitmap mask
			Mask = new Bitmap(Bitmap.Width, Bitmap.Height, Bitmap.PixelFormat);

			// Defining colors
			var aColor = Color.FromArgb(a, 0, 0, 0);
			var bColor = Color.FromArgb(b, 0, 0, 0);
			var cColor = Color.FromArgb(c, 0, 0, 0);

			// Defining rectangles
			var aRect = new Rectangle(0, 0, Mask.Width - width, Mask.Height - width);
			var bRect = new Rectangle(Mask.Width - width, 0, width, Mask.Height - width);
			var cRect = new Rectangle(0, Mask.Height - width, Mask.Width - width, width);

			// Drawing gradient rectangles onto mask
			using (var g = Graphics.FromImage(Mask)) {
				using (var brush = new SolidBrush(aColor)) {
					g.FillRectangle(brush, aRect);
				}
				using (var brush = new LinearGradientBrush(bRect, aColor, cColor, LinearGradientMode.Horizontal)) {
					g.FillRectangle(brush, bRect);
				}
				using (var brush = new LinearGradientBrush(cRect, aColor, bColor, LinearGradientMode.Vertical)) {
					g.FillRectangle(brush, cRect);
				}
			}
		}




		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose() {
			// ...
			Bitmap.Dispose();

			// ...
			if (Mask != null) {
				Mask.Dispose();
				Mask = null;
			}
		}




	}

}
