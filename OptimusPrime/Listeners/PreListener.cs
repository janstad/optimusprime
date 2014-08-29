using System;
using System.Collections.Generic;
using System.Xml;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using SKYPE4COMLib;
using System.ServiceModel.Syndication;
using Parse;
using System.Linq;


namespace OptimusPrime.Listeners
{
    public class PreListener : IListener
    {

        private Dictionary<string, string> mPreDictionary;
        private Dictionary<string, string> mNewPreDictionary;

        public PreListener()
        {
            mPreDictionary = new Dictionary<string, string>();
            mNewPreDictionary = new Dictionary<string, string>();

            GetPreList();
        }

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
                case "PRE":
                    return GetUpdatedPre();
                default:
                    return string.Empty;
            }
        }

        public string GetPre()
        {
            return GetUpdatedPre();
        }

        private string GetUpdatedPre()
        {
            GetPreRss();

            var preArray = mNewPreDictionary.Select(x => x.Value).ToArray();
            return string.Join("\n", preArray);
        }

        private void GetPreRss()
        {
            mNewPreDictionary.Clear();
            var url = "http://predb.me/?cats=movies-hd&rss=1";
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);

            reader.Close();

            foreach (SyndicationItem item in feed.Items)
            {
                var id = item.Id;
                var title = item.Title.Text;

                if (!mPreDictionary.ContainsKey(id))
                {
                    mPreDictionary.Add(id, title);

                    if (!mNewPreDictionary.ContainsKey(id))
                    {
                        mNewPreDictionary.Add(id, title);
                        SaveData(id, title);
                    }
                }
            }
        }

        private async void GetPreList()
        {
            //TODO: Try/Catch
            var query = ParseObject.GetQuery("PreDB");
            var results = await query.FindAsync();

            foreach (var result in results)
            {
                var Id = result.Get<string>("Id");
                var Title = result.Get<string>("Title");

                if (!mPreDictionary.ContainsKey(Id))
                {
                    mPreDictionary.Add(Id, Title);
                }

            }
        }

        private async void SaveData(string pId, string pTitle)
        {
            //Create ParseObject for saving in DB
            var parseObject = new ParseObject("PreDB");
            parseObject["Id"] = pId;
            parseObject["Title"] = pTitle;

            await parseObject.SaveAsync();
        }
    }
}