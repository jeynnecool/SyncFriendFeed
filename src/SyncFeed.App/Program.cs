using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace SyncFeed.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Nickname: ");
            string nickname = Console.ReadLine();

            Console.WriteLine("Remote Key: ");
            string remoteKey = Console.ReadLine();

            Console.WriteLine("Start to export...");
            bool result = GenerateFile(nickname, remoteKey);

            while (result == false)
            {
                Console.ReadKey();
            }

            Console.WriteLine("Export completed");
            Console.ReadKey();
        }

        static bool GenerateFile(string nickname, string remoteKey)
        {
            bool stop = false;
            bool result = false;

            int fetchIndex = 0;
            int nameIndex = 0;
            while (!stop)
            {
                string url = @"http://" + nickname + ":" + remoteKey + "@friendfeed-api.com/v2/feed/" + nickname + "?format=xml&start=" + fetchIndex + "&num=50&hidden=1&fof=1";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = null;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                WebHeaderCollection headers = response.Headers;    
                using (var reader = new StreamReader(response.GetResponseStream(), UTF8Encoding.UTF8))
                {
                    string responseText = reader.ReadToEnd();
                    var content = XDocument.Parse(responseText).Descendants("feed").Descendants("entry");

                    if (content.Count() <= 0)
                    {
                        stop = true;
                    }
                    else
                    {
                        string fileName = "friendfeed-backup-" + nameIndex + ".xml";
                        using (StreamWriter writer = new StreamWriter(fileName))
                        {
                            writer.Write(responseText);
                            Console.WriteLine("Download: " + fileName);
                        }
                    }
                }

                response.Close();
                fetchIndex += 30;
                nameIndex += 1;
            }

            result = true;
            return result;
        }
    }
}
