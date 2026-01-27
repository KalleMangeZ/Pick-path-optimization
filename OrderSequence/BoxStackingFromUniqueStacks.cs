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
        List<int> searchedLayer = new List<int>();
        List<UnitLoadConfiguration> someMatchingConfigurations = new List<UnitLoadConfiguration>();

        foreach(var order in uniqueOrderStacks) {
            searchedLayer.Add(order.bottom.orderNumber); //doesnt matter if top or bottom
        }

        foreach (UnitLoadConfiguration config in configurations)
        {
            foreach (BoxLayerCombination layer in config.Layers)
            {
                if (searchedLayer.All(o => layer.Boxes.Contains(o)))
                {
                   someMatchingConfigurations.Add(config);
                }
            }
        }

        someMatchingConfigurations.Sort((a, b) => a.ShortestCost.CompareTo(b.ShortestCost)); //sort by cost
        int count = 1;
        foreach(var ULC in someMatchingConfigurations) {
            if(count > 10) {
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

