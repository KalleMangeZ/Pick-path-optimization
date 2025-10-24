using ConsoleApp1;
using System;
using System.Collections.Generic;
/*
Skapar en serpentine layout för lager med givet antal aisles (gångar) och givet antal racks per aisle.
Layouten börjar alltid med att plockaren rör sig från ner till upp i den första aislen (fr. vänster t höger).
Vid plock i mitten aisles tar plockaren alltid item från varannan lane.
Exempel: (1, 3) ger

 I----I----I   
 I 11 I 21 I
 I----I----I
 I 12 I 22 I
 I----I----I
 I 13 I 23 I
 I----I----I
start       end
*/

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