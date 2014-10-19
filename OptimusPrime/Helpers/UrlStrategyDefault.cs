namespace OptimusPrime.Helpers
{
    public class UrlStrategyDefault : UrlStrategy
    {
        private readonly IHttpHelper _httpHelper;

        public UrlStrategyDefault(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public override string ExtractInformationFromUrl()
        {
            return _httpHelper.GetTitleFromUrl(Uri);
        }
    }
}