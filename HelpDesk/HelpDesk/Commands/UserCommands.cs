using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HelpDesk.Windows;
using HelpDesk.ViewModel;

namespace HelpDesk
{
    public class UserCommands
    {
        XMLApi xml = new XMLApi();

        public static PrincipalContext GetPrincipalContext()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.txt_Domain) || string.IsNullOrEmpty(Properties.Settings.Default.txt_AdminUser) || string.IsNullOrEmpty(Properties.Settings.Default.txt_AdminPassword))
            {
                Options o = new Options();
                o.ShowDialog();
                //GetPrincipalContext();
            }
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, Properties.Settings.Default.txt_Domain, Properties.Settings.Default.txt_AdminUser, Utility.AdminPassword);
            return pc;
        }

        public static UserPrincipal GetUserPrincipal(string UserName)
        {
            UserPrincipal user = null;
            try
            {
                PrincipalContext pc = GetPrincipalContext();
                user = UserPrincipal.FindByIdentity(pc, UserName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return user;
            }
            return user;
        }
        public static DateTime LastPasswordSet()
        {
            UserPrincipal up = GetUserPrincipal(Environment.UserName);
            DateTime date = (DateTime)up.LastPasswordSet;
            if (date != null)
            {
                return (date.Date);
            }
            return date;
        }
        internal void UserUnLock(string UserName)
        {
            UserPrincipal user = GetUserPrincipal(UserName);
            if (user != null)
            {
                if (user.IsAccountLockedOut())
                    user.UnlockAccount();
            }
        }
        public async Task<List<Principal>> GetAllUsersFromAD()
        {

            //string txt_UsersDistinguishedNameOU = Properties.Settings.Default.txt_UsersDistinguishedNameOU;
            List<DistinguishedNames> listDistinguishedNames = await xml.GetDistinguishedNames();


            List<Principal> ListPrincipal = new List<Principal>();

            listDistinguishedNames = listDistinguishedNames.Where(l => !string.IsNullOrEmpty(l.Users)).ToList();
            foreach (var item in listDistinguishedNames)
            {
                PrincipalContext pc = new PrincipalContext(ContextType.Domain, null, item.Users);
                UserPrincipal up = new UserPrincipal(pc);
                PrincipalSearcher ps = new PrincipalSearcher();
                ps.QueryFilter = up;

                Task T = Task.Run(() =>
                {
                    try
                    {
                        ((DirectorySearcher)ps.GetUnderlyingSearcher()).PageSize = 500;
                        ListPrincipal.AddRange(ps.FindAll().ToList());
                        Debug.WriteLine(ListPrincipal.Count);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, item.Users);
                    }
                });
                await Task.WhenAny(T);
            }

            return ListPrincipal;
            #region Old Code

            //if (string.IsNullOrEmpty(txt_UsersDistinguishedNameOU))
            //{
            //    string groupName = "Domain Users";

            //    ListPrincipal = new List<Principal>();

            //    Task T = Task.Run(() =>
            //    {
            //        PrincipalContext pContext = new PrincipalContext(ContextType.Domain);
            //        GroupPrincipal grp = GroupPrincipal.FindByIdentity(pContext, IdentityType.SamAccountName, groupName);
            //        if (grp != null)
            //        {
            //            foreach (Principal p in grp.GetMembers(false))
            //            {
            //                ListPrincipal.Add(p);
            //            }
            //        }
            //    }
            //    );
            //    await Task.WhenAll(T);

            //}
            //else
            //{
            //    #region NewCode
            //    PrincipalContext pc = new PrincipalContext(ContextType.Domain, null, txt_UsersDistinguishedNameOU);
            //    UserPrincipal cP = new UserPrincipal(pc);
            //    PrincipalSearcher ps = new PrincipalSearcher();
            //    ps.QueryFilter = cP;
            //    ((DirectorySearcher)ps.GetUnderlyingSearcher()).PageSize = 500;

            //    Task T = Task.Run(() =>
            //    {
            //        ListPrincipal = ps.FindAll().ToList();
            //    });
            //    await Task.WhenAny(T);
            //}



            //Principal smtp = ListPrincipal.Where(p => p.Name.ToLower().Contains("smtp")).FirstOrDefault();





            #endregion


        }

        internal void UserDesktop(string ComputerName, string UserName)
        {
            string all = string.Format(@"\\{0}\c$\Users\{1}\Desktop", ComputerName, UserName);
            StartProcess("explorer.exe", all);
        }
        internal void UserHomeDrive(string UserName)
        {

            UserPrincipal user = GetUserPrincipal(UserName);
            if (user != null)
            {
                string homeDirectory = user.HomeDirectory;

                StartProcess("explorer.exe", homeDirectory);
            }

        }

        public void RunRemoteSoftware(UserControls.RemoteSoftware sof, string ComputerName, string comboboxUserName)
        {
            string Options = sof.Options;


            Options = Options.Replace("{computerName}", ComputerName);
            if (Options.Contains("{userName}"))
                Options = Options.Replace("{userName}", Properties.Settings.Default.txt_AdminUser);
            if (Options.Contains("{password}"))
                Options = Options.Replace("{password}", Utility.AdminPassword);
            if (Options.Contains("{comboboxUserName}"))
                Options = Options.Replace("{comboboxUserName}", comboboxUserName);

            StartProcess(sof.ProgramPath, Options);
        }
        public void StartProcess(string app, string args)
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
                    Process p = Process.Start(psi);

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "שגיאה");
                }
            }
        }
    }
}
