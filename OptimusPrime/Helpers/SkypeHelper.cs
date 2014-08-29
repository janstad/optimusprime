using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimusPrime.Listeners;
using SKYPE4COMLib;
using OptimusPrime.Interfaces;
using System.Timers;

namespace OptimusPrime.Helpers
{
    public class SkypeHelper
    {
        private Skype mSkype;
        private const string cTrigger = "!"; //Say !command
        private List<IListener> mListeners;
        private const string cBotPrefix = "/me";
        private Timer mTimer;

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
                    new RandomListener(),
                    new LaEmpanadaListener(),
                    new StringListener(),
                    new MffListener(),
                    new KolliListener(),
                    new PreListener()
                };

            //Create timer for pre
            mTimer = new Timer() { Interval = 1800000  };
            mTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            mTimer.Start();
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

            if (!IsValidSender(pMsg.FromDisplayName)) return;

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

        private bool IsValidSender(string pSender)
        {
            return pSender != "BOT";
        }


        private void SendMessage(string pReturnMessage, ChatMessage pMsg)
        {
            var message = string.Format("{0} {1}", cBotPrefix, pReturnMessage); // Add prefix

            if (pReturnMessage.Contains("|\\n")) //message is multi line
            {
                message = message.Replace("|\\n", "\n");
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

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var preListener = (PreListener)mListeners.Last();
            var pre = preListener.GetPre();

            if (!string.IsNullOrEmpty(pre))
            mSkype.SendMessage("emil.janstad", pre);
        }
    }
}
