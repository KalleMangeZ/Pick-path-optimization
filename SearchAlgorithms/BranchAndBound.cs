namespace ConsoleApp1;

using System.Diagnostics;

public class BranchAndBound
{

private double bestCost;
private List<HashSet<int>> bestConfiguration;
Graph g;
Stopwatch stopwatch;
private double minLayerCost;

public BranchAndBound(Graph graph)
{
    this.g = graph;
      stopwatch = new Stopwatch();
      stopwatch.Start();
    RunBranchAndBound();
}

private void ComputeMinLayerCost()      
{
    minLayerCost = double.MaxValue;

    HashSet<int> allOrders = Enumerable.Range(1, g.orders).ToHashSet();
    var allOrdersList = Enumerable.Range(1, g.orders).ToList();
    int layerSize = g.nbrOrdersPerLayers;
    foreach (var combo in Combinations.GetCombinations_Even(allOrdersList, layerSize))
    {
        g.orderSet = new HashSet<int>(combo);
        List<GraphNode> path;
        double cost = g.FindShortestPath(
            g.nodes["R1"],
            g.nodes["end"],
            new HashSet<GraphNode>(),
            0,
            new List<GraphNode>(),
            out path);

        minLayerCost = Math.Min(minLayerCost, cost);
    }
} 

public void RunBranchAndBound()
    {
        bestCost = double.MaxValue;
        bestConfiguration = null;

        HashSet<int> allOrders = Enumerable.Range(1, g.orders).ToHashSet();

        ComputeMinLayerCost();      //TEST
        Search(new List<HashSet<int>>(), new HashSet<int>(), 0.0, allOrders);

        // Show result
        var layers = bestConfiguration
            .Select(l => new BoxLayerCombination(l, 0))
            .ToList();
        
        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        Combinations.ShowOptimalConfigurationRoutes(g, new UnitLoadConfiguration(layers, bestCost), "Branch and Bound", ts);
    }

    public void Search(List<HashSet<int>> currentLayers, HashSet<int> usedOrders, double costSoFar, HashSet<int> allOrders) 
    {
        //Complete solution
        if (usedOrders.Count == allOrders.Count)
        {
            if (costSoFar < bestCost)
            {
                bestCost = costSoFar;
                bestConfiguration = currentLayers
                    .Select(l => new HashSet<int>(l))
                    .ToList();
            }
            return;
        }

        //BOUND: stop if already worse
        var remaining = allOrders.Except(usedOrders).ToList();
        int remainingLayers =
            (int)Math.Ceiling((double)remaining.Count / g.nbrOrdersPerLayers);

        double lowerBound = costSoFar + remainingLayers * minLayerCost;

        if (lowerBound >= bestCost)
            return;

        // Remaining orders
        int layerSize = Math.Min(g.nbrOrdersPerLayers, remaining.Count);

        //Branch: try all next layers
        foreach (var combo in Combinations.GetCombinations_Even(remaining, layerSize)) 
        {
            g.orderSet = new HashSet<int>(combo);
            List<GraphNode> path;
            double layerCost = g.FindShortestPath(g.nodes["R1"], g.nodes["end"], new HashSet<GraphNode>(), 0, new List<GraphNode>(), out path);
            double newCost = costSoFar + layerCost;

            //BOUND
            if (newCost >= bestCost) {
                continue;
            }

            // Apply
            currentLayers.Add(new HashSet<int>(combo));
            foreach (int o in combo) {
                usedOrders.Add(o);
            }

            // Recurse
            Search(currentLayers, usedOrders, newCost, allOrders);

            // Undo (backtrack)
            currentLayers.RemoveAt(currentLayers.Count - 1);
            
            foreach (int o in combo) {
                usedOrders.Remove(o);
            }
        }
    }
}