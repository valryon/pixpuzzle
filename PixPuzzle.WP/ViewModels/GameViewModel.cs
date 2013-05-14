using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixPuzzle.WP.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public GameViewModel(WriteableBitmap img)
        {
            // Invert width and height cause of bmp?
            GridViewModel = new GridViewModel(img.PixelHeight, img.PixelWidth);

            // Get every pixel
            for (int x = 0; x < img.PixelWidth; x++)
            {
                for (int y = 0; y < img.PixelHeight; y++)
                {
                    Color color = img.GetPixel(x, y);

                    float r, g, b, a;

                    if (color.A < 20)
                    {
                        a = 1;
                        r = 1;
                        g = 1;
                        b = 1;
                    }
                    else
                    {
                        a = 1;
                        r = color.R / 255f;
                        g = color.G / 255f;
                        b = color.B / 255f;
                    }

                    // invert x and y
                    GridViewModel.SetPixelData(y, x, new Data.CellColor()
                    {
                        A = a,
                        R = r,
                        G = g,
                        B = b,
                    });
                }
            }

            GridViewModel.SetupGrid();
            GridViewModel.UpdateGrid();
        }

        public GridViewModel GridViewModel
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
