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
public List<OrderStack_3_Orders> orderStacks_3_Orders {get; set;} = new List<OrderStack_3_Orders>();
public List<OrderStack_3_Orders> uniqueOrderStacks_3_Orders {get; set;} = new List<OrderStack_3_Orders>();
public List<OrderStack_4_Orders> orderStacks_4_Orders {get; set;} = new List<OrderStack_4_Orders>();
public List<OrderStack_4_Orders> uniqueOrderStacks_4_Orders {get; set;} = new List<OrderStack_4_Orders>();
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
            if(g.layers > 1) {
                CreateUniqueOrderStacks();
            }
            
            if (g.layers > 2) {
                CreateUniqueOrderStack_3_Orders();
            }
            
             if (g.layers > 3) {
                CreateUniqueOrderStack_4_Orders();
            }

    /*
    if g.layers > 3 --> allocate (if pos.) 4-uos && (if pos.) 3-uos && (if pos.) uos 
    if g.layers > 2 --> allocate (if pos.) 3-uos && (if pos.) uos 
    if g.layers > 1 --> allocate (if pos.) uos 
    */

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

        if(g.layers > 3) {
            CreateOrderStack_4_Orders();
        }

        if(g.layers > 2) {
            CreateOrderStack_3_Orders();
        }
    }

    public void CreateOrderStack_3_Orders() {
        foreach(OrderStack stack1 in orderStacks) {
            foreach(OrderStack stack2 in orderStacks) {
                if(stack1.top.orderNumber == stack2.bottom.orderNumber) {  //checking by orderNumber works.
                    orderStacks_3_Orders.Add(new OrderStack_3_Orders(stack1.bottom, stack1.top, stack2.top));  //e.g stack1: 1-4, stack2: 4-5
                }
            }
        }
    }

    public void CreateOrderStack_4_Orders()
    {
        foreach(OrderStack stack1 in orderStacks) {
            foreach(OrderStack stack2 in orderStacks) {
                if(stack2.orderStackStart > stack1.orderStackEnd) {
                    orderStacks_4_Orders.Add(new OrderStack_4_Orders(stack1.bottom, stack1.top, stack2.bottom, stack2.top));
                }
            }
        }
    }

    public void CreateUniqueOrderStacks()
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

    public void CreateUniqueOrderStack_3_Orders() {
        uniqueOrderStacks_3_Orders.Clear();

        HashSet<int> usedOrders = new HashSet<int>();

        foreach (var stack in orderStacks_3_Orders) {
                int bottom = stack.bottom.orderNumber;
                int middle = stack.middle.orderNumber;
                int top = stack.top.orderNumber;

                //both orders must be unused
                if (!usedOrders.Contains(bottom) && !usedOrders.Contains(middle) && !usedOrders.Contains(top)) {
                    uniqueOrderStacks_3_Orders.Add(stack);
                    usedOrders.Add(bottom);
                    usedOrders.Add(middle);
                    usedOrders.Add(top);
                }
            }

            // Debug print
        foreach (var stack in uniqueOrderStacks_3_Orders) {
            Console.WriteLine(
            $"unique 3-order-stack: {stack.bottom.orderNumber}-{stack.middle.orderNumber}-{stack.top.orderNumber}"
            );
        }
    }

    public void CreateUniqueOrderStack_4_Orders()
    {
         uniqueOrderStacks_4_Orders.Clear();

        HashSet<int> usedOrders = new HashSet<int>();

        foreach (var stack in orderStacks_4_Orders) {
                int bottom = stack.bottom.orderNumber;
                int middleBottom = stack.middleBottom.orderNumber;
                int middleTop = stack.middleTop.orderNumber;
                int top = stack.top.orderNumber;

                //both orders must be unused
                if (!usedOrders.Contains(bottom) && !usedOrders.Contains(middleBottom) && !usedOrders.Contains(middleTop) && !usedOrders.Contains(top)) {
                    uniqueOrderStacks_4_Orders.Add(stack);
                    usedOrders.Add(bottom);
                    usedOrders.Add(middleBottom);
                    usedOrders.Add(middleTop);
                    usedOrders.Add(top);
                }
            }

            // Debug print
        foreach (var stack in uniqueOrderStacks_4_Orders) {
            Console.WriteLine(
            $"unique 4-order-stack: {stack.bottom.orderNumber}-{stack.middleBottom.orderNumber}-{stack.middleTop.orderNumber}-{stack.top.orderNumber}"
            );
        }
    }


    public void PrintOrderStack() {
        Console.WriteLine("------------------");
        for(int i = 0; i < orderStacks.Count; i++) {
            Console.WriteLine("order-stack: " + orderStacks[i].bottom.orderNumber + "-" + orderStacks[i].top.orderNumber);
        }

        if(orderStacks_3_Orders.Count != 0) {
            for(int i = 0; i < orderStacks_3_Orders.Count; i++) {
                Console.WriteLine("3-order-stack: " + orderStacks_3_Orders[i].bottom.orderNumber + "-" + orderStacks_3_Orders[i].middle.orderNumber + "-" + orderStacks_3_Orders[i].top.orderNumber);
            }
        }

        if(orderStacks_4_Orders.Count != 0) {
            for(int i = 0; i < orderStacks_4_Orders.Count; i++) {
                Console.WriteLine("4-order-stack: " + orderStacks_4_Orders[i].bottom.orderNumber + "-" + orderStacks_4_Orders[i].middleBottom.orderNumber + "-" + orderStacks_4_Orders[i].middleTop.orderNumber + "-" + orderStacks_4_Orders[i].top.orderNumber);
            }
        }
    }
}

