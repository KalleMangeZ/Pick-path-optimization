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

    public void CalculateShortestCost(Graph g)
    {
        double totalCost = 0.0;
        foreach (var layer in Layers)
        {
            g.orderSet = layer.Boxes;
            List<GraphNode> path;
            double cost = g.FindShortestPath(
                    g.nodes["R1"],
                    g.nodes["end"],
                    new HashSet<GraphNode>(),
                    0,
                    new List<GraphNode>(),
                    out path);
            totalCost += cost;
        }
        ShortestCost = totalCost;
    }

}
