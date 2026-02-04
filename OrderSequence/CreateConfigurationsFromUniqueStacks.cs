namespace ConsoleApp1;

public class CreateConfigurationsFromUniqueStacks
{
    public Graph g;
    public List<OrderStack> uniqueOrderStacks;

    public CreateConfigurationsFromUniqueStacks(Graph g, List<OrderStack> uniqueOrderStacks)
    {
        this.g = g;
        this.uniqueOrderStacks = uniqueOrderStacks;
        GenerateConfigurationsFromUniqueStacks();
    
    }

    public void GenerateConfigurationsFromUniqueStacks() {
        List<UnitLoadConfiguration> listConfigsWithCriteria = new List<UnitLoadConfiguration>();
        BoxLayerCombination boxesLayer1 = new BoxLayerCombination(new HashSet<int>(), 0.0);
        BoxLayerCombination boxesLayer2 = new BoxLayerCombination(new HashSet<int>(), 0.0);
        
        List<BoxLayerCombination> layers = new List<BoxLayerCombination>();
        layers.Add(boxesLayer1);
        layers.Add(boxesLayer2);
        
        UnitLoadConfiguration configWithCriteria = new UnitLoadConfiguration(layers, 0);
        foreach (var orderStack in uniqueOrderStacks)
        {
            boxesLayer1.Boxes.Add(orderStack.bottom.orderNumber);
            boxesLayer2.Boxes.Add(orderStack.top.orderNumber);
        }
        if(g.nbrOrdersPerLayers > boxesLayer1.Boxes.Count) {  //om det finns tomma platser kvar i lagren.
            int boxesToAdd = g.nbrOrdersPerLayers - boxesLayer1.Boxes.Count; //per lager

            List<int> usedOrders = new List<int>();
            List<int> availableOrders = new List<int>();
            HashSet<int> allOrders = new HashSet<int>();
            for(int i = 1; i <= g.orders; i++) {
                if(!boxesLayer1.Boxes.Contains(i) && !boxesLayer2.Boxes.Contains(i))
                availableOrders.Add(i); //5,6
            }
            
            int iterations = 50; //Select how many new configurations to generate (Random search approach)
            int count = 0; 
            Random rand = new Random();
            while(count < iterations && availableOrders.Count >= 2)
            {
                for(int i = 0; i < boxesToAdd; i++)  //new configuration generation
                {
                    //selects a random available box from availableOrders and removes it from the list
                    
                    var tempAvailable = new List<int>(availableOrders);
                    if (tempAvailable.Count < 2){
                    break;
                    }

                    int idx1 = rand.Next(tempAvailable.Count);
                    int box1 = tempAvailable[idx1];
                    tempAvailable.RemoveAt(idx1);

                    int idx2 = rand.Next(tempAvailable.Count);
                    int box2 = tempAvailable[idx2];
                    tempAvailable.RemoveAt(idx2);

                    boxesLayer1.Boxes.Add(box1);
                    boxesLayer2.Boxes.Add(box2);
                    count++;

                    listConfigsWithCriteria.Add(new UnitLoadConfiguration(
                            new List<BoxLayerCombination> {
                            new BoxLayerCombination(new HashSet<int>(boxesLayer1.Boxes), 0.0),
                            new BoxLayerCombination(new HashSet<int>(boxesLayer2.Boxes), 0.0)
                        },
                        0.0
                    ));
                    boxesLayer1.Boxes.Remove(box1);
                    boxesLayer2.Boxes.Remove(box2);
                }

            }
        }

       foreach(UnitLoadConfiguration ulc in listConfigsWithCriteria)
       {
            //calculate the cost of the configuration:
            ulc.CalculateShortestCost(g);
       }
        TestPrintSomeConfigurations(listConfigsWithCriteria, 50);
    }

    public void TestPrintSomeConfigurations(List<UnitLoadConfiguration> configs, int numberToPrint)
    {
        var sortedConfigs = configs.OrderBy(c => c.ShortestCost).Take(numberToPrint).ToList();
        int count = 1;
        foreach (var ulc in sortedConfigs)
        {
            Console.WriteLine();
            Console.Write($"{count}. Generated - Configuration boxes: ");
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
}
