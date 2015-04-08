using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk
{
    public class CommonBase : INotifyPropertyChanged
    {
        public CommonBase()
        {

        }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(property);
                handler(this, args);
            }
        }
        #endregion
    }
    public class SimpleViewModel: CommonBase
	{

        private bool _IsBenefitsCheckd = true;

        public bool IsBenefitsCheckd
        {
            get { return _IsBenefitsCheckd; }
            set
            {
                if (_IsBenefitsCheckd != value)
                {
                    _IsBenefitsCheckd = value;
                    RaisePropertyChanged("IsBenefitsCheckd");
                }
            }
        }

        async Task<List<Principal>> GetAllUsersFromAD()
        {
            string groupName = "Domain Users";

            List<Principal> AllUsers = new List<Principal>();

            Task T = Task.Run(() =>
            {
                PrincipalContext pContext = new PrincipalContext(ContextType.Domain);

                GroupPrincipal grp = GroupPrincipal.FindByIdentity(pContext, IdentityType.SamAccountName, groupName);
                if (grp != null)
                {
                    foreach (Principal p in grp.GetMembers(false))
                    {
                        AllUsers.Add(p);
                    }
                }
            }
            );
            await Task.WhenAll(T);

            return AllUsers;
        }

        public Task<List<Principal>> AllUsers
        {
            get { return GetAllUsersFromAD(); }
        }

	}
}
