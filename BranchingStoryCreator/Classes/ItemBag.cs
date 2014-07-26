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
    public class ItemBag
    {
        #region Variables
        public string itemImgPath;
        private Dictionary<string, Item> bag;

        const string IMG_EXT = ".png";

        #endregion

        #region Init Constructor
        public ItemBag(string itemImgPath)
        {
            Init(itemImgPath);
        }

        private void Init(string itemImgPath)
        {
            this.itemImgPath = itemImgPath;
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
                bag.Add(key, new Item(itemImgPath + key + IMG_EXT, "?", count));
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
                bag.Add(key, new Item(itemImgPath + key + IMG_EXT, desc, count));
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
                bag.Add(key, new Item(itemImgPath + key + IMG_EXT, "?", count));
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

        public List<Item> GetItems()
        {
            List<Item> items = new List<Item>();

            foreach (string key in bag.Keys)
                items.Add(bag[key]);

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


}
