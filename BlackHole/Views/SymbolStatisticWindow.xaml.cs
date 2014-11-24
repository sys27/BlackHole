using BlackHole.Library;
using BlackHole.ViewModels;
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
using System.Windows.Shapes;

namespace BlackHole.Views
{

    public partial class SymbolStatisticWindow : Window
    {

        public SymbolStatisticWindow(IEnumerable<Tuple<string, IEnumerable<SymbolCode>>> codes)
        {
            InitializeComponent();

            foreach (var file in codes)
            {
                var vwCodes = new List<SymbolViewModel>();
                var arr = file.Item2.ToArray();
                for (int i = 0; i < arr.Length; i++)
                    if (arr[i] != null)
                        vwCodes.Add(new SymbolViewModel
                        {
                            Symbol = (char)i,
                            Code = (byte)i,
                            NewCode = Convert.ToString(arr[i].Bits, 2).PadLeft(arr[i].Length, '0')
                        });

                tabControl.Items.Add(new StatisticTab
                {
                    Header = file.Item1,
                    DataContext = vwCodes
                });
            }
        }

    }

}
