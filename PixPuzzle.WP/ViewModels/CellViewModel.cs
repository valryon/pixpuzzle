using PixPuzzle.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace PixPuzzle.WP.ViewModels
{
    public class CellViewModel : Cell, INotifyPropertyChanged
    {
        public CellViewModel(int x, int y)
            : base(x,y)
        {
            Text = "*";
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

        private Color backgroundColor;
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                RaisePropertyChanged("BackgroundColor");
            }
        }

        private Color textColor;
        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
                RaisePropertyChanged("TextColor");
            }
        }

        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                RaisePropertyChanged("Text");
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
