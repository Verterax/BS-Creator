using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BranchingStoryCreator
{
    public partial class InputBox : Form
    {
        #region Constructors

        public DialogResult result { get; set; }
        public string textResponse { get; set; }

        public string Prompt;
        public string Title;

        public InputBox(string title, string prompt)
        {
            InitializeComponent();
            Init(title, prompt);
        }

        private void Init(string title, string prompt)
        {
            Title = title;
            Prompt = prompt;

            this.Text = title;
            lblPrompt.Text = prompt;
        }

        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            result = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            result = DialogResult.Cancel;
        }

        private void InputBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            textResponse = txtInput.Text;
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOK.PerformClick();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {

        }

    }
}
