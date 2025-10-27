using ConsoleApp1;
using System;
using System.Collections.Generic;


class Program {

    static void Main(string[] args)
    {
        Graph g = new Graph(4,10, 1, 1); //Graph(int aisles, int shelvesPerAisle, double shelfLength = 1, double shelfWidth = 1) 
        CreateStartupWindow(g);
        CreateWindow(g);
    }

    public static void CreateStartupWindow(Graph g) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new GUI_createPickLocations(g));
    }
    

    public static void CreateWindow(Graph g) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new GUI_solution(g, g.pathNodes)); // This launches the form
    }
}