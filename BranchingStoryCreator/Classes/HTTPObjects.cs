using System;
using System.IO;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace BranchingStoryCreator.Web
{
    #region Requests
    //The class containing all data needed to query the GamesManager
    public class PresentationRequest
    {
        #region Vars
        public string userID { get; set; }
        public string buttonID { get; set; }
        private string _textResponse { get; set; }
        public string textResponse { 
            get { return _textResponse; }
            set { _textResponse = value; hasTextResponse = "true"; }
        }
        public string hasTextResponse { get; private set; }
        public string gameName { get; set; }
        public bool newGame { get; set; }
        #endregion

        #region Load Init
        public PresentationRequest()
        {
        }

        public PresentationRequest(string gameName, string userID, string btnID)
        {
            Init();
            this.userID = userID;
            this.gameName = gameName;
            this.buttonID = btnID;
            
        }

        public PresentationRequest(string gameName, string userID, bool newGame = false)
        {
            Init();
            this.userID = userID;
            this.gameName = gameName;
            this.newGame = newGame;

        }

        private void Init()
        {
            this.userID = "";
            this.buttonID = "";
            this.textResponse = "";
            this.gameName = "";
            this.newGame = false;
            this.hasTextResponse = "false";
        }
        #endregion

        #region Request Validation
        public static string ValidateRequest(Dictionary<string, GameObject> games, PresentationRequest request)
        {
            string errCode = "";

            //Request was null, return that it's null.
            if (request == null)
                return "request was null";
            else
            {
                if (request.gameName == null)
                {
                    errCode += " game name is null";
                }
                else //have game name.
                {
                    if (!games.ContainsKey(request.gameName))
                    {
                        errCode += " game: " + request.gameName + " is not available.";
                    }
                }

                if (request.userID == null)
                    errCode += " user id is null";

                if (request.buttonID == null)
                    errCode += " button id is null";

                if (request.textResponse == null)
                    errCode += " text input was null";

                if (request.hasTextResponse == null)
                    errCode += " has text response was null";
            }


            return errCode;
        }
        #endregion
    }

    #endregion

    /// <summary>
    /// This Class is responsible for presenting data to the interface. Returned by the Game Manager.
    /// </summary>
    public class Presentation
    {

        #region Variables

        public string nodeID { get; private set; }
        public List<Item> items { get; private set; }
        public int life { get; private set; }
        public int mana { get; private set; }
        public int stamina { get; private set; }
        public string story { get; private set; }
        public List<GameButton> buttonData { get; private set; }
        public PresentSound sound { get; private set; }
        public string inputPrompt { get; private set; }
        public bool inputPending { get; private set; }
        public string imgURL { get; private set; }
        public string gameName { get; private set; }
        public string errMsg { get; set; }
        public string bgImgMissing { get; private set; }
        public string itemImgMissing { get; set; }
        

        #endregion

        #region Init / Load

        public Presentation(string bgImgDir, string itemImgDir, string soundDir, string gameName, PlayerData thisPlayer, DataNode data, PlayerScript script, bool isNewGame = false)
        {
            PlayerVariables dic = thisPlayer.dic;
            ItemBag bag = thisPlayer.items;
            Init(gameName);

            //No data to load.
            if (dic == null ||
                bag == null ||
                data == null)
            {
                this.story = "We're sorry. The story seems to have run aground. Like a when ship runs into an outcropping of rocks after the lighthouse has gone out.";
                this.errMsg = "Missing Player Variable Dictionary, ItemBag, or Tree Node.";
                return;
            }
                
            story = dic.ReplaceKeysWithValues(data.Story);
            imgURL = Path.Combine(bgImgDir, data.ImgPath);
            life = dic.GetLife();
            mana = dic.GetMana();
            stamina = dic.GetStamina();
            items = bag.GetItems(itemImgDir);
            this.gameName = gameName;
            nodeID = thisPlayer.currentPos;
            this.inputPending = thisPlayer.input.inputPending;
            this.inputPrompt = thisPlayer.input.prompt;

            sound = new PresentSound(soundDir, thisPlayer.sound, thisPlayer.music, isNewGame);
            errMsg = "";

            buttonData = GameButton.GetButtonData(dic, data, script);
        }

        /// <summary>
        /// Error declaration.
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="restartButton"></param>
        public Presentation(string gameName, GameButton restartButton, string errMessage)
        {
            Init(gameName);

            this.story = "We're sorry. The story seems to have run aground. Like a when ship runs into an outcropping of rocks after the lighthouse has gone out.";
            this.errMsg = errMessage;
            buttonData = new List<GameButton>();
            buttonData.Add(restartButton);
            this.imgURL = this.itemImgMissing;

        }

        private void Init(string gameName)
        {
            this.buttonData = new List<GameButton>();
            this.imgURL = "";
            this.items = new List<Item>();
            this.life = 0;
            this.mana = 0;
            this.stamina = 0;
            this.story = "";
            this.errMsg = "";
            this.gameName = gameName;
            this.nodeID = "";
        }


        #endregion

        #region Presenting

        public static Presentation Empty
        {
            get 
            {
                Presentation empty = new Presentation("", null, "Empty Presentation");
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
        public string ButtonID {get; set;}

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
            this.ButtonID = nodeID;
        }

        #region Presenting

        public static List<GameButton> GetButtonData(PlayerVariables dic, DataNode current, PlayerScript script)
        {
            if (dic == null ||
                script == null ||
                current == null)
                throw new NullReferenceException("Unable to Create Buttons");

            List<GameButton> buttonData = new List<GameButton>();
            int btnID = 0;

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
                        GameButton data = new GameButton(btnText, btnID.ToString());
                        buttonData.Add(data);
                    }
                }

                btnID++;
            }

            //If no buttons will be returned, create the restart button.
            if (buttonData.Count == 0)
            {
                GameButton restartButton = GameObject.GetRestartButton();
                buttonData.Add(restartButton);
            }

            return buttonData;
        }



        #endregion
    }

    public class PresentSound
    {
        public string musicFile { get; private set; }
        public string soundFile0 { get; private set; }
        public string soundFile1 { get; private set; }
        public string soundMissing { get; private set; }
        public bool stopAll { get; private set; }

        private static string SOUND_MISSING = "sound_missing.mp3";

        public PresentSound()
        {
            Init();
        }

        public PresentSound(string soundDir, Sound sound, Music music, bool isNewGame = false)
        {
            
            //Copy over respective paths to file names.
            musicFile = (music.music != "") ? Path.Combine(soundDir, music.music) : "";
            soundFile0 = (sound.Sounds.Count > 0) ? Path.Combine(soundDir, sound.Sounds[0]) : "";
            soundFile1 = (sound.Sounds.Count > 1) ? Path.Combine(soundDir, sound.Sounds[1]) : "";
            soundMissing = Path.Combine(soundDir, SOUND_MISSING);
            stopAll = sound.AllStop || isNewGame; //Stop sound from last game if new.
            
            //After getting the sounds, consider them "played" and clear the sound object.
            sound.Clear();
            music.Clear();

            
        }

        private void Init()
        {
            musicFile = "";
            soundFile0 = "";
            soundFile1 = "";
            stopAll = false;
        }
    }

    /// <summary>
    /// The class used to encapsulate requests to manipulate a GameObject.
    /// </summary>
    public class EditRequest
    {
        #region Vars
        public RequestType requestType { get; set; }
        public DataNode targetNode { get; set; }
        #endregion

        #region Load / Init
        public EditRequest()
        {
            Init();
        }

        public EditRequest(RequestType reqType, DataNode targNode)
        {
            Init(reqType, targNode);

        }

        private void Init(
            RequestType reqType = RequestType.Unknown,
            DataNode targNode = null)
        {
            requestType = reqType;
            targetNode = targNode;
        }

        #endregion
    }

    public enum RequestType 
    {
        Unknown,
        Set,
        AddChild,
        AddSibling,
        AddParent,
        RemoveSelectedOnly,
        RemoveSelectedAndChildren,
    }

    /// <summary>
    /// The class used to encapsulate responses about an EditRequest.
    /// </summary>
    public class EditResponse
    {
        public string errMsg { get; set; }
        public string gameName { get; set; }
        public string affectedID { get; set; }
        public int nodesAffected { get; set; }

        //public EditResponse()
        //{
        //    Init();
        //}

        public EditResponse(string gameName)
        {
            Init();
            this.gameName = gameName;
        }

        private void Init()
        {
            errMsg = "";
            gameName = "";
            affectedID = "";
            nodesAffected = 0;
        }
    }

    public class GameMeta
    {
        #region Vars
        public string author { get; set; }
        public string about { get; set; }
        public string gameImg { get; set; }
        public string gameName { get; set; }
        #endregion

        #region Load Init

        public GameMeta(string gameName)
        {
            Init(gameName);
        }

        public GameMeta(string gameName, string author, string about, string gameImg)
        {
            Init(gameName);
            this.author = author;
            this.about = about;
            this.gameImg = gameImg;
        }

        private void Init(string gameName)
        {
            this.gameName = gameName;
            author = "";
            about = "";
            gameImg = "";
        }

        #endregion

        #region Get

        public string GetHtmlLegalAbout()
        {
            if (this.about != null && this.about != "")
            {
                string returnStr = this.about.Replace("<script>", "<p>").Replace("</script>", "</p>");
                returnStr = returnStr.Replace("<img", "<p").Replace("</img", "</p");
                returnStr = returnStr.Replace("style", "data-ignore");
                returnStr = returnStr.Replace("src", "data-ignore");
                returnStr = returnStr.Replace("<object", "<p").Replace("</object", "</p");
                return returnStr;
            }
            else
                return "";          
        }
        #endregion

    }
}
