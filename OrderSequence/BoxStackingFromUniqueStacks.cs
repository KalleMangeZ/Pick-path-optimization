namespace ConsoleApp1;

public class BoxStackingFromUniqueOrderStacks
{
    Graph g;
    List<UnitLoadConfiguration> configurations;
    List<OrderStack> uniqueOrderStacks;
    List<UnitLoadConfiguration> finalConfigs;

    public BoxStackingFromUniqueOrderStacks(Graph g, List<UnitLoadConfiguration> configurations, List<OrderStack> uniqueOrderStacks)
    {
        this.g = g;
        this.configurations = configurations;
        this.uniqueOrderStacks = uniqueOrderStacks;

        if(g.layers == 1 || g.layers == g.orders) {
            return;      //no stacking if only one layer or if one order per layer
        }

        //To do!
        if(uniqueOrderStacks.Count > g.nbrOrdersPerLayers) {
            //Begränsa antal orderStacks? Här implementeras logik för "stackning av order-stacks". Dvs, om fler
            //än 2 lager så kan order-stacks sättas nästa lager dock med hänsyn till extended order stack?..
        }

        SearchMatchingConfigurations();
        RepresentConfigurations();
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

                    if (!uniqueConfigs.ContainsKey(key))
                    {
                        uniqueConfigs[key] = config;
                    }

                    break; // no need to check other layers
                }
            }
        }

        finalConfigs = uniqueConfigs.Values
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

    //Searches through 2 layers of the config. to check if there are existing configurations that match the order stacks.
    //regarding higher stacks, another approach needs to be developed.
    private void RepresentConfigurations() 
        {
            if(g.layers < 2){
                return;
            }
            
            int[] orderLayer1 = new int[g.nbrOrdersPerLayers];
            int[] orderLayer2 = new int[g.nbrOrdersPerLayers];
            
            HashSet<int> orderSetLayer1 = new HashSet<int>();
            HashSet<int> orderSetLayer2 = new HashSet<int>();

            //UnitLoadConfiguration bestConfig = finalConfigs[0];
            
            for(int i = 0; i < uniqueOrderStacks.Count; i++) 
            {
                orderLayer1[i] = uniqueOrderStacks[i].bottom.orderNumber;
                orderSetLayer1.Add(orderLayer1[i]);
                orderLayer2[i] = uniqueOrderStacks[i].top.orderNumber;
                orderSetLayer2.Add(orderLayer2[i]);
            }
            var requiredLayer1 = orderSetLayer1.Where(x => x != 0).ToHashSet();
            var requiredLayer2 = orderSetLayer2.Where(x => x != 0).ToHashSet();
            List<BoxLayerCombination> layers = [new BoxLayerCombination(orderSetLayer1, 0.0), new BoxLayerCombination(orderSetLayer2, 0.0)];
            
            Console.WriteLine();
            Console.WriteLine("Layer2: " + string.Join(",", orderLayer2));    
            Console.WriteLine("Layer1: " + string.Join(",", orderLayer1));

        //TEST
        bool matchNotFound = true;
        for (int i = 0; i < configurations.Count; i++)
        {
            if(!matchNotFound){
                break;
            }
            foreach (var layer in configurations[i].Layers)
            {
            var layerSet = layer.Boxes
                .Where(x => x != 0)
                .ToHashSet();
                
                if (requiredLayer1.All(x => layerSet.Contains(x)))
                {
                    foreach(var layer2 in configurations[i].Layers){
                        if(requiredLayer2.All(x => layer2.Boxes.Contains(x))){
                            Console.WriteLine(
                                $"MATCH FOUND → Layer: {string.Join(",", layer.Boxes)} " +
                                $"| Required: {string.Join(",", requiredLayer1)} " +
                                $"| Cost: {configurations[i].ShortestCost}");
                            FormatConfiguration(configurations[i]);
                            matchNotFound = false;
                        }
                    }
                }
            }
        }
        if(matchNotFound){
            Console.WriteLine("No matching configuration found for the unique order stacks. New stack generation needed");
            CreateConfigurationsFromUniqueStacks finishBuildingConfig = new CreateConfigurationsFromUniqueStacks(g, uniqueOrderStacks);
        }
    }

    /*e.g
    Unique order-stack: 2-3
    Unique order-stack: 1-4
    */
    public void FormatConfiguration(UnitLoadConfiguration config)
    {
        BoxLayerCombination boxesLayer1 = new BoxLayerCombination(new HashSet<int>(), 0.0);
        BoxLayerCombination boxesLayer2 = new BoxLayerCombination(new HashSet<int>(), 0.0);

        List<BoxLayerCombination> layers = new List<BoxLayerCombination>();
        layers.Add(boxesLayer1);
        layers.Add(boxesLayer2);

        UnitLoadConfiguration formattedConfig = new UnitLoadConfiguration(layers, 0);

        foreach (var orderStack in uniqueOrderStacks)
        {
            boxesLayer1.Boxes.Add(orderStack.bottom.orderNumber);
            boxesLayer2.Boxes.Add(orderStack.top.orderNumber);
        }

        //Track orders already placed via unique stacks
        HashSet<int> placedOrders = new HashSet<int>();
        foreach (var b in boxesLayer1.Boxes) placedOrders.Add(b);
        foreach (var b in boxesLayer2.Boxes) placedOrders.Add(b);

        Console.WriteLine("Formatted configuration:");
        for (int i = formattedConfig.Layers.Count; i >= 0; i--)
        {
            Console.WriteLine(
                $"Layer{i + 1}: " +
                string.Join(", ", formattedConfig.Layers[i].Boxes)
            );
        }



        
    }

}