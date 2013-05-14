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
            CreateGrid((x, y) =>
            {
                return new CellViewModel(x, y);
            });
        }

        public void UpdateGrid()
        {
            RaisePropertyChanged("CellViewModels");
        }

        public List<CellViewModel> CellViewModels
        {
            get
            {
                List<CellViewModel> cells = new List<CellViewModel>();

                foreach (Cell[] cellsLine in Cells)
                {
                    foreach (Cell c in cellsLine)
                    {
                        cells.Add((CellViewModel)c);
                    }
                }

                return cells;
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
