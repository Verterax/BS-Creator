using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BranchingStoryCreator.Web
{
    public class PlayerData
    {
        #region Vars
        public string userID { get; set; }
        public string currentPos { get; set; }
        public ItemBag items { get; set; }
        public PlayerVariables dic { get; set; }
        public Input input { get; set; }
        public Sound sound { get; set; }
        public Music music { get; set; }
        public DateTime lastUsed { get; set; }
        #endregion

        #region Load / Init

        public PlayerData(string userID)
        {
            Init();
            this.userID = userID;
        }

        private void Init()
        {
            userID = "";
            currentPos = "";
            items = new ItemBag();
            dic = new PlayerVariables();
            input = new Input();
            sound = new Sound();
            music = new Music();
            lastUsed = DateTime.Now;
        }

        #endregion

        #region Start New Game

        public void StartNewGame(StoryTree tree)
        {
            this.currentPos = tree.ID;
            this.dic.Clear();
            this.items.Clear();
            this.sound.StopAll();
        }

        #endregion

    }
}
