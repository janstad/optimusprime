using Autofac;
using OptimusPrime.Helpers;
using OptimusPrime.Interfaces;
using OptimusPrime.Listeners;
using Parse;
using System;
using System.Collections.Generic;
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
            builder.RegisterType<ConsoleWriter>().As<IOutputWriter>().SingleInstance();

            builder.RegisterType<ZoltanListener>().As<IListener>();
            builder.RegisterType<TorrentListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<QuoteListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<GameOnListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<SportListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<NordiskFilmListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<WebListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<RandomListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<LaEmpanadaListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<StringListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<MffListener>().As<IListener>().PreserveExistingDefaults();
            builder.RegisterType<KolliListener>().As<IListener>().PreserveExistingDefaults();

            var container = builder.Build();

            var skypeHelper = new SkypeHelper(
                container.Resolve<IOutputWriter>(),
                container.Resolve<IEnumerable<IListener>>());
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