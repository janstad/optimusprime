using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OptimusPrime.Domain;
using OptimusPrime.Interfaces;
using OptimusPrime.Shared;
using OptimusPrime.Specifications;
using Parse;
using SKYPE4COMLib;

namespace OptimusPrime.Listeners
{
    public class GameOnListener : IListener
    {
        private List<GameOn> mGameOnList;

        public GameOnListener()
        {
            mGameOnList = new List<GameOn>();
            GetGameOnList();
        }

        public string Call(string pCommand, ChatMessage pMsg)
        {
            if (new CommandSpec().IsSatisfiedBy(pCommand)) //Command?
            {
                pCommand = pCommand.RemoveTrigger();
            }
            else
            {
                if (IsUserAlreadyAdded(pMsg.FromDisplayName))
                {
                    CleanGameList();
                    var gameOn = mGameOnList.Find(x => x.Name == pMsg.FromDisplayName);
                    if (gameOn != null)
                    {
                        gameOn.ValidUntil = DateTime.Now.AddMinutes(20);
                    }
                }
                return string.Empty; 
            }

            var username = pMsg.FromDisplayName;

            switch (pCommand.CommandTrimToUpper())
            {
                case "GAMEON":
                    CleanGameList();
                    return SetGameOn(username);
                case "GAMEOFF":
                    CleanGameList();
                    return SetGameOff(username);
                case "GAME":
                    CleanGameList();
                    return GetGameOn();
                default:
                    return string.Empty;
            }
        }

        private async void GetGameOnList()
        {
            try
            {
                var query = ParseObject.GetQuery("GameOn");
                var results = await query.FindAsync();

                foreach (var result in results)
                {
                    var gameOn = new GameOn()
                    {
                        Name = result.Get<string>("Name"),
                        ValidUntil = result.Get<DateTime>("ValidUntil")
                    };
                    mGameOnList.Add(gameOn);
                }
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

        private string SetGameOn(string pUsername)
        {
            if (IsUserAlreadyAdded(pUsername))
            {
                mGameOnList.Find(x => x.Name == pUsername).ValidUntil = DateTime.Now.AddMinutes(20);
                return "Player was already added.";
            }

            var newGameOn = new GameOn()
            {
                Name = pUsername,
                ValidUntil = DateTime.Now.AddMinutes(20)
            };
            mGameOnList.Add(newGameOn);
            AddUserToOnlineDb(pUsername);

            var returnString = string.Format("Player successfully added.\n{0}", GetGameOn());

            return returnString;
        }

        private string SetGameOff(string pUsername)
        {
            if (IsUserAlreadyAdded(pUsername))
            {
                var index = mGameOnList.FindIndex(x => x.Name == pUsername);
                mGameOnList.RemoveAt(index);
                RemoveUserFromOnlineDb(pUsername);
                var returnString = string.Format("Player successfully removed.\n{0}", GetGameOn());
                return returnString;
            }
            return "Player not found in list. Nothing to remove.";
        }

        private string GetGameOn()
        {
            var count = 0;
            var sb = new StringBuilder();
            sb.Append(string.Format("Ready for game [{0}]: ", mGameOnList.Count));

            foreach (GameOn gameOn in mGameOnList)
            {

                var minutes = gameOn.ValidUntil.Subtract(DateTime.Now).Minutes;
                var seconds = gameOn.ValidUntil.Subtract(DateTime.Now).Subtract(new TimeSpan(minutes)).Seconds;

                if (minutes == 20 && seconds == 59)
                {
                    seconds = 0;
                }

                sb.Append(string.Format(
                    "{0} [{1}:{2}]",
                    gameOn.Name,
                    minutes,
                    seconds.ToString("00")));

                if (count < mGameOnList.Count - 1)
                {
                    sb.Append(", ");
                }
                count++;
            }

            if (mGameOnList.Count >= 5)
            {
                sb.Append("\n");
                sb.Append("--- FULL TEAM. JOIN TS AND SERVER NOW. ---");
            }

            return sb.ToString();
        }

        private bool IsUserAlreadyAdded(string pUsername)
        {
            return mGameOnList.Find(x => x.Name == pUsername) != null;
        }

        private void CleanGameList()
        {
            for (int i = mGameOnList.Count - 1; i >= 0; i--)
            {
                if (mGameOnList.ElementAt(i).ValidUntil < DateTime.Now)
                {
                    RemoveUserFromOnlineDb(mGameOnList[i].Name);
                    mGameOnList.RemoveAt(i);
                }
            }
        }

        private async void RemoveUserFromOnlineDb(string pUsername)
        {
            //TODO: Try/Catch.
            var query = from player in ParseObject.GetQuery("GameOn")
                        where player.Get<string>("Name") == pUsername
                        select player;

            var results = await query.FindAsync();

            foreach (var result in results)
            {
                await result.DeleteAsync(); //delete
            }
        }

        private async void AddUserToOnlineDb(string pUsername)
        {
            //TODO: Try/Catch
            var parseObject = new ParseObject("GameOn");
            parseObject["Name"] = pUsername;
            parseObject["ValidUntil"] = DateTime.Now.AddMinutes(20);

            await parseObject.SaveAsync();
        }
    }
}
