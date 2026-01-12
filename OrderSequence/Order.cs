namespace ConsoleApp1;

public class Order
{
        public int orderNumber { get; set; }
        public int orderStart { get; set; }
        public int orderEnd { get; set; }
        public int orderRouteLength { get; set; }
        //public int quantity { get; set; }

        public Order(int oN, int oS, int oE, int oRL) {
            orderNumber = oN;
            orderStart = oS;
            orderEnd = oE;
            orderRouteLength = oRL; //not used
        }



}
