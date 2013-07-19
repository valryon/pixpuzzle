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
		public static UIImage Filter (UIImage img)
		{
			// 0/ Get the main colors
			// So we have a color palette
			// -- Make a thumbnail
			UIImage thumb = ResizeRatio (img, 64);
			Bitmap thumbBitmap = new Bitmap (thumb);

			// -- Get each colors
			float threshold = 25;
			Dictionary<Color, int> colorList = new Dictionary<Color, int> ();
			for (int xq = 0; xq < thumb.Size.Width; xq++) {
				for (int yq = 0; yq < thumb.Size.Height; yq++) {
					Color c = thumbBitmap.GetPixel (xq, yq);

					// Look if we have a similar color already in the palette
					foreach(Color cl in colorList.Keys) {
						double distance = Math.Abs (
							Math.Pow(c.R - cl.R, 2) +
							Math.Pow(c.G - cl.G, 2) +
							Math.Pow(c.B - cl.B, 2)
							);

						if (distance > threshold) {
							colorList.Add (c, 1);
						}
						else {
							// Yes!
							c = cl;
							colorList [c] += 1;
							break;
						}
					}
				}
			}

			// -- Select the n most frequent colors
			int maxColours = 64;
			List<Color> colorPalette = colorList.OrderByDescending (t => t.Value).Select (t => t.Key).Take(maxColours).ToList ();

			// 1/ Resize & Load image as readable
			UIImage resizedImg = ResizeRatio (img, 512);
			Bitmap bitmap = new Bitmap (resizedImg);

			// 2/ Apply mosaic
			// - Parameters
			int width = 16; // tile width
			int height = 16; // tile height
			int outWidth = (int)(resizedImg.Size.Width - (resizedImg.Size.Width % width)); // Round image size
			int outHeight = (int)(resizedImg.Size.Height - (resizedImg.Size.Height % height));

			// -- Initialize buffer
			CGBitmapContext context = new CGBitmapContext (
				System.IntPtr.Zero, // data
				(int)outWidth,              // width
				(int)outHeight,             // height
				8,                          // bitsPerComponent
				outWidth * 4,          // bytesPerRow based on pixel width
				CGColorSpace.CreateDeviceRGB (), // colorSpace
				CGImageAlphaInfo.NoneSkipFirst);// bitmapInfo

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
					Color mosaicColor = new Color();
					double minDistance = int.MaxValue;

					foreach (Color c in colorPalette) {
						double distance = Math.Abs (
							Math.Pow(r_avg - c.R, 2) +
							Math.Pow(g_avg - c.G, 2) +
							Math.Pow(b_avg - c.B, 2)
							);

						if (distance < minDistance) {
							mosaicColor = c;
							minDistance = distance;
						}
					}

					// Apply mosaic
					for (int y = 0; y < height; y++) {
						for (int x = 0; x < width; x++) {

							context.SetFillColor (new CGColor (mosaicColor.R / 255f, mosaicColor.G / 255f, mosaicColor.B / 255f));
							context.FillRect (new RectangleF (
								xb * width,
								yb * height,
								width,
								height
								)
							                  );
						}
					}
				}
			}


			//-- image from buffer
			CGImage flippedImage = context.ToImage ();
			context.Dispose ();

			// -- Flip because bitmap has inverted coordinates
			//			UIImage finalImg = new UIImage (flippedImage, 0f, UIImageOrientation.DownMirrored); 
			UIImage finalImg = new UIImage (flippedImage);

			// -- Resize the final
			return ResizeRatio (finalImg, 128);
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

