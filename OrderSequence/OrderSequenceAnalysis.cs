namespace ConsoleApp1;

public class OrderSequenceAnalysis
{
Graph g;
List<UnitLoadConfiguration> configurations;
public int[,] lm;
public List<int> orderSequence {get; set;} = new List<int>();
public List<int> orderStartInSequence {get; set;} = new List<int>();
public List<int> orderEndInSequence {get; set;} = new List<int>();
public List<OrderStack> orderStacks {get; set;}
public List<OrderStack> uniqueOrderStacks {get; set;} = new List<OrderStack>();
public List<ExtendedOrderStack> extendedOrderStacks {get; set;} = new List<ExtendedOrderStack>();
public List<Order> orders {get; set;}
CreatePickingPath pp;

    public OrderSequenceAnalysis(Graph g, List<UnitLoadConfiguration> configurations) {
            this.g = g;
            this.configurations = configurations;
            lm = g.LayoutManager.LayoutMatrix;

            pp = new CreatePickingPath(g);
            CreateOrderSequence();
            AnalyzeOrderSequence();
            CreateOrderStack();
            PrintOrderStack();
            //CaseUniqueStackPossibilites();
            if(uniqueOrderStacks.Count == 0) {
               CreateUniqueFromOnlyOrderStacks();
            }
    }

    public void CreateOrderSequence() {
        for(int i = 0; i < pp.racks.Count; i++) {
        int r = pp.racks[i].rackNbr - 1;
        int c = pp.racks[i].laneNbr - 1;
            if(lm[r, c] != 0) {
                orderSequence.Add(lm[r, c]);
            }
        }
    }

    /*public void AnalyzeOrderSequence() {
            orderStartInSequence = new List<int>();
            orderEndInSequence = new List<int>();
            orders = new List<Order>();

            for(int orderNum = 1; orderNum < g.orders+1; orderNum++) {
                for(int orderStart = 0; orderStart < orderSequence.Count; orderStart++) {
                    if(orderSequence[orderStart] == orderNum) {
                        orderStartInSequence.Add(orderStart);
                        break;
                    }
                }    
            }

            for(int orderNum = 1; orderNum < g.orders+1; orderNum++) {
                for(int orderEnd = orderSequence.Count-1; orderEnd >= 0; orderEnd--) {                    
                    if(orderSequence[orderEnd] == orderNum) {
                        orderEndInSequence.Add(orderEnd);
                        break;  
                    }
                }
            }   

            for(int i = 0; i < g.orders; i++) {
                    orders.Add(new Order(i+1, orderStartInSequence[i], orderEndInSequence[i], orderEndInSequence[i]-orderStartInSequence[i]+1));
                    //Console.WriteLine("orderNumber: " + orders[i].orderNumber + " start: " + orders[i].orderStart + " end: " + orders[i].orderEnd + " span: " + orders[i].orderRouteLength + " orderStartInSequence: " + orderStartInSequence[i] + " orderEndInSequence: " + orderEndInSequence[i]);
            }
    }*/

    public void AnalyzeOrderSequence()
    {
        orderStartInSequence = new List<int>();
        orderEndInSequence = new List<int>();
        orders = new List<Order>();

        for (int orderNum = 1; orderNum <= g.orders; orderNum++)
        {
            int startIndex = orderSequence.IndexOf(orderNum);
            int endIndex = orderSequence.LastIndexOf(orderNum);

            // 👇 WRITE IT HERE
            if (startIndex == -1)
            {
                Console.WriteLine($"Order {orderNum} not found in sequence.");
                continue; // skip this order to prevent crash
            }

            orderStartInSequence.Add(startIndex);
            orderEndInSequence.Add(endIndex);

            orders.Add(new Order(
                orderNum,
                startIndex,
                endIndex,
                endIndex - startIndex + 1));
        }
    }


    //marks out if and so which box that can stack on top of another box
    public void CreateOrderStack() {
        orderStacks = new List<OrderStack>();
        for(int i = 0; i < orders.Count; i++) {
            for(int j = 0; j < orders.Count; j++) {
                if(orderStartInSequence[i] > orderEndInSequence[j]) {
                    //Console.WriteLine("Order " + (i+1) + " can be stacked on top of Order " + (j+1));
                    orderStacks.Add(new OrderStack(orders[j], orders[i]));
                }
            }
        }
            if(orderStacks.Count == 0) {  //choose lowest found cost configuration
                Console.WriteLine("\n No stacking possibilities found --> Pick Layer-By-Layer");
                Console.WriteLine("Pick layer-by-layer configuration:");
                UnitLoadConfiguration bestConfig = configurations[0];

                for (int i = bestConfig.Layers.Count - 1; i >= 0; i--)
                    {
                        Console.WriteLine(
                            $"Layer{i + 1}: " +
                            string.Join(", ", bestConfig.Layers[i].Boxes)
                        );
                    }
                bestConfig.CalculateShortestCost(g);
                Console.WriteLine("Pick layer-by-layer configuration cost: " + bestConfig.ShortestCost);
                }

        if(g.layers > 2) {
            CreateExtendedOrderStack();
        }
    }

    public void CreateExtendedOrderStack() {
        foreach(OrderStack stack1 in orderStacks) {
            foreach(OrderStack stack2 in orderStacks) {
                if(stack1.top.orderNumber == stack2.bottom.orderNumber) { 
                    extendedOrderStacks.Add(new ExtendedOrderStack(stack1.bottom, stack1.top, stack2.top));  //e.g stack1: 1-4, stack2: 4-5
                }
            }
        }
    }

    public void CreateUniqueFromOnlyOrderStacks()
    {
            uniqueOrderStacks.Clear();

            HashSet<int> usedOrders = new HashSet<int>();

            // Optional: sort to control priority (shorter spans, lower order numbers, etc.)
            var sortedStacks = orderStacks
                .OrderBy(s => s.top.orderNumber - s.bottom.orderNumber)
                .ToList();

            foreach (var stack in sortedStacks) {
                int bottom = stack.bottom.orderNumber;
                int top = stack.top.orderNumber;

                // both orders must be unused
                if (!usedOrders.Contains(bottom) && !usedOrders.Contains(top)) {
                    uniqueOrderStacks.Add(stack);
                    usedOrders.Add(bottom);
                    usedOrders.Add(top);
                }
            }

            // Debug print
            foreach (var stack in uniqueOrderStacks) {
                Console.WriteLine(
                    $"unique order-stack: {stack.bottom.orderNumber}-{stack.top.orderNumber}"
                );
            }
    }

    public void PrintOrderStack() {
        Console.WriteLine("------------------");
        for(int i = 0; i < orderStacks.Count; i++) {
            Console.WriteLine("order-stack: " + orderStacks[i].bottom.orderNumber + "-" + orderStacks[i].top.orderNumber);
        }

        if(extendedOrderStacks.Count != 0) {
            for(int i = 0; i < extendedOrderStacks.Count; i++) {
                Console.WriteLine("extended order-stack: " + extendedOrderStacks[i].bottom.orderNumber + "-" + extendedOrderStacks[i].middle.orderNumber + "-" + extendedOrderStacks[i].top.orderNumber);
            }
        }
    }

   public void CaseUniqueStackPossibilites()
    {
    Dictionary<int, int> stackPossibilities = new Dictionary<int, int>();

    for (int i = 0; i < orderStacks.Count; i++)
    {
        Order orderTop = orderStacks[i].top;
        Order orderBottom = orderStacks[i].bottom;

        // Top
        if (stackPossibilities.ContainsKey(orderTop.orderNumber))
            stackPossibilities[orderTop.orderNumber]++;
        else
            stackPossibilities[orderTop.orderNumber] = 1;
        // Bottom
        if (stackPossibilities.ContainsKey(orderBottom.orderNumber))
            stackPossibilities[orderBottom.orderNumber]++;
        else
            stackPossibilities[orderBottom.orderNumber] = 1;
    }
    //prints
        foreach(KeyValuePair<int, int> kvp in stackPossibilities) {
            //Console.WriteLine("Order: " + kvp.Key + "  Value: " + kvp.Value);
              if (kvp.Value == 1)
              {
                var stack = orderStacks.Find(os =>
                    os.top.orderNumber == kvp.Key ||
                    os.bottom.orderNumber == kvp.Key);

                if (stack != null && !uniqueOrderStacks.Any(u =>
                    u.top.orderNumber == stack.top.orderNumber &&
                    u.bottom.orderNumber == stack.bottom.orderNumber))
                {
                    uniqueOrderStacks.Add(stack);
                    //does not actually print:
                    Console.WriteLine($"'CaseUniqueStackPossibilitesUnique' order-stack: {stack.bottom.orderNumber}-{stack.top.orderNumber}");
                }
              }
        }
    }
}

