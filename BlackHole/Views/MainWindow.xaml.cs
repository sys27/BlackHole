using BlackHole.Library;
using BlackHole.ViewModels;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace BlackHole.Views
{

    public partial class MainWindow : Window
    {

        public static RoutedCommand NewCommand = new RoutedCommand();
        public static RoutedCommand OpenCommand = new RoutedCommand();
        public static RoutedCommand ExitCommand = new RoutedCommand();

        public static RoutedCommand ShowHelpCommand = new RoutedCommand();
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

        private void blockSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (int.TryParse(blockSizeTextBox.Text, out result))
            {
                archiver.HuffmanCode.BufferSize = result;

                App.WriteToReport(string.Format("Змінено розмір блоку на {0}.", result));
            }
        }

        private void OpenCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "BlackHole Archive (*.bh)|*.bh|All Files (*.*)|*.*"
            };
            if (ofd.ShowDialog(this) == true)
            {
                fileToArchive = ofd.FileName;
                var archive = archiver.ReadArchiveInfo(fileToArchive);

                foreach (var file in archive)
                    files.Add(new FileViewModel(null, file.Name, file.OriginalSize));

                App.WriteToReport(string.Format("Відкрито файл \"{0}\".", fileToArchive));
            }
        }

        private void ExitCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            Application.Current.Shutdown();
        }

        private void ShowHelpCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {

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
                fileToArchive = sfd.FileName;

                processingWindow.Compress(files.Select(file => file.FullName), sfd.FileName, new CancellationTokenSource());
                processingWindow.ShowDialog();
            }
        }

        private void CompressCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = filesListView != null && filesListView.Items.Count > 0;
        }

        private void ExtractAllCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            var fbd = new VistaFolderBrowserDialog();
            if (fbd.ShowDialog(this) == true)
            {
                var processingWindow = new ProcessingWindow { Owner = this };
                processingWindow.DecompressAll(fileToArchive, fbd.SelectedPath, new CancellationTokenSource());
                processingWindow.ShowDialog();
            }
        }

        private void ExtractAllCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = filesListView != null && filesListView.Items.Count > 0;
        }

        private void ExtractCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            var fbd = new VistaFolderBrowserDialog();
            if (fbd.ShowDialog(this) == true)
            {
                var processingWindow = new ProcessingWindow { Owner = this };
                processingWindow.Decompress(fileToArchive, files[filesListView.SelectedIndex].Name, fbd.SelectedPath, new CancellationTokenSource());
                processingWindow.ShowDialog();
            }
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
