using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.CoreImage;

namespace MonoTouch.UIKit
{
	public static class UIImageEx
	{
		public static UIImage Scale (this UIImage source, SizeF newSize)
		{
			UIGraphics.BeginImageContext (newSize);
//			var context = UIGraphics.GetCurrentContext ();
			//			context.DrawImage (new RectangleF (0, 0, newSize.Width, newSize.Height), source.CGImage);
			source.Draw(new RectangleF(0, 0, newSize.Width, newSize.Height));
			var scaledImage = UIGraphics.GetImageFromCurrentImageContext ();

			UIGraphics.EndImageContext ();

			return scaledImage;         
		}

		public static UIImage ResizeRatio (UIImage img, int size)
		{
			float ratio;
			SizeF newSize;

			if (img.Size.Width > img.Size.Height) {
				ratio = img.Size.Height / img.Size.Width;
				newSize = new SizeF (size, size * ratio);
			} else {
				ratio = img.Size.Width / img.Size.Height;
				newSize = new SizeF (size * ratio, size);
			}

			return UIImageEx.Scale (img, newSize);
		}

		public static UIImage Resize (UIImage img, float width, float height)
		{
			SizeF newSize = new SizeF (width, height);
			return UIImageEx.Scale (img, newSize);
		}

		public static UIImage GetImageWithOverlayColor (this UIImage self, UIColor color)
		{        
			RectangleF rect = new RectangleF (0.0f, 0.0f, self.Size.Width, self.Size.Height);

			UIGraphics.BeginImageContextWithOptions (self.Size, false, self.CurrentScale);

			self.DrawAsPatternInRect (rect);

			CGContext context = UIGraphics.GetCurrentContext ();
			context.SetBlendMode (CGBlendMode.SourceIn);

			context.SetFillColor (color.CGColor);
			context.FillRect (rect);

			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return image;
		}

		public static UIImage AdjustBrightnessSaturationAndContrast(this UIImage image, float brightness = 0, float saturation = 1, float contrast = 1) {

			CIColorControls colorCtrls = new CIColorControls () {
				Image = CIImage.FromCGImage (image.CGImage),
			};

			// Clamp values
			saturation = Math.Min (Math.Max (0, saturation), 2);
			brightness = Math.Min (Math.Max (-1, brightness), 1);
			contrast = Math.Min (Math.Max (0, contrast), 4);

			colorCtrls.Brightness = brightness; 
			colorCtrls.Saturation = saturation; 
			colorCtrls.Contrast = contrast;


			var output = colorCtrls.OutputImage;
			var context = CIContext.FromOptions (null);
			var result = context.CreateCGImage (output, output.Extent);

			return UIImage.FromImage(result);
		}
	}
}

