using PixPuzzle.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace PixPuzzle.WP.ViewModels
{
    public class GridViewModel : Grid<CellViewModel>, INotifyPropertyChanged
    {
        public GridViewModel(int width, int height)
            : base(width, height, 32)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }


    public class GridCellsAsListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO
            return new List<CellViewModel>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO
            return new List<CellViewModel>();
        }
    }
}
