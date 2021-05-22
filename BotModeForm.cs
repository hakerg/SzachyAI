﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SzachyAI
{
    public partial class BotModeForm : Form
    {
        //Make form dragable at every point
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        MenuForm menuForm;

        public BotModeForm(MenuForm menuForm)
        {
            this.menuForm = menuForm;
            InitializeComponent();
        }
        private void BotModeForm_Load(object sender, EventArgs e)
        {
            recognisedBoardPcBox.Visible = Settings.enableDebugMode;
            boardLabel.Visible = Settings.enableDebugMode;
            if (Settings.enableDebugMode)
            {
                this.Size = new Size(500, 500);
                commandLabel.Location = new Point(32, 407);
            }
            else 
            {
                commandLabel.Location = new Point(32, 207);
                this.Size = new Size(500, 300); 
            }
        }
        private void scanButton_Click(object sender, EventArgs e)
        {
            menuForm.detectBoardOnce = true;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (startButton.Text == "Start") {
                menuForm.runBot = true;
                startButton.Text = "Stop";
            } else {
                menuForm.runBot = false;
                startButton.Text = "Start";
            }
        }

        private void BotModeForm_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void BotModeForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void BotModeForm_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void BotModeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            menuForm.InvalidateBorder();
            menuForm.Show();
        }


        private void BotModeForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        public void UpdateStatus() {
            commandLabel.Text = menuForm.status;
            notifyIcon1.Text = "ChessAI: " + menuForm.status;
            recognisedBoardPcBox.Image = menuForm.constructedImage;
        }
    }
}
