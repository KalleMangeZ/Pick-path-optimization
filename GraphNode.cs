namespace ConsoleApp1
{
    public class GraphNode
    {
        public string Name;
        public List<GraphNode> Neighbors;
        public double x { get; set; }
        public double y { get; set; }
        public char nodeType { get; set; } //antingen R eller L
        public int nodeNbr { get; set; }

        public GraphNode(string name, double x, double y, char nodeType, int nodeNbr)
        {
            Name = name;
            Neighbors = new List<GraphNode>();
            this.x = x;
            this.y = y;
            this.nodeType = nodeType;
            this.nodeNbr = nodeNbr;
        }

    }
}