namespace ConsoleApp1;

public class StartupWindow : Form {
    Label aislesLabel;
    ComboBox aislesChoice;
    Label nbrShelvesLabel;
    ComboBox nbrShelvesChoice;
    Button CreatePickLocations;
    Label nbrOrdersLabel;
    ComboBox nbrOrdersChoice;
    Label nbrOrdersPerLayerLabel;
    ComboBox nbrOrdersPerLayerChoice;
    public int selectedAisles;
    public int selectedNbrShelves;
    public int selectedNbrOrders;
    public int selectedNbrOrdersPerLayer;
    Graph g;
    public int aisleToAisleDist = 200;

    //Org: StartupWindow(Graph g) {
    public StartupWindow() {
                //this.g = g;
                this.Text = "Create Pick Locations";
                this.Width = 400;
                this.Height = 450;
                this.CenterToScreen();
                
                aislesLabel = new Label();
                aislesLabel.Location = new Point(115, 56);
                aislesLabel.Text = "Enter number of aisles:"; 
                aislesLabel.Width = 100;
                aislesLabel.AutoSize = true;

                aislesChoice = new ComboBox();
                aislesChoice.Location = new Point(250, 50);
                aislesChoice.Width = 50;
                aislesChoice.Items.Add("3");
                aislesChoice.Items.Add("4");
                aislesChoice.Items.Add("5");
                aislesChoice.Items.Add("6"); 
                aislesChoice.DropDownStyle = ComboBoxStyle.DropDownList;

                nbrShelvesLabel = new Label();
                nbrShelvesLabel.Location = new Point(60, 100);
                nbrShelvesLabel.Text = "Enter number of shelves per aisle:"; 
                nbrShelvesLabel.Width = 100;
                nbrShelvesLabel.AutoSize = true;

                nbrShelvesChoice = new ComboBox();
                nbrShelvesChoice.Location = new Point(250, 94);
                nbrShelvesChoice.Width = 50;
                nbrShelvesChoice.Items.Add("3");
                nbrShelvesChoice.Items.Add("4");
                nbrShelvesChoice.Items.Add("5");
                nbrShelvesChoice.Items.Add("6");
                nbrShelvesChoice.Items.Add("7");
                nbrShelvesChoice.Items.Add("8");
                nbrShelvesChoice.Items.Add("9");
                nbrShelvesChoice.Items.Add("10");                               
                nbrShelvesChoice.Items.Add("16");
                nbrShelvesChoice.DropDownStyle = ComboBoxStyle.DropDownList;

                nbrOrdersLabel = new Label();
                nbrOrdersLabel.Location = new Point(115, 144);
                nbrOrdersLabel.Text = "Enter number of orders:"; 
                nbrOrdersLabel.Width = 100;
                nbrOrdersLabel.AutoSize = true;

                nbrOrdersChoice = new ComboBox();
                nbrOrdersChoice.Location = new Point(250, 138);
                nbrOrdersChoice.Width = 50;
                nbrOrdersChoice.Items.Add("1");
                nbrOrdersChoice.Items.Add("2");
                nbrOrdersChoice.Items.Add("3");
                nbrOrdersChoice.Items.Add("4");
                nbrOrdersChoice.Items.Add("5");
                nbrOrdersChoice.Items.Add("6");
                nbrOrdersChoice.Items.Add("7");
                nbrOrdersChoice.Items.Add("8");  
                nbrOrdersChoice.Items.Add("9");
                nbrOrdersChoice.Items.Add("10");    //TEST!    
                nbrOrdersChoice.Items.Add("12");    //EXPERIMENTAL        
                nbrOrdersChoice.Items.Add("48");    //EXPERIMENTAL  
                   
                nbrOrdersChoice.DropDownStyle = ComboBoxStyle.DropDownList;

                nbrOrdersPerLayerLabel = new Label();
                nbrOrdersPerLayerLabel.Location = new Point(35, 188);
                nbrOrdersPerLayerLabel.Text = "Enter number of orders per pallet layer:"; 
                nbrOrdersPerLayerLabel.Width = 100;
                nbrOrdersPerLayerLabel.AutoSize = true;

                nbrOrdersPerLayerChoice = new ComboBox();
                nbrOrdersPerLayerChoice.Location = new Point(250, 182);
                nbrOrdersPerLayerChoice.Width = 50;
                nbrOrdersPerLayerChoice.Items.Add("1");
                nbrOrdersPerLayerChoice.Items.Add("2");
                nbrOrdersPerLayerChoice.Items.Add("3");
                nbrOrdersPerLayerChoice.Items.Add("4");    
                nbrOrdersPerLayerChoice.Items.Add("5"); 
                nbrOrdersPerLayerChoice.Items.Add("6");
                nbrOrdersPerLayerChoice.Items.Add("7");
                nbrOrdersPerLayerChoice.Items.Add("8");
                nbrOrdersPerLayerChoice.Items.Add("9");
                nbrOrdersPerLayerChoice.Items.Add("10");

                                                    //  TEST!    C(10,5) ...blir ... = 113400. 
                                                   //  113400/C(10,5) = 113400/252 = 450 layers
                                                   //  450/5 = 90 configs
                nbrOrdersPerLayerChoice.DropDownStyle = ComboBoxStyle.DropDownList;

                aislesChoice.SelectedIndexChanged += AislesChoice_SelectedIndexChanged;
                nbrShelvesChoice.SelectedIndexChanged += NbrShelvesChoice_SelectedIndexChanged;
                nbrOrdersChoice.SelectedIndexChanged += NbrOrdersChoice_SelectedIndexChanged;
                nbrOrdersPerLayerChoice.SelectedIndexChanged += NbrOrdersPerLayerChoice_SelectedIndexChanged;

                CreatePickLocations = new Button();
                CreatePickLocations.Location = new Point(150, 250);
                CreatePickLocations.Text = "Choose";
                CreatePickLocations.Click += new EventHandler(CreatePickLocations_Click);

                this.Controls.Add(aislesLabel);
                this.Controls.Add(aislesChoice);
                this.Controls.Add(nbrShelvesLabel);
                this.Controls.Add(nbrShelvesChoice);
                this.Controls.Add(nbrOrdersLabel);
                this.Controls.Add(nbrOrdersChoice);
                this.Controls.Add(nbrOrdersPerLayerLabel);
                this.Controls.Add(nbrOrdersPerLayerChoice);
                this.Controls.Add(CreatePickLocations);
    }

    private void AislesChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (int.TryParse(aislesChoice.SelectedItem?.ToString(), out int parsedAisles))
        {
            selectedAisles = parsedAisles;
        }
    }

    private void NbrShelvesChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (int.TryParse(nbrShelvesChoice.SelectedItem?.ToString(), out int parsedShelves)) 
        {
            selectedNbrShelves = parsedShelves;
        }
    }

    private void NbrOrdersChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (int.TryParse(nbrOrdersChoice.SelectedItem?.ToString(), out int parsedOrders))
        {
            selectedNbrOrders = parsedOrders;
        }
    }

    //NbrOrdersPerLayerChoice
    private void NbrOrdersPerLayerChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (int.TryParse(nbrOrdersPerLayerChoice.SelectedItem?.ToString(), out int parsedOrdersPerLayer))
        {
            selectedNbrOrdersPerLayer = parsedOrdersPerLayer;
        }
    }

    private void CreatePickLocations_Click(object sender, EventArgs e) {
       if (aislesChoice.SelectedIndex == -1 || nbrShelvesChoice.SelectedIndex == -1) {
            MessageBox.Show("Both number of aisles and number of shelves per aisle must be chosen");
            return;
        }
       
        g = new Graph(selectedAisles, selectedNbrShelves, selectedNbrOrders, selectedNbrOrdersPerLayer, 1, 1);
       
        if (selectedNbrOrders == 1) { 
        CreatePickLocationsWindow(g);
        } else {
        CreatePickLocationsForManyOrdersWindow(g);
        }

        Console.WriteLine("Pick locations created with: " + selectedAisles + " aisles, " + selectedNbrShelves + " shelves per aisle, " + selectedNbrOrders + " orders and " + selectedNbrOrdersPerLayer + " orders per pallet layer.");
    }
    
    //lets user define pick locations only for one order.
    private void CreatePickLocationsWindow(Graph g) {
        GUI_createPickLocations window = new GUI_createPickLocations(selectedAisles, selectedNbrShelves, g, null, null);
        window.ShowDialog(); 
    }
            
    private void CreatePickLocationsForManyOrdersWindow(Graph g)
    {
        GUI_createPickLocationsManyOrders window = new GUI_createPickLocationsManyOrders(g, null, null);
        window.ShowDialog();
    }

}
