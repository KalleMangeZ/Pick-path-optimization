namespace ConsoleApp1
{
    public class BoxLayerCombination
    {
        public HashSet<int> Boxes { get; set; }  //BORDE EJ VARA SET PGA MULTIPLE ORDRAR MED SAMMA KUND
        public double ShortestCost { get; set; }

        public BoxLayerCombination(HashSet<int> boxes, double shortestCost)
        {
            Boxes = boxes;
            ShortestCost = shortestCost;
        }

    }
}








/*namespace ConsoleApp1
{
    public class BoxLayerCombination
    {
    public int Box1 { get; set; }
    public int Box2 { get; set; }
    public double ShortestCost { get; set; }

    public BoxLayerCombination(int box1, int box2, double shortestCost)
    {
        Box1 = box1;
        Box2 = box2;
        ShortestCost = shortestCost;
    }

    }
}*/