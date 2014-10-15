using System;
using System.Configuration;
using System.Text;
using OptimusPrime.Helpers;
using Parse;

namespace OptimusPrime
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Initialize parse

            var appId = ConfigurationManager.AppSettings["ParseApplicationId"];
            var winKey = ConfigurationManager.AppSettings["ParseWindowsKey"];

            ParseClient.Initialize(appId, winKey);
            ParseAnalytics.TrackAppOpenedAsync();

            var skypeHelper = new SkypeHelper();
            skypeHelper.Initialize();

            var sb = new StringBuilder();

            Console.ForegroundColor = ConsoleColor.Green;
            sb.Append("++++++++++++++++++++++++++++++++++++++++");
            sb.Append(Environment.NewLine);
            sb.Append("+++++ OPTIMUS PRIME SKYPE BOT v1.0 +++++");
            sb.Append(Environment.NewLine);
            sb.Append("++++++++++++++++++++++++++++++++++++++++");
            sb.Append(Environment.NewLine);

            Console.WriteLine(sb.ToString());
            Console.Title = "OptimusPrime Auto-Bot";
            Console.ResetColor();

            var isRunning = true;
            while (isRunning)
            {
                var input = Console.ReadLine();
                if (input.ToUpper().Equals("QUIT"))
                {
                    isRunning = false;
                }
            }
        }
    }
}
