using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace BranchingStoryCreator.Web
{
    #region Tree Postion
    /// <summary>
    /// Provides a simple interface for scripting the player's location property.
    /// </summary>
    [ComVisible(true)]
    public class LocWrapper
    {
        private PlayerData playerData;

        public string pos
        {
            get { return playerData.currentPos; }
            set { playerData.currentPos = value; }
        }

        public LocWrapper(PlayerData player)
        {
            Init(player);
        }

        private void Init(PlayerData player)
        {
            this.playerData = player;
        }

    }

    #endregion

    #region Sound
    [ComVisible(true)]
    public class Sound
    {
        #region Variables

        public List<string> Sounds { get; private set; }
        public string Music { get; private set; }

        public bool AllStop { get; private set; }

        #endregion

        #region Consts
        public static string SOUND_MISSING = "sound_missing.mp3";
        #endregion

        #region Init / Constructor
        public Sound()
        {
            Init();
        }

        private void Init()
        {
            Sounds = new List<string>();
            Music = "";
            AllStop = false;
        }

        #endregion

        public string this[string soundName]
        {
            get
            {
                Sounds.Add(soundName + ".mp3");          
                return "";
            }
        }

        public string music(string musicName)
        {
            this.Music = musicName;
            return "";
        }

        public void StopAll()
        {
            this.Clear();
            AllStop = true;
        }

        #region Misc

        /// <summary>
        /// Called after the a presentation object is constructed with the Sound object, considered "played".
        /// </summary>
        public void Clear()
        {
            Sounds.Clear();
            Music = "";
            AllStop = false;
        }

        #endregion

    }

    [ComVisible(true)]
    public class Music
    {
        #region Variables
        public string music { get; private set; }
        #endregion

        #region Init / Constructor
        public Music()
        {
            Init();
        }

        private void Init()
        {
            music = "";
        }

        #endregion

        public string this[string soundName]
        {
            get
            {
                music = soundName + ".mp3";
                return "";
            }
        }

        public void Clear()
        {
            music = "";
        }

    }

    #endregion

    #region Item Bag / Item
    [ComVisible(true)]
    public class ItemBag
    {
        #region Variables
        //public string itemImgPath;
        private Dictionary<string, Item> bag;

        const string IMG_EXT = ".png";

        #endregion

        #region Init Constructor
        public ItemBag()
        {
            Init();
        }

        private void Init()
        {
            //this.itemImgPath = itemImgPath;
            bag = new Dictionary<string, Item>();
        }

        #endregion

        #region Bag Access

        public void Inc(string key, int count)
        {
            if (bag.ContainsKey(key))
            {
                bag[key].count += count;

                if (bag[key].count <= 0)
                    bag.Remove(key);
            }
            else
            {
                bag.Add(key, new Item(key + IMG_EXT, "?", count));
            }
        }

        /// <summary>
        /// Add Item to bag. ItemName is key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="desc"></param>
        /// <param name="count"></param>
        public void Add(string key, string desc, int count)
        {
            if (bag.ContainsKey(key))
            {
                bag[key].desc = desc;
                Add(key, count);
            }
            else
            {
                bag.Add(key, new Item(key + IMG_EXT, desc, count));
            }
        }
        public void Add(string key, int count)
        {
            if (bag.ContainsKey(key))
            {
                bag[key].count += count;

                if (bag[key].count <= 0)
                    bag.Remove(key);
            }
            else
            {
                bag.Add(key, new Item(key + IMG_EXT, "?", count));
            }
        }



        public void Set(string key, int count)
        {
            if (bag.ContainsKey(key))
                bag[key].count = count;
            else
                this.Add(key, "", count);
        }

        public void Set(string key, string desc, int count)
        {
            if (bag.ContainsKey(key))
            {
                bag[key].count = count;
                bag[key].desc = desc;
            }
            else
                this.Add(key, desc, count);
        }

        public void Remove(string key)
        {
            if (bag.ContainsKey(key))
                bag.Remove(key);
        }

        public int Count(string key)
        {
            if (bag.ContainsKey(key))
                return bag[key].count;
            else
                return 0;
        }

        public string Desc(string key)
        {
            if (bag.ContainsKey(key))
                return bag[key].desc;
            else
                return "";
        }

        public bool Has(string key)
        {
            if (bag.ContainsKey(key))
                return true;
            else
                return false;
        }

        #endregion

        #region Misc

        public List<string> GetFormattedValues()
        {
            List<string> values = new List<string>();

            foreach (string key in bag.Keys)
            {
                string item = "";
                if (bag[key].count > 1)
                    item = string.Format("{0} ( {1} )", Path.GetFileNameWithoutExtension(bag[key].imgURL), bag[key].count);
                else
                    item = string.Format("{0} ( {1} )", Path.GetFileNameWithoutExtension(bag[key].imgURL), bag[key].count);

                values.Add(item);
            }

            return values;
        }

        public void Clear()
        {
            bag.Clear();
        }

        #endregion

        #region Presentation

        public List<Item> GetItems(string itemImgDir)
        {
            List<Item> items = new List<Item>();

            foreach (string key in bag.Keys)
            {
                bag[key].imgURL = Path.Combine(itemImgDir, bag[key].imgURL);
                items.Add(bag[key]);
                
            }

            return items;
        }

        #endregion



    }

    [ComVisible(true)]
    public class Item
    {
        public string imgURL;
        public string desc;
        public int count;

        public Item(string imgURL, string desc, int count)
        {
            this.imgURL = imgURL;
            this.desc = desc;
            this.count = count;
        }

        public Item()
        {

        }
    }
    #endregion

    #region Player Data

    /// <summary>
    /// A wrapper class that provides a scripable interface to the PlayerData class.
    /// </summary>
    [ComVisible(true)]
    public class PlayerVariables
    {
        public static string c_LIFE = "life";
        public static string c_MANA = "mana";
        public static string c_STAMINA = "stamina";


        #region Variables
        private Dictionary<string, string> _dic;
        #endregion
        #region Init / Constructors
        public PlayerVariables()
        {
            Init();
        }

        private void Init()
        {
            _dic = new Dictionary<string, string>();
        }

        #endregion

        #region Functionality

        public string this[string key]
        {
            get
            {

                if (key == null)
                    return "null";

                if (_dic.ContainsKey(key))
                {
                    return _dic[key];
                }
                else
                {
                    return "";
                }
            }

            set
            {
                if (key == null)
                    return;

                if (_dic.ContainsKey(key))
                {
                    _dic[key] = value;
                }
                else
                {
                    _dic.Add(key, value);
                }
            }
        }

        #endregion

        #region Presentation

        public int GetLife()
        {
            if (_dic.ContainsKey(c_LIFE))
                return int.Parse(this[c_LIFE]);
            else
                return -1;
        }

        public int GetMana()
        {
            if (_dic.ContainsKey(c_MANA))
                return int.Parse(this[c_MANA]);
            else
                return -1;
        }

        public int GetStamina()
        {
            if (_dic.ContainsKey(c_STAMINA))
                return int.Parse(this[c_STAMINA]);
            else
                return -1;
        }

        #endregion

        #region Misc

        public List<string> GetFormattedPairs()
        {
            List<string> pairs = new List<string>();

            foreach (string key in _dic.Keys)
            {
                pairs.Add(string.Format("{0} = {1}", key, _dic[key]));
            }

            return pairs;
        }

        /// <summary>
        /// Replaces the signature [value] with the corresponding data in the string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns> Text after [value] is replaced. </returns>
        public string ReplaceKeysWithValues(string text)
        {
            foreach (string key in _dic.Keys)
            {
                string find = string.Format("[{0}]", key);

                if (text.Contains(find))
                    text = text.Replace(find, this[key]);
            }

            return text;
        }

        public void Clear()
        {
            _dic.Clear();
        }

        #endregion

    }

    /// <summary>
    /// Provides a scriptable way to request and accept user text input.
    /// </summary>
    [ComVisible(true)]
    public class Input
    {
        #region Vars
        public string destVarName { get; private set; }
        public string prompt { get; private set; }
        public bool inputPending { get; private set; }
        #endregion

        #region Load / Init
        public Input()
        {
            Init();
        }

        private void Init()
        {
            destVarName = "";
            prompt = "";
            inputPending = false;
        }

        #endregion

        #region Functionality

        /// <summary>
        /// Scriptable function to activate the input box.
        /// </summary>
        public void box(string destVarName, string prompt)
        {
            this.destVarName = destVarName;
            this.prompt = prompt;
            this.inputPending = true;
        }

        /// <summary>
        /// Recieves a value that the box will insert into the Player Variables dictionary.
        /// </summary>
        /// <param name="value"></param>
        public void RecieveBox(string value, PlayerVariables playerVariables)
        {
            playerVariables[destVarName] = value;
            inputPending = false;
        }

        #endregion


    }

    #endregion
}
