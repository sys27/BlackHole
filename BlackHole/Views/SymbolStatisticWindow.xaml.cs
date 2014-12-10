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

        public SymbolStatisticWindow(IEnumerable<Tuple<string, IEnumerable<long>, IEnumerable<SymbolCode>>> codes)
        {
            InitializeComponent();

            foreach (var file in codes)
            {
                var vwCodes = new List<SymbolViewModel>();
                var weights = file.Item2.ToArray();
                var arr = file.Item3.ToArray();
                for (int i = 0; i < arr.Length; i++)
                    if (arr[i] != null)
                        vwCodes.Add(new SymbolViewModel
                        {
                            Symbol = (char)i,
                            Weight = weights[i],
                            Code = (byte)i,
                            NewCode = Convert.ToString(arr[i].Bits, 2).PadLeft(arr[i].Length, '0')
                        });

                tabControl.Items.Add(new StatisticTab
                {
                    Header = file.Item1,
                    DataContext = vwCodes
                });

                var sb = new StringBuilder();
                sb.AppendFormat("Статстика символів для файлу \"{0}\":<br><table border=1><thead><tr><td>Символ</td><td>Вага символу</td><td>Код символу</td><td>Новий код</td></tr></thead><tbody>", file.Item1);
                foreach (var item in vwCodes)
                    sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", item.Symbol, item.Weight, item.Code, item.NewCode);
                sb.Append("</tbody></table>");
                App.WriteToReport(sb.ToString());
            }
        }

    }

}
