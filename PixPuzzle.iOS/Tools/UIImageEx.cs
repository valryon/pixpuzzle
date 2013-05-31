using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace MonoTouch.UIKit
{
	public static class UIImageEx
	{
		public static UIImage Scale (this UIImage source, SizeF newSize)
		{
			UIGraphics.BeginImageContext (newSize);
			var context = UIGraphics.GetCurrentContext ();
			context.TranslateCTM (0, newSize.Height);
			context.ScaleCTM (1f, -1f);

			context.DrawImage (new RectangleF (0, 0, newSize.Width, newSize.Height), source.CGImage);

			var scaledImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return scaledImage;         
		}


		public static UIImage GetImageWithOverlayColor(this UIImage self, UIColor color)
		{        
			RectangleF rect = new RectangleF(0.0f, 0.0f, self.Size.Width, self.Size.Height);

			UIGraphics.BeginImageContextWithOptions(self.Size, false, self.CurrentScale);

			self.DrawAsPatternInRect (rect);

			CGContext context = UIGraphics.GetCurrentContext();
			context.SetBlendMode (CGBlendMode.SourceIn);

			context.SetFillColor(color.CGColor);
			context.FillRect(rect);

			UIImage image = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return image;
		}
	}
}

