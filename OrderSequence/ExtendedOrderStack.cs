namespace ConsoleApp1;

public class ExtendedOrderStack{
public Order bottom { get; set; }
public Order middle { get; set; }
public Order top { get; set; }

    public ExtendedOrderStack(Order b, Order m, Order t) {
        bottom = b;
        middle = m;
        top = t;
    }
}