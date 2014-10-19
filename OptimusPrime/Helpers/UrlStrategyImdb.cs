using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace OptimusPrime.Helpers
{
    public class UrlStrategyImdb : UrlStrategy
    {
        private const string COmdbUrl = "http://www.omdbapi.com/?i=";

        public override string ExtractInformationFromUrl()
        {
            using (var wc = new WebClient())
            {
                var stream = wc.OpenRead(COmdbUrl + (Uri.Segments[Uri.Segments.Length - 1]).Replace("/", ""));
                var sr = new StreamReader(stream);

                var json = JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());

                if (json["Response"] == "False")
                {
                    return string.Empty;
                }

                var title = json["Title"];
                var year = json["Year"];
                var runtime = json["Runtime"];
                var genre = json["Genre"];
                var rating = json["imdbRating"];
                var votes = json["imdbVotes"];
                var actors = json["Actors"];

                return string.Format("{0} ({1}) {2} - {3}/10 ({4} votes)\n{5}\n{6}",
                    title, //0
                    year, //1
                    runtime, //2
                    rating, //3
                    votes, //4
                    genre, //5
                    actors); //6
            }
        }
    }
}