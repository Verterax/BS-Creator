using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // Confirm Delete Node
            Properties.Settings.Default.confirmNodeDelete = chkNodeDeleteConfirm.Checked;
            //Auto Load last project.
            Properties.Settings.Default.autoLoadLastProject = chkAutoLoadLast.Checked;







            Properties.Settings.Default.Save();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {

        }
    }
}
