namespace ConsoleApp1;

public class BoxStackingFromUniqueOrderStacks
{
    Graph g;
    List<UnitLoadConfiguration> configurations;
    List<OrderStack> uniqueOrderStacks;

    public BoxStackingFromUniqueOrderStacks(Graph g, List<UnitLoadConfiguration> configurations, List<OrderStack> uniqueOrderStacks)
    {
        this.g = g;
        this.configurations = configurations;
        this.uniqueOrderStacks = uniqueOrderStacks;

        if(g.layers == 1 || g.layers == g.orders) {
            return;      //no stacking if only one layer or if one order per layer
        }
        SearchMatchingConfigurations();
    }
        /*From LocalRandomSearch
        1. Select bottom layer orders and search each one of them through each layer in a configuration in configurations
        2. If match found, check if top layer orders match as well 
        */
    public void SearchMatchingConfigurations()
        {
        List<int> searchedLayer = uniqueOrderStacks
            .Select(o => o.bottom.orderNumber)
            .Distinct()
            .ToList();

        Dictionary<string, UnitLoadConfiguration> uniqueConfigs =
            new Dictionary<string, UnitLoadConfiguration>();

        foreach (UnitLoadConfiguration config in configurations)
        {
            foreach (BoxLayerCombination layer in config.Layers)
            {
                if (searchedLayer.All(o => layer.Boxes.Contains(o)))
                {
                    string key = GetCanonicalKey(config);

                    // keep only one instance per canonical configuration
                    if (!uniqueConfigs.ContainsKey(key))
                    {
                        uniqueConfigs[key] = config;
                    }

                    break; // no need to check other layers
                }
            }
        }

        var finalConfigs = uniqueConfigs.Values
            .OrderBy(c => c.ShortestCost)
            .Take(10)
            .ToList();

        int count = 1;
        foreach (var ulc in finalConfigs)
        {
            Console.WriteLine();
            Console.Write($"{count}. Configuration boxes: ");
            Console.Write(
                string.Join(" | ",
                    ulc.Layers
                    .Select(b => "(" + string.Join(",", b.Boxes.OrderBy(x => x)) + ")")
                    .OrderBy(s => s)
                )
            );
            Console.Write($" | Cost: {ulc.ShortestCost}");
            count++;
        }
    }

    private static string GetCanonicalKey(UnitLoadConfiguration config)
    {
        var normalizedLayers = config.Layers
            .Select(layer => layer.Boxes.OrderBy(x => x).ToList())
            .OrderBy(layer => string.Join(",", layer))
            .Select(layer => $"({string.Join(",", layer)})");

        return string.Join(" | ", normalizedLayers);
    }


 }

