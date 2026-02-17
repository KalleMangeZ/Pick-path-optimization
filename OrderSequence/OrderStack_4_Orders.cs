namespace ConsoleApp1;

public class OrderStack_4_Orders{
public Order bottom { get; set; }
public Order middleBottom { get; set; }

public Order middleTop { get; set; }
public Order top { get; set; }

    public OrderStack_4_Orders(Order b, Order mB, Order mT, Order t) {
        bottom = b;
        middleBottom = mB;
        middleTop = mT;
        top = t;
    }
}