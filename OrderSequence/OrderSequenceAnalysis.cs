namespace ConsoleApp1;

public class OrderSequenceAnalysis
{
Graph g;
public int[,] lm;
public List<int> orderSequence {get; set;} = new List<int>();
public List<int> orderStartInSequence {get; set;} = new List<int>();
public List<int> orderEndInSequence {get; set;} = new List<int>();
public List<OrderStack> orderStacks {get; set;}
public List<OrderStack> uniqueOrderStacks {get; set;} = new List<OrderStack>();
public List<ExtendedOrderStack> extendedOrderStacks {get; set;} = new List<ExtendedOrderStack>();
public List<Order> orders {get; set;}
CreatePickingPath pp;

    public OrderSequenceAnalysis(Graph g) {
            this.g = g;
            lm = g.LayoutManager.LayoutMatrix;

            pp = new CreatePickingPath(g);
            CreateOrderSequence();
            AnalyzeOrderSequence();
            CreateOrderStack();
            PrintOrderStack();
            CaseUniqueStackPossibilites();
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

    public void AnalyzeOrderSequence() {
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
            }
    }

    //marks out if and so which box that can stack on top of another box
    public void CreateOrderStack() {
        orderStacks = new List<OrderStack>();
        for(int i = 0; i < orderStartInSequence.Count; i++) {
            for(int j = 0; j < orderEndInSequence.Count; j++) {
                if(orderStartInSequence[i] > orderEndInSequence[j]) {
                    //Console.WriteLine("Order " + (i+1) + " can be stacked on top of Order " + (j+1));
                    orderStacks.Add(new OrderStack(orders[j], orders[i]));
                }
            }
        }
            if(orderStacks.Count == 0) {
                Console.WriteLine("\n No stacking possibilities found --> Pick Layer-By-Layer");
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
                    Console.WriteLine($"Unique order-stack: {stack.bottom.orderNumber}-{stack.top.orderNumber}");
                }
              }
        }
    }
}

