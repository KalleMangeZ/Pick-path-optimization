namespace ConsoleApp1;

public class Layout
{
    private int shelvesPerAisle;
    private int aisles;
    private int[,] layout;
    public int[,] LayoutMatrix => layout;

    public Layout(int shelvesPerAisle, int aisles)
    {
        this.shelvesPerAisle = shelvesPerAisle;
        this.aisles = aisles;
        layout = new int[shelvesPerAisle, aisles * 2];
    }

    public void CreateLayout(int[,] pickLocations)
    {
        Console.WriteLine("Picking Locations: ");
        for (int i = shelvesPerAisle - 1; i >= 0; i--)
        {
            for (int j = 0; j < layout.GetLength(1); j++)
            {
                if (pickLocations[i, j] != 0)
                {
                    layout[i, j] = pickLocations[i, j];
                    continue;
                }
                layout[i, j] = 0;
            }
        }
        printLayout();
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

    public void CreateRandomPickingLocations()
    {
        Random rand = new Random();
        int[,] pickLocations = new int[shelvesPerAisle, aisles * 2];
        for (int col = 0; col < aisles * 2; col++)
        {
            int randomRow = rand.Next(0, shelvesPerAisle);
            pickLocations[randomRow, col] = 1;
        }
        CreateLayout(pickLocations);
    }
    
    public void CreateStaticPickLocations()
    {
        int[,] pickLocations = new int[shelvesPerAisle, aisles * 2];
            pickLocations[7, 0] = 1;
            pickLocations[3, 2] = 1;
            pickLocations[9, 3] = 1;
            pickLocations[8, 4] = 1;
            pickLocations[5, 5] = 1;
            pickLocations[2, 6] = 1;
            
        CreateLayout(pickLocations);
    }

    public void printLayout()
    {
        Console.WriteLine("Layout and pick locations:");

        String aisleLine = new String("            __________________");

        for (int i = 0; i < layout.GetLength(0); i++)
        {
            for (int j = 0; j < layout.GetLength(1); j++)
            {
                Console.Write(" (" + i + ", " + j + "): " + layout[i, j]);
            }
            Console.WriteLine();
        }
    }
}
