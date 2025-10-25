using ConsoleApp1;
using System;
using System.Collections.Generic;


class Program {

    static void Main(string[] args)
    {
        Graph g = new Graph(3, 4, 1, 1); //Graph(int aisles, int shelvesPerAisle, double shelfLength, double shelfWidth) 
        CreateWindow(g);
    }

    public static void CreateWindow(Graph g) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new RectangleForm(g, g.pathNodes)); // This launches the form
        //RectangleForm rf = new RectangleForm(g, g.pathNodes); onödig?
    }
}