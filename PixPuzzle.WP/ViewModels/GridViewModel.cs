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
    public class GridViewModel : Grid, INotifyPropertyChanged
    {
        public GridViewModel(int width, int height)
            : base(width, height, 32)
        {
            CreateGrid();
        }

        public override void UpdateView(List<Cell> cellsToUpdate)
        {
            if (cellsToUpdate == null)
            {

            }
            // TODO
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
