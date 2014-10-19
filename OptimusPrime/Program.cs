using Autofac;
using OptimusPrime.Helpers;
using Parse;
using System;
using System.Configuration;
using System.Text;

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

            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleWriter>().As<IOutputWriter>();
            var container = builder.Build();

            var skypeHelper = new SkypeHelper(container.Resolve<IOutputWriter>());
            skypeHelper.Initialize();

            try
            {
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
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }
}