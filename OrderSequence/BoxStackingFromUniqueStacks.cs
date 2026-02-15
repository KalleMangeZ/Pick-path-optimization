namespace ConsoleApp1;

public class BoxStackingFromUniqueOrderStacks
{
    Graph g;
    List<UnitLoadConfiguration> configurations;
    List<OrderStack> uniqueOrderStacks;

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
        // Create same number of layers as in the found configuration
        List<BoxLayerCombination> layers = new List<BoxLayerCombination>();
        for (int i = 0; i < g.layers; i++)
        {
            layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));
        }

        UnitLoadConfiguration config = new UnitLoadConfiguration(layers, 0);
        HashSet<int> placedOrders = new HashSet<int>();
        int layerPos = 0; 

        foreach (var orderStack in uniqueOrderStacks) //as long as uniqueOrderStacks.Count <= g.nbrOrdersPerLayers
        {   
            if(layerPos < g.nbrOrdersPerLayers) { 
                config.Layers[0].Boxes.Add(orderStack.bottom.orderNumber);
                config.Layers[1].Boxes.Add(orderStack.top.orderNumber);
                placedOrders.Add(orderStack.bottom.orderNumber);
                placedOrders.Add(orderStack.top.orderNumber);
                layerPos += 1;
            } else if (layerPos < g.nbrOrdersPerLayers*2) { 
                config.Layers[2].Boxes.Add(orderStack.bottom.orderNumber);
                config.Layers[3].Boxes.Add(orderStack.top.orderNumber);
                placedOrders.Add(orderStack.bottom.orderNumber);
                placedOrders.Add(orderStack.top.orderNumber);
                layerPos += 1;
            } else if (layerPos < g.nbrOrdersPerLayers*3) { 
                config.Layers[4].Boxes.Add(orderStack.bottom.orderNumber);
                config.Layers[5].Boxes.Add(orderStack.top.orderNumber);
                placedOrders.Add(orderStack.bottom.orderNumber);
                placedOrders.Add(orderStack.top.orderNumber);
                layerPos += 1;
            } else if (layerPos < g.nbrOrdersPerLayers*4) {     //Will probably not reach here because probable upper limit of 7 layers
                config.Layers[6].Boxes.Add(orderStack.bottom.orderNumber);
                config.Layers[7].Boxes.Add(orderStack.top.orderNumber);
                placedOrders.Add(orderStack.bottom.orderNumber);
                placedOrders.Add(orderStack.top.orderNumber);
                layerPos += 1;
            }
        }

        //2. Collect remaining orders
        List<int> remainingOrders = Enumerable.Range(1, g.orders)
        .Where(o => !placedOrders.Contains(o))
        .ToList();

        // Optional: shuffle to randomize
        Random rnd = new Random();
        remainingOrders = remainingOrders.OrderBy(x => rnd.Next()).ToList();

    //3. Fill layers bottom-up
        int layerIndex = 0;
        foreach (int order in remainingOrders)
        {
            // Skip layers that are already full
            while (layerIndex < config.Layers.Count && config.Layers[layerIndex].Boxes.Count >= g.nbrOrdersPerLayers)
            {
                layerIndex++;
            }

        if (layerIndex >= config.Layers.Count) break; // all layers full

        config.Layers[layerIndex].Boxes.Add(order);
        placedOrders.Add(order);
        }

        Console.WriteLine("Built configuration:");
        for (int i = config.Layers.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(
                    $"Layer{i + 1}: " +
                    string.Join(", ", config.Layers[i].Boxes)
                );
            }
        config.CalculateShortestCost(g);
        Console.WriteLine("| Cost: " + config.ShortestCost);
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