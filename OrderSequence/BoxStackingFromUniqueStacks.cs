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
            Console.WriteLine("entered FillFirstLayerWithAllOrders");
            FillFirstLayerWithAllOrders();
            return;     
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
            Console.Write($"{count}. finalConfigs Configuration boxes: ");
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
                                $"\nMATCH FOUND → Layer: {string.Join(",", layer.Boxes)} " +
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
            Console.WriteLine("\n No matching configuration found for the unique order stacks. New stack generation needed");
            if(g.nbrOrdersPerLayers > uniqueOrderStacks.Count){
                Console.WriteLine("Generating new config from finalConfigs:");
                CreateConfigurationFromFinalConfig();
            } else {  //Using else here for TEST PURPOSES
            CreateConfigurationsFromUniqueStacks finishBuildingConfig = new CreateConfigurationsFromUniqueStacks(g, uniqueOrderStacks);
            }
        }
    }

    public void CreateConfigurationFromFinalConfig()
    {
        UnitLoadConfiguration bestConfig = finalConfigs[0];
        FixWrongLayerOrientation(bestConfig);
    }

    /*e.g
    Unique order-stack: 2-3
    Unique order-stack: 1-4
    */
    public void FormatConfiguration(UnitLoadConfiguration config)
    {
        // Create same number of layers as in the found configuration
        List<BoxLayerCombination> layers = new List<BoxLayerCombination>();
        for (int i = 0; i < config.Layers.Count; i++)
        {
            layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));
        }

        UnitLoadConfiguration formattedConfig = new UnitLoadConfiguration(layers, 0);

        HashSet<int> placedOrders = new HashSet<int>();

        foreach (var orderStack in uniqueOrderStacks)
        {
            formattedConfig.Layers[0].Boxes.Add(orderStack.bottom.orderNumber);
            formattedConfig.Layers[1].Boxes.Add(orderStack.top.orderNumber);

            placedOrders.Add(orderStack.bottom.orderNumber);
            placedOrders.Add(orderStack.top.orderNumber);
        }

        for (int layerIndex = 0; layerIndex < config.Layers.Count; layerIndex++)
        {
            foreach (var box in config.Layers[layerIndex].Boxes)
            {
                if (placedOrders.Contains(box))
                    continue;

                if (formattedConfig.Layers[layerIndex].Boxes.Count < g.nbrOrdersPerLayers)
                {
                    formattedConfig.Layers[layerIndex].Boxes.Add(box);
                    placedOrders.Add(box);
                }
            }
        }

    foreach (var layer in formattedConfig.Layers)
        {
            if (layer.Boxes.Count >= g.nbrOrdersPerLayers)
                continue;

            foreach (var originalLayer in config.Layers)
            {
                foreach (var box in originalLayer.Boxes)
                {
                    if (placedOrders.Contains(box))
                        continue;

                    if (layer.Boxes.Count < g.nbrOrdersPerLayers)
                    {
                        layer.Boxes.Add(box);
                        placedOrders.Add(box);
                    }
                }
            }
        }

        if(formattedConfig.Layers[1].Boxes.Count > formattedConfig.Layers[0].Boxes.Count){
            FixWrongLayerOrientation(formattedConfig);
            return;
        }

        Console.WriteLine("Formatted configuration:");
        for (int i = formattedConfig.Layers.Count - 1; i >= 0; i--)
        {
            Console.WriteLine(
                $"Layer{i + 1}: " +
                string.Join(", ", formattedConfig.Layers[i].Boxes)
            );
        }
        formattedConfig.CalculateShortestCost(g);
        Console.WriteLine("Formatted configuration cost: " + formattedConfig.ShortestCost);
    }

    public void FixWrongLayerOrientation(UnitLoadConfiguration config)
    {
    // Swap Layer 0 and Layer 1
    BoxLayerCombination temp = config.Layers[0];
    config.Layers[0] = config.Layers[1];
    config.Layers[1] = temp;

    // Ensure unique order stacks are correctly oriented: lower number in Layer1, higher in Layer2
    foreach (var orderStack in uniqueOrderStacks)
    {
        int lower = Math.Min(orderStack.bottom.orderNumber, orderStack.top.orderNumber);
        int higher = Math.Max(orderStack.bottom.orderNumber, orderStack.top.orderNumber);

        // Remove them from any layer if they exist
        config.Layers[0].Boxes.Remove(lower);
        config.Layers[0].Boxes.Remove(higher);
        config.Layers[1].Boxes.Remove(lower);
        config.Layers[1].Boxes.Remove(higher);

        // Add them in the correct layers
        if (!config.Layers[0].Boxes.Contains(lower)) config.Layers[0].Boxes.Add(lower);
        if (!config.Layers[1].Boxes.Contains(higher)) config.Layers[1].Boxes.Add(higher);
    }
    //TEST
     while(config.Layers[0].Boxes.Count < g.nbrOrdersPerLayers){
        config.Layers[0].Boxes.Add(config.Layers[1].Boxes.Last());
        config.Layers[1].Boxes.Remove(config.Layers[1].Boxes.Last());
    }

    // Test print
    Console.WriteLine("Fixed Formatted configuration:");
    for (int i = config.Layers.Count - 1; i >= 0; i--)
    {
        Console.WriteLine(
            $"Layer{i + 1}: " +
            string.Join(", ", config.Layers[i].Boxes)
        );
    }
    }

    public void FillFirstLayerWithAllOrders()
    {
        BoxLayerCombination firstLayer = new BoxLayerCombination(new HashSet<int>(), 0.0);
        List<BoxLayerCombination> layer = new List<BoxLayerCombination> { firstLayer };

        for(int i = 1; i < g.orders+1; i++)
        {
            firstLayer.Boxes.Add(i);
        }
        UnitLoadConfiguration oneLayerConfig = new UnitLoadConfiguration(layer, 0);

        //testprint
        Console.WriteLine("One-layer configuration:");
        for (int i = oneLayerConfig.Layers.Count - 1; i >= 0; i--)
        {
            Console.WriteLine(
                $"Layer{i + 1}: " +
                string.Join(", ", oneLayerConfig.Layers[i].Boxes)
            );
        }
    }

}