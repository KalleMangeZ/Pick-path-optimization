namespace ConsoleApp1;

public class UnitLoadConfiguration
{
    public List<BoxLayerCombination> Boxes { get; set; }
    public double ShortestCost { get; set; }
    
    public UnitLoadConfiguration(List<BoxLayerCombination> boxes, double shortestCost)
    {
        Boxes = boxes;
        ShortestCost = shortestCost;
    }

}
