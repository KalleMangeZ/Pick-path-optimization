namespace ConsoleApp1;

using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

/*
Heuristic search algorithm that generates random unit load configurations. Avoids exhaustive search.
*/

public class RandomSearch
{
    Graph g;
    public List<UnitLoadConfiguration>? configurations {get; set;}
    Stopwatch stopwatch;

    public RandomSearch(Graph g)
    {
        this.g = g;
        stopwatch = new Stopwatch();
        stopwatch.Start();
        GenerateRandomConfigurations(5000);
    }

    public void GenerateRandomConfigurations(int nbrRandomConfigurations)
    {      
        configurations = new List<UnitLoadConfiguration>();
        Random rand = new Random();

        for(int i = 0; i < nbrRandomConfigurations; i++)
        {
            List<HashSet<int>> configuration = new List<HashSet<int>>();
            List<HashSet<int>> usedOrders = new List<HashSet<int>>();
            HashSet<int> allOrders = new HashSet<int>();
            for(int j = 1; j <= g.orders; j++) { allOrders.Add(j); }

            List<BoxLayerCombination> listOfLayers = new List<BoxLayerCombination>();

            var remainingOrders = new HashSet<int>(Enumerable.Range(1, g.orders));
            for (int layer = 0; layer < g.layers && remainingOrders.Count > 0; layer++)
            {
                int capacity = Math.Min(g.nbrOrdersPerLayers, remainingOrders.Count);
                HashSet<int> layerOrders = new HashSet<int>();

                for (int c = 0; c < capacity; c++)
                {
                    int idx = rand.Next(remainingOrders.Count);
                    int order = remainingOrders.ElementAt(idx);
                    layerOrders.Add(order);
                    remainingOrders.Remove(order);
                }

                configuration.Add(layerOrders);
            }

            //calculate cost of this configuration
            double totalCost = 0.0;
            foreach(var layer in configuration)
            {
                g.orderSet = layer;
                List<GraphNode> path;
                double cost = g.FindShortestPath(
                    g.nodes["R1"],
                    g.nodes["end"],
                    new HashSet<GraphNode>(),
                    0,
                    new List<GraphNode>(),
                    out path);
                BoxLayerCombination boxLayer = new BoxLayerCombination(layer, cost);
                listOfLayers.Add(boxLayer);
                totalCost += cost;
            }

            UnitLoadConfiguration config = new UnitLoadConfiguration(listOfLayers, totalCost);
            configurations.Add(config);
        }
            //TestPrintTop_n_Configurations(configurations, 500);
            TestPrint_n_RandomConfigurations(configurations, 500);

            configurations.Sort((a, b) => a.ShortestCost.CompareTo(b.ShortestCost)); //sort by cost
            UnitLoadConfiguration optimal = configurations[0];
            Console.WriteLine("\n#Random configurations generated: " + nbrRandomConfigurations);
                double fullSerpentinePickingRoute = g.layers*((g.aisles*g.shelfWidth*2)+((g.aisles+1)*g.shelvesPerAisle));
                double efficiency = (fullSerpentinePickingRoute / optimal.ShortestCost - 1) * 100;
            Console.WriteLine($"Only serpentine path traversals cost: {fullSerpentinePickingRoute}" +
                              $" | Found solution is: {efficiency:F2}% more distance efficient"); 
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Combinations.ShowOptimalConfigurationRoutes(g, optimal, "Random Search", ts); 
        }

    public void TestPrintTop_n_Configurations(List<UnitLoadConfiguration> configurations, int n)
    {
        Console.WriteLine("\n --- RANDOM SEARCH --- ");
        int count = 1;
        
        configurations.Sort((a, b) => a.ShortestCost.CompareTo(b.ShortestCost)); //sort by cost
        UnitLoadConfiguration optimal = configurations[0];
        Console.WriteLine("Minimal unit load configuration cost: " + optimal.ShortestCost);
        
        foreach(UnitLoadConfiguration ULC in configurations)
        {
            if(count > n) { 
                break; 
            } 
            Console.WriteLine();
            Console.Write(count + ". ");
            Console.Write("Configuration boxes: " + string.Join(" | ", ULC.Layers.Select(b => "(" + string.Join(",", b.Boxes) + ")")));
            Console.Write(" | Cost: " + ULC.ShortestCost);
            count++;
        }
    }

    public void TestPrint_n_RandomConfigurations(List<UnitLoadConfiguration> configurations, int n)
    {
        Console.WriteLine("\n --- RANDOM SEARCH --- ");
        int count = 1;
        foreach(UnitLoadConfiguration ULC in configurations)
        {
            if(count > n) { 
                break; 
            } 
            Console.WriteLine();
            Console.Write(count + ". ");
            Console.Write("Configuration boxes: " + string.Join(" | ", ULC.Layers.Select(b => "(" + string.Join(",", b.Boxes) + ")")));
            Console.Write(" | Cost: " + ULC.ShortestCost);
            count++;
        }
    }
}

