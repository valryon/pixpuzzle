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
using System.Windows.Media.Imaging;

namespace PixPuzzle.WP.Views
{
    public partial class GameView : UserControl
    {
        private GameViewModel viewModel;

        public GameView()
        {
            InitializeComponent();

            // Load image
            var writeableBmp = new WriteableBitmap(0, 0).FromContent("Images/chip.png");

            viewModel = new GameViewModel(writeableBmp);

            gridView.SetGrid(viewModel.GridViewModel);
        }
    }
}
