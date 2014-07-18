namespace BranchingStoryCreator
{
    partial class Preferences
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkNodeDeleteConfirm = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkAutoLoadLast = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkNodeDeleteConfirm
            // 
            this.chkNodeDeleteConfirm.AutoSize = true;
            this.chkNodeDeleteConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNodeDeleteConfirm.Location = new System.Drawing.Point(33, 29);
            this.chkNodeDeleteConfirm.Name = "chkNodeDeleteConfirm";
            this.chkNodeDeleteConfirm.Size = new System.Drawing.Size(220, 28);
            this.chkNodeDeleteConfirm.TabIndex = 34;
            this.chkNodeDeleteConfirm.Text = "Confirm Node Deletion";
            this.chkNodeDeleteConfirm.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(123, 216);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(91, 45);
            this.btnOK.TabIndex = 35;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // chkAutoLoadLast
            // 
            this.chkAutoLoadLast.AutoSize = true;
            this.chkAutoLoadLast.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoLoadLast.Location = new System.Drawing.Point(33, 63);
            this.chkAutoLoadLast.Name = "chkAutoLoadLast";
            this.chkAutoLoadLast.Size = new System.Drawing.Size(216, 28);
            this.chkAutoLoadLast.TabIndex = 36;
            this.chkAutoLoadLast.Text = "Auto Load Last Project";
            this.chkAutoLoadLast.UseVisualStyleBackColor = true;
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 273);
            this.Controls.Add(this.chkAutoLoadLast);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkNodeDeleteConfirm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Preferences";
            this.Text = "Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Preferences_FormClosing);
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkNodeDeleteConfirm;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkAutoLoadLast;
    }
}