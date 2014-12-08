using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlackHole
{

    public partial class App : Application
    {

        public static void WriteToReport(string message)
        {
            string file = "report.html";

            var html = new HtmlDocument();
            HtmlNode body;
            if (!File.Exists(file))
            {
                body = HtmlNode.CreateNode("<body><table border=1><thead><th>Час</th><th>Повідомлення</th></thead><tbody></tbody></table></body>");
                var head = HtmlNode.CreateNode("<head><title>Звіт.</title></head>");
                var doc = HtmlNode.CreateNode("<html></html>");

                doc.AppendChild(head);
                doc.AppendChild(body);
                html.DocumentNode.AppendChild(doc);

                html.Save(file, Encoding.UTF8);
            }

            html.Load(file);

            body = html.DocumentNode.SelectSingleNode("//tbody");
            var tr = HtmlNode.CreateNode(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", DateTime.Now, message));
            body.AppendChild(tr);


            html.Save(file, Encoding.UTF8);
        }

    }

}
