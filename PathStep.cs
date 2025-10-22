
namespace ConsoleApp1;

public class PathStep
{
    public GraphNode prev { get; set; }
    public GraphNode neighbor { get; set; }
    public double distance { get; set; }

    public PathStep(GraphNode prev, GraphNode neighbor, double distance)
    {
        this.prev = prev;
        this.neighbor = neighbor;
        this.distance = distance;
    }

    
    public override bool Equals(object obj)
    {
        if (obj is PathStep other)
        {
            return prev.Name == other.prev.Name &&
                   neighbor.Name == other.neighbor.Name &&
                   distance == other.distance;
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(prev.Name, neighbor.Name, distance);
    }

    public override string ToString()
    {
        return $"{prev.Name} -> {neighbor.Name} = {distance}";
    }
}
