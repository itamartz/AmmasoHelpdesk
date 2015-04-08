using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\gpmc.msc";
            Console.WriteLine(path);
            Console.Read();
        }

        private static void spiceworks(string email, string pass, string server, string port)
        {
            //string Username = email;
            //string Password = pass;
            //string authInfo = Username + ":" + Password;

            //string BaseUrl = string.Format("http://{0}:{1}/api/devices.json?category=Workstations", server, port);

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseUrl);

            //authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            //request.Headers["Authorization"] = "Basic " + authInfo;

            ////request.Credentials = new NetworkCredential(Username, Password);
            //request.ContentType = "application/json; charset=utf-8";

            //using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            //{
            //    if (response.StatusCode != HttpStatusCode.OK) throw new Exception(string.Format("Server returned {0}\n {1}", response.StatusCode, response.ToString()));
            //    // Cheat and always expect utf-8
            //    string result = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8).ReadToEnd();
            //}

            string url = string.Format("http://{0}:{1}/login", server, port);

            //WebRequest request = WebRequest.Create(url);
            //WebResponse response = request.GetResponse();
            // HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            // request.Credentials = new NetworkCredential(email, pass);

            //request.CookieContainer = new CookieContainer();
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //WebResponse response = request.GetResponse();



            //WebClient web = new WebClient();
            //web.Credentials = new NetworkCredential(email, pass);

            //// url = string.Format("http://{0}:{1}/api/devices.json?category=Workstations", server, port);
            //string htmlCode = web.DownloadString(url);
            //Console.WriteLine(htmlCode);
            //response.Headers["Set-Cookie"]


        }



        private static void NewMethod()
        {
            Process rdcProcess = new Process();
            rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
            rdcProcess.StartInfo.Arguments = "/generic:TERMSRV/carmeltestpc /user:" + "administrator" + " /pass:" + "sharone1969";
            rdcProcess.Start();

            rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
            rdcProcess.StartInfo.Arguments = "/v " + "carmeltestpc"; // ip or name of computer to connect
            rdcProcess.Start();

            //rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe");
            //rdcProcess.StartInfo.Arguments = "/delete:TERMSRV/carmeltestpc";
            //rdcProcess.Start();
        }
    }
}
