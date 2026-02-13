namespace ConsoleApp1;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class GUI_createPickLocationsManyOrders : Form {
    Graph g;
    GUI_solution window { get; set; }
    private OrderSequenceAnalysis graphAnalysis;
    int aisleToAisleDist = 200;
    int shelfLength = 50;
    int shelfWidth = 50;

    int comboBoxNbr = 0;
    int sequenceTextBoxLength;

    Button confirmButton;
    Button GenerateRandomPickLocationsButton;
    Button usePreset_1_Button;
    Label sequenceLabel;
    Label selectOrderLabel;
    TextBox sequenceTextBox;
    List<ComboBox> comboBoxes;
    CheckedListBox orderCheckListBox;
    CheckedListBox algorithmCheckListBox;
    HashSet<int> selectedAlgorithms;
    private Pen bluePen = new Pen(Color.Blue, 2);

    public GUI_createPickLocationsManyOrders(Graph graph, object sender, EventArgs e) {
        this.g = graph;
        this.Size = new Size(1000, 750);
        this.Text = $"Choose Pick Locations for {g.orders} Orders.";
        this.AutoScroll = true;            
        this.AutoScrollMinSize = new Size(0, 2000); 

        this.Load += GUI_createPickLocationsManyOrders_Load;
        comboBoxes = new List<ComboBox>();

        confirmButton = new Button();
        confirmButton.Text = "Confirm Pick Locations";
        confirmButton.Width = 200;
        confirmButton.Height = 40;
        confirmButton.Location = new Point(50, g.shelvesPerAisle * shelfWidth + 100);
        confirmButton.Click += (sender, e) =>
            {
            ApplyCheckedSearchAlgorithmsToGraph();
            ApplyCheckedOrdersToGraph();
            g.path.Clear();      //lägga till i Combinations
            g.pathNodes.Clear();
            g.nodes.Clear();
            g.CreateGraph();
            Combinations.RunCombinations(g, selectedAlgorithms); 
            CreateSolution_Click(sender, e); //--> graphAnalysis --> CreateUniqueFromOnlyOrderStacks
                if(window.OrderSequenceAnalysis.uniqueOrderStacks.Count > 0) {
                BoxStackingFromUniqueOrderStacks boxStacking =      //--> FillFirstLayerWithAllOrders (if...)
                    new BoxStackingFromUniqueOrderStacks(g, Combinations.LocalRandomSearch.configurations, window.OrderSequenceAnalysis.uniqueOrderStacks);
                }
            };

        GenerateRandomPickLocationsButton = new Button();
        GenerateRandomPickLocationsButton.Text = "Generate Random Pick Locations";
        GenerateRandomPickLocationsButton.Width = 200;
        GenerateRandomPickLocationsButton.Height = 40;
        GenerateRandomPickLocationsButton.Location = new Point(50, g.shelvesPerAisle * shelfWidth + 150);
        GenerateRandomPickLocationsButton.Click += (sender, e) =>
        {
            Random random = new Random();
            foreach (ComboBox comboBox in comboBoxes)
            {
                int randomIndex = random.Next(0, comboBox.Items.Count);
                comboBox.SelectedIndex = randomIndex;
            }
        };

        selectOrderLabel = new Label();
        selectOrderLabel.Text = "Select Orders:";
        selectOrderLabel.Location = new Point(340, g.shelvesPerAisle * shelfWidth + 96);
        selectOrderLabel.AutoSize = true;

        orderCheckListBox = new CheckedListBox();
        orderCheckListBox.Location = new Point(340, g.shelvesPerAisle * shelfWidth + 112);
        for (int i = 1; i < g.orders + 1; i++) {
            orderCheckListBox.Items.Add($"Order {i}");
        }
        orderCheckListBox.Width = 120;
        orderCheckListBox.Height = 80;        
        orderCheckListBox.CheckOnClick = true;

        Label selectSearchAlgorithms = new Label();
        selectSearchAlgorithms.Text = "Select Search Algorithm:";
        selectSearchAlgorithms.Location = new Point(550, g.shelvesPerAisle * shelfWidth + 96);
        selectSearchAlgorithms.AutoSize = true;
        algorithmCheckListBox = new CheckedListBox();
        algorithmCheckListBox.Location = new Point(550, g.shelvesPerAisle * shelfWidth + 112); 
        algorithmCheckListBox.Width = 150;
        algorithmCheckListBox.Height = 80;
        algorithmCheckListBox.CheckOnClick = true;
                algorithmCheckListBox.Items.Add("Brute Force");
                algorithmCheckListBox.Items.Add("Branch and Bound");
                algorithmCheckListBox.Items.Add("Random Search");
                
                for(int i = 0; i < algorithmCheckListBox.Items.Count; i++) {
                 if(i == 2) { //standard only random search for now.
                 algorithmCheckListBox.SetItemChecked(i, true);
                 }
                }

        sequenceLabel = new Label();
        sequenceLabel.Text = "Order sequence:";
        sequenceLabel.Location = new Point(50, g.shelvesPerAisle * shelfWidth + 200);
        sequenceLabel.AutoSize = true;

        sequenceTextBox = new TextBox();
        sequenceTextBox.Location = new Point(50, g.shelvesPerAisle * shelfWidth + 216);
        sequenceTextBox.Width = 200;
        // Only allow digits and spaces
        sequenceTextBox.KeyPress += SequenceTextBox_KeyPress;
        sequenceTextBox.TextChanged += SequenceTextBox_TextChanged;

        usePreset_1_Button = new Button();
        usePreset_1_Button.Text = "Preset 1 item locations";
        usePreset_1_Button.Width = 150;
        usePreset_1_Button.Height = 40;
        usePreset_1_Button.Location = new Point(50, g.shelvesPerAisle * shelfWidth + 256);
        usePreset_1_Button.Click += (sender, e) =>
        {
            UseDefinedItemLocations();
        };

        this.Controls.Add(sequenceLabel);
        this.Controls.Add(sequenceTextBox);
        this.Controls.Add(selectSearchAlgorithms);
        this.Controls.Add(algorithmCheckListBox);
        this.Controls.Add(confirmButton);
        this.Controls.Add(GenerateRandomPickLocationsButton);
        this.Controls.Add(selectOrderLabel);
        this.Controls.Add(usePreset_1_Button);
        this.Controls.Add(orderCheckListBox);
    }

    private void SequenceTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        // Allow control keys (Backspace, Delete, etc.)
        if (char.IsControl(e.KeyChar))
            return;
        // Allow digits
        if (char.IsDigit(e.KeyChar))
            return;
        // OPTIONAL: allow spaces
        if (e.KeyChar == ' ')
            return;
        // Block everything else
        e.Handled = true;
    }

    private void SequenceTextBox_TextChanged(object sender, EventArgs e)
    {
        List<int> sequence = sequenceTextBox.Text
            .Where(char.IsDigit)
            .Select(c => c - '0')
            .ToList();

        int selectedIndex = 0;
        sequence = ReverseInChunks(sequence, g.shelvesPerAisle);
        for(int i = 0; i < comboBoxes.Count; i = i + 2*g.shelvesPerAisle) {
           for(int j = 0; j < g.shelvesPerAisle; j++) {
                if ((i+j >= comboBoxes.Count) || (selectedIndex >= sequence.Count)) {
                return;
                }
                
                comboBoxes[i + j].SelectedIndex = sequence[selectedIndex];
                selectedIndex++;
            }
        }
    }

    public static List<int> ReverseInChunks(List<int> input, int x)
    {
        List<int> result = new List<int>();
        bool reverse = true; // first chunk reversed

        for (int i = 0; i < input.Count; i += x)
        {
            int end = Math.Min(i + x, input.Count);

            if (reverse)
            {
                for (int j = end - 1; j >= i; j--)
                {
                    result.Add(input[j]);
                }
            }
            else
            {
                for (int j = i; j < end; j++)
                {
                    result.Add(input[j]);
                }
            }

            reverse = !reverse; // toggle for next chunk
        }

        return result;
    }

    private void ApplyCheckedSearchAlgorithmsToGraph() {
        selectedAlgorithms = new HashSet<int>(); //0=RS, 1=BB, 2=BF
        foreach(int indexChecked in algorithmCheckListBox.CheckedIndices) {
            selectedAlgorithms.Add(indexChecked);
        }
    }

    private void ApplyCheckedOrdersToGraph() {
    HashSet<int> selectedOrders = new HashSet<int>();
        foreach(int indexChecked in orderCheckListBox.CheckedIndices) {
            selectedOrders.Add(indexChecked + 1);
        }

        if (selectedOrders.Count == 0)
            {
                MessageBox.Show("Please select at least one order.", "No orders selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        g.orderSet = selectedOrders;
    }

    private void GUI_createPickLocationsManyOrders_Load(object sender, EventArgs e)
    {
        CreateComboBoxesForOrders();
    }

    private void CreateSolution_Click(object sender, EventArgs e) {
        CreateSolutionWindow();
    }

     private void CreateSolutionWindow() {
        if (IsAnyComboBoxClicked() && g.IsEmptyLayout() == false && graphAnalysis == null) {
            graphAnalysis = new OrderSequenceAnalysis(g, Combinations.LocalRandomSearch.configurations); // Only created once
            OrderSequenceVisualization osv =
            new OrderSequenceVisualization(g, graphAnalysis);
            osv.Show();

        window = new GUI_solution(g, g.pathNodes, graphAnalysis);
        window.ShowDialog();
        }
    }

    private bool IsAnyComboBoxClicked() {
        foreach (ComboBox comboBox in comboBoxes) {
            if (comboBox.SelectedItem != null) {
                return true;
            }
        }
        return false;
    }

    private void CreateComboBoxesForOrders() {
    int firstAisleCol = 0;
        for (int i = 0; i < g.aisles; i++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < g.shelvesPerAisle; y++) {
                    int xLoc = x * shelfLength + shelfLength + i * aisleToAisleDist;
                    int yLoc = y * shelfWidth + shelfWidth;
                    CreateComboBox(xLoc, yLoc, x, y, firstAisleCol, i);
                }
            }
            firstAisleCol += 2;
        }
    }

        private void CreateComboBox(int xLoc, int yLoc, int xIndexLayout, int yIndexLayout, int firstAisleCol, int aisleIndex) {
            ComboBox comboBox = new ComboBox();
            comboBoxes.Add(comboBox);

            comboBox.Location = new Point(xLoc + shelfLength / 10, yLoc + shelfWidth / 4);
            comboBox.Width = shelfLength-10;
            comboBox.Height = shelfWidth / 2;

            comboBox.Items.Add("0"); // Option for no pick location
            for(int i = 1; i < g.orders+1; i++) {
                comboBox.Items.Add(i.ToString());
            }
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Event handler for selection change
            comboBox.SelectedIndexChanged += (sender, e) =>
            {
                if (int.TryParse(comboBox.SelectedItem?.ToString(), out int selectedValue))
                {
                    g.LayoutManager.LayoutMatrix[yIndexLayout, xIndexLayout + firstAisleCol] = selectedValue;
                }
            };

            this.Controls.Add(comboBox);
        }

        protected override void OnPaint(PaintEventArgs e) {
        base.OnPaint(e);

        // Move drawing based on scroll position
        var gTrans = e.Graphics;
        gTrans.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
        DrawLayout(gTrans);

        DrawLayout(e.Graphics);
        }

    private void DrawLayout(Graphics graphics) {
        using (Pen pen = new Pen(Color.Blue, 2)) {
            int firstAisleCol = 0;
            for (int i = 0; i < g.aisles; i++) {
                for (int x = 0; x < 2; x++) {
                    for (int y = 0; y < g.shelvesPerAisle; y++) {
                        int xLoc = x * shelfLength + shelfLength + i * aisleToAisleDist;
                        int yLoc = y * shelfWidth + shelfWidth;
                        graphics.DrawRectangle(pen, xLoc, yLoc, shelfLength, shelfWidth);
                    }
                }
                firstAisleCol += 2;
            }
        }
    }
    
    public void UseDefinedItemLocations() {
        //preset 1:
        if(g.aisles !=5 && g.shelvesPerAisle != 8 && g.orders != 20)  {
        return;
        }
        comboBoxes[0].SelectedIndex  = 3;   comboBoxes[16].SelectedIndex = 17;  comboBoxes[32].SelectedIndex = 13;  comboBoxes[48].SelectedIndex = 10;  comboBoxes[64].SelectedIndex = 4;
        comboBoxes[1].SelectedIndex  = 15;  comboBoxes[17].SelectedIndex = 15;  comboBoxes[33].SelectedIndex = 5;   comboBoxes[49].SelectedIndex = 17;  comboBoxes[65].SelectedIndex = 5;
        comboBoxes[2].SelectedIndex  = 8;   comboBoxes[18].SelectedIndex = 10;  comboBoxes[34].SelectedIndex = 5;   comboBoxes[50].SelectedIndex = 4;   comboBoxes[66].SelectedIndex = 0;
        comboBoxes[3].SelectedIndex  = 17;  comboBoxes[19].SelectedIndex = 11;  comboBoxes[35].SelectedIndex = 8;   comboBoxes[51].SelectedIndex = 8;   comboBoxes[67].SelectedIndex = 16;
        comboBoxes[4].SelectedIndex  = 9;   comboBoxes[20].SelectedIndex = 18;  comboBoxes[36].SelectedIndex = 9;   comboBoxes[52].SelectedIndex = 1;   comboBoxes[68].SelectedIndex = 6;
        comboBoxes[5].SelectedIndex  = 9;   comboBoxes[21].SelectedIndex = 6;   comboBoxes[37].SelectedIndex = 10;  comboBoxes[53].SelectedIndex = 7;   comboBoxes[69].SelectedIndex = 16;
        comboBoxes[6].SelectedIndex  = 11;  comboBoxes[22].SelectedIndex = 7;   comboBoxes[38].SelectedIndex = 19;  comboBoxes[54].SelectedIndex = 7;   comboBoxes[70].SelectedIndex = 0;
        comboBoxes[7].SelectedIndex  = 16;  comboBoxes[23].SelectedIndex = 9;   comboBoxes[39].SelectedIndex = 3;   comboBoxes[55].SelectedIndex = 9;   comboBoxes[71].SelectedIndex = 18;
        comboBoxes[8].SelectedIndex  = 20;  comboBoxes[24].SelectedIndex = 20;  comboBoxes[40].SelectedIndex = 8;   comboBoxes[56].SelectedIndex = 2;   comboBoxes[72].SelectedIndex = 15;
        comboBoxes[9].SelectedIndex  = 18;  comboBoxes[25].SelectedIndex = 8;   comboBoxes[41].SelectedIndex = 18;  comboBoxes[57].SelectedIndex = 6;   comboBoxes[73].SelectedIndex = 3;
        comboBoxes[10].SelectedIndex = 19;  comboBoxes[26].SelectedIndex = 20;  comboBoxes[42].SelectedIndex = 7;   comboBoxes[58].SelectedIndex = 6;   comboBoxes[74].SelectedIndex = 17;
        comboBoxes[11].SelectedIndex = 2;   comboBoxes[27].SelectedIndex = 10;  comboBoxes[43].SelectedIndex = 20;  comboBoxes[59].SelectedIndex = 12;  comboBoxes[75].SelectedIndex = 3;
        comboBoxes[12].SelectedIndex = 6;   comboBoxes[28].SelectedIndex = 3;   comboBoxes[44].SelectedIndex = 4;   comboBoxes[60].SelectedIndex = 3;   comboBoxes[76].SelectedIndex = 4;
        comboBoxes[13].SelectedIndex = 7;   comboBoxes[29].SelectedIndex = 2;   comboBoxes[45].SelectedIndex = 17;  comboBoxes[61].SelectedIndex = 16;  comboBoxes[77].SelectedIndex = 7;
        comboBoxes[14].SelectedIndex = 1;   comboBoxes[30].SelectedIndex = 8;   comboBoxes[46].SelectedIndex = 4;   comboBoxes[62].SelectedIndex = 6;   comboBoxes[78].SelectedIndex = 4;
        comboBoxes[15].SelectedIndex = 14;  comboBoxes[31].SelectedIndex = 16;  comboBoxes[47].SelectedIndex = 6;   comboBoxes[63].SelectedIndex = 11;  comboBoxes[79].SelectedIndex = 3;
    } 
}
