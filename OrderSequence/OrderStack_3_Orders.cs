namespace ConsoleApp1;

public class OrderStack_3_Orders{
public Order bottom { get; set; }
public Order middle { get; set; }
public Order top { get; set; }

    public OrderStack_3_Orders(Order b, Order m, Order t) {
        bottom = b;
        middle = m;
        top = t;
    }
}