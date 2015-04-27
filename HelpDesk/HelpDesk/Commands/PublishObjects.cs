using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Commands
{

    public class DefaultProgram
    {
    }
    public class CDrive
    {

    }
    public class RDPProgram
    {

    }
    public class PingProgram
    {

    }

    public class Refresh
    {

    }
    public class ActiveDirectoryObjectPublish
    {
        public ActiveDirectoryObject ActiveDirectoryObject { get; set; }
        public bool Ischeck { get; set; }
    }
    public class ActiveDirectoryUserPublish
    {
        public bool Ischeck { get; set; }
    }
    public enum ActiveDirectoryObject
    {
        Computers = 1, Users = 2, Printers = 3
    }
    public class ComputerRestart
    {
        public string ComputerName { get; set; }
    }
    public class ComputerShutdown
    {
        public string ComputerName { get; set; }
    }
    public class ActiveDirectorySave { }
    public class UsersViewViewModelSave { }
    public class ComputerSelected
    {
        public string ComputerName { get; set; }
    }
    public class PublishHubConnection { }


    
}
