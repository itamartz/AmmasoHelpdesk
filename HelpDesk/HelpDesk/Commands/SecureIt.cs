using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk
{
    public class SecureIt
    {
        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt Is Not A Password");

        public static string EncryptString(System.Security.SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(DecryptString(input)),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Return SecureString object base string value
        /// </summary>
        /// <param name="encryptedData">Encrypted Data</param>
        /// <returns>SecureString</returns>
        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), entropy, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string DecryptString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }

    }

    public static class SecureItHelper
    {
        public static string ToSecureString(this string current)
        {
            //string str = SecureIt.EncryptString(SecureIt.ToSecureString(current));
            
            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(current);
            string str = System.Text.Encoding.Unicode.GetString(b1);
            return str;
        }

        public static string DecryptSecureString(this string current)
        {
             //Encoding.ASCII.GetString()
            string str = SecureIt.DecryptString(SecureIt.DecryptString(current));
            return str;
        }
    }
}
