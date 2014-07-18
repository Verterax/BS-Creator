using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchingStoryCreator
{
    /// <summary>
    /// This Class is responsible for presenting data to the interface.
    /// </summary>
    public class PresentationObject
    {

        #region Variables

        public string story { get; private set; }
        public string imgURL { get; private set; }
        public int life { get; private set; }
        public int mana { get; private set; }
        public int stamina { get; private set; }
        public List<Item> items { get; private set; }
        public List<GameButton> buttonData { get; private set; }  

        #endregion

        #region Init / Load

        public PresentationObject(string bgImgDir, GameDic dic, ItemBag bag, DataNode data, GameScript script)
        {
            //No data to load.
            if (dic == null ||
                bag == null ||
                data == null)
            {
                this.buttonData = new List<GameButton>();
                this.imgURL = "";
                this.items = new List<Item>();
                this.life = 0;
                this.mana = 0;
                this.stamina = 0;
                this.story = "We're sorry. The story seems to have run aground. Like a when ship runs into an outcropping of rocks after the lighthouse has gone out.";
                return;
            }
                
            story = dic.ReplaceKeysWithValues(data.Story);
            imgURL = Path.Combine(bgImgDir, data.ImgPath);
            life = dic.GetLife();
            mana = dic.GetMana();
            stamina = dic.GetStamina();
            items = bag.GetItems();

            buttonData = GetButtonData(dic, data, script);
        }

        public PresentationObject()
        {

        }


        #endregion

        #region Presenting

        private static List<GameButton> GetButtonData(GameDic dic, DataNode current, GameScript script)
        {
            if (dic == null ||
                script == null ||
                current == null)
                throw new NullReferenceException("Unable to Create Buttons");

            List<GameButton> buttonData = new List<GameButton>();

            foreach (DataNode node in current.Nodes)
            {
                bool isVisible = false;
                string output = script.EvalShorthand(node.AvailIf);
                bool outputIsBool = bool.TryParse(output, out isVisible);

                if (!outputIsBool)
                {
                    //Error
                    isVisible = true;
                    MSScriptControl.Error err = script.Error;
                    string btnText = string.Format("Eval Err in {0} line: {1} col: {2}", err.Text, err.Line, err.Column);
                    GameButton data = new GameButton(btnText, "0");
                    buttonData.Add(data);
                }
                else
                {
                    if (!isVisible)
                        continue;
                    else
                    {
                        string btnText = dic.ReplaceKeysWithValues(node.ButtonText);
                        GameButton data = new GameButton(btnText, node.ID);
                        buttonData.Add(data);
                    }
                }

            }

            return buttonData;
        }

        public static PresentationObject Empty
        {
            get 
            {
                PresentationObject empty = new PresentationObject();
                empty.buttonData = new List<GameButton>();
                empty.imgURL = "";
                empty.items = new List<Item>();
                empty.life = 0;
                empty.mana = 0;
                empty.stamina = 0;
                empty.story = "We're sorry. The story seems to have run aground. Like a when ship runs into an outcropping of rocks after the lighthouse has gone out.";
                return empty;
            }
        }


        #endregion

    }

    /// <summary>
    /// The class which encapsulates data for displaying a button's text, and navigating to its associated Node.
    /// </summary>
    public class GameButton
    {
        public string PreText {get; set;}
        public string NodeID {get; set;}

        public GameButton()
        {
        }

        public GameButton(string preText, string nodeID)
        {
            Init(preText, nodeID);
        }

        private void Init(string preText, string nodeID)
        {
            this.PreText = preText;
            this.NodeID = nodeID;
        }
    }

}
