using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using HelpDesk;
using System.Net;
using System.Globalization;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Collections;
using System.Drawing.Printing;
using MSTSCLib;
using System.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace HelpDeskUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        public void GetMacAddress()
        {
            //getmac /s razi
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "getmac.exe";
            p.StartInfo.Arguments = string.Format("/s {0}", "itamar-pc");
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            string[] o = output.Split(new string[] { "\r\n" }, int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
            List<string> macaddress = new List<string>();
            foreach (string item in o)
            {
                //int count = item.TakeWhile(c => c == '-').Count();
                if (item.Count(f => f == '-') >= 5)
                {
                    string[] splite = item.Split(' ');
                    macaddress.Add(splite[0]);
                }
            }


            p.WaitForExit();
            Assert.IsNotNull(macaddress);

        }

        [TestMethod]
        public void UnlockUser()
        {
            string userName = "ilqyqmepb1-sys2";
            //DirectoryEntry usr = new DirectoryEntry("LDAP://dc=InsertDomainHere, dc=COM");
            //DirectorySearcher searcher = new DirectorySearcher(userName);
            //searcher.Filter = "(SAMAccountName=" + UserName + ")";
            //searcher.CacheResults = false;
            //SearchResult result = searcher.FindOne();
            //usr = result.GetDirectoryEntry();
            //usr.Properties["LockOutTime"].Value = 0x0000;
            //usr.CommitChanges();
            var pc = new PrincipalContext(ContextType.Domain);
            var u = UserPrincipal.FindByIdentity(pc, userName);
            var email = u.EmailAddress;
            var name = u.DisplayName;

            Debug.WriteLine(u.IsAccountLockedOut());
            u.UnlockAccount();

            //UserPrincipal oUserPrincipal = new UserPrincipal(pc);
            //oUserPrincipal.UnlockAccount();
        }

        [TestMethod]
        public void Computer()
        {
            string ComputerName = "itamar-pc";
            ComputerPrincipal com = GetComputerPrincipal(ComputerName);

        }

        [TestMethod]
        public void DropBox()
        {
            string appDataPath = Environment.GetFolderPath(
                                  Environment.SpecialFolder.ApplicationData);
            string dbPath = System.IO.Path.Combine(appDataPath, "Dropbox\\host.db");
            string[] lines = System.IO.File.ReadAllLines(dbPath);
            byte[] dbBase64Text = Convert.FromBase64String(lines[1]);
            string folderPath = System.Text.ASCIIEncoding.ASCII.GetString(dbBase64Text);
            Console.WriteLine(folderPath);
        }

        [TestMethod]
        public void Printer()
        {
            string printerName = "na_224";
            DirectoryEntry entry = new DirectoryEntry();
            // StreamWriter logs = new StreamWriter("printQueue.txt");
            DirectorySearcher searcher = new DirectorySearcher(entry);
            SearchResultCollection results;
            //searcher.Filter = string.Format("(&(ObjectCategory=printQueue)(CN=*{0}*))", printerName);
            searcher.Filter = "(ObjectCategory=printQueue)";
            //SearchResultCollection results = mySearcher.FindAll();
            //ResultPropertyCollection fields = result.Properties;

            string uncname = string.Empty;
            using (searcher)
            {
                results = searcher.FindAll();

                foreach (SearchResult result in results)
                {
                    foreach (var item in result.Properties)
                    {
                        Debug.WriteLine(item);
                    }
                    if (result.Path.Contains(printerName))
                        uncname = (result.Properties["uncname"][0].ToString());
                }
            }

            Debug.WriteLine(uncname);

        }

        [TestMethod]
        public void printer2()
        {
            string printerName = "pl_223";
            string unc = new HelpDesk.PrinterCommands().GetUNCPrinter(printerName);
            string[] str = unc.Split('\\');
            Debug.WriteLine(str[2]);

        }

        [TestMethod]
        public void GetAllPrinters()
        {
            List<string> Allprinters = new List<string>();
            DirectoryEntry entry = new DirectoryEntry();
            DirectorySearcher searcher = new DirectorySearcher(entry);
            SearchResultCollection results;
            searcher.Filter = ("(ObjectCategory=printQueue)");
            using (searcher)
            {
                results = searcher.FindAll();

                foreach (SearchResult result in results)
                {
                    string unc =
                           result.Properties["uncname"][0].ToString();

                    string[] str = unc.Split('\\');
                    Allprinters.Add(str[3]);
                    string portname =
                          result.Properties["portname"][0].ToString();
                    if (portname.ToLower().Contains("usb"))
                    {
                    }
                    else if (portname.ToLower().Contains("_"))
                    {
                        string[] split = portname.Split('_');
                        Debug.WriteLine(split[1]);

                    }
                    else
                        Debug.WriteLine(portname);

                }
            }


            ////DirectoryEntry entry = new DirectoryEntry("LDAP://" + OU);
            //DirectoryEntry entry = new DirectoryEntry();

            //DirectorySearcher mySearcher = new DirectorySearcher(entry);
            //mySearcher.Filter = ("(ObjectCategory=printQueue)");
            //foreach (SearchResult resEnt in mySearcher.FindAll())
            //{

            //    foreach (var item in resEnt.Properties)
            //    {
            //        Debug.WriteLine(item);

            //    }

            //    printerList.Add(resEnt.GetDirectoryEntry().Name.ToString().Replace("CN=", ""));

            //}
            //Debug.WriteLine(printerList);
        }

        [TestMethod]
        public void GetUNCPrinter()
        {
            string printerName = "pl_231";
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
            Debug.WriteLine(uncname);
        }

        [TestMethod]
        public void AllObject()
        {
            DirectoryEntry entry = new DirectoryEntry();
            StreamWriter logs = new StreamWriter("FindAllObject.txt");
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            mySearcher.Filter = ("(ObjectClass=*)");
            SearchResultCollection fields = mySearcher.FindAll();
            //ResultPropertyCollection fields = result.Properties;

            SearchResultCollection rrr = mySearcher.FindAll();

            foreach (SearchResult resEnt in rrr)
            {
                foreach (DictionaryEntry item in resEnt.Properties)
                {
                    logs.WriteLine("resEnt: " + resEnt.Path + " Key: " + item.Key.ToString() + " Values: " + item.Value.ToString());
                    logs.Flush();
                }

            }

            logs.Close();
        }


        [TestMethod]
        public void AllPrinters()
        {
            DirectoryEntry entry = new DirectoryEntry();
            //StreamWriter logs = new StreamWriter("printQueue.txt");
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            mySearcher.Filter = ("(ObjectCategory=printQueue)");
            SearchResultCollection results = mySearcher.FindAll();
            //ResultPropertyCollection fields = result.Properties;


            SearchResultCollection rrr = mySearcher.FindAll();
            foreach (SearchResult resEnt in rrr)
            {
                foreach (DictionaryEntry item in resEnt.Properties)
                {

                    Debug.WriteLine("resEnt: " + resEnt.Path + " Key: " + item.Key.ToString() + " Values: " + item.Value.ToString());
                    //logs.Flush();

                }
                #region 1
                foreach (var item in resEnt.Properties.PropertyNames)
                {
                    foreach (ResultPropertyValueCollection v in resEnt.Properties.Values)
                    {
                        foreach (var a in v)
                        {

                            Debug.WriteLine("PropertyNames: " + item + " Values: " + a);
                            //logs.Flush();
                            break;
                        }

                        break;
                    }
                }
                //logs.WriteLine();

            }

            //logs.Flush();
            //logs.Close();
                #endregion
        }

        [TestMethod]
        public void PrinterIP()
        {
            PrinterCommands pri = new PrinterCommands();
            pri.PrinterPing("na_224");
        }

        [TestMethod]
        public void RDP()
        {
           //MSTSCLib. MsRdpClient8 rdp = new MSTSCLib.MsRdpClient8();
           // rdp.Server = "namalfs";
           // rdp.UserName = "administrator";
           // //IMsTscNonScriptable secured = (IMsTscNonScriptable)rdp.GetOcx();
           // rdp.AdvancedSettings2.ClearTextPassword = "sharone1969";
           // rdp.Connect();
        }
        #region Principal
        private static PrincipalContext GetPrincipalContext()
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, "Carmla"
                   , "administrator", "sharone1969");
            return pc;
        }
        private static UserPrincipal GetUserPrincipal(string UserName)
        {
            UserPrincipal user;
            try
            {
                PrincipalContext pc = GetPrincipalContext();
                user = UserPrincipal.FindByIdentity(pc, UserName);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
            return user;
        }
        private ComputerPrincipal GetComputerPrincipal(string ComputerName)
        {
            PrincipalContext pc = GetPrincipalContext();
            ComputerPrincipal computer = ComputerPrincipal.FindByIdentity(pc, ComputerName);
            return computer;
        }

        #endregion
    }

    [TestClass]
    public class Unitest2
    {
        [TestMethod]
        public void SecureText()
        {
            HelpDesk.Utility.AdminPassword = "Some string to encrypt";
            Debug.WriteLine(HelpDesk.Utility.AdminPassword);
        }
        [TestMethod]
        public void ExistFile()
        {
            bool b = File.Exists(Environment.SpecialFolder.Windows + @"\system32\dsa.msc");
            Debug.Write(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows).ToString(), @"\system32\dsa.msc"));
            Assert.IsFalse(b);
        }


        [TestMethod]
        public void GetMacAddress()
        {
            //string computername = "LOGISTICS";
            string computername = "itamartz";
            IPAddress[] ips;

            ips = Dns.GetHostAddresses(computername);



            foreach (IPAddress ip in ips)
            {
                Debug.WriteLine("IP {0}", ip.ToString());



                //getmac /s razi
                Process p = new Process();
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "arp";
                p.StartInfo.Arguments = string.Format("/a");
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string[] output = p.StandardOutput.ReadToEnd().Split(new string[] { "\r\n" }, int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
                string[] strs = output.Where(s => s.Contains(ip.ToString())).FirstOrDefault().Split(new string[] { " " }, int.MaxValue, StringSplitOptions.RemoveEmptyEntries);

                
            }
        }

        [TestMethod]
        public void getPassword()
        {
            string pas = ">AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAHsYyELoU5kq95vbVrm3IUwAAAAACAAAAAAADZgAAwAAAABAAAACGqRnajmQcShedcYylZIhQAAAAAASAAACgAAAAEAAAAESdcjf2H23zZTVgM4PZZ9oYAAAAsJOXIdskZWsYNkqqKL3W3FdQJqzX+tiwFAAAAH4QfBULLxWMRMx969VOunb0wcMp";
            Debug.WriteLine(SecureIt.DecryptString(pas));
        }
    }



}
