namespace ConsoleApp1;

public class UnitLoadConfiguration
{
    public List<BoxLayerCombination> Layers { get; set; }        //multiple layers with pickLoadCarriers
    public double ShortestCost { get; set; }

    public UnitLoadConfiguration(List<BoxLayerCombination> layers, double shortestCost)
    {
        Layers = layers;
        ShortestCost = shortestCost;
    }

}
