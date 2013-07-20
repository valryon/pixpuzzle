using System;
using System.Linq;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace PixPuzzle
{
	public class ImageFilters
	{
		public struct PaletteTempItem
		{
			public int Count;
			public int R;
			public int G;
			public int B;
		}

		public static int PaletteColorNumbers = 32;
		/// <summary>
		/// Threshold to determine if colors are neighbor or not after adding each components
		/// </summary>
		public static float PaletteColorDifferenceThreshold = 50;

		public static UIImage Filter (UIImage img, int puzzleSize)
		{
			if (puzzleSize < 16)
				puzzleSize = 16;

			int tileSize = puzzleSize / 16;

			// 1/ Get the main colors
			// So we have a color palette
			var colorPalette = getColorPalette (img);

			// 1/ Resize & Load image as readable
			UIImage resizedImg = ResizeRatio (img, puzzleSize);
			Bitmap bitmap = new Bitmap (resizedImg);

			// 2/ Apply mosaic
			var flippedImage = applyMosaic (tileSize, colorPalette, resizedImg, bitmap);

			// -- Flip because bitmap has inverted coordinates
//			UIImage finalImg = new UIImage (flippedImage, 0f, UIImageOrientation.DownMirrored); 
			UIImage finalImg = new UIImage (flippedImage);

			// -- Resize the final
//			return ResizeRatio (finalImg, FinalSize);
			return finalImg;
		}

		private static List<Color> getColorPalette (UIImage img)
		{
			// -- Make a thumbnail
			UIImage thumb = ResizeRatio (img, 64);
			Bitmap thumbBitmap = new Bitmap (thumb);

			// -- Get each colors
			Dictionary<Color, PaletteTempItem> colorList = new Dictionary<Color, PaletteTempItem> ();
			for (int xq = 0; xq < thumb.Size.Width; xq++) {
				for (int yq = 0; yq < thumb.Size.Height; yq++) {
					Color c = thumbBitmap.GetPixel (xq, yq);
					// Look if we have a similar color already in the palette
					bool hasBeenAdded = false;
					foreach (Color cl in colorList.Keys) {
						double distance = Math.Abs (Math.Pow (c.R - cl.R, 2) + Math.Pow (c.G - cl.G, 2) + Math.Pow (c.B - cl.B, 2));
						if (distance < PaletteColorDifferenceThreshold) {
							hasBeenAdded = true;
							// Yes!
							c = cl;
							PaletteTempItem p = colorList [c];
							p.Count += 1;
							p.R += cl.R;
							p.G += cl.G;
							p.B += cl.B;
							colorList [c] = p;
							break;
						}
					}
					// foreach
					if (hasBeenAdded == false) {
						colorList.Add (c, new PaletteTempItem () {
							R = c.R,
							G = c.G,
							B = c.B,
							Count = 1
						});
					}
				} // for y
			} // for x

			// -- Select the n most frequent colors
			List<Color> colorPalette = new List<Color> ();
			foreach (PaletteTempItem t in colorList.OrderByDescending (t => t.Value.Count).Take (PaletteColorNumbers).Select (t => t.Value)) {
				Color c = Color.FromArgb (t.R / t.Count, t.G / t.Count, t.B / t.Count);
				Console.WriteLine ("Palette color usage count: " + t.Count);
				colorPalette.Add (c);
			}

			return colorPalette;
		}

		static CGImage applyMosaic (int tileSize, List<Color> colorPalette, UIImage resizedImg, Bitmap bitmap)
		{
			// - Parameters
			int width = tileSize; // tile width
			int height = tileSize; // tile height
			int outWidth = (int)(resizedImg.Size.Width - (resizedImg.Size.Width % width)); // Round image size
			int outHeight = (int)(resizedImg.Size.Height - (resizedImg.Size.Height % height));

			// -- Initialize buffer
			CGBitmapContext context = new CGBitmapContext (System.IntPtr.Zero, // data
			                                               (int)outWidth, // width
			                                               (int)outHeight, // height
			                                               8, // bitsPerComponent
			                                               outWidth * 4, // bytesPerRow based on pixel width
			                                               CGColorSpace.CreateDeviceRGB (), // colorSpace
			                                               CGImageAlphaInfo.NoneSkipFirst);

			// bitmapInfo
			for (int yb = 0; yb < outHeight / height; yb++) {
				for (int xb = 0; xb < outWidth / width; xb++) {

					// -- Do the average colors on the source image for the
					// corresponding mosaic square
					int r_avg = 0;
					int g_avg = 0;
					int b_avg = 0;
					for (int y = yb * height; y < (yb * height) + height; y++) {
						for (int x = xb * width; x < (xb * width) + width; x++) {
							Color c = bitmap.GetPixel (x, y);
							// Retrieve color values of the source image
							r_avg += c.R;
							g_avg += c.G;
							b_avg += c.B;
						}
					}

					// Make average of R,G and B on filter size
					r_avg = r_avg / (width * height);
					g_avg = g_avg / (width * height);
					b_avg = b_avg / (width * height);

					// Find the nearest color in the palette
					Color mosaicColor = new Color ();
					double minDistance = int.MaxValue;

					foreach (Color c in colorPalette) {
						double distance = Math.Abs (Math.Pow (r_avg - c.R, 2) + Math.Pow (g_avg - c.G, 2) + Math.Pow (b_avg - c.B, 2));
						if (distance < minDistance) {
							mosaicColor = c;
							minDistance = distance;
						}
					}

					// Apply mosaic
					for (int y = 0; y < height; y++) {
						for (int x = 0; x < width; x++) {
							context.SetFillColor (new CGColor (mosaicColor.R / 255f, mosaicColor.G / 255f, mosaicColor.B / 255f));
							context.FillRect (new RectangleF (xb * width, yb * height, width, height));
						}
					}
				}
			}

			//-- image from buffer
			CGImage flippedImage = context.ToImage ();
			context.Dispose ();

			return flippedImage;
		}

		static UIImage ResizeRatio (UIImage img, int size)
		{
			float ratio = img.Size.Width / img.Size.Height;
			SizeF newSize = new SizeF (size * ratio, size);
			return UIImageEx.Scale (img, newSize);
		}

		static UIImage Resize (UIImage img, float width, float height)
		{
			SizeF newSize = new SizeF (width, height);
			return UIImageEx.Scale (img, newSize);
		}
	}
}

