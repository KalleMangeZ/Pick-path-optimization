namespace ConsoleApp1;

public class OrderSequenceAnalysis
{
Graph g;
public int[,] lm;
public List<int> orderSequence {get; set;} = new List<int>();
CreatePickingPath pp;

    public OrderSequenceAnalysis(Graph g) {
            this.g = g;
            lm = g.LayoutManager.LayoutMatrix;

            pp = new CreatePickingPath(g);
            CreateOrderSequence();
    }

    public void CreateOrderSequence() {
        /*for(int i = 0; i < lm.GetLength(0); i++) {
            Console.WriteLine();
            for(int j = 0; j < lm.GetLength(1); j++) {
                Console.Write("["+i+"]["+j+"]: "+lm[i,j] + " ");
            }
        }*/

        for(int i = 0; i < pp.racks.Count; i++) {
        int r = pp.racks[i].rackNbr - 1;
        int c = pp.racks[i].laneNbr - 1;
            if(lm[r, c] != 0) {
                orderSequence.Add(lm[r, c]);
            }
        }

        Console.WriteLine("\nSerpentine order sequence: ");
        for(int i = 0; i < orderSequence.Count; i++) {
            Console.Write(orderSequence[i] + " ");
        }
    }

    public void AnalyzeOrderSequence() {

    }
}
