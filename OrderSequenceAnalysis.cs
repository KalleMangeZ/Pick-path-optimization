namespace ConsoleApp1;

public class OrderSequenceAnalysis
{
Graph g;
public int[,] lm;

public OrderSequenceAnalysis(Graph g) {
        this.g = g;
        lm = g.LayoutManager.LayoutMatrix;

        AnalyzeOrderSequence();
}

public void AnalyzeOrderSequence() {

    for(int i = 0; i < lm.GetLength(0); i++) {
        Console.WriteLine();
        for(int j = 0; j < lm.GetLength(1); j++) {
            Console.Write(lm[i,j] + " ");
        }
    }
}





}
