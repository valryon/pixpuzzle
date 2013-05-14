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
            GridViewModel gridViewModel = new GridViewModel(img.PixelWidth, img.PixelHeight);

            // Get every pixel
            for (int x = 0; x < img.PixelWidth; x++)
            {
                for (int y = 0; y < img.PixelHeight; y++)
                {
                    Color color = img.GetPixel(x, y);

                    gridViewModel.SetPixelData(x, y, new Data.CellColor()
                    {
                        A = color.A * 255,
                        R = color.R * 255,
                        G = color.G * 255,
                        B = color.B * 255,
                    });
                }
            }
            

        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
