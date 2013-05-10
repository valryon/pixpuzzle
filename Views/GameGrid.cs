using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace PixPuzzle
{
	public class GameGrid : UIView
	{
		public const int CellSize = 32;
		private GridCell[][] cells;
		private int width, height;

		public GameGrid (int imageWidth, int imageHeight)
			: base(new RectangleF(0,0, imageWidth * CellSize, imageHeight * CellSize))
		{
			cells = new GridCell[imageWidth][];
			width = imageWidth;
			height = imageHeight;

			for (int x=0; x<imageWidth; x++) {
				cells [x] = new GridCell[imageHeight];

				for (int y=0; y<imageHeight; y++) {

					cells [x] [y] = new GridCell (
						new RectangleF (x * CellSize, y * CellSize, CellSize, CellSize)
					);

					cells [x] [y].BackgroundColor = UIColor.White;

					AddSubview (cells [x] [y]);
				}
			}
		}

		public void SetPixelData (int x, int y, UIColor color)
		{
			cells [x] [y].SetColor(color);
		}
	}
}

