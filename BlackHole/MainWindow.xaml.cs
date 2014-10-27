using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlackHole
{

    public partial class MainWindow : Window
    {

        public static RoutedCommand ExitCommand = new RoutedCommand();

        public static RoutedCommand AboutCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExitCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            Application.Current.Shutdown();
        }

        private void AboutCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            var aboutView = new AboutWindow { Owner = this };
            aboutView.ShowDialog();
        }

    }

}
