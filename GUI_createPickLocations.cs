namespace ConsoleApp1;

public class GUI_createPickLocations : Form {
    private Button showPopupButton;

    public GUI_createPickLocations(Graph g) {
                this.Text = "Create Pick Locations";
                this.Width = 400;
                this.Height = 250;

                showPopupButton = new Button();
                showPopupButton.Text = "Show Pop-up";
                showPopupButton.Width = 120;
                showPopupButton.Height = 40;
                showPopupButton.Left = (this.ClientSize.Width - showPopupButton.Width) / 2;
                showPopupButton.Top = (this.ClientSize.Height - showPopupButton.Height) / 2;
                showPopupButton.Anchor = AnchorStyles.None;

                showPopupButton.Click += ShowPopupButton_Click;

                this.Controls.Add(showPopupButton);
    }


    private void ShowPopupButton_Click(object sender, EventArgs e) {
            // Option 1: simple message box
            // MessageBox.Show("This is a pop-up message!", "Info");

            // Option 2: open a custom pop-up form
            using (PopupForm popup = new PopupForm())
            {
                popup.ShowDialog();  // modal pop-up window
            }
    }

}
