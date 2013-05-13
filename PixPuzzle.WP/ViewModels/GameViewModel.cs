using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace PixPuzzle.WP.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public GameViewModel(string filename = "chip.png")
        {
            // Load the image
            Uri uri = new Uri("/Images/" + filename, UriKind.Relative);
            BitmapImage img = new BitmapImage(uri);
            WriteableBitmap wImg = new WriteableBitmap(img);

            GridViewModel gridViewModel = new GridViewModel(wImg.PixelWidth, wImg.PixelHeight);

            int x = 0;
            int y = 0;

            for(int i=0;i<wImg.Pixels.Length; i++) {

                int pixel = wImg.Pixels[i];

                byte a = (byte)(pixel >> 24);
                byte r = (byte)(pixel >> 16);
                byte g = (byte)(pixel >> 8);
                byte b = (byte)pixel;

                x++;

                if (x % i == wImg.PixelWidth)
                {
                    x = 0;
                    y++;
                }

                gridViewModel.SetPixelData(x, y, new Data.CellColor()
                {
                    A  = a * 255,
                    R = r * 255,
                    G = g * 255,
                    B = b * 255,
                });
            }

            // Get every pixel

        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
