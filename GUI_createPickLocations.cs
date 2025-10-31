namespace ConsoleApp1;

public class GUI_createPickLocations : Form {
    Graph g;
    int aisles;
    int shelvesPerAisle;
    int aisleToAisleDist = 200;
    int shelfLength = 50;
    int shelfWidth = 50;
    Button confirmButton;

    public GUI_createPickLocations(int aisles, int shelvesPerAisle, Graph g, object sender, EventArgs e) {
        this.aisles = aisles;
        this.shelvesPerAisle = shelvesPerAisle;
        this.g = g;
        this.Size = new Size(1000, 700);
        this.Text = "Choose Pick Locations for " + aisles + " Aisles and " + shelvesPerAisle + " Shelves per Aisle.";
        this.Paint += GUI_createPickLocations_Load;

        confirmButton = new Button();
        confirmButton.Text = "Confirm Pick Locations";
        confirmButton.Width = 200;
        confirmButton.Height = 40;
        confirmButton.Location = new Point(50, shelvesPerAisle * shelfWidth + 100);
        confirmButton.Click += new EventHandler(CreateSolution_Click);

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
                    CreateRackButton(xLoc, yLoc, x, y, firstAisleCol, i);
                }
            }
            firstAisleCol = firstAisleCol + 2;
        }
        //DrawStartAndEndCircle(pen2, graphics, shelfLength, g.shelvesPerAisle);

        confirmButton.Click += (sender, e) =>
            {
            // 1. Recreate layout from current GUI selections
            //g.LayoutManager.CreatePickLocationsFromGUI();
            // 2. Rebuild the graph and recompute shortest path
            g.path.Clear();
            g.pathNodes.Clear();
            g.nodes.Clear();
            g.createGraph();
            // 3. Optionally open the result visualization
            //CreateSolutionWindow();
            };

        this.Controls.Add(confirmButton);
    }

    private void CreateSolution_Click(object sender, EventArgs e) {
        CreateSolutionWindow();
    }

    private void CreateSolutionWindow() {
            GUI_solution window = new GUI_solution(g, g.pathNodes);
            window.ShowDialog();
    }
   
    /* Decide where in the layout[][] there will be a zero or one depending on click. */
    private void CreateRackButton(int xLoc, int yLoc, int xIndexLayout, int yIndexLayout, int firstAisleCol, int i) {
        Button rackButton = new Button();

        rackButton.Location = new Point(xLoc, yLoc);          
        rackButton.Width = 50;
        rackButton.Height = 50;
        rackButton.Text = "0";
        rackButton.Tag = false;

        rackButton.Click += (sender, e) => {
            bool isClicked = (bool)rackButton.Tag;

            if (isClicked) {
                rackButton.Text = "0";
                rackButton.Tag = false;
                g.LayoutManager.LayoutMatrix[yIndexLayout, xIndexLayout + firstAisleCol] = int.Parse(rackButton.Text);
            }
            else {
                rackButton.Text = "1";
                rackButton.Tag = true;
                Console.WriteLine("CLICKED - row: " + yIndexLayout + " col: " + (xIndexLayout + firstAisleCol));
                g.LayoutManager.LayoutMatrix[yIndexLayout, xIndexLayout + firstAisleCol] = int.Parse(rackButton.Text);
            }
           
            g.LayoutManager.LayoutMatrix[yIndexLayout, xIndexLayout + firstAisleCol] = int.Parse(rackButton.Text);
            g.LayoutManager.printLayout();
        };
        this.Controls.Add(rackButton);
    }

}
