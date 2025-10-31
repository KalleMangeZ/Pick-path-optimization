using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;


class Program {

    static void Main(string[] args) {
        //Org: Graph g = new Graph(3, 4, 1, 1); //Graph(int aisles, int shelvesPerAisle, double shelfLength = 1, double shelfWidth = 1) 
        //Org: CreateStartWindow(g);
        CreateStartWindow();
        //CreateWindow(g);
    }

    //Org: public static void CreateStartWindow(Graph g) {
    public static void CreateStartWindow() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        //Org: Application.Run(new StartupWindow(g));
        Application.Run(new StartupWindow());
    }

     /*
    public static void CreateWindow(Graph g) {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new GUI_solution(g, g.pathNodes));
    }*/

}