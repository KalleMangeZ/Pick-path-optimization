namespace ConsoleApp1;

public class Layout
{
    private int shelvesPerAisle;
    private int aisles;
    public HashSet<Lane> lanes  { get; set; }
    public int[,] layout;
    public int[,] LayoutMatrix
    {
        get => layout;
        set => layout = value;
    }    
    public int orders;
    public int layers;

    public Layout(int shelvesPerAisle, int aisles, HashSet<Lane> lanes, int orders, int layers)
    {
        this.shelvesPerAisle = shelvesPerAisle;
        this.aisles = aisles;
        this.lanes = lanes;
        this.orders = orders;
        this.layers = layers;
        layout = new int[shelvesPerAisle, aisles * 2];
    }

    public void CreateLayout(int[,] pickLocations)
    {
        Console.WriteLine("-----------------------------------");
        for (int i = shelvesPerAisle - 1; i >= 0; i--)  //rows
        {
            for (int j = 0; j < layout.GetLength(1); j++) //cols
            {
                if (pickLocations[i, j] != 0)
                {
                    layout[i, j] = pickLocations[i, j];
                    continue;
                }
                layout[i, j] = 0;
            }
        }
        //printLayout();
    }

    public void CreatePickLocationsFromGUI() {
        int[,] pickLocations = new int[shelvesPerAisle, aisles * 2];
        for (int i = 0; i < layout.GetLength(0); i++) {
            for (int j = 0; j < layout.GetLength(1); j++) {
                pickLocations[i, j] = LayoutMatrix[i, j];
            }
        }
        CreateLayout(pickLocations);
    }

    public void CreateRandomPickingLocations() {
        Random rand = new Random();
        int[,] pickLocations = new int[shelvesPerAisle, aisles * 2];
        for (int col = 0; col < aisles * 2; col++) {
            int randomRow = rand.Next(0, shelvesPerAisle);
            double probIsPickLocation = 1 - (1 / (double)shelvesPerAisle);
            double prob = rand.NextDouble();
            if (prob < probIsPickLocation) {
                pickLocations[randomRow, col] = 1;
            }
            else {
                Console.WriteLine("col with 0: " + col);
                pickLocations[randomRow, col] = 0;
            }
        }
        CreateLayout(pickLocations);
    }
    
    public void CreateStaticPickLocations()
    {
        int[,] pickLocations = new int[shelvesPerAisle, aisles * 2];
            pickLocations[0, 0] = 1;
            pickLocations[4, 2] = 1;
            pickLocations[0, 4] = 1;
            pickLocations[4, 6] = 1;
            pickLocations[0, 7] = 1;
            
        CreateLayout(pickLocations);
    }

    public void printLayout() {
        Console.WriteLine("Layout and pick locations:");

        Console.Write("     ");
        for (int i = 0; i < lanes.Count + 1; i++) {
            String nodeStringName = "L" + Convert.ToString(i + 2);
            Console.Write(" " + "   ____________   " + nodeStringName);
        }
        Console.WriteLine();

        for (int i = 0; i < layout.GetLength(0); i++) {
            Console.Write("     ");
            for (int j = 0; j < layout.GetLength(1); j++) {
                Console.Write(" (" + i + ", " + j + "): " + layout[i, j]);
            }
            Console.WriteLine();
        }

        Console.Write("   R1");
        for (int i = 0; i < lanes.Count + 1; i++) {
            String nodeStringName = "R" + Convert.ToString(i + 2);
            Console.Write(" " + "   ____________   " + nodeStringName);
        }
        Console.Write("   end");
        Console.WriteLine();
    }

     public void CreatePickLocations()
    {
        Console.Write("Enter number of pick locations: ");
        int nbrPickLocations;

        while (!int.TryParse(Console.ReadLine(), out nbrPickLocations) ||
               nbrPickLocations < 0 || nbrPickLocations > aisles * shelvesPerAisle * 2)
        {
            Console.WriteLine($"Error: number of pick locations must be between 0 and {aisles * shelvesPerAisle * 2}. Try again:");
        }

        int[,] pickLocations = new int[shelvesPerAisle, aisles * 2];

        for (int i = 0; i < nbrPickLocations; i++)
        {
            Console.WriteLine($"Enter pick location {i + 1}/{nbrPickLocations}");

            int col;
            while (true)
            {
                Console.Write("Column: ");
                if (int.TryParse(Console.ReadLine(), out col) && col >= 0 && col < aisles * 2)
                    break;
                Console.WriteLine($"Error: column must be between 0 and {aisles * 2 - 1}. Try again:");
            }

            int row;
            while (true)
            {
                Console.Write("Row: ");
                if (int.TryParse(Console.ReadLine(), out row) && row >= 0 && row < shelvesPerAisle)
                    break;
                Console.WriteLine($"Error: row must be between 0 and {shelvesPerAisle - 1}. Try again:");
            }

            pickLocations[row, col] = 1; // Mark pick location
        }

        CreateLayout(pickLocations);
    }
}
