using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PixPuzzle.WP.ViewModels;

namespace PixPuzzle.WP.Views
{
    public partial class CellView : UserControl
    {
        private CellViewModel viewModel;

        /// <summary>
        /// Designer
        /// </summary>
        public CellView()
            : this(0,0)
        {
        }

        public CellView(int x, int y)
        {
            InitializeComponent();

            viewModel = new CellViewModel(0, 0);
            this.DataContext = viewModel;
        }
    }
}
