namespace ConsoleApp1;

public class OrderStack{
public Order bottom { get; set; }
public Order top { get; set; }

    public OrderStack(Order b, Order t) {
        bottom = b;
        top = t;
    }
}
    


