using System;

namespace PixPuzzle.Data
{
	public struct CellColor
	{
		public float A;
		public float R;
		public float G;
		public float B;

		public CellColor (float a, float r, float g, float b)
		{
			A = a;
			R = r;
			G = g;
			B = b;
		}

		public override bool Equals (object obj)
		{
			if (obj is CellColor) {
				CellColor otherColor = (CellColor)obj;

				return A == otherColor.A && R == otherColor.R && G == otherColor.G && B == otherColor.B;
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

#if IOS
		public MonoTouch.UIKit.UIColor UIColor {
			get {
				return new MonoTouch.UIKit.UIColor (R, G, B, A);
			}
		}
#endif
		public override string ToString ()
		{
			return string.Format ("R:{0} G:{0} B:{0} A:{0}", (int)(R * 255), (int)(G * 255), (int)(B * 255), (int)(A * 100));
		}
	}
}

