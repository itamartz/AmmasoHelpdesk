using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HelpDesk.Windows;
using MSTSCLib;
using System.Management;
using System.Net.NetworkInformation;
using System.DirectoryServices;
using HelpDesk.ViewModel;
using HelpDesk.UserControls;
using HelpDesk.Commands;

namespace HelpDesk
{
    public class ComputerCommands
    {
        XMLApi xml = new XMLApi();



        #region ComputerCommands
        public static PrincipalContext GetPrincipalContext()
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, null);
            return pc;
        }
        string windir = Environment.GetEnvironmentVariable("windir");
        string mmc = Path.Combine(Environment.GetEnvironmentVariable("windir"), @"System32\mmc.exe");

        internal void ComputerRDP(string ComputerName)
        {

            Process process = new Process();
            process.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
            process.StartInfo.Arguments = string.Format("/generic:TERMSRV/{0} /user:{1} /pass:{2}", ComputerName, Properties.Settings.Default.txt_AdminUser, Utility.AdminPassword);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();

            process.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
            process.StartInfo.Arguments = string.Format("/admin /v:{0}", ComputerName); // ip or name of computer to connect
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.Start();
            App.RunProcess = process;

            AdminCredentialsAndRemoteSoftware acr = App.AdminCredentialsAndRemoteSoftware;

            acr.UserName = Properties.Settings.Default.txt_AdminUser;
            acr.Password = Utility.AdminPassword;
            acr.FileName = "mstsc";
            acr.Domain = Properties.Settings.Default.txt_Domain;
            acr.ComputerName = ComputerName;



        }
        internal void ComputerCDrive(string ComputerName)
        {
            //C:\Windows\System32\PING.EXE
            string proc = "explorer.exe";
            string all = string.Format(@"\\{0}\c$", ComputerName);
            StartProcess(proc, all);

        }
        internal void ComputerDDrive(string ComputerName)
        {
            //C:\Windows\System32\PING.EXE
            string proc = "explorer.exe";
            string all = string.Format(@"\\{0}\D$", ComputerName);
            StartProcess(proc, all);

        }
        internal void ComputerPing(string ComputerName)
        {
            //C:\Windows\System32\PING.EXE
            string SystemDirectory = Environment.SystemDirectory;
            string proc = Path.Combine(SystemDirectory, "ping.exe");
            string all = string.Format("-t {0} -4", ComputerName);
            StartProcess(proc, all);

        }
        internal void ComputerManage(string ComputerName)
        {
            string all = string.Format("compmgmt.msc /Computer={0}", ComputerName);
            StartProcess(mmc, all);

        }
        internal void ComputerShutdown(string ComputerName)
        {
            string all = string.Format(@"/s /m \\{0} /t 0 /f ", ComputerName);
            StartProcess("shutdown.exe", all);

        }
        internal void ComputerRestart(string ComputerName)
        {
            string all = string.Format(@"/r /m \\{0} /t 0 /f ", ComputerName);
            StartProcess("shutdown.exe", all);
        }

        #region WOL
        internal async void ComputerWOL(string ComputerName)
        {
            List<string> macs = new List<string>();

            bool sent = false;
            if (ComputerName.Contains(':'))
                macs.Add(ComputerName);
            string[] str = ComputerName.Split('-');
            if (str.Count() == 6)
                macs.Add(ComputerName);


            if (macs.Count == 0)
            {
                //Parallel.Invoke(() =>
                //{
                //  macs =  GetMacAddreess(ComputerName);
                //},
                //() =>
                //{
                //    macs = GetMacAddreessByArp(ComputerName);
                //}
                //);



                Task taskGetMacAddreess = Task.Run(() =>
                    {
                        macs = GetMacAddreess(ComputerName);
                    });
                Task taskGetMacAddreessByArp = Task.Run(() =>
                    {
                        macs = GetMacAddreessByArp(ComputerName);
                        //sent = SendWOL(macs);
                    });

                await Task.WhenAny(taskGetMacAddreess, taskGetMacAddreessByArp);
                foreach (string item in macs)
                {
                    Debug.WriteLine(item);
                }
            }
            sent = SendWOL(macs);
            if (sent)
                MessageBox.Show("WOL Success", "הודעה");
            else
                MessageBox.Show("cant wol remote computer - pleas supply mac address menual", "הודעה");

        }

        private bool SendWOL(List<string> macs)
        {
            string MacAddress = string.Empty;
            bool result = false;
            foreach (var item in macs)
            {
                MacAddress = item;
                MacAddress = MacAddress.Replace("-", "");
                MacAddress = MacAddress.Replace(":", "");
                MacAddress = MacAddress.Replace(".", "");
                MacAddress = MacAddress.Replace("_", "");
                MacAddress = MacAddress.Replace(" ", "");
                MacAddress = MacAddress.Replace(",", "");
                MacAddress = MacAddress.Replace(";", "");

                if (MacAddress.Length != 12)
                {
                    //MessageBox.Show("Cant find mac address please supply it menual", "Alert");
                    return result;
                }

                else
                {
                    //{
                    //    string all = MacAddress;
                    //    StartProcess(Properties.Settings.Default.txt_WOL, all);
                    //    sent = true;
                    //    MessageBox.Show("Wol sent to computer");

                    byte[] mac = new byte[6];

                    /*Fill String in ByteArray*/
                    for (int k = 0; k < 6; k++)
                    {
                        mac[k] = Byte.Parse(MacAddress.Substring(k * 2, 2), System.Globalization.NumberStyles.HexNumber);
                    }

                    // WOL packet is sent over UDP 255.255.255.0:40000.
                    System.Net.Sockets.UdpClient client = new System.Net.Sockets.UdpClient();
                    client.Connect(System.Net.IPAddress.Broadcast, 40000);
                    // WOL packet contains a 6-bytes trailer and 16 times a 6-bytes sequence containing the MAC address.
                    byte[] packet = new byte[17 * 6];
                    // Trailer of 6 times 0xFF.
                    for (int i = 0; i < 6; i++)
                        packet[i] = 0xFF;
                    // Body of magic packet contains 16 times the MAC address.
                    for (int i = 1; i <= 16; i++)
                        for (int j = 0; j < 6; j++)
                            packet[i * 6 + j] = mac[j];
                    client.Send(packet, packet.Length);
                    // return true;
                    //MessageBox.Show("הדלקת מחשב מרוחק הסתיימה", "הודעה");
                    result = true;


                }
            }
            return result;


            //catch
            //{
            //    MessageBox.Show("לא הצלחתי להדליק מחשב מרוחק", "שגיאה");
            //}
        }

        private List<string> GetMacAddreess(string ComputerName)
        {

            //getmac /s razi
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "getmac.exe";
            p.StartInfo.Arguments = string.Format("/s {0}", ComputerName);
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
            if (macaddress.Count == 0)
                macaddress = (GetMacAddreessByArp(ComputerName));

            return macaddress;
        }

        private List<string> GetMacAddreessByArp(string ComputerName)
        {
            //string computername = "LOGISTICS";

            IPAddress[] ips;

            ips = Dns.GetHostAddresses(ComputerName);


            List<string> li = new List<string>();
            foreach (IPAddress ip in ips)
            {
                Debug.WriteLine("IP {0}", ip.ToString());


                li.Add(GetMacAddressbyARP(ip.ToString()));
            }
            return li;
        }

        public string GetMacAddressbyARP(string ipAddress)
        {
            string macAddress = string.Empty;
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a " + ipAddress;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2)) + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6] + "-" + substrings[7] + "-" +
                        substrings[8].Substring(0, 2);
                return macAddress;
            }

            else
            {
                return "";
            }
        }

        #endregion

        internal void Logoff(string ComputerName)
        {
            string all = @" /l /f /m \\" + ComputerName;
            StartProcess(Path.Combine(windir, @"System32\shutdown.exe"), all);

        }

        public async Task<List<Principal>> GetAllComputers()
        {

            string txt_ComputersDistinguishedNameOU = Properties.Settings.Default.txt_ComputersDistinguishedNameOU;
            //string txt_prefixComputerName = Properties.Settings.Default.txt_prefixComputerName;

            List<Principal> ListPrincipal = new List<Principal>();

            #region NewCode
            List<DistinguishedNames> listDistinguishedNames = await xml.GetDistinguishedNames();

            List<Task> actions = new List<Task>();


            foreach (DistinguishedNames item in listDistinguishedNames)
            {

                PrincipalContext pc = new PrincipalContext(ContextType.Domain, null, item.Computers);
                ComputerPrincipal cP = new ComputerPrincipal(pc);
                PrincipalSearcher ps = new PrincipalSearcher();
                ps.QueryFilter = cP;
                Task T = Task.Run(() =>
                {
                    try
                    {
                        ListPrincipal.AddRange(ps.FindAll().ToList());
                    }
                    catch
                    {

                    }
                });
                await Task.WhenAny(T);

            }

            //PrincipalContext pc = new PrincipalContext(ContextType.Domain, null, txt_ComputersDistinguishedNameOU);
            //ComputerPrincipal cP = new ComputerPrincipal(pc);
            //PrincipalSearcher ps = new PrincipalSearcher();
            //ps.QueryFilter = cP;
            //Task T = Task.Run(() =>
            //{
            //    ListPrincipal = ps.FindAll().ToList();
            //});
            //await Task.WhenAny(T);


            return ListPrincipal;
            #endregion

            #region OldCode
            //string groupName = "Domain Computers";


            //List<Principal> AllComputers = new List<Principal>();

            //Task T = Task.Run(() =>
            //{
            //    GroupPrincipal grp = GroupPrincipal.FindByIdentity(pContext, IdentityType.SamAccountName, groupName);
            //    if (grp != null)
            //    {
            //        foreach (Principal p in grp.GetMembers(false))
            //        {
            //            AllComputers.Add(p);
            //        }
            //    }
            //}
            //);
            //await Task.WhenAll(T);



            //return AllComputers; 
            #endregion
        }
        #endregion
        public static List<string> FindServerInOU(string OU)
        {
            List<string> ServerList = new List<string>();

            DirectoryEntry entry = new DirectoryEntry("LDAP://" + OU);
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            mySearcher.Filter = ("(objectClass=computer)");
            foreach (SearchResult resEnt in mySearcher.FindAll())
            {

                ServerList.Add(resEnt.GetDirectoryEntry().Name.ToString().Replace("CN=", ""));

            }
            return ServerList;
        }
        internal void RunRemoteSoftware(UserControls.RemoteSoftware sof, string ComputerName)
        {
            string Options = sof.Options;

            Options = Options.Replace("{computerName}", ComputerName);
            if (Options.Contains("{userName}"))
                Options = Options.Replace("{userName}", Properties.Settings.Default.txt_AdminUser);
            if (Options.Contains("{password}"))
                Options = Options.Replace("{password}", Utility.AdminPassword);

            Task.Run(() =>
                {
                    StartProcess(sof.ProgramPath, Options);
                });

            AdminCredentialsAndRemoteSoftware acr = App.AdminCredentialsAndRemoteSoftware;

            acr.UserName = Properties.Settings.Default.txt_AdminUser;
            // acr.SecureStringPassword = Utility.AdminPassword.ToSecure();
            acr.Password = Utility.AdminPassword;
            acr.Arguments = Options;
            acr.FileName = sof.ProgramPath;
            acr.Domain = Properties.Settings.Default.txt_Domain;
            acr.ComputerName = ComputerName;

        }
        internal void NetSend(string msg, string ComputerName)
        {

            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(ComputerName, 200);
                if (reply.Status != IPStatus.Success)
                    return;
            }
            catch (Exception)
            {

                // continue;
            }

            string option = string.Format("* /TIME:{0} /server:{1} {2}", 0, ComputerName, msg);
            string app = @"C:\Windows\System32\msg.exe ";

            StartProcess(app, option);
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
                ProcessStartInfo psi = new ProcessStartInfo();

                psi.FileName = app;
                psi.UserName = Properties.Settings.Default.txt_AdminUser;
                psi.Domain = Properties.Settings.Default.txt_Domain;
                psi.Password = Utility.AdminPassword.ToSecure();
                psi.UseShellExecute = false;
                //psi.RedirectStandardOutput = true;
                //psi.RedirectStandardError = true;
                psi.Arguments = args == null ? string.Empty : args;


                try
                {
                    App.RunProcess = Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "שגיאה");
                }
            }
        }

    }



    public static class Helper
    {
        public static SecureString ToSecure(this string current)
        {
            var secure = new SecureString();
            foreach (var c in current.ToCharArray()) secure.AppendChar(c);
            return secure;
        }

    }


}
