using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Commands
{
  public  class AdminCredentialsAndRemoteSoftware
    {
        public string UserName { get; set; }
        public SecureString SecureStringPassword { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }

        public string Arguments { get; set; }

        public string FileName { get; set; }

        public string ComputerName { get; set; }

        #region OverrideTostring
        private PropertyInfo[] _PropertyInfos = null;
        public override string ToString()
        {
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                sb.AppendLine(info.Name + ":\t" + info.GetValue(this, null).ToString());
            }

            return sb.ToString();
        }
        #endregion


        public bool IsRunning { get; set; }
    }
}
