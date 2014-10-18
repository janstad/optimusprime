using System;
using System.Collections.Generic;
using OptimusPrime.Listeners;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using SKYPE4COMLib;

namespace OptimusPrime.Helpers
{
    public class SkypeHelper
    {
        private Skype _mSkype;
        private List<IListener> _mListeners;
        private const string CBotPrefix = "/me";

        public void Initialize()
        {
            //Register with skype
            AttachSkype();

            //Initialize listeners
            _mListeners = new List<IListener>()
                {
                    new ZoltanListener(),
                    new TorrentListener(),
                    new QuoteListener(),
                    new GameOnListener(),
                    new SportListener(),
                    new NordiskFilmListener(),
                    new WebListener(),
                    new RandomListener(),
                    new LaEmpanadaListener(),
                    new StringListener(),
                    new MffListener(),
                    new KolliListener()
                };
        }

        private void AttachSkype()
        {
            if (_mSkype != null) return;

            _mSkype = new Skype();
            //Use mSkype protocol version 7 
            _mSkype.Attach(7, false);

            //Listen 
            _mSkype.MessageStatus += skype_MessageStatus;
        }

        private void skype_MessageStatus(ChatMessage pMsg, TChatMessageStatus pStatus)
        {
            if (!IsValidMessageStatus(pStatus)) return;
            
            WriteConsoleMessage(pMsg);

            if (!IsValidSender(pMsg.FromDisplayName)) return;

            var command = pMsg.Body;
            var returnMessage = string.Empty;

            foreach (var listener in _mListeners)
            {
                returnMessage = listener.Call(command, pMsg);

                if (!string.IsNullOrEmpty(returnMessage)) break; //We have a match
            }

            if (string.IsNullOrEmpty(returnMessage)) return;

            SendMessage(returnMessage, pMsg);
        }

        private static bool IsValidMessageStatus(TChatMessageStatus status)
        {
            return status == TChatMessageStatus.cmsReceived || status == TChatMessageStatus.cmsSent;
        }

        private static bool IsValidSender(string pSender)
        {
            return pSender != "BOT";
        }


        private void SendMessage(string pReturnMessage, IChatMessage pMsg)
        {
            var message = string.Format("{0} {1}", CBotPrefix, pReturnMessage); // Add prefix

            if (pReturnMessage.Contains(OpConstants.NewLineChar)) //message is multi line
            {
                message = message.Replace(OpConstants.NewLineChar, "\n");
                _mSkype.SendMessage(pMsg.Sender.Handle, message); //send pm
            }
            else
            {
                pMsg.Chat.SendMessage(message); //send to chat
            }
        }

        private static void WriteConsoleMessage(IChatMessage pMsg)
        {
            string chat;

            if (pMsg.Chat.Name.Contains("$63bb4364f4abbd9"))
            {
                Console.ResetColor();
                chat = "MF";
            }
            else if (pMsg.Chat.Name.Contains("$flippid;f742f5ee5cbe0c71"))
            {
                Console.ForegroundColor = ConsoleColor.White;
                chat = "GR";
            }
            else if (pMsg.Chat.Name.Contains("ed20b9c00e34dd8b"))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                chat = "GR+";
            }
            else if (pMsg.Chat.Name.Contains("19:f87666a242fc410a8b2ad4630dd2161e"))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                chat = "NiP";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                chat = "PM";
            }

            var line = string.Format(
                "{0} <{1}><{2}> {3}",
                pMsg.Timestamp.ToString("HH:mm:ss"),
                chat,
                pMsg.FromDisplayName,
                pMsg.Body);

            Console.WriteLine(line);
        }
    }
}
