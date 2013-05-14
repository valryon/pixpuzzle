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
    public partial class GridView : UserControl
    {
        private GridViewModel viewModel;

        public GridView()
        {
            InitializeComponent();

            viewModel = new GridViewModel(12, 12);
            DataContext = viewModel;
        }

        public void SetGrid(GridViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
