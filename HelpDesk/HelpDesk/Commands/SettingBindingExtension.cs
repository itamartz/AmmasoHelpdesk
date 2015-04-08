using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HelpDesk.Commands
{
    public class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.txt_Domain))
            {
                Properties.Settings.Default.txt_Domain = Environment.UserDomainName;
            }
            this.Source = Properties.Settings.Default;
           
            this.Mode = BindingMode.TwoWay;
        }
    }
}
