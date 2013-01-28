using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace UpdateChecker
{
    class Program
    {
        public static int Main(string[] args)
        {
            //TODO have a GUI, ask if they want to update.

            int exitCode = 0;

            if (args.Length != 4)
            { exitCode = 1; }
            else
            {
                string versionAddress = args[0];
                string versionPath = args[1];
                string downloadAddress = args[2];
                string fileName = args[3];

                string webVersion = getWebVersion(versionAddress);

                if (webVersion == "incorrect url format")
                { exitCode = 2; }
                else if (webVersion == "no page here")
                {
                    //TODO don't leave immediately. Try again.
                }
                else if (webVersion == "incorrect version format")
                { exitCode = 2; }
                else
                {
                    string fileVersion = getFileVersion(versionPath);

                    if (fileVersion == "file not found")
                    { exitCode = 2; }
                    else if (fileVersion == "incorrect version format")
                    { exitCode = 2; }
                    else
                    {
                        if (fileVersion != webVersion)
                        {
                            string downloadStatus = downloadFiles(downloadAddress, fileName);

                            if (downloadStatus == "no page here")
                            { exitCode = 2; }
                            else if (downloadStatus == "success")
                            {
                                StreamWriter writer = new StreamWriter(versionPath);
                                writer.Write(webVersion);
                                writer.Flush();
                                writer.Close();
                                exitCode = 0;
                            }
                        }
                    }
                }
            }

            return exitCode;
        }

        public static string getFileVersion(string path)
        {
            try
            {
                StreamReader reader = new StreamReader(path);
                string returned = reader.ReadToEnd();
                reader.Close();

                if (!checkVersionFormat(returned))
                { return "incorrect version format"; }
                else
                { return returned; }
            }
            catch (IOException)
            { return "file not found"; }
        }

        public static string getWebVersion(string address)
        {
            try
            {
                WebRequest request = WebRequest.Create(address);
                WebResponse response = request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());
                string returned = reader.ReadToEnd();
                reader.Close();

                if (!checkVersionFormat(returned))
                { return "incorrect version format"; }
                else
                { return returned; }
            }
            catch (UriFormatException)
            { return "incorrect url format"; }
            catch (WebException)
            { return "no page here"; }
        }

        public static bool checkVersionFormat(string version)
        {
            int countPeriods = 0;
            for (int i = 0; i < version.Length; ++i)
            {
                if (version[i] == '.')
                { ++countPeriods; }
            }

            if (countPeriods == 2)
            { return true; }
            else
            { return false; }
        }

        public static string downloadFiles(string address, string fileName)
        {
            try
            {
                WebClient webClient = new WebClient();

                webClient.DownloadFile(address, "temp_" + fileName);

                if (File.Exists(fileName))
                { File.Delete(fileName); }

                File.Copy("temp_" + fileName, fileName);
                File.Delete("temp_" + fileName);

                return "success";
            }
            catch (WebException)
            { return "no page here"; }
        }
    }
}
