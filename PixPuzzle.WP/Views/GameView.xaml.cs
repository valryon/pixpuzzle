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
    public partial class GameView : UserControl
    {
        private GameViewModel viewModel;

        public GameView()
        {
            InitializeComponent();

            viewModel = new GameViewModel("chip.png");
        }
    }
}
