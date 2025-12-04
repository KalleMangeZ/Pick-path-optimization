namespace ConsoleApp1;

using System;
using System.Collections.Generic;
using System.Linq;

public class Combinations
{
    // Generate all k-combinations of a list
    public static IEnumerable<List<int>> GetCombinations(List<int> list, int k)
    {
        if (k == 0)
        {
            yield return new List<int>();
            yield break;
        }
        for (int i = 0; i <= list.Count - k; i++)
        {
            var head = list[i];
            foreach (var tail in GetCombinations(list.Skip(i + 1).ToList(), k - 1))
            {
                var combination = new List<int> { head };
                combination.AddRange(tail);
                yield return combination;
            }
        }
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
        foreach (var firstGroup in GetCombinations(list, k))
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

    public static void RunCombinations(Graph g)
    {
        int n = g.orders;  
        int k = n/g.layers;

        if (n % k != 0)
        {
            Console.WriteLine("n must be divisible by k. FIXA DETTA");
            /*skapa alla möjliga kombinationer av längd k med numrena från n. Sätt resten av ordrarna på toppen.
            //alltså C(n, k) och ta den/de bästa kombinationerna. Sätt resten av oanvända

            - Fixa för ojämnt antal ordrar

            - Fixa för ojämnt antal ordrar per lager

            */

            return;
        }

        var numbers = Enumerable.Range(1, n).ToList();
        var partitions = GetPartitions(numbers, k).ToList();

        int count = 1;
        List<BoxLayerCombination> allCombinations = new List<BoxLayerCombination>();
        foreach (var partition in partitions)       //list<int> in list<list<int>>
        {
            Console.Write($"{count}. ");
            Console.WriteLine(string.Join(" | ", partition.Select(g => "(" + string.Join(",", g) + ")")));

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
        calculateUnitLoadConfigurationCost(allCombinations, g);

        Console.WriteLine($"\nTotal partitions: {partitions.Count}");
        Console.WriteLine($"Number of boxes per layer: {k}");
        Console.WriteLine($"Number of layers: {g.layers}");
    }

    public static void calculateUnitLoadConfigurationCost(List<BoxLayerCombination> allCombinations, Graph g) 
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
                Console.WriteLine("Total cost for configuration " + (i+1) + ": " + listCost);
                configurations.Add(new UnitLoadConfiguration(list, listCost));
           }

    configurations.Sort((a, b) => a.ShortestCost.CompareTo(b.ShortestCost));
    
    UnitLoadConfiguration best = configurations[0];
    Console.WriteLine("\nMinimal unit load configuration cost: " + best.ShortestCost);
    Console.WriteLine("Configuration boxes: " + string.Join(" | ",
        best.Boxes.Select(b => "(" + string.Join(",", b.Boxes) + ")")));    }

}