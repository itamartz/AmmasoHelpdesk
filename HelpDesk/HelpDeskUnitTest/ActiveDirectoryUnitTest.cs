using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Diagnostics;

namespace HelpDeskUnitTest
{
    [TestClass]
    public class ActiveDirectoryUnitTest
    {
        public static PrincipalContext GetPrincipalContext()
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, null);
            return pc;
        }

        [TestMethod]
        public void TestMethodGetAllComputersDirectoryEntry()
        {
            List<string> ServerList = FindServerInOU("OU=ILQHFAATC1,OU=IL,OU=Laptops,OU=Computers,OU=RIVER2-000,OU=CODE,DC=code1,DC=emi,DC=philips,DC=com");
        }
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

        public void TestMethodGetAllComputers()
        {

        }
    }

    [TestClass]
    public class AllGroup
    {
        [TestMethod]
        public void FindAllGroup()
        {
            //PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, null, "OU=Mailgroups,OU=ILQHFAATC1,OU=CODE,DC=code1,DC=emi,DC=philips,DC=com");

            // define a "query-by-example" principal - here, we search for a GroupPrincipal 
            GroupPrincipal qbeGroup = new GroupPrincipal(ctx);

            // create your principal searcher passing in the QBE principal    
            PrincipalSearcher srch = new PrincipalSearcher(qbeGroup);
            ((DirectorySearcher)srch.GetUnderlyingSearcher()).PageSize = 500;


            foreach (var item in srch.FindAll())
            {
                //Debug.WriteLine(item);
                Object obj = item.GetUnderlyingObject();
                DirectoryEntry entry = null;
                if (obj != null)
                    entry = (DirectoryEntry)obj;
                if (entry != null)
                {
                    foreach (PropertyValueCollection prop in entry.Properties)
                    {
                        //var Active = prop.PropertyName["extensionAttribute1"];
                    }
                    //entry.Properties.PropertyNames
                }
            }
        }


    }

    [TestClass]
    public class UserP
    {
        [TestMethod]
        public void GetUserPrincipal()
        {
            UserPrincipal up = GetUserPrincipal(Environment.UserName);
            DateTime date = (DateTime)up.LastPasswordSet;
            if(date!=null)
            {
                Debug.WriteLine(date.Date);
            }
        }

        public UserPrincipal GetUserPrincipal(string UserName)
        {
            UserPrincipal user = null;
            try
            {
                PrincipalContext pc = GetPrincipalContext();
                user = UserPrincipal.FindByIdentity(pc, UserName);
            }
            catch (Exception ex)
            {
                return user;
            }
            return user;
        }
        public static PrincipalContext GetPrincipalContext()
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, Environment.UserDomainName, "310188147", "Carmel2$");
            return pc;
        }
    }

}
