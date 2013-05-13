using System;
using System.ComponentModel;
using System.Windows;

namespace PixPuzzle.WP.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Execute some code on the UI Thread
        /// </summary>
        public void ExecuteUI(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }

        #region Orientation

        private Microsoft.Phone.Controls.PageOrientation m_orientation;
        public Microsoft.Phone.Controls.PageOrientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                m_orientation = value;
                RaisePropertyChanged("Orientation");
                RaisePropertyChanged("ScrollingState");
            }
        }
        public System.Windows.Controls.ScrollBarVisibility ScrollingState
        {
            get
            {
                bool landscape = Orientation == Microsoft.Phone.Controls.PageOrientation.Landscape
                    || Orientation == Microsoft.Phone.Controls.PageOrientation.LandscapeLeft
                    || Orientation == Microsoft.Phone.Controls.PageOrientation.LandscapeRight;

                if (landscape) return System.Windows.Controls.ScrollBarVisibility.Auto;
                else return System.Windows.Controls.ScrollBarVisibility.Disabled;
            }
        }

        #endregion
    }
}
