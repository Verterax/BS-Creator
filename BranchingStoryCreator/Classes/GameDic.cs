using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace BranchingStoryCreator
{
    [ComVisible(true)]
    public class GameDic
    {
        public static string c_LIFE = "life";
        public static string c_MANA = "mana";
        public static string c_STAMINA = "stamina";


        #region Variables
        private Dictionary<string, string> _dic;
        #endregion
        #region Init / Constructors
        public GameDic()
        {
            Init();
        }

        private void Init()
        {
            _dic = new Dictionary<string,string>();
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
}
