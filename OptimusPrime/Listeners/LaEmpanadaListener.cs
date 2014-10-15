using System.IO;
using System.Net;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using SKYPE4COMLib;
using Newtonsoft.Json;

namespace OptimusPrime.Listeners
{
    public class LaEmpanadaListener : IListener
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
                case "LAEMPANADA":
                    return GetLikes();
                default:
                    return string.Empty;
            }
        }

        public string GetLikes()
        {
            string likes;

            using (var wc = new WebClient())
            {
                var foo = wc.OpenRead("http://graph.facebook.com/211608792207500");
                var sr = new StreamReader(foo);

                var json = JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());
                likes = (string)json["likes"];
            }

            return string.Format("La Empanada has {0} likes.", likes);
           
        }
    }
}
