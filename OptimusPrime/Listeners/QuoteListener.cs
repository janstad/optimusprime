﻿using System;
using System.Collections.Generic;
using System.Linq;
using OptimusPrime.Enums;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using Parse;
using OptimusPrime.Domain;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
    public class QuoteListener : IListener
    {
        private readonly List<Quote> _mQuoteList;
        private readonly List<Quote> _mFullQuoteList;

        public QuoteListener()
        {
            _mQuoteList = new List<Quote>();
            _mFullQuoteList = new List<Quote>();
            GetQuoteList();
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
                case "SAVEQUOTE":
                    return SaveQuote(pCommand.RemoveCommand());
                case "QUOTE":
                    return GetQuote(pCommand);
                default:
                    return string.Empty;
            }
        }

        private string SaveQuote(string command)
        {
            //Get username and quote from pCommand
            if (command.IndexOf(":", StringComparison.Ordinal) == -1)
            {
                return "Unable to save. Could not find username...";
            }
            
            var username = command.Substring(0, command.IndexOf(":", StringComparison.Ordinal));
            var quote = command.Substring(
                command.IndexOf(":", StringComparison.Ordinal) + 1).Trim();

            var newQuote = new Quote()
                {
                    Username = username,
                    UserQuote = quote
                };

            //Add new quote to list so that it will be included right away
            _mQuoteList.Add(newQuote);
            _mFullQuoteList.Add(newQuote);

            //TODO: Fulfix?
            var sendStatus = new Status();

            //Send it to cloud
            SaveData(newQuote, sendStatus);

            return sendStatus.StatusCode == StatusCode.Ok ? "Successfully added to database." : "Error saving quote to database.";
        }

        private static async void SaveData(Quote quote, Status status)
        {
            //Create ParseObject for saving in DB
            var parseObject = new ParseObject("Quotes");
            parseObject["Name"] = quote.Username;
            parseObject["Quote"] = quote.UserQuote;
            
            try
            {
                await parseObject.SaveAsync();
                status.StatusCode = StatusCode.Ok;
            }
            catch (Exception)
            {
                Console.WriteLine(@"Error saving quote to database.");
                status.StatusCode = StatusCode.Error;
                throw;
            }
        }

        private string GetQuote(string command)
        {
            if (_mQuoteList.Count == 0)
            {
                GetQuoteList();
            }

            string returnQuote;
            var commandList = command.Split(' ');
            var rndQuote = new Random();

            if (commandList.Length > 1) //Search variable included?
            {
                var quotes =
                    (from st in _mFullQuoteList
                     where st.UserQuote.ToLower().Contains(commandList[1].ToLower())
                     || st.Username.ToLower().Contains(commandList[1].ToLower())
                     select st).ToArray();
                if (quotes.Length == 0) return "No quotes found";

                var quote = quotes[rndQuote.Next(0, quotes.Length - 1)];
                returnQuote = string.Format("{0}: {1}", quote.Username, quote.UserQuote);

                //Remove quote to avoid same quote posting over and over
                //mQuoteList.RemoveAt(
                //    mQuoteList.FindIndex(x => x.UserQuote.Trim() == quote.UserQuote.Trim()));

                return returnQuote;
            }

            var quoteIndex = rndQuote.Next(0, _mQuoteList.Count);

            var q = _mQuoteList[quoteIndex];
            returnQuote = string.Format("{0}: {1}", q.Username, q.UserQuote);

            //Remove quote to avoid same quote posting over and over
            _mQuoteList.RemoveAt(quoteIndex);

            return returnQuote;
        }

        private async void GetQuoteList()
        {
            //TODO: Try/Catch
            var query = ParseObject.GetQuery("Quotes");
            var results = await query.FindAsync();

            foreach (var result in results)
            {
                var leQuote = new Quote()
                    {
                        Username = result.Get<string>("Name"),
                        UserQuote = result.Get<string>("Quote")
                    };
                _mQuoteList.Add(leQuote);
                _mFullQuoteList.Add(leQuote);
            }
        }
    }
}
