namespace ConsoleApp1;

/*
StartupWindow (GUI) --> new graph --> CreatePickLocationsManyOrders_GUI(g) --> new OrderSequenceAnalysis --> 
                            --> pathNodes (contains solution)
*/


public class RunAlgorithm
{
    int aisles = 2; //CHANGE
    int shelvesPerAisle = 2;    //CHANGE
    int orders = 2; //CHANGE
    int nbrOrdersPerLayers = 2; //CHANGE
    double shelfLength = 1; //FINAL
    double shelfWidth = 1;  //FINAL

    //Search Algorithm will be Random by default

    int[,] layoutMatrix { get; set; }

    public RunAlgorithm()
    {
        getWarehouseLayoutCharacteristics();
        getUnitLoadConfigurationParameters();

        Graph g = new Graph(aisles,  shelvesPerAisle,  orders,  nbrOrdersPerLayers,  shelfLength,  shelfWidth);
        g.LayoutManager.LayoutMatrix = getItemIventoryLocations();
    }

    public int[,] getItemIventoryLocations()
    {
        //Fix and process from server data
        return null;
    }

    public void getWarehouseLayoutCharacteristics()
    {
        //Fix and process from server data
        //Aisles
        //ShelverPerAisle
    }

    public void getUnitLoadConfigurationParameters()
    {
        //Fix and process from server data
        //Orders
        //NbrOrdersPerLayers
    }

}