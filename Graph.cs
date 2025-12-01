using System.Globalization;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

namespace ConsoleApp1;
/* Assumption: Picking is only done from Left-Right. No back-tracking allowed. Picker start in the first aisle, and ends picking in the last aisle.
Picker can:
- pass through the aisle from Lnode to R-node, picking as required
- pick as required in the aisle, then return to the origin node.

Future development: add half-aisle to left of start and to right of end.
                   
*/

public class Graph
{
    public Dictionary<string, GraphNode> nodes = new Dictionary<string, GraphNode>();
    public Layout LayoutManager { get; set; }
    public HashSet<Lane> lanes  { get; set; }
    public HashSet<PathStep> path = new HashSet<PathStep>();
    public List<GraphNode> pathNodes { get; set; } = new List<GraphNode>();  
    public HashSet<int> orderSet { get; set; }
    public int aisles           { get; set; }
    public int shelvesPerAisle  { get; set; }
    public int orders           { get; set; }
    public int layers           { get; set; }
    public double shelfLength   { get; set; }
    public double shelfWidth    { get; set; }
    public double aisleLength   { get; set; }
    public double aisleWidth    { get; set; }
    public double shortestDistance { get; set; }
    public int orderNbr { get; set; } = 1;      //!

    public Graph(int aisles, int shelvesPerAisle, int orders, int layers, double shelfLength, double shelfWidth)
    {
        this.aisles = aisles;
        this.shelvesPerAisle = shelvesPerAisle;
        this.shelfLength = shelfLength;
        this.shelfWidth = shelfWidth;
        this.orders = orders;
        this.layers = layers;

        orderSet = new HashSet<int>();

        //create lanes:
        int nbrCols = aisles * 2;
        int nbrLanes = nbrCols / 2 - 1;
        lanes = new HashSet<Lane>();

        for (int i = 1; i <= nbrLanes * 2; i = i + 2)
        {
            lanes.Add(new Lane(i, i + 1));
        }

        LayoutManager = new Layout(shelvesPerAisle, aisles, lanes, orders, layers);
        //LayoutManager.CreateStaticPickLocations();
        //LayoutManager.CreatePickLocations();
        //LayoutManager.CreateRandomPickingLocations();
        //LayoutManager.CreatePickLocationsFromGUI();
        //createGraph();
    }

    public void printPathSteps()
    {
        List<PathStep> pathListOfL = new List<PathStep>();
        List<PathStep> pathListOfR = new List<PathStep>();
        List<PathStep> diagonalPath = new List<PathStep>();

        foreach (PathStep ps in path)
        {
            if (ps.prev.nodeType == 'L' && ps.neighbor.nodeType == 'L' || ps.prev.nodeType == 'L' && ps.neighbor.Name == "end")
            {
                pathListOfL.Add(ps);
            }
            else if (ps.prev.nodeType == 'R' && ps.neighbor.nodeType == 'R' || ps.prev.nodeType == 'R' && ps.neighbor.Name == "end")
            {
                pathListOfR.Add(ps);
            }
            else if ((ps.prev.nodeType == 'L' && ps.neighbor.nodeType == 'R') || (ps.prev.nodeType == 'R' && ps.neighbor.nodeType == 'L') ||
             (ps.neighbor.nodeType == 'L' && ps.prev.nodeType == 'R') || (ps.neighbor.nodeType == 'R' && ps.prev.nodeType == 'L'))
            {
                diagonalPath.Add(ps);
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("-----------------------------------");
        Console.WriteLine("Current horizontal path distances: ");
        pathListOfL.Sort((a, b) => a.prev.nodeNbr.CompareTo(b.neighbor.nodeNbr));
        for (int i = 0; i < pathListOfL.Count; i++)
        {
            Console.WriteLine(pathListOfL[i].ToString());
        }

        pathListOfR.Sort((a, b) => a.prev.nodeNbr.CompareTo(b.neighbor.nodeNbr));
        for (int i = 0; i < pathListOfR.Count; i++)
        {
            Console.WriteLine(pathListOfR[i].ToString());
        }

        Console.WriteLine("Diagonal path distances: ");
        for (int i = 0; i < diagonalPath.Count; i++)
        {
            Console.WriteLine(diagonalPath[i].ToString());
        }
        Console.WriteLine("-----------------------------------");
    }

    public string ListedOrderString()
    {
        String orderString = "";
        foreach (int order in orderSet)
        {
            orderString += order.ToString() + ", ";
        }
        return orderString.ToString();
    }

    public void CreateGraph()
    {
        int layers = aisles + 1;
        aisleLength = shelfLength * shelvesPerAisle;    //4
        aisleWidth = shelfWidth * 2;      //2
        double xCoord = 0;
        double yCoord = aisleLength;

        nodes["R1"] = new GraphNode("R1", xCoord, 0, 'R', 1);  //start node    (2 neighbors)
        //create nodes for each layer

        for (int i = 2; i < layers + 1; i++)
        {
            xCoord = xCoord + aisleWidth;
            nodes["L" + i] = new GraphNode("L" + i, xCoord, yCoord, 'L', i);
            nodes["R" + i] = new GraphNode("R" + i, xCoord, 0, 'R', i);
        }
        nodes["end"] = new GraphNode("end", aisleWidth * aisles, 0, 'R', (aisles * 2) - 1); //end node  (2 neighbors)
        //connect start node (run 1 time)
        nodes["R1"].Neighbors.Add(nodes["L2"]); //diag
        nodes["R1"].Neighbors.Add(nodes["R2"]); //diag

        //connnect second first layers (run 1 time)
        for (int i = 2; i < aisles; i++)
        {
            nodes["L" + i].Neighbors.Add(nodes["L" + (i + 1)]); //hori
            nodes["L" + i].Neighbors.Add(nodes["R" + (i + 1)]); //diag
            nodes["R" + i].Neighbors.Add(nodes["L" + (i + 1)]);
            nodes["R" + i].Neighbors.Add(nodes["R" + (i + 1)]);
        }

        //connect intermediate layers (nbr run = "layers"- 3 times)
        for (int i = 3; i < layers; i++)
        {
            nodes["L" + i].Neighbors.Add(nodes["L" + (i + 1)]); //hori
            nodes["L" + i].Neighbors.Add(nodes["R" + (i + 1)]); //diag
            nodes["R" + i].Neighbors.Add(nodes["L" + (i + 1)]);
            nodes["R" + i].Neighbors.Add(nodes["R" + (i + 1)]);
        }

        //connect second last layers (run 1 time)
        for (int i = layers; i < layers + 1; i++)
        {
            nodes["L" + i].Neighbors.Add(nodes["end"]);         //diag
            nodes["R" + i].Neighbors.Add(nodes["end"]);         //hori )
        }

        foreach (var node in nodes.Values)
        {
            string neighbors = string.Join(", ", node.Neighbors.ConvertAll(n => n.Name));
        }
        //CreatePickLocations();    //För manuell input av pick locations
        //CreateRandomPickingLocations();

        List<GraphNode> shortestPath;
        shortestDistance = FindShortestPath(nodes["R1"], nodes["end"], new HashSet<GraphNode>(), 0,
        new List<GraphNode>(), out shortestPath);
      //  printPathSteps();
        Console.WriteLine("Shortest distance cost from R1 to end: " + shortestDistance);
        Console.Write("Shortest path route: ");
        for (int i = 0; i < shortestPath.Count; i++) {
            pathNodes.Add(shortestPath[i]);
            Console.Write(shortestPath[i].Name);
            if (i < shortestPath.Count - 1)
                Console.Write(" -> ");
        }
        Console.WriteLine();
    }

    public double FindShortestPath(GraphNode current, GraphNode target, HashSet<GraphNode> visited, double currentDist,
    List<GraphNode> pathSoFar, out List<GraphNode> shortestPath)
    {
        if (current == target)
        {
            shortestPath = new List<GraphNode>(pathSoFar) { current };
            return currentDist;
        }
        visited.Add(current);
        pathSoFar.Add(current);

        double minDist = double.MaxValue;
        shortestPath = null;

        foreach (GraphNode neighbor in current.Neighbors)
        {
            if (!visited.Contains(neighbor))
            {
                GraphNode prev = pathSoFar[pathSoFar.Count - 1];
                double dist = FindDistType(prev, neighbor);
                path.Add(new PathStep(prev, neighbor, dist));
                List<GraphNode> tempPath;
                double totalDist = FindShortestPath(neighbor, target, new HashSet<GraphNode>(visited), currentDist + dist, new List<GraphNode>(pathSoFar), out tempPath);

                if (totalDist < minDist)
                {
                    minDist = totalDist;
                    shortestPath = tempPath;
                }

            }
        }
        return minDist;
    }

    public double FindDistType(GraphNode prev, GraphNode curr)
    {
        if (prev.nodeType == 'L' && curr.nodeType == 'L')
        {
            return getColPickDist_L(prev);
        }
        else if (prev.nodeType == 'R' && curr.nodeType == 'R')
        {
            return getColPickDist_R(prev);
        }
        else if (curr.Neighbors.Count == 0)
        {
            return getColPickDist_L_to_end(prev);
        }
        else
        {
            return getDiagonalDist();
        }
    }

    public double getColPickDist_L_to_end(GraphNode prev)
    {
        return aisleLength;
    }

    /*söker genom efter item som ligger längst bort från rad 3 mellan colLeft och colRight. (För L noder)
    (1,2) och (3,4) är OK. 
    kod för kolla om n = startnod eller n = endnod
    0,1 , 2,3 , 4,5 är ej möjligt.
    om n = startnod eller endnod --> bara kolla en col (högercol resp. vänstercol) */
    public double getColPickDist_L(GraphNode n)
    {
        int colLeft = lanes.ElementAt(n.nodeNbr - 2).left;
        int colRight = lanes.ElementAt(n.nodeNbr - 2).right;

        var layout = LayoutManager.LayoutMatrix;

        if (isLane(colLeft, colRight) == true) {
            for (int row = shelvesPerAisle - 1; row >= 0; row--) {
                for (int col = colLeft; col <= colRight; col++) {
                    //if (layout[row, col] == orderNbr) {                                            //change 1 to orderNbr                         
                     if (orderSet.Contains(layout[row, col])) {                                                
                        return 2 * shelfLength * (row + 1) + aisleWidth;
                    }
                }
            }
        }
        return aisleWidth;
    }

    /*söker genom efter item som ligger längst bort från rad 0 mellan colLeft och colRight. (För R noder)
    (1,2) och (3,4) är OK. 
    kod för kolla om n = startnod eller n = endnod
    0,1 , 2,3 , 4,5 är ej möjligt.
    om n = startnod eller endnod --> bara kolla en col (högercol resp. vänstercol) */
    public double getColPickDist_R(GraphNode n)
    {
        var layout = LayoutManager.LayoutMatrix;

        if (isEnd(n.nodeNbr, n.nodeNbr + 1)) //om end bara kolla raderna för leftCol (alltså sista kolumnen)
        {
            int lastLeftCol = aisles * 2 - 1;
            for (int row = 0; row < shelvesPerAisle; row++) {
              //if (layout[row, lastLeftCol] == orderNbr) {                                               //change 1 to orderNbr
                if (orderSet.Contains(layout[row, lastLeftCol])) {                                               
                    return 2 * shelfLength * (shelvesPerAisle - row);
                }
            }
            return 0;
        }
        //if-statements för att överensstämma "Lane"-vektorn med nodeNbr.
        if (n.nodeNbr - 1 > lanes.Count)  //t.ex om nodeNbr = 4 och lanes.Count = 2
        {
           return aisleWidth;
        }
        else if (n.nodeNbr == 1)  //IsStart
        {
            for (int row = 0; row < shelvesPerAisle; row++) {
                //if (layout[row, 0] == orderNbr) {                                                          //change 1 to orderNbr     
                if (orderSet.Contains(layout[row, 0])) {                                                          
                    return 2 * shelfLength * (shelvesPerAisle - row) + aisleWidth;
                }
            }
            return aisleWidth;
        }
        else if (n.nodeNbr - 2 < 0) {
            return aisleWidth;
        }
        
        int colLeft = lanes.ElementAt(n.nodeNbr - 2).left;
        int colRight = lanes.ElementAt(n.nodeNbr - 2).right;

        if (isLane(colLeft, colRight) == true)
        {
            for (int row = 0; row < shelvesPerAisle; row++)
            {
                for (int col = colLeft; col <= colRight; col++)
                {
                    //if (layout[row, col] == orderNbr)                                                      //change 1 to orderNbr
                    if (orderSet.Contains(layout[row, col]))
                    {
                        return 2 * shelfLength*(shelvesPerAisle - row) + aisleWidth;
                    }
                }
            }
        }
        return aisleWidth;
    }

    public double getDiagonalDist()
    {
        return aisleLength + aisleWidth;
    }

    //1,2 och 3,4 ska returnera true. Resten skall ej.
    public bool isLane(int colLeft, int colRight)
    {
        Lane laneToCheck = new Lane(colLeft, colRight);

        if (lanes.Contains(laneToCheck))
        {
            return true;
        }
        return false;
    }

    public bool isEnd(int colLeft, int colRight) {
        if (colLeft == aisles + 1 && colRight == aisles + 2) {
            return true;
        }
        return false;
    }

    //TESTMETHOD
    public bool IsEmptyLayout() {
        for (int row = 0; row < shelvesPerAisle; row++) {
                for (int col = 0; col < aisles*2 ; col++)
                {
                    //if (LayoutManager.LayoutMatrix[row, col] == orderNbr) {                                //change 1 to orderNbr
                    if (orderSet.Contains(LayoutManager.LayoutMatrix[row, col])) {
                    return false;
                    }
                }
         }
        return true;
    }
}
