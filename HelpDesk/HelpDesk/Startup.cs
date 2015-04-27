using HelpDesk.Commands;
using HelpDesk.UserControls;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HelpDesk
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);
            //app.MapSignalR();
            //GlobalHost.HubPipeline.RequireAuthentication();

        }

    }

    public class MyHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        public void RunRemoteSoftware(AdminCredentialsAndRemoteSoftware acr)
        {
            if (acr.FileName.ToLower().Contains("mstsc"))
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
                process.StartInfo.Arguments = string.Format("/generic:TERMSRV/{0} /user:{1} /pass:{2}", acr.ComputerName, acr.UserName, acr.Password);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();

                process.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
                process.StartInfo.Arguments = string.Format("/admin /v:{0}", acr.ComputerName); // ip or name of computer to connect
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.Start();
            }
            else
            {

                ProcessStartInfo psi = new ProcessStartInfo();

                string pass = acr.Password;

                psi.FileName = acr.FileName;
                psi.UserName = acr.UserName;
                psi.Domain = acr.Domain;
                psi.Password = pass.ToSecure();
                psi.UseShellExecute = false;
                //psi.RedirectStandardOutput = true;
                //psi.RedirectStandardError = true;
                psi.Arguments = acr.Arguments;

                //string str = string.Format("psi.FileName {0} | psi.UserName {1} | psi.Password {2} psi.Arguments {3}", acr.FileName, acr.UserName, pass, acr.Arguments);
                //System.Windows.Forms.MessageBox.Show(str);

                Process.Start(psi);
                try
                {
                    Process.Start(psi);
                    //Clients.Caller.
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException(ex.Message, ex);
                }
            }
        }
        public override Task OnConnected()
        {
            //Application.Current.Dispatcher.Invoke(() =>
            //    ((MainWindow)Application.Current.MainWindow).WriteToConsole("Client connected: " + Context.ConnectionId));

            string version = Context.QueryString["version"];
            
            //System.Windows.Forms.MessageBox.Show(version);
            Debug.WriteLine(Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
    }
}
