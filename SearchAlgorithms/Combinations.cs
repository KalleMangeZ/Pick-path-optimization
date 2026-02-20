namespace ConsoleApp1;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class Combinations
{
    static int k;
    static int n;
    static Stopwatch? stopwatch;
    public static RandomSearch? RandomSearch {get; set;}
    public static BranchAndBound? BranchAndBound {get; set;}
    public static OrderSequenceAnalysis? analysis {get; set;}

    public static void RunCombinations(Graph g, HashSet<int> searchAlgorithms)     
    {
        n = g.orders;
        k = g.nbrOrdersPerLayers;

        if(k >= n) {
            k = n;
        }
       
        //searchAlgorithms(0) = Brutal-Force,
        //searchAlgorithms(1) = Branch and Bound,
        //searchAlgorithms(2) = Local Random Search
        //searchAlgorithms(3) = Beam Search ... implement?

        if(searchAlgorithms.Contains(0)) {
            if(n % k == 0) { //works when full layers are possible
                RunCombinationsEvenNumberOfOrders(n, k, g); //works for uneven orders if n%k == 0       //Brutal-Force approach
            }  
            else {
                RunCombinationsUnevenNumberOfOrders(n, k, g);                                           //Brutal-Force approach
            }
        } 
        if (searchAlgorithms.Contains(1)) {
            BranchAndBound = new BranchAndBound(g); 
        } 
        if (searchAlgorithms.Contains(2)) {
            RandomSearch = new RandomSearch(g);
        }
    }
    
    public static void RunCombinationsEvenNumberOfOrders(int n, int k, Graph g) {
        var numbers = Enumerable.Range(1, n).ToList();
        var partitions = GetPartitions(numbers, k).ToList();

        stopwatch = new Stopwatch();
        stopwatch.Start();

        int count = 1;
        List<BoxLayerCombination> allCombinations = new List<BoxLayerCombination>();
        foreach (var partition in partitions)       //list<int> in list<list<int>>
        {
            foreach (var orders in partition)   //list<int>
            {
                g.orderSet = new HashSet<int>(orders);
                List<GraphNode> shortestPath;
                double shortestDistance = g.FindShortestPath(g.nodes["R1"], g.nodes["end"], new HashSet<GraphNode>(), 0,
                new List<GraphNode>(), out shortestPath);
                allCombinations.Add(new BoxLayerCombination(new HashSet<int>(orders), shortestDistance));
            }
            count++;
        }
        calculateUnitLoadConfigurationCost_Even(allCombinations, g);

        Console.WriteLine($"\nTotal partitions: {partitions.Count}");
        Console.WriteLine($"Number of boxes per layer: {k}");
        Console.WriteLine($"Number of layers: {g.layers}");
    }

     // Recursive function to generate all partitions into sets of size k
    static IEnumerable<List<List<int>>> GetPartitions(List<int> list, int k)
    {
        if (list.Count == 0)
        {
            yield return new List<List<int>>();
            yield break;
        }

        // Generate all k-combinations for the first group
        foreach (var firstGroup in GetCombinations_Even(list, k))
        {
            var remaining = list.Except(firstGroup).ToList();

            foreach (var partition in GetPartitions(remaining, k))
            {
                var newPartition = new List<List<int>> { firstGroup };
                newPartition.AddRange(partition);
                yield return newPartition;
            }
        }
    }
    
    // Generate all k-combinations of a list
    public static IEnumerable<List<int>> GetCombinations_Even(List<int> list, int k)
    {
        if (k == 0)
        {
            yield return new List<int>();
            yield break;
        }
        for (int i = 0; i <= list.Count - k; i++) 
        {
            var head = list[i];
            foreach (var tail in GetCombinations_Even(list.Skip(i + 1).ToList(), k - 1))
            {
                var combination = new List<int> { head };
                combination.AddRange(tail);
                yield return combination;
            }
        }
    }

    public static void calculateUnitLoadConfigurationCost_Even(List<BoxLayerCombination> allCombinations, Graph g) 
    {
        List<UnitLoadConfiguration> configurations = new List<UnitLoadConfiguration>(); //TEST
        int nbrConfigurations = allCombinations.Count/g.layers;
           
           for(int i = 0; i < nbrConfigurations; i++) //run 12 times
           {
            List<BoxLayerCombination> list = new List<BoxLayerCombination>();
            double listCost = 0;
        
                for(int j = 0; j < g.layers; j++) //for each layer
                {
                    int index = i * g.layers + j;
                    BoxLayerCombination comb = allCombinations[index];
                    list.Add(comb);
                    listCost += comb.ShortestCost;
                }
            configurations.Add(new UnitLoadConfiguration(list, listCost));
           }

    configurations.Sort((a, b) => a.ShortestCost.CompareTo(b.ShortestCost));
    UnitLoadConfiguration optimal = configurations[0];

    /*int count = 1;
    Console.WriteLine("--- All Configurations --- ");
    foreach(UnitLoadConfiguration ULC in configurations)
    {
        Console.WriteLine();
        Console.Write(count + ". ");
        Console.Write("Configuration boxes: " + string.Join(" | ",          //Brutal-Force approach
        ULC.Layers.Select(b => "(" + string.Join(",", b.Boxes) + ")")));
        Console.Write(" | Cost: " + ULC.ShortestCost);
        count++;
    }*/
    Console.WriteLine();
    
    stopwatch.Stop();
    TimeSpan ts = stopwatch.Elapsed;
    ShowOptimalConfigurationRoutes(g, optimal, "Brute Force", ts);
    }

    public static void ShowOptimalConfigurationRoutes(Graph g, UnitLoadConfiguration config, String searchMethod, TimeSpan ts)
    {
        if(searchMethod == "Brute Force") {
        Console.WriteLine("\n --- BRUTE FORCE ---");
        } else if(searchMethod == "Branch and Bound") {
        Console.WriteLine("\n --- BRANCH AND BOUND ---");
        } else if(searchMethod == "Local Random Search") {
        Console.WriteLine("\n --- LOCAL RANDOM SEARCH ---");
        }
        //print the minimal configuration routes in console
        Console.WriteLine("Minimal unit load configuration cost: " + config.ShortestCost);
        Console.WriteLine($"Hours: {ts.Hours:F2} Minutes: {ts.Minutes:F2}, Seconds: {ts.Seconds + ts.Milliseconds / 1000.0:F2}");        
        Console.WriteLine("Configuration boxes: " + string.Join(" | ", 
        config.Layers.Select(b => "(" + string.Join(",", b.Boxes) + ")"))); 
        
        foreach (BoxLayerCombination boxLayer in config.Layers)
        {
            g.orderSet = new HashSet<int>(boxLayer.Boxes);
            g.path.Clear();      
            g.pathNodes.Clear();
            g.nodes.Clear();
            g.CreateGraph();
            GUI_solution gui_solution = new GUI_solution(g, g.pathNodes, analysis);
            gui_solution.ShowDialog();
        }
    }
    public static void RunCombinationsUnevenNumberOfOrders(int n, int k, Graph g) {
        stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var firstGroups = GetCombinations_Uneven(n, k);
        HashSet<string> uniqueArrangements = new HashSet<string>();
        List<BoxLayerCombination> combinations = new List<BoxLayerCombination>();
        List<UnitLoadConfiguration> unitLoadConfigurations = new List<UnitLoadConfiguration>();

        foreach (var firstGroup in firstGroups)
        {
            var remaining = Enumerable.Range(1, n).Except(firstGroup).ToList();
            var remainingPermutations = GetPermutations(remaining, remaining.Count);

            foreach (var perm in remainingPermutations)
            {
                // Split permutation into groups of size k
                var groups = new List<List<int>>();
                int fullGroupsCount = perm.Count / k;

                for (int i = 0; i < fullGroupsCount * k; i += k)
                {
                    var group = perm.Skip(i).Take(k).ToList();
                    group.Sort(); // sort inside group
                    groups.Add(group);
                }

                // Add leftover as last group if any
                int leftoverCount = perm.Count % k;
                if (leftoverCount > 0)
                {
                    var leftoverGroup = perm.Skip(fullGroupsCount * k).Take(leftoverCount).ToList();
                    leftoverGroup.Sort();
                    groups.Add(leftoverGroup);
                }

                // Include the first group and sort inside
                var allGroups = new List<List<int>> { firstGroup.OrderBy(x => x).ToList() };
                allGroups.AddRange(groups);

                // Sort full groups by first element (except leftover)
                var normalGroups = allGroups.Take(allGroups.Count - 1)
                                            .OrderBy(g => g.First())
                                            .ToList();
                var lastGroup = allGroups.Last(); // keep leftover last
                normalGroups.Add(lastGroup);      

                // Compute cost for the whole configuration
                double configCost = 0;
                List<BoxLayerCombination> layerList = new List<BoxLayerCombination>();

                foreach (var group in allGroups)
                {
                    g.orderSet = new HashSet<int>(group);

                    List<GraphNode> shortestPath;
                    double shortestDistance = g.FindShortestPath(
                        g.nodes["R1"],
                        g.nodes["end"],
                        new HashSet<GraphNode>(),
                        0,
                        new List<GraphNode>(),
                        out shortestPath
                    );

                    BoxLayerCombination layer = new BoxLayerCombination(new HashSet<int>(group), shortestDistance);
                        combinations.Add(layer);
                        layerList.Add(layer);
                        configCost += shortestDistance;
                }

                string key = string.Join(" | ", normalGroups.Select(g => string.Join(",", g)));

                // Print only if unique
                if (!uniqueArrangements.Contains(key))
                {
                    uniqueArrangements.Add(key);
                    unitLoadConfigurations.Add(new UnitLoadConfiguration(layerList, configCost));
                }
            }
        }
        calculateUnitLoadConfigurationCost_Uneven(unitLoadConfigurations, g); 

        Console.WriteLine($"\nTotal partitions: {firstGroups.Count}");
    }

    public static List<List<int>> GetPermutations(List<int> list, int length)
    {
        if (length == 1)
            return list.Select(x => new List<int> { x }).ToList();

        var perms = new List<List<int>>();

        foreach (var i in list)
        {
            var remaining = list.Where(x => x != i).ToList();
            var subPerms = GetPermutations(remaining, length - 1);
            foreach (var sub in subPerms)
            {
                var perm = new List<int> { i };
                perm.AddRange(sub);
                perms.Add(perm);
            }
        }
        return perms;
    }

    public static void calculateUnitLoadConfigurationCost_Uneven(List<UnitLoadConfiguration> unitLoadConfigurations, Graph g){
       unitLoadConfigurations.Sort((a, b) => a.ShortestCost.CompareTo(b.ShortestCost)); //sort by cost
       UnitLoadConfiguration optimal = unitLoadConfigurations[0];

        /*int count = 1;
        Console.Write("--- All Configurations --- ");
        foreach(UnitLoadConfiguration ULC in unitLoadConfigurations)
        {
            Console.WriteLine();
            Console.Write(count + ". ");
            Console.Write("Configuration boxes: " + string.Join(" | ",          //Naive approach
            ULC.Layers.Select(b => "(" + string.Join(",", b.Boxes) + ")")));
            Console.Write(" | Cost: " + ULC.ShortestCost);
            count++;
        }*/
        Console.WriteLine();

        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        ShowOptimalConfigurationRoutes(g, optimal, "Brute Force", ts); 
    }

   //Generate combinations C(n,k)
    public static List<List<int>> GetCombinations_Uneven(int n, int k)
    {
        List<List<int>> results = new List<List<int>>();
        int[] combo = new int[k];

        void Backtrack(int start, int depth)
        {
            if (depth == k)
            {
                results.Add(new List<int>(combo));
                return;
            }

            for (int i = start; i <= n - (k - depth) + 1; i++)
            {
                combo[depth] = i;
                Backtrack(i + 1, depth + 1);
            }
        }
        Backtrack(1, 0);
        return results;
    }
}