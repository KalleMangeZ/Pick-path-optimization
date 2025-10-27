//namespace ConsoleApp1;
using System;
using System.Windows.Forms;
 // Custom pop-up window
    public class PopupForm : Form
    {
        private Label label;
        private Button okButton;

        public PopupForm()
        {
            this.Text = "Pop-up Window";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Width = 250;
            this.Height = 150;

            label = new Label();
            label.Text = "This is a custom pop-up window!";
            label.Dock = DockStyle.Top;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            label.Height = 50;

            okButton = new Button();
            okButton.Text = "OK";
            okButton.Dock = DockStyle.Bottom;
            okButton.Click += (s, e) => this.Close();

            this.Controls.Add(label);
            this.Controls.Add(okButton);
        }
    }
