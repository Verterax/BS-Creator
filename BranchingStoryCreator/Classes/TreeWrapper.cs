using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace BranchingStoryCreator
{
    /// <summary>
    /// Provides a simple interface for scripting tree goto commands.
    /// </summary>
    [ComVisible(true)]
    public class TreeWrapper
    {
        private StoryTree tree;

        public string pos 
        {
            get { return tree.pos; }
            set { tree.pos = value; }
        }

        public TreeWrapper(StoryTree tree)
        {
            Init(tree);
        }

        private void Init(StoryTree tree)
        {
            this.tree = tree;
        }

        


    }
}
