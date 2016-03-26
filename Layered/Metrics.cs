namespace Layered {

	using System.Drawing;




	/// <summary>Represents a class, that contains information about position and size of each corner and side of <see cref="System.Drawing.Bitmap"/></summary>
	public sealed class Metrics {

		internal readonly Rectangle Left, Top, Right, Bottom;
		internal readonly Corners Vertical, Horizontal;

		internal class Corners {
			internal Rectangle TopLeft, TopRight, BottomLeft, BottomRight;
		}




		/// <summary>Initializes a new instance of the <see cref="Metrics"/> class.</summary>
		/// <param name="bitmapSize">The <see cref="System.Drawing.Size"/> of the <see cref="System.Drawing.Bitmap"/></param>
		/// <param name="innerRect"><see cref="System.Drawing.Rectangle"/> that contains information about position and size of inner corners.</param>
		public Metrics(Size bitmapSize, Rectangle innerRect) {

			// From one rectangle, we're creating 12, each of them represents
			// corner (vertical/horizontal) or side. Yes, we could calculate
			// them later, right before updating layered windows, but, then
			// it would becomes a mess. Also, instead of creating 12 rectangles
			// once, we would create them each update, so it's an issue.

			// Calculating Metrics
			Left = new Rectangle(new Point(0, innerRect.Top), new Size(innerRect.Left, innerRect.Bottom - innerRect.Top));
			Top = new Rectangle(new Point(innerRect.Left, 0), new Size(innerRect.Right - innerRect.Left, innerRect.Top));
			Right = new Rectangle(new Point(innerRect.Right, innerRect.Top), new Size(bitmapSize.Width - innerRect.Right, innerRect.Bottom - innerRect.Top));
			Bottom = new Rectangle(new Point(innerRect.Left, innerRect.Bottom), new Size(innerRect.Right - innerRect.Left, bitmapSize.Height - innerRect.Bottom));

			// Corners for vertical sides (left, right)
			Vertical = new Corners {
				TopLeft = new Rectangle(new Point(0, 0), new Size(innerRect.Left, innerRect.Top)),
				TopRight = new Rectangle(new Point(innerRect.Right, 0), new Size(bitmapSize.Width - innerRect.Right, innerRect.Top)),
				BottomLeft = new Rectangle(new Point(0, innerRect.Bottom), new Size(innerRect.Left, bitmapSize.Height - innerRect.Bottom)),
				BottomRight = new Rectangle(new Point(innerRect.Right, innerRect.Bottom), new Size(Right.Width, Bottom.Height)),
			};

			// Corners for horizontal sides (top, bottom)
			Horizontal = new Corners {
				TopLeft = new Rectangle(new Point(innerRect.Left, innerRect.Top), new Size(innerRect.Left, innerRect.Top)),
				TopRight = new Rectangle(new Point(innerRect.Right - Vertical.TopRight.Width, innerRect.Top), Vertical.TopRight.Size),
				BottomLeft = new Rectangle(new Point(innerRect.Left, innerRect.Top * 2), new Size(innerRect.Left, innerRect.Bottom - innerRect.Top * 2)),
				BottomRight = new Rectangle(new Point(Right.X - Right.Width, Bottom.Y - Bottom.Height), new Size(Right.Width, Bottom.Height)),
			};

		}




	}




}
