using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.DirectoryServices;
using HelpDesk.Windows;
using System.Diagnostics;
using System.Windows;
using System.IO;
using SnmpSharpNet;
using HelpDesk.Commands;
using HelpDesk.ViewModel;

namespace HelpDesk
{
    public class PrinterCommands
    {
        XMLApi xml = new XMLApi();
        private SimpleSnmp simplesnmp;
        List<PrinterToner> Printertoners = new List<PrinterToner>();
        public async Task<List<string>> GetAllPrinters()
        {
            List<string> Allprinters = new List<string>();

        
            List<DistinguishedNames> listDistinguishedNames = await xml.GetDistinguishedNames();


            listDistinguishedNames = listDistinguishedNames.Where(l => !string.IsNullOrEmpty(l.Printers)).ToList();

            string txt_PrintersDistinguishedNameOU = Properties.Settings.Default.txt_PrintersDistinguishedNameOU;
            DirectoryEntry entry = null;

            foreach (DistinguishedNames item in listDistinguishedNames)
            {
                entry = new DirectoryEntry("LDAP://" + item.Printers);
                Task T = Task.Run(() =>
                {
                    // DirectoryEntry entry = new DirectoryEntry();
                    DirectorySearcher searcher = new DirectorySearcher(entry);
                    SearchResultCollection results;
                    searcher.Filter = ("(ObjectCategory=printQueue)");
                    searcher.PageSize = 500;
                    searcher.ServerTimeLimit = TimeSpan.FromMinutes(1);
                    using (searcher)
                    {
                        results = searcher.FindAll();

                        foreach (SearchResult result in results)
                        {
                            string unc =
                                   result.Properties["uncname"][0].ToString();

                            //Debug.WriteLine(result.Properties["portname"][0].ToString().ToLower());
                            if (result.Properties["portname"].Count == 0)
                            {
                                string[] str = unc.Split('\\');
                                Allprinters.Add(str[3]);
                            }
                            else if (!result.Properties["portname"][0].ToString().ToLower().Contains("usb"))
                            {
                                string[] str = unc.Split('\\');
                                Allprinters.Add(str[3]);
                            }
                        }
                    }
                }
            );
                await Task.WhenAll(T);
            }

            //if (string.IsNullOrEmpty(txt_PrintersDistinguishedNameOU))
            //{
            //    entry = new DirectoryEntry();
            //}
            //else
            //{
            //    entry = new DirectoryEntry("LDAP://" + txt_PrintersDistinguishedNameOU);
            //}


            return (Allprinters);
        }
        public string GetUNCPrinter(string printerName)
        {
            string uncname = string.Empty;
            DirectoryEntry entry = new DirectoryEntry();
            DirectorySearcher searcher = new DirectorySearcher(entry);
            SearchResultCollection results;
            searcher.Filter = string.Format("(&(ObjectCategory=printQueue)(CN=*{0}*))", printerName);
            using (searcher)
            {
                results = searcher.FindAll();

                foreach (SearchResult result in results)
                {
                    uncname =
                       result.Properties["uncname"][0].ToString();
                }
            }
            return uncname;
        }

        public void PrinterProperties(string printerName)
        {
            string all = string.Format(@"PRINTUI.DLL,PrintUIEntry /p /n{0}", GetUNCPrinter(printerName));
            StartProcess(@"C:\Windows\System32\rundll32.exe", all);
        }
        public void PrinterQueue(string printerName)
        {
            string all = string.Format(@"PRINTUI.DLL,PrintUIEntry /o /n{0}", GetUNCPrinter(printerName));
            StartProcess(@"C:\Windows\System32\rundll32.exe", all);
        }
        public async void PrinterRDP(string printerName)
        {
            Task T = Task.Run(() =>
             {
                 string all = GetPrinterServer(printerName);
                 ComputerCommands com = new ComputerCommands();
                 com.ComputerRDP(all);
             });
            await Task.WhenAny(T);
        }
        public async void PrinterPing(string printerName)
        {
            Task T = Task.Run(() =>
              {
                  string all = GetPrinterIP(printerName);
                  ComputerCommands com = new ComputerCommands();
                  com.ComputerPing(all);
              });
            await Task.WhenAny(T);
        }
        public async void PrinterHttp(string printerName)
        {
            Task T = Task.Run(() =>
            {
                string all = GetPrinterIP(printerName);
                //"C:\Program Files\Internet Explorer\iexplore.exe"
                string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string path = Path.Combine(ProgramFiles, @"Internet Explorer\iexplore.exe");
                StartProcess(path, all);
            });
            await Task.WhenAny(T);
        }

        internal void PrinterTestPage(string printerName)
        {
            string all = string.Format(@"PRINTUI.DLL,PrintUIEntry /k /n\\{0}\{1}", GetPrinterServer(printerName), printerName);
            try
            {
                StartProcess(@"C:\Windows\System32\rundll32.exe", all);
                System.Windows.Forms.MessageBox.Show("Test page send to " + printerName);
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        internal async void PrinterInkLevel(string printerName)
        {
            Task T = Task.Run(() =>
              {

                  try
                  {
                      string all = "";
                      if (printerName.Contains("."))
                      {
                          all = printerName;
                      }
                      else
                          all = GetPrinterIP(printerName);
                      simplesnmp = new SimpleSnmp(all, 161, "public", 2000, 3);

                  }
                  catch (Exception ex)
                  {
                      System.Windows.Forms.MessageBox.Show(ex.Message);
                      return;
                  }
                  if (simplesnmp != null)
                  {
                      GetToner();
                  }
              });
            await Task.WhenAny(T);
            InkLevelWindow inkWindow = new InkLevelWindow(Printertoners, printerName);

            inkWindow.ShowDialog();

        }
        private void GetToner()
        {
            Printertoners.Clear();
            simplesnmp.MaxRepetitions = 200;
            Dictionary<Oid, AsnType> Inks = simplesnmp.GetBulk(new string[] { ".1.3.6.1.2.1.43.11.1" });
            Dictionary<Oid, AsnType> SysObjectId = simplesnmp.Get(SnmpVersion.Ver1, new string[] { ".1.3.6.1.2.1.1.2.0" });
            if (Inks == null)
            {
                Console.WriteLine("No results received.");
                Inks = simplesnmp.Walk(SnmpVersion.Ver1, ".1.3.6.1.2.1.43.11.1.1");
            }
            //.1.3.6.1.2.1.43.11.1.1
            int numToner = 0;
            if (Inks == null) return;

            foreach (KeyValuePair<Oid, AsnType> kvp in Inks)
            {
                if (kvp.Key.ToString().StartsWith("1.3.6.1.2.1.43.11.1.1.5.1"))
                {                                 //.1.3.6.1.2.1.43.11.1.1.5.1.1
                    if (kvp.Value.ToString() == "3")
                    {
                        numToner++;
                    }
                    if (SysObjectId.Any(k => k.Value.ToString() == "1.3.6.1.4.1.11.2.3.9.1"))
                    {
                        if (kvp.Value.ToString() == "21") numToner++;
                    }

                }
            }

            GetPrinterToner(Inks, numToner);

        }

        private void GetPrinterToner(Dictionary<Oid, AsnType> Inks, int numToner)
        {

            for (int i = 0; i < numToner; i++)
            {
                PrinterToner toner = new PrinterToner();

                //1.3.6.1.2.1.43.11.1.1.6.1.
                string MarkerSuppliesDescription = "1.3.6.1.2.1.43.11.1.1.6.1." + (i + 1).ToString();
                toner.MarkerSuppliesDescription = Inks[new Oid(MarkerSuppliesDescription)].ToString();

                //.1.3.6.1.2.1.43.11.1.1.9.1.1
                string MarkerSuppliesLevel = "1.3.6.1.2.1.43.11.1.1.9.1." + (i + 1).ToString();
                toner.MarkerSuppliesLevel = Inks[new Oid(MarkerSuppliesLevel)].ToString();

                //1.3.6.1.2.1.43.11.1.1.8.1.
                string MarkerSuppliesMaxCapacity = "1.3.6.1.2.1.43.11.1.1.8.1." + (i + 1).ToString();
                toner.MarkerSuppliesMaxCapacity = Inks[new Oid(MarkerSuppliesMaxCapacity)].ToString();

                toner.Precent = (int)(double.Parse(toner.MarkerSuppliesLevel) / double.Parse(toner.MarkerSuppliesMaxCapacity) * 100);
                Printertoners.Add(toner);
            }
        }
        public string GetPrinterServer(string printerName)
        {
            string unc = GetUNCPrinter(printerName);
            string[] str = unc.Split('\\');
            return (str[2]);
        }
        public string GetPrinterIP(string printerName)
        {
            string returnportName = string.Empty;
            DirectoryEntry entry = new DirectoryEntry();
            DirectorySearcher searcher = new DirectorySearcher(entry);
            SearchResultCollection results;
            searcher.Filter = string.Format("(&(ObjectCategory=printQueue)(CN=*{0}*))", printerName);
            using (searcher)
            {
                results = searcher.FindAll();

                foreach (SearchResult result in results)
                {
                    string portname =
                           result.Properties["portname"][0].ToString();
                    if (portname.ToLower().Contains("usb"))
                    {
                        returnportName = portname;
                    }
                    else if (portname.ToLower().Contains("_"))
                    {
                        string[] split = portname.Split('_');
                        returnportName = (split[1]);

                    }
                    else
                        returnportName = (portname);
                }
            }
            return returnportName;
        }

        private void StartProcess(string app, string args)
        {
            if (string.IsNullOrEmpty(app))
            {
                Options o = new Options();
                o.ShowDialog();
            }
            else
            {
                var psi = new ProcessStartInfo
                {
                    FileName = app,
                    UserName = Properties.Settings.Default.txt_AdminUser,
                    Domain = Properties.Settings.Default.txt_Domain,
                    Password = Utility.AdminPassword.ToSecure(),
                    UseShellExecute = false,
                    //RedirectStandardOutput = true,
                    //RedirectStandardError = true,
                    Arguments = args == null ? string.Empty : args

                };
                try
                {
                    Process.Start(psi);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "שגיאה");
                }
            }
        }





    }
}
