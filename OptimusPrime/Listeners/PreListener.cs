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

        private readonly Dictionary<string, string> _mPreDictionary;
        private readonly Dictionary<string, string> _mNewPreDictionary;

        public PreListener()
        {
            _mPreDictionary = new Dictionary<string, string>();
            _mNewPreDictionary = new Dictionary<string, string>();

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

            var preArray = _mNewPreDictionary.Select(x => x.Value).ToArray();
            return string.Join("\n", preArray);
        }

        private void GetPreRss()
        {
            _mNewPreDictionary.Clear();
            const string url = "http://predb.me/?cats=movies-hd&rss=1";
            var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);

            reader.Close();

            foreach (var item in feed.Items)
            {
                var id = item.Id;
                var title = item.Title.Text;

                if (!_mPreDictionary.ContainsKey(id))
                {
                    _mPreDictionary.Add(id, title);

                    if (!_mNewPreDictionary.ContainsKey(id))
                    {
                        _mNewPreDictionary.Add(id, title);
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
                var id = result.Get<string>("Id");
                var title = result.Get<string>("Title");

                if (!_mPreDictionary.ContainsKey(id))
                {
                    _mPreDictionary.Add(id, title);
                }

            }
        }

        private static async void SaveData(string pId, string pTitle)
        {
            //Create ParseObject for saving in DB
            var parseObject = new ParseObject("PreDB");
            parseObject["Id"] = pId;
            parseObject["Title"] = pTitle;

            await parseObject.SaveAsync();
        }
    }
}