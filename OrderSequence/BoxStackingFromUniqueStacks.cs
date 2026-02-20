using System.ComponentModel;

namespace ConsoleApp1;

public class BoxStackingFromUniqueOrderStacks
{
    Graph g;
    List<UnitLoadConfiguration> configurations;
    List<OrderStack> uniqueOrderStacks;
    List<OrderStack_3_Orders> uniqueOrderStacks_3_Orders;
    List<OrderStack_4_Orders> uniqueOrderStacks_4_Orders;
    
    List<OrderStack> placedUniqueOrderStacks = new List<OrderStack>();
    List<OrderStack_3_Orders> placedUniqueOrderStacks_3_Orders = new List<OrderStack_3_Orders>();
    List<OrderStack_4_Orders> placedUniqueOrderStacks_4_Orders = new List<OrderStack_4_Orders>();
    
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

    public BoxStackingFromUniqueOrderStacks(Graph g, 
        List<UnitLoadConfiguration> configurations,
        List<OrderStack> uniqueOrderStacks, 
        List<OrderStack_3_Orders> uniqueOrderStacks_3_Orders, 
        List<OrderStack_4_Orders> uniqueOrderStacks_4_Orders)
    {
        this.g = g;
        this.configurations = configurations;
        this.uniqueOrderStacks = uniqueOrderStacks;
        this.uniqueOrderStacks_3_Orders = uniqueOrderStacks_3_Orders;
        this.uniqueOrderStacks_4_Orders = uniqueOrderStacks_4_Orders;

        if(g.layers == 1 || g.layers == g.orders) {
            Console.WriteLine("entered FillFirstLayerWithAllOrders");
            FillFirstLayerWithAllOrders();
            return;     
        }

        /*if(g.layers > 3) //Allocate 4-order-stacks. if g.layers > 3
        {
            AllocateUniqueOrderStacksToConfiguration_4_Orders();
        }
        if(g.layers > 2) //Allocate 3-order-stacks. if g.layers > 2
        {
            AllocateUniqueOrderStacksToConfiguration_3_Orders();
        }
        AllocateUniqueOrderStacksToConfiguration();*/

        BuildConfigurationWithAllStacks();
        //BuildConfigurationWithAllStacks_Horizontal(); //priority filling layers with order-stack
                                                        //rather than priority of stacking with 
                                                        //high-number stacks.
        printPlacedOrderStacks();
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
        } 
        else if (layerPos < g.nbrOrdersPerLayers * 2)  //case: 2 layers
        { 
            config.Layers[2].Boxes.Add(orderStack.bottom.orderNumber);
            config.Layers[3].Boxes.Add(orderStack.top.orderNumber);
        } 
        else if (layerPos < g.nbrOrdersPerLayers * 3)  //case: 3 layers
        { 
            config.Layers[4].Boxes.Add(orderStack.bottom.orderNumber);
            config.Layers[5].Boxes.Add(orderStack.top.orderNumber);
        } 
        else if (layerPos < g.nbrOrdersPerLayers * 4)  //Will probably not reach here because probable upper limit of 7 layers
        { 
            config.Layers[6].Boxes.Add(orderStack.bottom.orderNumber);
            config.Layers[7].Boxes.Add(orderStack.top.orderNumber);
        }
        placedOrders.Add(orderStack.bottom.orderNumber);
        placedOrders.Add(orderStack.top.orderNumber);
        layerPos += 1;
    }

    //Collect remaining orders. CHANGE DO THIS AFTER ALL ALLOCATION-VERSIONS?
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
        foreach (var layer in config.Layers) {
            layersCopy.Add(new BoxLayerCombination(new HashSet<int>(layer.Boxes), 0));
        }
        UnitLoadConfiguration tempConfig = new UnitLoadConfiguration(layersCopy, 0);

        // Shuffle remaining orders
        List<int> shuffledOrders = remainingOrders.OrderBy(x => rnd.Next()).ToList();

        // Place remaining orders bottom-up
        int layerIndex = 0;
        HashSet<int> placedTemp = new HashSet<int>();
        foreach (var layer in tempConfig.Layers) {
            foreach (var order in layer.Boxes) {
                placedTemp.Add(order);
            }
        }

        foreach (int order in shuffledOrders)
        {
            while (layerIndex < tempConfig.Layers.Count && tempConfig.Layers[layerIndex].Boxes.Count >= g.nbrOrdersPerLayers) {
                layerIndex++;
            }
            if (layerIndex >= tempConfig.Layers.Count) {break;}

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
        Console.WriteLine("uos: Lowest cost randomized configuration:");
        for (int i = bestConfig.Layers.Count - 1; i >= 0; i--)
        {
            Console.WriteLine(
                $"Layer{i + 1}: " + string.Join(", ", bestConfig.Layers[i].Boxes)
            );
        }
        Console.WriteLine("| Cost: " + bestConfig.ShortestCost);
    }

    public void AllocateUniqueOrderStacksToConfiguration_3_Orders()
    {
        List<BoxLayerCombination> layers = new List<BoxLayerCombination>();
        for (int i = 0; i < g.layers; i++)
        {
            layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));
        }

        //Modify the #layers to fit all the #uniqueOrderStacks
        int requiredLayers = 3*(int) Math.Ceiling(uniqueOrderStacks_3_Orders.Count / (double)g.nbrOrdersPerLayers);
        Console.WriteLine("Required #layers to fit all uOs_3-orders: " + requiredLayers + ", current #layers: " + g.layers);
        if(requiredLayers > g.layers) {
            int nbrMissingLayers = requiredLayers - g.layers; //hmm,är kan det bildas mycket uppstickande ordrar. införa villkor på g.layers > 5?
            for(int i = 0; i < nbrMissingLayers; i++) {
                layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));
            }
        }

        UnitLoadConfiguration config = new UnitLoadConfiguration(layers, 0);
        HashSet<int> placedOrders = new HashSet<int>();
        int layerPos = 0; 

        foreach (var orderStack in uniqueOrderStacks_3_Orders) 
        {   
            if (layerPos < g.nbrOrdersPerLayers) //case: 1 layer
            { 
                config.Layers[0].Boxes.Add(orderStack.bottom.orderNumber);
                config.Layers[1].Boxes.Add(orderStack.middle.orderNumber);
                config.Layers[2].Boxes.Add(orderStack.top.orderNumber);
            } 
            else if (layerPos < g.nbrOrdersPerLayers * 2)  //case: 2 layers (would generate 6 layers)
            { 
                config.Layers[3].Boxes.Add(orderStack.bottom.orderNumber);
                config.Layers[4].Boxes.Add(orderStack.middle.orderNumber);
                config.Layers[5].Boxes.Add(orderStack.top.orderNumber);
            } 
            placedOrders.Add(orderStack.bottom.orderNumber);
            placedOrders.Add(orderStack.middle.orderNumber);
            placedOrders.Add(orderStack.top.orderNumber);
            layerPos += 1;   
        }

        Console.WriteLine("3-uos: Lowest cost randomized configuration:");
        for (int i = config.Layers.Count - 1; i >= 0; i--)
        {
            Console.WriteLine(
                $"Layer{i + 1}: " + string.Join(", ", config.Layers[i].Boxes)
            );
        }
        config.CalculateShortestCost(g);
        Console.WriteLine("| Cost: " + config.ShortestCost);
    }

    public void AllocateUniqueOrderStacksToConfiguration_4_Orders()
    {
        List<BoxLayerCombination> layers = new List<BoxLayerCombination>();
        for (int i = 0; i < g.layers; i++)
        {
            layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));
        }

        //Modify the #layers to fit all the #uniqueOrderStacks
        int requiredLayers = 4*(int) Math.Ceiling(uniqueOrderStacks_4_Orders.Count / (double)g.nbrOrdersPerLayers);
        Console.WriteLine("Required #layers to fit all uOs_4-orders: " + requiredLayers + ", current #layers: " + g.layers);
        if(requiredLayers > g.layers) {
            int nbrMissingLayers = requiredLayers - g.layers; //hmm, här kan det bildas mycket uppstickande ordrar. införa villkor på g.layers > 7?
            for(int i = 0; i < nbrMissingLayers; i++) {
                layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));
            }
    }

    UnitLoadConfiguration config = new UnitLoadConfiguration(layers, 0);
    HashSet<int> placedOrders = new HashSet<int>();
    int layerPos = 0; 

    foreach (var orderStack in uniqueOrderStacks_4_Orders) 
    {   
        if (layerPos < g.nbrOrdersPerLayers) //case: 1 layer
        { 
            config.Layers[0].Boxes.Add(orderStack.bottom.orderNumber);
            config.Layers[1].Boxes.Add(orderStack.middleBottom.orderNumber);
            config.Layers[2].Boxes.Add(orderStack.middleTop.orderNumber);
            config.Layers[3].Boxes.Add(orderStack.top.orderNumber);
        } 
        else if (layerPos < g.nbrOrdersPerLayers * 2)  //case: 2 layers (would generate 8 layers)
        { 
            config.Layers[4].Boxes.Add(orderStack.bottom.orderNumber);       //will probably not be used
            config.Layers[5].Boxes.Add(orderStack.middleBottom.orderNumber); //will probably not be used
            config.Layers[6].Boxes.Add(orderStack.middleTop.orderNumber);    //will probably not be used
            config.Layers[7].Boxes.Add(orderStack.top.orderNumber);          //will probably not be used
        } 
        placedOrders.Add(orderStack.bottom.orderNumber);
        placedOrders.Add(orderStack.middleBottom.orderNumber);
        placedOrders.Add(orderStack.middleTop.orderNumber);
        placedOrders.Add(orderStack.top.orderNumber);
        layerPos += 1;   
    }

        Console.WriteLine("4-uos: Lowest cost randomized configuration:");
        for (int i = config.Layers.Count - 1; i >= 0; i--)
        {
            Console.WriteLine(
                $"Layer{i + 1}: " + string.Join(", ", config.Layers[i].Boxes)
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

    public void BuildConfigurationWithAllStacks()
    {
    List<BoxLayerCombination> layers = new List<BoxLayerCombination>();

    for (int i = 0; i < g.layers; i++)
        layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));

    UnitLoadConfiguration baseConfig = new UnitLoadConfiguration(layers, 0);

    HashSet<int> placedOrders = new HashSet<int>();
    int layerPos = 0;

    layerPos = Allocate4Stacks(baseConfig, placedOrders, layerPos);

    layerPos = Allocate3Stacks(baseConfig, placedOrders, layerPos);

    layerPos = Allocate2Stacks(baseConfig, placedOrders, layerPos);

    DistributeRemainingOrders(baseConfig, placedOrders);
    }

    public void BuildConfigurationWithAllStacks_Horizontal() //ADJUST THIS METHOD
    {
        List<BoxLayerCombination> layers = new List<BoxLayerCombination>();

    for (int i = 0; i < g.layers; i++)
        layers.Add(new BoxLayerCombination(new HashSet<int>(), 0.0));

    UnitLoadConfiguration baseConfig = new UnitLoadConfiguration(layers, 0);

    HashSet<int> placedOrders = new HashSet<int>();
    int layerPos = 0;

    if(uniqueOrderStacks.Count < g.nbrOrdersPerLayers) {
        layerPos = Allocate2Stacks(baseConfig, placedOrders, layerPos);
    } else {
        layerPos = Allocate4Stacks(baseConfig, placedOrders, layerPos);
        layerPos = Allocate3Stacks(baseConfig, placedOrders, layerPos);
        layerPos = Allocate2Stacks(baseConfig, placedOrders, layerPos);
    }

    DistributeRemainingOrders(baseConfig, placedOrders);
    }

    private int Allocate4Stacks(UnitLoadConfiguration config, HashSet<int> placed, int layerPos)
    {
        foreach (var stack in uniqueOrderStacks_4_Orders)
        {
            if (placed.Contains(stack.bottom.orderNumber) ||
                placed.Contains(stack.middleBottom.orderNumber) ||
                placed.Contains(stack.middleTop.orderNumber) ||
                placed.Contains(stack.top.orderNumber))
                continue;

            int column = layerPos % g.nbrOrdersPerLayers;

            if (config.Layers.Count < 4) break;

            // Check capacity in this column
            if (config.Layers[0].Boxes.Count > column ||
                config.Layers[1].Boxes.Count > column ||
                config.Layers[2].Boxes.Count > column ||
                config.Layers[3].Boxes.Count > column)
            {
                layerPos++;
                continue;
            }

            config.Layers[0].Boxes.Add(stack.bottom.orderNumber);
            config.Layers[1].Boxes.Add(stack.middleBottom.orderNumber);
            config.Layers[2].Boxes.Add(stack.middleTop.orderNumber);
            config.Layers[3].Boxes.Add(stack.top.orderNumber);

            placed.Add(stack.bottom.orderNumber);
            placed.Add(stack.middleBottom.orderNumber);
            placed.Add(stack.middleTop.orderNumber);
            placed.Add(stack.top.orderNumber);

            placedUniqueOrderStacks_4_Orders.Add(stack);

            layerPos++;
        }

        return layerPos;
    }


   private int Allocate3Stacks(UnitLoadConfiguration config, HashSet<int> placed, int layerPos)
    {
        foreach (var stack in uniqueOrderStacks_3_Orders)
        {
            //  Skip if ANY order already placed
            if (placed.Contains(stack.bottom.orderNumber) ||
                placed.Contains(stack.middle.orderNumber) ||
                placed.Contains(stack.top.orderNumber))
                continue;

            if (config.Layers.Count < 3)
                break;

            int column = layerPos % g.nbrOrdersPerLayers;

            // Check layer capacity for this column
            if (config.Layers[0].Boxes.Count > column ||
                config.Layers[1].Boxes.Count > column ||
                config.Layers[2].Boxes.Count > column)
            {
                layerPos++;
                continue;
            }

            //  Place vertically in same column
            config.Layers[0].Boxes.Add(stack.bottom.orderNumber);
            config.Layers[1].Boxes.Add(stack.middle.orderNumber);
            config.Layers[2].Boxes.Add(stack.top.orderNumber);

            placed.Add(stack.bottom.orderNumber);
            placed.Add(stack.middle.orderNumber);
            placed.Add(stack.top.orderNumber);

            placedUniqueOrderStacks_3_Orders.Add(stack);

            layerPos++;
        }

        return layerPos;
    }



    private int Allocate2Stacks(UnitLoadConfiguration config, HashSet<int> placed, int layerPos)
    {
        foreach (var stack in uniqueOrderStacks)
        {
            // Skip if ANY order already placed
            if (placed.Contains(stack.bottom.orderNumber) ||
                placed.Contains(stack.top.orderNumber))
                continue;

            if (config.Layers.Count < 2)
                break;

            int column = layerPos % g.nbrOrdersPerLayers;

            // Check layer capacity
            if (config.Layers[0].Boxes.Count > column ||
                config.Layers[1].Boxes.Count > column)
            {
                layerPos++;
                continue;
            }

            // Place vertically
            config.Layers[0].Boxes.Add(stack.bottom.orderNumber);
            config.Layers[1].Boxes.Add(stack.top.orderNumber);

            placed.Add(stack.bottom.orderNumber);
            placed.Add(stack.top.orderNumber);

            placedUniqueOrderStacks.Add(stack);

            layerPos++;
        }

        return layerPos;
    }

    private void DistributeRemainingOrders(UnitLoadConfiguration baseConfig, HashSet<int> placed)
    {
        List<int> remaining = Enumerable.Range(1, g.orders)
            .Where(o => !placed.Contains(o))
            .ToList();

        Random rnd = new Random();
        List<UnitLoadConfiguration> candidates = new List<UnitLoadConfiguration>();

        for (int i = 0; i < nbrConfigs; i++)
        {
            var layersCopy = baseConfig.Layers
                .Select(l => new BoxLayerCombination(new HashSet<int>(l.Boxes), 0))
                .ToList();

            UnitLoadConfiguration temp = new UnitLoadConfiguration(layersCopy, 0);

            var shuffled = remaining.OrderBy(x => rnd.Next()).ToList();

            int layerIndex = 0;

            foreach (int order in shuffled)
            {
                while (layerIndex < temp.Layers.Count &&
                    temp.Layers[layerIndex].Boxes.Count >= g.nbrOrdersPerLayers)
                    layerIndex++;

                if (layerIndex >= temp.Layers.Count) break;

                temp.Layers[layerIndex].Boxes.Add(order);
            }

            temp.CalculateShortestCost(g);
            candidates.Add(temp);
        }

        var best = candidates.OrderBy(c => c.ShortestCost).First();

        Console.WriteLine("FINAL CONFIG:");
        for (int i = best.Layers.Count - 1; i >= 0; i--)
            Console.WriteLine($"Layer{i + 1}: {string.Join(", ", best.Layers[i].Boxes)}");

        Console.WriteLine("| Cost: " + best.ShortestCost);
    }

    public void printPlacedOrderStacks()
    {
        foreach(var stack in placedUniqueOrderStacks_4_Orders)
        {
            Console.WriteLine("placed uniqueOrderStacks_4_Orders: " + stack.bottom.orderNumber + "-" + stack.middleBottom.orderNumber + "-" + stack.middleTop.orderNumber + "-" + stack.top.orderNumber);
        }

         foreach(var stack in placedUniqueOrderStacks_3_Orders)
        {
            Console.WriteLine("placed uniqueOrderStacks_3_Orders: " + stack.bottom.orderNumber + "-" + stack.middle.orderNumber + "-" + stack.top.orderNumber);
        }

        foreach (var stack in placedUniqueOrderStacks)
        {
            Console.WriteLine("placed uniqueOrderStacks:" +stack.bottom.orderNumber + "-" + stack.top.orderNumber);
        }
    }


}