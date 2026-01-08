namespace ConsoleApp1
{
    public class BoxLayerCombination
    {
        public HashSet<int> Boxes { get; set; }  //BORDE egentligen EJ VARA "SET" PGA MULTIPLE ORDRAR (BOXAR) MED SAMMA KUND ...
        public double ShortestCost { get; set; }

        public BoxLayerCombination(HashSet<int> boxes, double shortestCost)
        {
            Boxes = boxes;
            ShortestCost = shortestCost;
        }

    }
}