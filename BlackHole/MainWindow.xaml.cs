using BlackHole.Library;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        public static RoutedCommand CompressCommand = new RoutedCommand();
        public static RoutedCommand ExtractAllCommand = new RoutedCommand();
        public static RoutedCommand ExtractCommand = new RoutedCommand();

        public static RoutedCommand AddCommand = new RoutedCommand();
        public static RoutedCommand RemoveCommand = new RoutedCommand();

        private string fileToArchive;
        private ObservableCollection<FileViewModel> files;
        private Archiver archiver;

        public MainWindow()
        {
            files = new ObservableCollection<FileViewModel>();
            archiver = new Archiver();

            InitializeComponent();

            filesListView.DataContext = files;
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

        private void CompressCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                Filter = "BlackHole Archive (*.bh)|*.bh"
            };
            if (sfd.ShowDialog(this) == true)
            {
                var processingWindow = new ProcessingWindow { Owner = this };
                processingWindow.ShowDialog();
                archiver.Create(files.Select(file => file.FullName).ToArray(), sfd.FileName);
                fileToArchive = sfd.FileName;
            }
        }

        private void CompressCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = filesListView != null && filesListView.Items.Count > 0;
        }

        private void ExtractAllCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            var fbd = new VistaFolderBrowserDialog
            {
            };
            if (fbd.ShowDialog(this) == true)
            {
                archiver.ExtractAll(fileToArchive, fbd.SelectedPath);
            }
        }

        private void ExtractAllCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = filesListView != null && filesListView.Items.Count > 0;
        }

        private void ExtractCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {

        }

        private void ExtractCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = filesListView != null && filesListView.SelectedItem != null;
        }

        private void AddCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = true
            };
            if (ofd.ShowDialog(this) == true)
                foreach (var file in ofd.FileNames)
                    files.Add(new FileViewModel(file, System.IO.Path.GetFileName(file), new FileInfo(file).Length));
        }

        private void RemoveCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            var file = filesListView.SelectedItem as FileViewModel;
            files.Remove(file);
        }

        private void RemoveCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = filesListView != null && filesListView.SelectedItem != null;
        }

        public ObservableCollection<FileViewModel> Files
        {
            get
            {
                return files;
            }
        }

    }

}
