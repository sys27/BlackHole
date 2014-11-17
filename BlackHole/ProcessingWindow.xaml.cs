using BlackHole.Library;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace BlackHole
{

    public partial class ProcessingWindow : Window
    {

        public static RoutedCommand CancelCommand = new RoutedCommand();

        private Archiver archiver;

        private CancellationTokenSource compressTokenSource;
        private CancellationTokenSource decompressTokenSource;

        public ProcessingWindow()
        {
            archiver = new Archiver();

            InitializeComponent();
        }

        public async void Compress(IEnumerable<string> files, string outputFile, CancellationTokenSource token)
        {
            compressTokenSource = token;

            try
            {
                await archiver.CreateAsync(files.ToArray(), outputFile, token);
            }
            catch (OperationCanceledException)
            {

            }

            if (closeAfterCheckBox.IsChecked == true)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        public async Task Decompress(string file, string folder, CancellationTokenSource token)
        {
            decompressTokenSource = token;

            try
            {
                await archiver.ExtractAllAsync(file, folder, token);
            }
            catch (OperationCanceledException)
            {

            }

            if (closeAfterCheckBox.IsChecked == true)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CancelCommand_Executed(object o, ExecutedRoutedEventArgs args)
        {
            if (compressTokenSource != null)
                compressTokenSource.Cancel();
            if (decompressTokenSource != null)
                decompressTokenSource.Cancel();

            this.DialogResult = false;
            this.Close();
        }

    }

}
