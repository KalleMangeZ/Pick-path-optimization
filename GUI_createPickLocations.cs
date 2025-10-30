namespace ConsoleApp1;

public class GUI_createPickLocations : Form {
    Graph g;
    int aisles;
    int shelvesPerAisle;
    int aisleToAisleDist = 200;
    int shelfLength = 50;
    int shelfWidth = 50;

    public GUI_createPickLocations(int aisles, int shelvesPerAisle, Graph g, object sender, EventArgs e) {
        this.aisles = aisles;
        this.shelvesPerAisle = shelvesPerAisle;
        this.g = g;
        this.Size = new Size(1000, 700);

        this.Text = "Choose Pick Locations for " + aisles + " Aisles and " + shelvesPerAisle + " Shelves per Aisle.";

        this.Paint += GUI_createPickLocations_Load;

    }

    private void GUI_createPickLocations_Load(object sender, PaintEventArgs e) {
        displayNonConfiguredLayout(e.Graphics);
    }

    private void displayNonConfiguredLayout(Graphics graphics) {
        
        Pen pen = new Pen(Color.Blue, 2);
        Pen pen2 = new Pen(Color.Red, 2);

        int firstAisleCol = 0;
        for (int i = 0; i < aisles; i++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < shelvesPerAisle; y++) {
                    int xLoc = x * shelfLength + shelfLength + i * aisleToAisleDist;
                    int yLoc = y * shelfWidth + shelfWidth;
                    Rectangle rack = new Rectangle(xLoc, yLoc, shelfLength, shelfWidth); //(x, y, width, height)
                    graphics.DrawRectangle(pen, rack);
                    CreateRackButton(xLoc, yLoc, firstAisleCol, i);
                    /*using (Font font = new Font("Arial", 8))
                    {
                        SizeF textSize = graphics.MeasureString(text, font);
                        float textX = rack.X + (rack.Width - textSize.Width) / 2;
                        float textY = rack.Y + (rack.Height - textSize.Height) / 2;
                        graphics.DrawString(text, font, Brushes.Black, textX, textY);
                    } */
                }
            }
            firstAisleCol = firstAisleCol + 2;
        }
        //DrawStartAndEndCircle(pen2, graphics, shelfLength, g.shelvesPerAisle);
    }
    
    /* Decide where in the layout[][] there will be a zero or one depending on click. */
    private void CreateRackButton(int xLoc, int yLoc, int firstAisleCol, int i) {
        Button rackButton = new Button();

        rackButton.Location = new Point(xLoc, yLoc);          
        rackButton.Width = 50;
        rackButton.Height = 50;
        rackButton.Text = "0";
        rackButton.Tag = false;

        rackButton.Click += (sender, e) => {
            bool isClicked = (bool)rackButton.Tag;
            xLoc = xLoc / (xLoc * shelfLength + shelfLength + i * aisleToAisleDist);
            yLoc = yLoc / (yLoc * shelfWidth + shelfWidth);

            if (isClicked) {
                rackButton.Text = "0";
                g.LayoutManager.LayoutMatrix[yLoc, xLoc + firstAisleCol] = int.Parse(rackButton.Text);
                rackButton.Tag = false;
            }
            else {
                Console.WriteLine("CLICKED - xLoc: " + xLoc + " yLoc: " + yLoc);
                rackButton.Text = "1";
                rackButton.Tag = true;
            }
            
            //xLoc = xLoc / (xLoc * shelfLength + shelfLength + i * aisleToAisleDist);
            //yLoc = yLoc / (yLoc * shelfWidth + shelfWidth);
            //Console.WriteLine("yLoc: " + yLoc + " xLoc: " + xLoc + " firstAisleCol: " + firstAisleCol);
            g.LayoutManager.LayoutMatrix[yLoc, xLoc + firstAisleCol] = int.Parse(rackButton.Text);
            g.LayoutManager.printLayout();
        };
        this.Controls.Add(rackButton);
    }

}
