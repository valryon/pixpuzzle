using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace MonoTouch.UIKit
{
	public static class UIImageEx
	{
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

