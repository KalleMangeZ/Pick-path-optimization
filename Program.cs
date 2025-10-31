using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;


class Program {

    static void Main(string[] args)
    {
        Graph g = new Graph(4,8, 1, 1); //Graph(int aisles, int shelvesPerAisle, double shelfLength = 1, double shelfWidth = 1) 
        //CreateStartWindow(g);
        CreateWindow(g);
    }

    public static void CreateWindow(Graph g) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new GUI_solution(g, g.pathNodes)); 
    }

    public static void CreateStartWindow(Graph g) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new StartupWindow(g)); 
    }

}