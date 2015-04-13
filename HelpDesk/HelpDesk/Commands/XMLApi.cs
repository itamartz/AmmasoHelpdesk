using HelpDesk.Commands;
using HelpDesk.UserControls;
using HelpDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace HelpDesk
{
    public class XMLApi
    {
        private string xmlPathlocation;
        private string DistinguishedNamesPathLocation;
        public event Action XmlConfigurationFileLocation;
        private string UsersRemoteSoftwares;
        public XMLApi()
        {
            Remote2.ConfigurationFileLocation += Remote2_ConfigurationFileLocation;

        }

        void Remote2_ConfigurationFileLocation(bool obj)
        {
            //SetXmlConfigurationFileLocation();
        }

        async void SetXmlConfigurationFileLocation()
        {
            await GetXMLButtons();
            if (XmlConfigurationFileLocation != null)
            {
                XmlConfigurationFileLocation();
            }
        }
        public async Task<List<RemoteSoftware>> GetXMLButtons()
        {

            if (!string.IsNullOrEmpty(App.XmlConfigurationFileLocation))
                xmlPathlocation = Path.Combine(App.XmlConfigurationFileLocation, "RemoteSoftwares.xml");
            else
            {
                App.XmlConfigurationFileLocation = System.AppDomain.CurrentDomain.BaseDirectory;
                xmlPathlocation = Path.Combine(App.XmlConfigurationFileLocation, "RemoteSoftwares.xml");



                //xmlPathlocation = "RemoteSoftwares.xml";
            }
            List<RemoteSoftware> listbuttons = new List<RemoteSoftware>();
            #region Linq
            Task t = Task.Run(() =>
            {
                XDocument xml = null;
                if (string.IsNullOrEmpty(xmlPathlocation) || !xmlPathlocation.Contains(".xml"))
                {
                    return;
                }
                try
                {
                    xml = XDocument.Load(xmlPathlocation);
                }
                catch (Exception ex)
                {
                    string str = xmlPathlocation + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;
                    System.Windows.Forms.MessageBox.Show(str, "GetXMLButtons");
                    #region Old Software
                    //  listbuttons.Add(
                    //      new RemoteSoftware()
                    //      {
                    //          ProgramPath = @"C:\Windows\System32\msra.exe",
                    //          Options = "/offerRA {computerName}",
                    //          Name = "Remote Assist"
                    //      });
                    //  listbuttons.Add(
                    // new RemoteSoftware()
                    //     {
                    //         ProgramPath = @"C:\Program Files\SolarWinds\DameWare Mini Remote Control 10.0\DWRCC.exe",
                    //         Options = "-c: -h: -m:{computerName} -vnc -a:1",
                    //         Name = "VNC"
                    //     });
                    //  listbuttons.Add(
                    //new RemoteSoftware()
                    //{
                    //    ProgramPath = @"C:\Program Files\SolarWinds\DameWare Remote Support 11.0\DWRCC.exe",
                    //    Options = "-c: -h: -m:{computerName} -a:1",
                    //    Name = "DameWare",
                    //    Default = true
                    //});
                    //  listbuttons.Add(
                    //new RemoteSoftware()
                    //{
                    //    ProgramPath = @"C:\Program Files\TeamViewer\Version9\TeamViewer.exe",
                    //    Options = "-i {computerName}",
                    //    Name = "TeamViewer"
                    //}); 
                    #endregion

                }

                if (xml != null)
                {

                    var Software = xml.Root.Elements("Software").ToList();

                    foreach (var item in Software)
                    {
                        RemoteSoftware _xmlRemoteSoftware = new RemoteSoftware();

                        _xmlRemoteSoftware.Name = item.Element("Name") == null ? "Name Error" : item.Element("Name").Value.ToString();
                        _xmlRemoteSoftware.Options = item.Element("Options") == null ? "Options Error" : item.Element("Options").Value.ToString();
                        _xmlRemoteSoftware.ProgramPath = item.Element("ProgramPath") == null ? "ProgramPath Error" : item.Element("ProgramPath").Value.ToString();
                        _xmlRemoteSoftware.Default = Convert.ToBoolean(item.Element("Default").Value.ToString());
                        //if (item.Element("Column") != null)
                        //    _xmlRemoteSoftware.Column = Convert.ToInt32(item.Element("Column").Value.ToString());
                        listbuttons.Add(_xmlRemoteSoftware);

                    }
                }
            }
            );


            #endregion
            await Task.WhenAll(t);
            return listbuttons;
        }

        public async Task<List<ViewModel.DistinguishedNames>> GetDistinguishedNames()
        {
            List<ViewModel.DistinguishedNames> list = new List<DistinguishedNames>();
            if (string.IsNullOrEmpty(App.XmlConfigurationFileLocation))
            {
                App.XmlConfigurationFileLocation = System.AppDomain.CurrentDomain.BaseDirectory;
            }
            DistinguishedNamesPathLocation = Path.Combine(App.XmlConfigurationFileLocation, "DistinguishedNames.xml");

            XDocument xml = null;

            try
            {
                xml = XDocument.Load(DistinguishedNamesPathLocation);
            }
            catch (Exception ex)
            {
                //File.Create(DistinguishedNamesPathLocation);
                list.Add(new DistinguishedNames());
                return list;
            }
            Task T = Task.Run(() =>
                {
                    var DistinguishedNames = xml.Root.Elements("DistinguishedNames").ToList();

                    foreach (var item in DistinguishedNames)
                    {

                        DistinguishedNames dn = new DistinguishedNames();
                        dn.Computers = item.Element("Computers") == null ? null : item.Element("Computers").Value.ToString();
                        dn.Users = item.Element("Users") == null ? null : item.Element("Users").Value.ToString();
                        dn.Printers = item.Element("Printers") == null ? null : item.Element("Printers").Value.ToString();
                        dn.Index = list.Count;
                        list.Add(dn);
                    }
                });
            await Task.WhenAny(T);

            if (list.Count == 0)
                list.Add(new DistinguishedNames());
            return list;
        }

        public async Task<List<RemoteSoftware>> GetUsersRemoteSoftwares()
        {
            List<RemoteSoftware> listbuttons = new List<RemoteSoftware>();

            if (!string.IsNullOrEmpty(App.XmlConfigurationFileLocation))
                UsersRemoteSoftwares = Path.Combine(App.XmlConfigurationFileLocation, "UsersRemoteSoftwares.xml");
            else
            {
                App.XmlConfigurationFileLocation = System.AppDomain.CurrentDomain.BaseDirectory;
                UsersRemoteSoftwares = Path.Combine(App.XmlConfigurationFileLocation, "UsersRemoteSoftwares.xml");
            }
            #region Linq
            Task t = Task.Run(() =>
            {
                XDocument xml = null;
                if (string.IsNullOrEmpty(UsersRemoteSoftwares) || !UsersRemoteSoftwares.Contains(".xml"))
                {
                    return;
                }
                try
                {
                    xml = XDocument.Load(UsersRemoteSoftwares);
                }
                catch (FileNotFoundException ex)
                {
                    if (!File.Exists(UsersRemoteSoftwares))
                    {
                        //File.Create(UsersRemoteSoftwares);
                        var xEle = new XElement("RemoteSoftwares",
                                    new XElement("Software",
                                        new XElement("ProgramPath", ""),
                                        new XElement("Options", ""),
                                        new XElement("Name", "Default Name")
                             ));
                        xEle.Save(UsersRemoteSoftwares);
                        GetUsersRemoteSoftwares();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + "  " + UsersRemoteSoftwares, "GetUsersRemoteSoftwares Load Software from xml: ");
                }

                if (xml != null)
                {

                    var Software = xml.Root.Elements("Software").ToList();

                    foreach (var item in Software)
                    {
                        RemoteSoftware _xmlRemoteSoftware = new RemoteSoftware();

                        _xmlRemoteSoftware.Name = item.Element("Name") == null ? "Name Error" : item.Element("Name").Value.ToString();
                        _xmlRemoteSoftware.Options = item.Element("Options") == null ? "Options Error" : item.Element("Options").Value.ToString();
                        _xmlRemoteSoftware.ProgramPath = item.Element("ProgramPath") == null ? "ProgramPath Error" : item.Element("ProgramPath").Value.ToString();
                        listbuttons.Add(_xmlRemoteSoftware);

                    }
                }
            }
            );


            #endregion
            await Task.WhenAll(t);
            if (listbuttons.Count == 0)
                listbuttons.Add(new RemoteSoftware() { Name = "Default Name" });
            return listbuttons;
        }

        /// <summary>
        /// Computers RemoteSoftware
        /// </summary>
        /// <param name="obRemoteSoftware"></param>
        internal void Save(System.Collections.ObjectModel.ObservableCollection<RemoteSoftware> obRemoteSoftware)
        {
            var xEle = new XElement("RemoteSoftwares",
                 from emp in obRemoteSoftware
                 select new XElement("Software",
                                new XElement("ProgramPath", emp.ProgramPath),
                                new XElement("Options", emp.Options),
                                new XElement("Default", emp.Default),
                                new XElement("Name", emp.Name)
                            ));

            xEle.Save(xmlPathlocation);
            Debug.WriteLine("Converted to XML");

        }

        /// <summary>
        /// Users RemoteSoftware
        /// </summary>
        /// <param name="obRemoteSoftware"></param>
        internal void SaveUsersRemoteSoftware(System.Collections.ObjectModel.ObservableCollection<RemoteSoftware> obRemoteSoftware)
        {
            var xEle = new XElement("RemoteSoftwares",
                 from emp in obRemoteSoftware
                 select new XElement("Software",
                                new XElement("ProgramPath", emp.ProgramPath),
                                new XElement("Options", emp.Options),
                                new XElement("Name", emp.Name)
                            ));

            xEle.Save(UsersRemoteSoftwares);
            Debug.WriteLine("Converted to XML");

        }

        /// <summary>
        /// Active Directory DistinguishedNames
        /// </summary>
        /// <param name="obDistinguishedNames"></param>
        internal void Save(System.Collections.ObjectModel.ObservableCollection<DistinguishedNames> obDistinguishedNames)
        {
            var xEle = new XElement("DistinguishedNamesRoot",
                 from ds in obDistinguishedNames
                 select new XElement("DistinguishedNames",
                                new XElement("Computers", ds.Computers),
                                new XElement("Users", ds.Users),
                                new XElement("Printers", ds.Printers)
                            ));

            xEle.Save(DistinguishedNamesPathLocation);
            Debug.WriteLine("Converted to XML");
        }

    }
}
