namespace ConsoleApp1;

public class StartupWindow : Form {
    Label aislesLabel;
    ComboBox aislesChoice;
    Label nbrShelvesLabel;
    ComboBox nbrShelvesChoice;
    Button CreatePickLocations;
    public int selectedAisles;
    public int selectedNbrShelves;
    Graph g;
    public int aisleToAisleDist = 200;

    //Org: StartupWindow(Graph g) {
    public StartupWindow() {
                //this.g = g;
                this.Text = "Create Pick Locations";
                this.Width = 400;
                this.Height = 250;
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
                //aislesChoice.Items.Add("8");

                aislesChoice.DropDownStyle = ComboBoxStyle.DropDownList;

                nbrShelvesLabel = new Label();
                nbrShelvesLabel.Location = new Point(60, 100);
                nbrShelvesLabel.Text = "Enter number of shelves per aisle:"; 
                nbrShelvesLabel.Width = 100;
                nbrShelvesLabel.AutoSize = true;

                nbrShelvesChoice = new ComboBox();
                nbrShelvesChoice.Location = new Point(250, 100);
                nbrShelvesChoice.Width = 50;
                nbrShelvesChoice.Items.Add("3");
                nbrShelvesChoice.Items.Add("4");
                nbrShelvesChoice.Items.Add("5");
                nbrShelvesChoice.Items.Add("6");
                nbrShelvesChoice.Items.Add("7");
                nbrShelvesChoice.Items.Add("8");
                nbrShelvesChoice.Items.Add("9");
                nbrShelvesChoice.Items.Add("10");                               
                //nbrShelvesChoice.Items.Add("16");

                nbrShelvesChoice.DropDownStyle = ComboBoxStyle.DropDownList;

                aislesChoice.SelectedIndexChanged += AislesChoice_SelectedIndexChanged;
                nbrShelvesChoice.SelectedIndexChanged += NbrShelvesChoice_SelectedIndexChanged;

                CreatePickLocations = new Button();
                CreatePickLocations.Location = new Point(150, 150);
                CreatePickLocations.Text = "Choose pick locations";
                CreatePickLocations.Click += new EventHandler(CreatePickLocations_Click);

                this.Controls.Add(aislesLabel);
                this.Controls.Add(aislesChoice);
                this.Controls.Add(nbrShelvesLabel);
                this.Controls.Add(nbrShelvesChoice);
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

    private void CreatePickLocations_Click(object sender, EventArgs e) {
       if (aislesChoice.SelectedIndex == -1 || nbrShelvesChoice.SelectedIndex == -1) {
            MessageBox.Show("Both number of aisles and number of shelves per aisle must be chosen");
            return;
        }
       
        g = new Graph(selectedAisles, selectedNbrShelves, 1, 1);
        CreatePickLocationsWindow(g);
        Console.WriteLine("Pick locations created with: " + selectedAisles + " aisles and " + selectedNbrShelves + " shelves per aisle.");
        //Application.Exit();
    }

    private void CreatePickLocationsWindow(Graph g) {
        GUI_createPickLocations window = new GUI_createPickLocations(selectedAisles, selectedNbrShelves, g, null, null);
        window.ShowDialog(); 
    }
}
