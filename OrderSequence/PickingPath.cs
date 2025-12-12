namespace ConsoleApp1;
 
public class PickingPath
{
    public PickingPath(Graph g)
    {
        CreateLayout(g.shelvesPerAisle, g.aisles); //(R,C)
    }
 
    public void CreateLayout(int aisles, int racksPerAisle) {
        int[][] layout = new int[aisles][];
 
        for (int i = layout.Length - 1; i >= 0; i--)
        {
            layout[i] = new int[racksPerAisle * 2];
        }
        //Console.WriteLine("Rows: " + layout.Length + " Columns: " + layout[0].Length);
    }
}