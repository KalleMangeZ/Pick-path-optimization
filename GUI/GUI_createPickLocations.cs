namespace ConsoleApp1;

public class GUI_createPickLocations : Form {
    Graph g;
    OrderSequenceAnalysis? analysis;
    int aisles;
    int shelvesPerAisle;
    int aisleToAisleDist = 200;
    int shelfLength = 50;
    int shelfWidth = 50;
    Button confirmButton;
    private List<Button> rackButtons;
    private Pen bluePen = new Pen(Color.Blue, 2);
    public GUI_createPickLocations(int aisles, int shelvesPerAisle, Graph g, object sender, EventArgs e) {
        this.aisles = aisles;
        this.shelvesPerAisle = shelvesPerAisle;
        this.g = g;
        this.Size = new Size(1000, 700);
        this.Text = "Choose Pick Locations for " + aisles + " Aisles and " + shelvesPerAisle + " Shelves per Aisle.";
        this.Load += GUI_createPickLocations_Load;
        rackButtons = new List<Button>();

        confirmButton = new Button();
        confirmButton.Text = "Confirm Pick Locations";
        confirmButton.Width = 200;
        confirmButton.Height = 40;
        confirmButton.Location = new Point(50, shelvesPerAisle * shelfWidth + 100);

         confirmButton.Click += (sender, e) =>
            {
            // 1. Recreate layout from current GUI selections
            //g.LayoutManager.CreatePickLocationsFromGUI();
            // 2. Rebuild the graph and recompute shortest path
            g.path.Clear();
            g.pathNodes.Clear();
            g.nodes.Clear();
            g.CreateGraph();
            CreateSolution_Click(sender, e);
            };
        this.Controls.Add(confirmButton);
    }

    private void GUI_createPickLocations_Load(object sender, EventArgs e) {
    CreateRackButtons();
    }

    private void CreateRackButtons() {
    int firstAisleCol = 0;
    for (int i = 0; i < aisles; i++) {
        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < shelvesPerAisle; y++) {
                int xLoc = x * shelfLength + shelfLength + i * aisleToAisleDist;
                int yLoc = y * shelfWidth + shelfWidth;
                CreateRackButton(xLoc, yLoc, x, y, firstAisleCol, i);
            }
        }
        firstAisleCol += 2;
    }
}

protected override void OnPaint(PaintEventArgs e) {
    base.OnPaint(e);
    DrawLayout(e.Graphics);
}

private void DrawLayout(Graphics g) {
    using (Pen pen = new Pen(Color.Blue, 2)) {
        int firstAisleCol = 0;
        for (int i = 0; i < aisles; i++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < shelvesPerAisle; y++) {
                    int xLoc = x * shelfLength + shelfLength + i * aisleToAisleDist;
                    int yLoc = y * shelfWidth + shelfWidth;
                    g.DrawRectangle(pen, xLoc, yLoc, shelfLength, shelfWidth);
                }
            }
            firstAisleCol += 2;
        }
    }
}
    private void CreateSolution_Click(object sender, EventArgs e) {
        CreateSolutionWindow();
    }

    private void CreateSolutionWindow() {
        if (IsAnyRackButtonClicked() && g.IsEmptyLayout() == false) {
            GUI_solution window = new GUI_solution(g, g.pathNodes, analysis);
            window.ShowDialog();
        }
    }

    /* Decide where in the layout[][] there will be a zero or one depending on click. */
    //Change to
    private void CreateRackButton(int xLoc, int yLoc, int xIndexLayout, int yIndexLayout, int firstAisleCol, int i) {
        Button rackButton = new Button();
        rackButtons.Add(rackButton);

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
                g.LayoutManager.LayoutMatrix[yIndexLayout, xIndexLayout + firstAisleCol] = int.Parse(rackButton.Text);
            }

            g.LayoutManager.LayoutMatrix[yIndexLayout, xIndexLayout + firstAisleCol] = int.Parse(rackButton.Text);
            //g.LayoutManager.printLayout();
        };
        this.Controls.Add(rackButton);
    }
    
    public bool IsAnyRackButtonClicked() {
        foreach (Button rackButton in rackButtons) {
            if (rackButton.Tag is bool clicked && clicked) {
                return true;
            }
        }
        return false;
    }

}
