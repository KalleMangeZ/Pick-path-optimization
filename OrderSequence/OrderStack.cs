namespace ConsoleApp1;

public class OrderStack{
public Order bottom { get; set; }
public Order top { get; set; }

public int orderStackStart { get; set; }
public int orderStackEnd { get; set; }

    public OrderStack(Order b, Order t) {
        bottom = b;
        top = t;
        orderStackStart = bottom.orderStart;
        orderStackEnd = top.orderEnd;
    }
}
    


