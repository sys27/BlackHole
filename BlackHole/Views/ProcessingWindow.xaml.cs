using BlackHole.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace BlackHole.Views
{

    public partial class ProcessingWindow : Window
    {

        public static RoutedCommand StatisticCommand = new RoutedCommand();
        public static RoutedCommand CancelCommand = new RoutedCommand();

        private Archiver archiver;

        private IEnumerable<string> filesToCompress;
        private string fileToDecompress;

        private CancellationTokenSource compressTokenSource;
        private CancellationTokenSource decompressTokenSource;

        private Stopwatch stopwatch;
        private TimeSpan prevTime;
        private long prevSize;

        public ProcessingWindow()
        {
            stopwatch = new Stopwatch();

            archiver = new Archiver();
            archiver.ProgressChanged += (obj, args) =>
            {
                if (args.TotalSize != 0)
                {
                    var percent = args.CurrentSize * 100.0 / args.TotalSize;
                    percentLabel.Content = percent.ToString("F2") + "%";
                    progressBar.Value = percent;

                    filesLabel.Content = args.CurrentFile + " / " + args.TotalFiles;
                    currentSizeLabel.Content = Math.Round(args.CurrentSize / 1048576.0, 2);
                    totalSizeLabel.Content = Math.Round(args.TotalSize / 1048576.0, 2);

                    var timeRate = 1 / (stopwatch.Elapsed - prevTime).TotalSeconds;
                    var sizeRate = args.CurrentSize - prevSize;
                    var sizePerSecond = sizeRate * timeRate;

                    leftTime.Content = new TimeSpan(0, 0, (int)((args.TotalSize - args.CurrentSize) / sizePerSecond)).ToString(@"hh\:mm\:ss");

                    prevTime = stopwatch.Elapsed;
                    elapsedTimeLabel.Content = prevTime.ToString(@"hh\:mm\:ss");

                    prevSize = args.CurrentSize;
                }
            };

            InitializeComponent();
        }

        public async void Compress(IEnumerable<string> files, string outputFile, CancellationTokenSource token)
        {
            compressTokenSource = token;
            filesToCompress = files;

            stopwatch.Start();
            try
            {
                await archiver.CreateAsync(files.ToArray(), outputFile, token);
            }
            catch (OperationCanceledException)
            {

            }
            stopwatch.Stop();

            if (closeAfterCheckBox.IsChecked == true)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        public async Task DecompressAll(string file, string folder, CancellationTokenSource token)
        {
            decompressTokenSource = token;
            fileToDecompress = file;

            stopwatch.Start();
            try
            {
                await archiver.ExtractAllAsync(file, folder, token);
            }
            catch (OperationCanceledException)
            {

            }
            stopwatch.Stop();

            if (closeAfterCheckBox.IsChecked == true)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        public async Task Decompress(string archive, string file, string folder, CancellationTokenSource token)
        {
            decompressTokenSource = token;
            fileToDecompress = archive;

            stopwatch.Start();
            try
            {
                await archiver.ExtractAsync(archive, file, folder, token);
            }
            catch (OperationCanceledException)
            {

            }
            stopwatch.Stop();

            if (closeAfterCheckBox.IsChecked == true)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void StatisticCommand_Executed(object o, ExecutedRoutedEventArgs args)
        {
            var codes = new List<Tuple<string, IEnumerable<SymbolCode>>>();
            if (compressTokenSource != null)
            {
                foreach (var file in filesToCompress)
                    codes.Add(new Tuple<string, IEnumerable<SymbolCode>>(System.IO.Path.GetFileName(file), archiver.GetCodes(file)));
            }
            else if (decompressTokenSource != null)
            {
                var info = archiver.ReadArchiveInfo(fileToDecompress);
                foreach (var file in info)
                    codes.Add(new Tuple<string, IEnumerable<SymbolCode>>(file.Name, file.Codes));
            }

            var statWindow = new SymbolStatisticWindow(codes) { Owner = this };
            statWindow.Show();
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
