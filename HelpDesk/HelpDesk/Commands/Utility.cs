using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk
{
    public static class Utility
    {
        private static string _AdminPassword;

        public static string AdminPassword
        {
            get
            {
                if (string.IsNullOrEmpty(_AdminPassword))
                    _AdminPassword = Properties.Settings.Default.txt_AdminPassword;
                return SecureIt.DecryptString(SecureIt.DecryptString(_AdminPassword)); ;
            }
            set
            {
                _AdminPassword = SecureIt.EncryptString(SecureIt.ToSecureString(value));
                Properties.Settings.Default.txt_AdminPassword = _AdminPassword;
                Properties.Settings.Default.Save();
            }
        }

        public static string GetUserDomainName()
        {
            string domain = String.Empty;
            try
            {
                domain = Environment.UserDomainName;
                string machineName = Environment.MachineName;

                if (machineName.Equals(domain, StringComparison.OrdinalIgnoreCase))
                {
                    domain = String.Empty;
                }
            }
            catch
            {
                // Handle exception if desired, otherwise returns null
            }
            return domain;
        }

        /// <summary>
        /// Returns the Domain which the computer is joined to.  Note: if user is logged in as local account the domain of computer is still returned!
        /// </summary>
        /// <seealso cref="GetUserDomainName"/>
        /// <returns>A string with the domain name if it's joined.  String.Empty if it isn't.</returns>
        public static string GetComputerDomainName()
        {
            string domain = String.Empty;
            try
            {
                domain = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name;
            }
            catch
            {
                // Handle exception here if desired.
            }
            return domain;
        }

        public static SecureString GetAdminPasswordSecureString()
        {
            return SecureIt.DecryptString(_AdminPassword);
        }
        
    }


}
