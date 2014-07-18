using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VerticalProgressBar;
using System.IO;

namespace BranchingStoryCreator
{
    public partial class GameViewer : Form
    {

        const int MARGIN = 3;
        const int ITEM_SIZE = 50;

        GameObject GameObj;
        List<string> StoryChunks;       
        Button[] Buttons;
        Point[] ButtonLocs;
        int ButtonAddSpace;
        int chunkLoc;
        

        #region Init Load Construct

        public GameViewer(GameObject gameObj)
        {
            InitializeComponent();
            Init(gameObj);
        }

        private void GameViewer_Load(object sender, EventArgs e)
        {
        }

        private void Init(GameObject gameObj)
        {
            this.GameObj = gameObj;
            this.DoubleBuffered = true;                
            InitButtons();
            AddEvents();

            BindScene();            
        }

        private void InitButtons()
        {
            Buttons = new Button[5];
            Buttons[0] = btn0;
            Buttons[1] = btn1;
            Buttons[2] = btn2;
            Buttons[3] = btn3;
            Buttons[4] = btn4;

            ButtonLocs = new Point[5];

            for (int i = 0; i < Buttons.Count(); i++)
                ButtonLocs[i] = new Point(Buttons[i].Location.X, Buttons[i].Location.Y);

            ButtonAddSpace = (Buttons[1].Location.X - Buttons[0].Location.X) / 2;
        }

        #endregion

        #region Run Game

        private void BindScene()
        {
            PresentationObject presentation = GameObj.presentation;
            rtfStory.Text = presentation.story;

            DisplayMainImg(presentation.imgURL);

            DisplayLife(presentation.life);
            DisplayMana(presentation.mana);
            DisplayStamina(presentation.stamina);

            HandleButtons(presentation);
            HandleItems(presentation);

        }

        private void DisplayMainImg(string mainImg)
        {
            if (File.Exists(mainImg))
                picImg.BackgroundImage = new Bitmap(mainImg);
            else
                picImg.BackgroundImage = null;
        }

        private void DisplayLife(int life)
        {
            if (life > -1)
            {
                pbrLife.Value = life;
                lblHealth.Text = life.ToString();
            }
            else
            {
                pbrLife.Value = 0;
                lblHealth.Text = pbrLife.Value.ToString();
            }

        }

        private void DisplayMana(int mana)
        {
            if (mana > 0)
            {
                lblMana.Visible = true;
                pbrMana.Visible = true;
                pbrMana.Value = mana;
                lblMana.Text = mana.ToString();
            }
            else
            {
                lblMana.Visible = false;
                pbrMana.Visible = false;
            }
        }

        private void DisplayStamina(int stamina)
        {
            if (stamina > 0)
            {
                pbrStamina.Visible = true;
                lblStamina.Visible = true;
                pbrStamina.Value = stamina;
                lblStamina.Text = stamina.ToString();
            }
            else
            {
                lblStamina.Visible = false;
                pbrStamina.Visible = false;
            }
        }

        private void MakeStoryChunks(string rawStory)
        {
            chunkLoc = 0;
            string[] chunks = rawStory.Split('\n');
            StoryChunks = chunks.ToList();
        }

        private string GetStoryChunk()
        {
            string storyChunk = "Should not see.";
            
            if (StoryChunks.Count > chunkLoc)         
                storyChunk = StoryChunks[chunkLoc];

            return storyChunk;
        }

        private void LoadImg(string filePath)
        {
            if (File.Exists(filePath))
            {
                Bitmap img = new Bitmap(filePath);
                picImg.BackgroundImage = img;
            }
            else
            {
                picImg.Image = null;
            }

        }

        public void ForceUpdate()
        {
            GameObj.InvalidatePresentation();
            BindScene();
        }

        #endregion

        #region Items

        private void HandleItems(PresentationObject presentation)
        {
            int numberOfItems = presentation.items.Count;

            panBag.Visible = false; //Hide bag while repositioning.

            ResizeBag(numberOfItems);
            ArrangeItems(presentation, numberOfItems);

            panBag.Visible = true; //Show bag after everything in it is set.
        }

        private void ResizeBag(int numberOfItems)
        {
            int bag_width = ITEM_SIZE * 2 + MARGIN * 3;

            Control relativeControl = rtfStory;
            Point bagBottomLeft = new Point(
                relativeControl.Location.X + relativeControl.Width + MARGIN * 2,
                relativeControl.Location.Y + relativeControl.Height);

            int bagHeight = 0;// ITEM_SIZE + (MARGIN * 2); //Min height
            int extraHeight = (((numberOfItems + 1) / 2) * (ITEM_SIZE + MARGIN)); // + Item height for every 2 items in the bag.
            bagHeight += extraHeight;

            Size bagSize = new Size(bag_width, bagHeight);
            panBag.Size = bagSize;
            panBag.Location = new Point(bagBottomLeft.X, bagBottomLeft.Y - bagHeight);
        }

        private void ArrangeItems(PresentationObject presentation, int numberOfItems)
        {
            panBag.Controls.Clear();
              int leftColX = MARGIN;
            int rightColX = ITEM_SIZE + MARGIN * 2;
            int rowY = MARGIN;
            bool left = true;
            string itemURL = "";

            for (int i = 0; i < numberOfItems; i++)
            {
                Item item = presentation.items[i];

                PictureBox itemBox = new PictureBox();
                itemBox.BackColor = Color.Transparent;
                panBag.Controls.Add(itemBox);
                itemBox.Size = new Size(ITEM_SIZE, ITEM_SIZE);
                itemBox.Tag = presentation.items[i];
                itemURL = item.imgURL;

                //Try to load the image for the item.
                try
                {
                    if (!File.Exists(itemURL)) //If img not found, set img missing.
                        itemURL = Path.Combine(GameObj.GetItemImgPath(), GameObject.ITEM_MISSING);
                    itemBox.Image = new Bitmap(itemURL);                              
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to load item image {0}", item.imgURL);
                    continue;
                }

                ToolTip tip = new ToolTip();
                string tooltipText = string.Format("( {0} ) {1}", item.count, item.desc);
                tip.SetToolTip(itemBox, tooltipText);    
                
                //Place item frames left to right, top to bottom.
                if (left)
                {
                    itemBox.Location = new Point(leftColX, rowY);
                    left = false;
                }
                else
                {
                    itemBox.Location = new Point(rightColX, rowY);
                    left = true;
                    rowY += ITEM_SIZE + MARGIN;
                }         
            }
        }     

        #endregion

        #region Buttons

        private void PerformButtonLayout(int activeButtons)
        {
            int buttonShiftRightAmount = 0;

            if (activeButtons == 0)
            {
                ShowRestartButton();
                return;
            }

            PlaceButtonsHome();

            int placesToMove = 5 - activeButtons;
            buttonShiftRightAmount = placesToMove * ButtonAddSpace;

            for (int i = 0; i < Buttons.Count(); i++)
            {
                Buttons[i].Location = new Point(
                    Buttons[i].Location.X + buttonShiftRightAmount,
                    Buttons[i].Location.Y);

                if (Buttons[i].Text != "")
                    Buttons[i].Visible = true;
                else
                    Buttons[i].Visible = false;
            }

        }

        private void PlaceButtonsHome()
        {
            for (int i = 0; i < Buttons.Count(); i++)
            {
                Buttons[i].Location = new Point(ButtonLocs[i].X, ButtonLocs[i].Y);
            }
        }

        private void HandleButtons(PresentationObject presentation)
        {
            List<GameButton> gameButtons = presentation.buttonData;
            int showCount = gameButtons.Count;
            int maxButtons = Buttons.Count();

            for (int i = 0; i < maxButtons; i++)
            {
                if (i < showCount)
                {
                    Buttons[i].Text = gameButtons[i].PreText;
                    Buttons[i].Tag = gameButtons[i];
                }
                else
                {
                    Buttons[i].Text = "";
                    Buttons[i].Tag = null;        
                }
            }

                PerformButtonLayout(showCount);       
        }

        private void ShowRestartButton()
        {
            PlaceButtonsHome();
            foreach (Button btn in Buttons)
                btn.Visible = false;

            Buttons[2].Text = "End. Restart?";
            Buttons[2].Visible = true;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (IsRestartShowing())
            {
                GameObj.StartFromBeginning();
            }
            else
            {
                GameButton gameButton = btn.Tag as GameButton;
                if (!GameObj.ButtonPress(gameButton.NodeID, true))
                    MessageBox.Show("Unable to navigate to node: " + gameButton.NodeID);
            }
        }

        private bool IsRestartShowing()
        {
            return (!Buttons[1].Visible &&
                     Buttons[2].Visible && //Buttons 1,3 not visible, button 2 visible = Restart condition.
                    !Buttons[3].Visible);
        }

        #endregion

        #region Events

        private void AddEvents()
        {
            GameObj.Presentation_Changed += OnPositionChanged;

            foreach(Button btn in Buttons)
                btn.Click += btn_Click;
        }


        private void RemoveEvents()
        {
            GameObj.Presentation_Changed -= OnPositionChanged;

            foreach (Button btn in Buttons)
                btn.Click += btn_Click;
        }

        private void OnPositionChanged(object sender, EventArgs e)
        {
            BindScene();
        }

        private void GameViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //RemoveEvents();
        }

        private void GameViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }


        #endregion

    }
}
