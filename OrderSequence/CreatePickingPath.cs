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
 
public class CreatePickingPath {
    public List<Rack> racks { get; set; } = new List<Rack>();
    private int firstLane;
    private int currLane;
    private int currRack;
    private int aisleRackLength;
    
    public CreatePickingPath(Graph g) {
        PickingPath pp = new PickingPath(g);  
        int aisles = g.aisles;
        int racksPerAisle = g.shelvesPerAisle;
        createLayout(aisles, racksPerAisle);
        //printRacks();
    }
 
    public void createLayout(int nbrAisles, int aisleRackLength)
    {
        firstLane = 1;
        currLane = firstLane + 1;
        currRack = 1;
        this.aisleRackLength = aisleRackLength;
 
        createFirstLane();
        //OM UNEVEN NBR OF AISLES. End är neråt
        if (nbrAisles % 2 == 1)
        {
            for (int i = 0; i < (nbrAisles - 1) / 2; i++)
            {
                createMiddleLanesUPtoDOWN();
                currLane = currLane + 2;
                currRack = 1;
                createMiddleLanesDOWNtoUP();
                currLane = currLane + 2;
                currRack = 1;
            }
            createLastLaneUPtoDOWN();
        }
        else
        {
            //OM EVEN NBR OF AISLES. End är uppåt
            for (int i = 0; i < nbrAisles / 2 - 1; i++)
            {
                createMiddleLanesUPtoDOWN();
                currLane = currLane + 2;
                currRack = 1;
                createMiddleLanesDOWNtoUP();
                currLane = currLane + 2;
                currRack = 1;
            }
            createMiddleLanesUPtoDOWN();
            currLane = currLane + 2;
            createLastLaneDOWNtoUP();
        }
    }
 
    //OM PICKER RÖR SIG UPP --> NER  
    public void createMiddleLanesUPtoDOWN()
    {
        //Create middle lane laneNbr
        int[] lanes = new int[aisleRackLength * 2];
 
            for (int j = 0; j < lanes.Length; j++)
            {
                if (j % 2 == 0)
                {     //om jämn
                    lanes[j] = currLane;
                }
                else
                {
                    lanes[j] = currLane + 1;
                }
            }

        //Create middle lane rackNbr
        for (int i = 0; i < lanes.Length; i++)
        {
            racks.Add(new Rack(currRack, lanes[i]));
            if (i % 2 != 0)
            {
                currRack++;
            }
        }
    }
 
    public void createMiddleLanesDOWNtoUP()
    {
        //Create middle lane laneNbr
        int[] lanes = new int[aisleRackLength * 2];
 
            for (int j = 0; j < lanes.Length; j++)
            {
                if (j % 2 == 0)
                {     //om jämn
                    lanes[j] = currLane;
                }
                else
                {
                    lanes[j] = currLane + 1;
                }
            }
        //Create middle lane rackNbr
        currRack = aisleRackLength;
        for (int i = 0; i < lanes.Length; i++)
        {
            racks.Add(new Rack(currRack, lanes[i]));
            if (i % 2 != 0)
            {
                currRack--;
            }
        }
    }
 
    public void createFirstLane()
    {
        for (int i = aisleRackLength; i > 0; i--)
        {
            racks.Add(new Rack(i, firstLane));
        }
    }
 
    public void createLastLaneUPtoDOWN()
    {
        for (int i = 1; i < aisleRackLength + 1; i++)
        {
            racks.Add(new Rack(i, currLane));
        }
    }
 
    public void createLastLaneDOWNtoUP()
    {
        for (int i = aisleRackLength; i > 0; i--)
        {
            racks.Add(new Rack(i, currLane));
        }
    }
 
    public void printRacks()
    {
        foreach (Rack r in racks)
        {
            Console.WriteLine(r.rackNbr+" " +r.laneNbr);
        }
        Console.WriteLine("racks length: " + racks.Count);
    }
}