namespace ConsoleApp1;

public class BoxStackingFromUniqueOrderStacks
{
    Graph g;
    List<UnitLoadConfiguration> configurations;
    List<OrderStack> uniqueOrderStacks;
    int nbrConfigs = 1000;

    /*
    New strategy:
    Build new ULCs with uniqueOrders as constraint.
    idea: place unique orders in lower layers, then fill 
    the pallet with remaining orders. 

    The filling with remaining orders can be further done Randomly.
    Then, create many different randomized configs (in terms of remaining orders)
    and select the lowest cost config.
    */

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

        AllocateUniqueOrderStacksToConfiguration();
    }

    public void AllocateUniqueOrderStacksToConfiguration()
    {
    List<BoxLayerCombination> layers = new List<BoxLayerCombination>();
    for (int i = 0; i < g.layers; i++)
    {
        layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));
    }

    //Modify the #layers to fit all the #uniqueOrderStacks
    int requiredLayers = 2*(int) Math.Ceiling(uniqueOrderStacks.Count / (double)g.nbrOrdersPerLayers);
    Console.WriteLine("Required #layers to fit all uOs: " + requiredLayers + ", current #layers: " + g.layers);
    if(requiredLayers > g.layers) {
        int nbrMissingLayers = requiredLayers - g.layers;
        for(int i = 0; i < nbrMissingLayers; i++) {
            layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));
        }
    }
    //test end

    UnitLoadConfiguration config = new UnitLoadConfiguration(layers, 0);
    HashSet<int> placedOrders = new HashSet<int>();
    int layerPos = 0; 

    //place unique orders in lower layers
    foreach (var orderStack in uniqueOrderStacks) 
    {   
        if (layerPos < g.nbrOrdersPerLayers) //case: 1 layer
        { 
            config.Layers[0].Boxes.Add(orderStack.bottom.orderNumber);
            config.Layers[1].Boxes.Add(orderStack.top.orderNumber);
            placedOrders.Add(orderStack.bottom.orderNumber);
            placedOrders.Add(orderStack.top.orderNumber);
            layerPos += 1;
        } 
        else if (layerPos < g.nbrOrdersPerLayers * 2)  //case: 2 layers
        { 
            config.Layers[2].Boxes.Add(orderStack.bottom.orderNumber);
            config.Layers[3].Boxes.Add(orderStack.top.orderNumber);
            placedOrders.Add(orderStack.bottom.orderNumber);
            placedOrders.Add(orderStack.top.orderNumber);
            layerPos += 1;
        } 
        else if (layerPos < g.nbrOrdersPerLayers * 3)  //case: 3 layers
        { 
            config.Layers[4].Boxes.Add(orderStack.bottom.orderNumber);
            config.Layers[5].Boxes.Add(orderStack.top.orderNumber);
            placedOrders.Add(orderStack.bottom.orderNumber);
            placedOrders.Add(orderStack.top.orderNumber);
            layerPos += 1;
        } 
        else if (layerPos < g.nbrOrdersPerLayers * 4)  //Will probably not reach here because probable upper limit of 7 layers
        { 
            config.Layers[6].Boxes.Add(orderStack.bottom.orderNumber);
            config.Layers[7].Boxes.Add(orderStack.top.orderNumber);
            placedOrders.Add(orderStack.bottom.orderNumber);
            placedOrders.Add(orderStack.top.orderNumber);
            layerPos += 1;
        }
    }

    //Collect remaining orders
    List<int> remainingOrders = Enumerable.Range(1, g.orders)
        .Where(o => !placedOrders.Contains(o))
        .ToList();

    Random rnd = new Random();
    List<UnitLoadConfiguration> candidateConfigs = new List<UnitLoadConfiguration>();

    //Generate multiple randomized configurations
    for (int i = 0; i < nbrConfigs; i++)
    {
        // Copy base configuration with unique stacks
        List<BoxLayerCombination> layersCopy = new List<BoxLayerCombination>();
        foreach (var layer in config.Layers)
            layersCopy.Add(new BoxLayerCombination(new HashSet<int>(layer.Boxes), 0));

        UnitLoadConfiguration tempConfig = new UnitLoadConfiguration(layersCopy, 0);

        // Shuffle remaining orders
        List<int> shuffledOrders = remainingOrders.OrderBy(x => rnd.Next()).ToList();

        // Place remaining orders bottom-up
        int layerIndex = 0;
        HashSet<int> placedTemp = new HashSet<int>();
        foreach (var layer in tempConfig.Layers)
            foreach (var order in layer.Boxes)
                placedTemp.Add(order);

        foreach (int order in shuffledOrders)
        {
            while (layerIndex < tempConfig.Layers.Count && tempConfig.Layers[layerIndex].Boxes.Count >= g.nbrOrdersPerLayers)
                layerIndex++;

            if (layerIndex >= tempConfig.Layers.Count) break;

            tempConfig.Layers[layerIndex].Boxes.Add(order);
            placedTemp.Add(order);
        }

        // Calculate cost
        tempConfig.CalculateShortestCost(g);

        // Store configuration
        candidateConfigs.Add(tempConfig);
    }

    //Pick the configuration with the lowest total cost
    UnitLoadConfiguration bestConfig = candidateConfigs.OrderBy(c => c.ShortestCost).First();

    /*Print the top 10 cost-effective configurations
    int rank = 1;
    foreach (var c in candidateConfigs.OrderBy(c => c.ShortestCost).Take(10))
    {
        Console.WriteLine($"{rank}. Cost: {c.ShortestCost}");
        for (int i = c.Layers.Count - 1; i >= 0; i--)
            Console.WriteLine($"Layer{i + 1}: {string.Join(", ", c.Layers[i].Boxes)}");
        Console.WriteLine();
        rank++;
    }*/

    //Output the best configuration
    Console.WriteLine("Lowest cost randomized configuration:");
    for (int i = bestConfig.Layers.Count - 1; i >= 0; i--)
    {
        Console.WriteLine(
            $"Layer{i + 1}: " + string.Join(", ", bestConfig.Layers[i].Boxes)
        );
    }
    Console.WriteLine("| Cost: " + bestConfig.ShortestCost);
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