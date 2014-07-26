namespace BranchingStoryCreator
{
    partial class GameViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameViewer));
            this.picImg = new System.Windows.Forms.PictureBox();
            this.rtfStory = new System.Windows.Forms.RichTextBox();
            this.btn4 = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn0 = new System.Windows.Forms.Button();
            this.panBag = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.GameTree = new System.Windows.Forms.TreeView();
            this.lblMana = new System.Windows.Forms.Label();
            this.lblHealth = new System.Windows.Forms.Label();
            this.lblStamina = new System.Windows.Forms.Label();
            this.chkSoundEnabled = new System.Windows.Forms.CheckBox();
            this.pbrMana = new BranchingStoryCreator.VerticalProgressBar();
            this.pbrLife = new BranchingStoryCreator.VerticalProgressBar();
            this.pbrStamina = new BranchingStoryCreator.VerticalProgressBar();
            this.bwStoryWriter = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).BeginInit();
            this.panBag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // picImg
            // 
            this.picImg.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.picImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picImg.Location = new System.Drawing.Point(127, 3);
            this.picImg.Name = "picImg";
            this.picImg.Size = new System.Drawing.Size(640, 480);
            this.picImg.TabIndex = 0;
            this.picImg.TabStop = false;
            // 
            // rtfStory
            // 
            this.rtfStory.BackColor = System.Drawing.Color.Black;
            this.rtfStory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtfStory.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtfStory.ForeColor = System.Drawing.SystemColors.Window;
            this.rtfStory.Location = new System.Drawing.Point(127, 483);
            this.rtfStory.Name = "rtfStory";
            this.rtfStory.ReadOnly = true;
            this.rtfStory.Size = new System.Drawing.Size(640, 112);
            this.rtfStory.TabIndex = 1;
            this.rtfStory.Text = "Story Goes here.";
            // 
            // btn4
            // 
            this.btn4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn4.ForeColor = System.Drawing.Color.White;
            this.btn4.Location = new System.Drawing.Point(717, 597);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(172, 54);
            this.btn4.TabIndex = 28;
            this.btn4.Text = "button4";
            this.btn4.UseVisualStyleBackColor = false;
            // 
            // btn3
            // 
            this.btn3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn3.ForeColor = System.Drawing.Color.White;
            this.btn3.Location = new System.Drawing.Point(539, 597);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(172, 54);
            this.btn3.TabIndex = 27;
            this.btn3.Text = "button3";
            this.btn3.UseVisualStyleBackColor = false;
            // 
            // btn2
            // 
            this.btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn2.ForeColor = System.Drawing.Color.White;
            this.btn2.Location = new System.Drawing.Point(361, 597);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(172, 54);
            this.btn2.TabIndex = 26;
            this.btn2.Text = "button2";
            this.btn2.UseVisualStyleBackColor = false;
            // 
            // btn1
            // 
            this.btn1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn1.ForeColor = System.Drawing.Color.White;
            this.btn1.Location = new System.Drawing.Point(183, 597);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(172, 54);
            this.btn1.TabIndex = 25;
            this.btn1.Text = "button1";
            this.btn1.UseVisualStyleBackColor = false;
            // 
            // btn0
            // 
            this.btn0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn0.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn0.ForeColor = System.Drawing.Color.White;
            this.btn0.Location = new System.Drawing.Point(6, 597);
            this.btn0.Name = "btn0";
            this.btn0.Size = new System.Drawing.Size(171, 54);
            this.btn0.TabIndex = 24;
            this.btn0.Text = "button0";
            this.btn0.UseVisualStyleBackColor = false;
            // 
            // panBag
            // 
            this.panBag.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panBag.BackgroundImage")));
            this.panBag.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panBag.Controls.Add(this.pictureBox2);
            this.panBag.Controls.Add(this.pictureBox1);
            this.panBag.Location = new System.Drawing.Point(773, 540);
            this.panBag.Name = "panBag";
            this.panBag.Size = new System.Drawing.Size(116, 56);
            this.panBag.TabIndex = 29;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(60, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(50, 50);
            this.pictureBox2.TabIndex = 35;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(5, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 50);
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            // 
            // GameTree
            // 
            this.GameTree.Location = new System.Drawing.Point(920, 6);
            this.GameTree.Name = "GameTree";
            this.GameTree.Size = new System.Drawing.Size(121, 97);
            this.GameTree.TabIndex = 30;
            // 
            // lblMana
            // 
            this.lblMana.AutoSize = true;
            this.lblMana.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMana.ForeColor = System.Drawing.Color.Blue;
            this.lblMana.Location = new System.Drawing.Point(87, 402);
            this.lblMana.Name = "lblMana";
            this.lblMana.Size = new System.Drawing.Size(40, 16);
            this.lblMana.TabIndex = 31;
            this.lblMana.Text = "1000";
            this.lblMana.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHealth
            // 
            this.lblHealth.AutoSize = true;
            this.lblHealth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHealth.ForeColor = System.Drawing.Color.DarkRed;
            this.lblHealth.Location = new System.Drawing.Point(46, 575);
            this.lblHealth.Name = "lblHealth";
            this.lblHealth.Size = new System.Drawing.Size(40, 16);
            this.lblHealth.TabIndex = 32;
            this.lblHealth.Text = "1000";
            this.lblHealth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblStamina
            // 
            this.lblStamina.AutoSize = true;
            this.lblStamina.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStamina.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblStamina.Location = new System.Drawing.Point(1, 402);
            this.lblStamina.Name = "lblStamina";
            this.lblStamina.Size = new System.Drawing.Size(40, 16);
            this.lblStamina.TabIndex = 33;
            this.lblStamina.Text = "1000";
            this.lblStamina.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkSoundEnabled
            // 
            this.chkSoundEnabled.AutoSize = true;
            this.chkSoundEnabled.BackColor = System.Drawing.Color.Transparent;
            this.chkSoundEnabled.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkSoundEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSoundEnabled.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.chkSoundEnabled.Location = new System.Drawing.Point(28, 12);
            this.chkSoundEnabled.Name = "chkSoundEnabled";
            this.chkSoundEnabled.Size = new System.Drawing.Size(70, 42);
            this.chkSoundEnabled.TabIndex = 34;
            this.chkSoundEnabled.Text = "Sound";
            this.chkSoundEnabled.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkSoundEnabled.UseVisualStyleBackColor = false;
            this.chkSoundEnabled.CheckedChanged += new System.EventHandler(this.chkToggleSound_CheckedChanged);
            // 
            // pbrMana
            // 
            this.pbrMana.BackColor = System.Drawing.Color.Black;
            this.pbrMana.ForeColor = System.Drawing.Color.Blue;
            this.pbrMana.Location = new System.Drawing.Point(91, 422);
            this.pbrMana.Maximum = 1000;
            this.pbrMana.Name = "pbrMana";
            this.pbrMana.Size = new System.Drawing.Size(30, 170);
            this.pbrMana.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbrMana.TabIndex = 2;
            this.pbrMana.Value = 50;
            // 
            // pbrLife
            // 
            this.pbrLife.BackColor = System.Drawing.Color.Black;
            this.pbrLife.ForeColor = System.Drawing.Color.DarkRed;
            this.pbrLife.Location = new System.Drawing.Point(49, 402);
            this.pbrLife.Maximum = 1000;
            this.pbrLife.Name = "pbrLife";
            this.pbrLife.Size = new System.Drawing.Size(30, 170);
            this.pbrLife.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbrLife.TabIndex = 1;
            this.pbrLife.Value = 50;
            // 
            // pbrStamina
            // 
            this.pbrStamina.BackColor = System.Drawing.Color.Black;
            this.pbrStamina.ForeColor = System.Drawing.Color.LimeGreen;
            this.pbrStamina.Location = new System.Drawing.Point(6, 422);
            this.pbrStamina.Maximum = 1000;
            this.pbrStamina.Name = "pbrStamina";
            this.pbrStamina.Size = new System.Drawing.Size(30, 170);
            this.pbrStamina.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbrStamina.TabIndex = 0;
            this.pbrStamina.Value = 50;
            // 
            // bwStoryWriter
            // 
            this.bwStoryWriter.WorkerSupportsCancellation = true;
            this.bwStoryWriter.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwStoryWriter_DoWork);
            this.bwStoryWriter.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwStoryWriter_RunWorkerCompleted);
            // 
            // GameViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SeaShell;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(895, 658);
            this.Controls.Add(this.chkSoundEnabled);
            this.Controls.Add(this.lblStamina);
            this.Controls.Add(this.lblHealth);
            this.Controls.Add(this.lblMana);
            this.Controls.Add(this.pbrMana);
            this.Controls.Add(this.GameTree);
            this.Controls.Add(this.pbrLife);
            this.Controls.Add(this.pbrStamina);
            this.Controls.Add(this.panBag);
            this.Controls.Add(this.btn4);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.btn0);
            this.Controls.Add(this.rtfStory);
            this.Controls.Add(this.picImg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GameViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adventure!";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameViewer_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameViewer_FormClosed);
            this.Load += new System.EventHandler(this.GameViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).EndInit();
            this.panBag.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picImg;
        private System.Windows.Forms.RichTextBox rtfStory;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn0;
        private System.Windows.Forms.Panel panBag;
        private System.Windows.Forms.TreeView GameTree;
        private VerticalProgressBar pbrMana;
        private VerticalProgressBar pbrLife;
        private VerticalProgressBar pbrStamina;
        private System.Windows.Forms.Label lblMana;
        private System.Windows.Forms.Label lblHealth;
        private System.Windows.Forms.Label lblStamina;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox chkSoundEnabled;
        private System.ComponentModel.BackgroundWorker bwStoryWriter;
    }
}