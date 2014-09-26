using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BranchingStoryCreator
{
    public partial class Preferences : Form
    {
        public Preferences()
        {
            InitializeComponent();
        }

        private void Preferences_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Confirm Delete Node
                Properties.Settings.Default.confirmNodeDelete = chkNodeDeleteConfirm.Checked;
                //Auto Load last project.
                Properties.Settings.Default.autoLoadLastProject = chkAutoLoadLast.Checked;
                Properties.Settings.Default.Save();
            }
            catch (Exception){ }
        }

        private void Preferences_Load(object sender, EventArgs e)
        {

        }
    }
}
