using PixPuzzle.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PixPuzzle.WP.ViewModels
{
    public class CellViewModel : Cell
    {
        public CellViewModel(int x, int y)
            : base(x,y)
        {
        }

        public override void BuildView()
        {
        }

        public override void UpdateView()
        {
        }

        public override void SelectCell()
        {
        }

        public override void UnselectCell(bool success)
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
