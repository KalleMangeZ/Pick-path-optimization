using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace ConsoleApp1;

public class GUI_createPickLocationsManyOrders : Form {
    Graph g;
    int aisleToAisleDist = 200;
    int shelfLength = 50;
    int shelfWidth = 50;
    Button confirmButton;
    List<ComboBox> comboBoxes;
    ComboBox selectOrderComboBox;


    private Pen bluePen = new Pen(Color.Blue, 2);

    public GUI_createPickLocationsManyOrders(Graph graph, object sender, EventArgs e) {
        this.g = graph;
        this.Size = new Size(1000, 700);
        this.Text = $"Choose Pick Locations for {g.orders} Orders.";
        this.Load += GUI_createPickLocationsManyOrders_Load;
        comboBoxes = new List<ComboBox>();

        confirmButton = new Button();
        confirmButton.Text = "Confirm Pick Locations";
        confirmButton.Width = 200;
        confirmButton.Height = 40;
        confirmButton.Location = new Point(50, g.shelvesPerAisle * shelfWidth + 100);

        Label selectOrderLabel = new Label();
        selectOrderLabel.Text = "Select Order Number:";
        selectOrderLabel.Location = new Point(350, g.shelvesPerAisle * shelfWidth + 112);
        selectOrderLabel.AutoSize = true;

        selectOrderComboBox = new ComboBox();
        selectOrderComboBox.Location = new Point(475, g.shelvesPerAisle * shelfWidth + 109);
        selectOrderComboBox.Width = 50;
        selectOrderComboBox.Height = 20;
        selectOrderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        for (int i = 1; i < g.orders + 1; i++) {
            selectOrderComboBox.Items.Add(i.ToString());
        }
        selectOrderComboBox.AutoSize = true;

        selectOrderComboBox.SelectedIndexChanged += (sender, e) =>
        {
            if (int.TryParse(selectOrderComboBox.SelectedItem?.ToString(), out int selectedOrder))
            {
                g.orderNbr = selectedOrder;
            }
        };


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
        this.Controls.Add(selectOrderLabel);
        this.Controls.Add(selectOrderComboBox);
    }

    private void GUI_createPickLocationsManyOrders_Load(object sender, EventArgs e)
    {
        CreateComboBoxesForOrders();
    }

    private void CreateSolution_Click(object sender, EventArgs e) {
        CreateSolutionWindow();
    }

     private void CreateSolutionWindow() {
        if (IsAnyComboBoxClicked() && g.IsEmptyLayout() == false) {
            GUI_solution window = new GUI_solution(g, g.pathNodes);
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
        comboBoxes.Add(comboBox);
    }

    protected override void OnPaint(PaintEventArgs e) {
    base.OnPaint(e);
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

 private void ChooseEmptyOrder_Click(object sender, EventArgs e) {
       if (selectOrderComboBox.SelectedItem == null) {
            MessageBox.Show("Please select an order number before confirming pick locations.");
            return;
        }
}

}
