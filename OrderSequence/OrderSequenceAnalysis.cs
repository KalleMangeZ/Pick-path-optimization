namespace ConsoleApp1;

public class OrderSequenceAnalysis
{
Graph g;
public int[,] lm;
public List<int> orderSequence {get; set;} = new List<int>();
public List<int> orderStartInSequence {get; set;} = new List<int>();
public List<int> orderEndInSequence {get; set;} = new List<int>();
CreatePickingPath pp;

    public OrderSequenceAnalysis(Graph g) {
            this.g = g;
            lm = g.LayoutManager.LayoutMatrix;

            pp = new CreatePickingPath(g);
            CreateOrderSequence();
            AnalyzeOrderSequence();
            CreateStackPair();
            OrderSequenceVisualization osv = new OrderSequenceVisualization(g, this);
            osv.Show();
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

        Console.WriteLine("Index Sequence:");
        for(int i = 0; i < orderSequence.Count; i++) {
            Console.Write(i + " ");
        }

        Console.WriteLine("\nSerpentine order sequence: ");
        for(int i = 0; i < orderSequence.Count; i++) {
            Console.Write(orderSequence[i] + " ");
        }
    }

    public void AnalyzeOrderSequence() {
            orderStartInSequence = new List<int>();
            orderEndInSequence = new List<int>();

            for(int orderNum = 1; orderNum < g.orders+1; orderNum++) {
                for(int orderStart = 0; orderStart < orderSequence.Count; orderStart++) {
                    if(orderSequence[orderStart] == orderNum) {
                        orderStartInSequence.Add(orderStart);
                        break;
                    }
                }    
            }

            for(int orderNum = 1; orderNum < g.orders+1; orderNum++) {
                for(int orderEnd = orderSequence.Count-1; orderEnd >= 0; orderEnd--) {
                    if(orderSequence[orderEnd] == orderNum) {
                        orderEndInSequence.Add(orderEnd);
                        break;  
                }
            }
            }    

            Console.WriteLine();
            for(int i = 0; i < g.orders; i++) {
                Console.WriteLine("Order " + (i+1) + " , Start: " + orderStartInSequence[i] + " , End: " + orderEndInSequence[i]);
            }
        }

    //marks out if and so which box that can stack on top of another box
    public void CreateStackPair() {
        for(int i = 0; i < orderStartInSequence.Count; i++) {
            for(int j = 0; j < orderEndInSequence.Count; j++) {
                if(orderStartInSequence[i] > orderEndInSequence[j]) {
                    Console.WriteLine("Order " + (i+1) + " can be stacked on top of Order " + (j+1));
                }
            }
        }
    }

}
