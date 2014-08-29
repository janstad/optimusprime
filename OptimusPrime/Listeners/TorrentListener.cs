using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Xml;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
    public class TorrentListener : IListener
    {
        public string Call(string pCommand, ChatMessage pMsg)
        {
            if (new CommandSpec().IsSatisfiedBy(pCommand)) //Command?
            {
                pCommand = pCommand.RemoveTrigger();
            }
            else
            {
                return string.Empty;
            }

            switch (pCommand.CommandTrimToUpper())
            {
                case "TL":
                    return GetTlRss(pCommand.RemoveCommand().Split(' '));
                default:
                    return string.Empty;
            }
        }


        private string GetTlRss(string[] pFilter)
        {
            try
            {
                var xmlsrc = ConfigurationManager.AppSettings["TlRssKey"];

                var wr = (HttpWebRequest)WebRequest.Create(xmlsrc);

                // Set the HTTP properties
                wr.Timeout = 10000;
                // 10 seconds

                // Read the Response
                var resp = wr.GetResponse();
                var stream = resp.GetResponseStream();

                // Load XML Document
                XmlDocument doc;

                using (var reader = new XmlTextReader(stream))
                {
                    reader.XmlResolver = null;
                    doc = new XmlDocument();

                    doc.Load(reader);
                }

                var nodes = doc.SelectNodes("/rss/channel/item");
                var count = 0;
                var arrNodes = new string[nodes.Count];

                foreach (XmlNode xn in nodes)
                {
                    if (pFilter.Length > 0)
                    {
                        var match = true;
                        foreach (var str in pFilter)
                        {
                          
                                if (!xn["title"].InnerText.ToLower().Contains(str.ToLower()))
                                {
                                    match = false;
                                    break;
                                }
                        }
                        if (match)
                        {
                            arrNodes[count] = xn["title"].InnerText;
                            count++;
                        }
                    }
                    else
                    {
                        arrNodes[count] = xn["title"].InnerText;
                        count++;
                        if (count == 10) { break; }
                    }

                }
                Array.Sort(arrNodes); //Sort A-Z
                return String.Join("|\\n", arrNodes);

            }
            catch (Exception e)
            {
                var errMessage = String.Format("Error: {0}", e.Message);
                Console.WriteLine(errMessage);
                return errMessage;
            }

        }
    }
}
