using OptimusPrime.Helpers;
using OptimusPrime.Interfaces;
using SKYPE4COMLib;
using System.Linq;

namespace OptimusPrime.Listeners
{
    public class WebListener : IListener
    {
        private readonly IHttpHelper _httpHelper;
        private readonly IUrlStrategyFactory _urlStrategyFactory;

        public WebListener(
            IHttpHelper httpHelper,
            IUrlStrategyFactory urlStrategyFactory)
        {
            _httpHelper = httpHelper;
            _urlStrategyFactory = urlStrategyFactory;
        }

        public string Call(string pCommand, ChatMessage pMsg)
        {
            var uris = _httpHelper.ExtractUris(pCommand).ToList();
            if (uris.Count() > 3)
                return "I refuse to process more than three URLs at a time.";
            return string.Join(
                "\n",
                uris.Select(uri => _urlStrategyFactory.Create(uri).ExtractInformationFromUrl()).ToList());
        }
    }
}