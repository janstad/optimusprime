using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimusPrime.Listeners;
using SKYPE4COMLib;
using OptimusPrime.Interfaces;

namespace OptimusPrime.Helpers
{
    public class SkypeHelper
    {
        private Skype mSkype;
        private const string cTrigger = "!"; //Say !command
        private List<IListener> mListeners;
        private const string cBotPrefix = "/me";

        public void Initialize()
        {
            //Register with skype
            AttachSkype();

            //Initialize listeners
            mListeners = new List<IListener>()
                {
                    new ZoltanListener(),
                    new TorrentListener(),
                    new QuoteListener(),
                    new GameOnListener(),
                    new SportListener(),
                    new NordiskFilmListener(),
                    new WebListener(),
                    new RandomListener()
                };
        }

        private void AttachSkype()
        {
            if (mSkype != null) return;

            mSkype = new Skype();
            //Use mSkype protocol version 7 
            mSkype.Attach(7, false);

            //Listen 
            mSkype.MessageStatus += skype_MessageStatus;
        }

        private void skype_MessageStatus(ChatMessage pMsg, TChatMessageStatus pStatus)
        {
            if (!IsValidMessageStatus(pStatus)) return;

            WriteConsoleMessage(pMsg);

            var command = pMsg.Body;
            var returnMessage = string.Empty;

            foreach (var listener in mListeners)
            {
                returnMessage = listener.Call(command, pMsg);

                if (!string.IsNullOrEmpty(returnMessage)) break; //We have a match
            }

            if (string.IsNullOrEmpty(returnMessage)) return;

            SendMessage(returnMessage, pMsg);
        }

        private bool IsValidMessageStatus(TChatMessageStatus status)
        {
            return status == TChatMessageStatus.cmsReceived || status == TChatMessageStatus.cmsSent;
        }

        private void SendMessage(string pReturnMessage, ChatMessage pMsg)
        {
            var message = string.Format("{0} {1}", cBotPrefix, pReturnMessage); // Add prefix

            if (pReturnMessage.Contains("|")) //message is multi line
            {
                message = message.Replace("|", "\n");
                mSkype.SendMessage(pMsg.Sender.Handle, message); //send pm
            }
            else
            {
                pMsg.Chat.SendMessage(message); //send to chat
            }
        }

        private void WriteConsoleMessage(ChatMessage pMsg)
        {
            var chat = string.Empty;

            if (pMsg.Chat.Name.Contains("$63bb4364f4abbd9"))
            {
                Console.ResetColor();
                chat = "MF";
            }
            else if(pMsg.Chat.Name.Contains("$flippid;f742f5ee5cbe0c71"))
            {
                Console.ForegroundColor = ConsoleColor.White;
                chat = "GR";
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
