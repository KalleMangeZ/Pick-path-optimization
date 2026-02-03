namespace ConsoleApp1;

using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

/*
Heuristic search algorithm that generates random unit load configurations. Avoids exhaustive search.
*/

public class LocalRandomSearch
{
    Graph g;
    public List<UnitLoadConfiguration> configurations {get; set;}
    Stopwatch stopwatch;

    public LocalRandomSearch(Graph g)
    {
        this.g = g;
        stopwatch = new Stopwatch();
        stopwatch.Start();
        GenerateRandomConfigurations(5000);
        //TestPrintSomeConfigurations(configurations, 500);
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
            /*for(int layer = 0; layer < g.layers; layer++) 
            {
                HashSet<int> layerOrders = new HashSet<int>(); //orders in this layer
                while(layerOrders.Count < g.nbrOrdersPerLayers)
                {
                    int order = rand.Next(1, g.orders + 1);
                    if(!layerOrders.Contains(order) && !usedOrders.Any(u => u.Contains(order)))
                    {
                        layerOrders.Add(order);
                    }
                }
                configuration.Add(layerOrders);
                usedOrders.Add(layerOrders);
            }*/

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
            TestPrintSomeConfigurations(configurations, 50);
            configurations.Sort((a, b) => a.ShortestCost.CompareTo(b.ShortestCost)); //sort by cost
            UnitLoadConfiguration optimal = configurations[0];
            Console.WriteLine("\n#Random configurations generated: " + nbrRandomConfigurations);
                double fullSerpentinePickingRoute = g.layers*((g.aisles*g.shelfWidth*2)+((g.aisles+1)*g.shelvesPerAisle));
                double efficiency = (fullSerpentinePickingRoute / optimal.ShortestCost - 1) * 100;
            Console.WriteLine($"Only serpentine path traversals cost: {fullSerpentinePickingRoute}" +
                              $" | Found solution is: {efficiency:F2}% more distance efficient"); 
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Combinations.ShowOptimalConfigurationRoutes(g, optimal, "Local Random Search", ts); //ts = TimeSpan.Zero to revert
            // (Implementation of storing best configuration goes here)*/
        }

    public void TestPrintSomeConfigurations(List<UnitLoadConfiguration> configurations, int n)
    {
        Console.WriteLine("\n --- LOCAL RANDOM SEARCH --- ");
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
            Console.Write("Configuration boxes: " + string.Join(" | ",          
            ULC.Layers.Select(b => "(" + string.Join(",", b.Boxes) + ")")));
            Console.Write(" | Cost: " + ULC.ShortestCost);
            count++;
        }
    }
}

