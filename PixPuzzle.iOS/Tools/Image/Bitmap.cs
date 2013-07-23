using System;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PixPuzzle
{
	public class Bitmap
	{
		private UIImage _backingImage = null;
		byte[] pixelData = new byte[0];
		int width = 0;
		int height = 0;
		
		public Bitmap (UIImage image)
		{
			_backingImage = image;
			CGImage imageRef = _backingImage.CGImage;
			width = imageRef.Width;
			height = imageRef.Height;
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
			
			IntPtr rawData = Marshal.AllocHGlobal(height*width*4);
			CGContext context = new CGBitmapContext(
				rawData, width, height, 8, 4*width, colorSpace, CGImageAlphaInfo.PremultipliedLast
				);
			context.DrawImage(new RectangleF(0.0f,0.0f,(float)width,(float)height),imageRef);
			
			pixelData = new byte[height*width*4];
			Marshal.Copy(rawData,pixelData,0,pixelData.Length);
		}
		
		public Color GetPixel(int x, int y)
		{
			try {				
				byte bytesPerPixel = 4;
				int bytesPerRow = width * bytesPerPixel;
				int rowOffset = y * bytesPerRow;
				int colOffset = x * bytesPerPixel;
				int pixelDataLoc = rowOffset + colOffset;
				
				Color ret = Color.FromArgb(pixelData[pixelDataLoc+3],pixelData[pixelDataLoc+0],pixelData[pixelDataLoc+1],pixelData[pixelDataLoc+2]);
				return ret;
			} catch (Exception ex) {
//				Console.WriteLine("Orig: {0}x{1}", _backingImage.Size.Width,_backingImage.Size.Height);
//				Console.WriteLine("Req:  {0}x{1}", x, y);
				throw ex;
			}
		}
	}

}

