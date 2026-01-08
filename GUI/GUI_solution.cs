namespace ConsoleApp1;

using System;
using System.Drawing;
using System.Windows.Forms;

public class GUI_solution : Form { 
    Graph g;
    private List<GraphNode> shortestNodePath { get; set; }
    private int centerXStart, centerYStart, centerXEnd, centerYEnd;
    private int xMove, Y_R, Y_L;
    private int shelfLength, shelfWidth;
    private int aisleLength;
    private int aisleWidth;
    private int aisleToAisleDist;
    private int yDistStartToRNode;
    private Pen bluePen = new Pen(Color.Blue, 2);
    private Pen redPen = new Pen(Color.Red, 2);
    private Pen greyPen = new Pen(Color.FromArgb(240, 240, 240), 2);
    private Font smallFont = new Font("Arial", 7);
    private Font normalFont = new Font("Arial", 10);
    private static OrderSequenceAnalysis? _instance;

    public GUI_solution(Graph g, List<GraphNode> pathNodes) {
        this.g = g;
        this.Text = $"Warehouse Pick Locations for order(s) " + g.ListedOrderString();
        this.Size = new Size(1000, 700);
        this.Location = new Point(0, 0);
        shelfLength = 50;
        shelfWidth = 50;
        aisleToAisleDist = 200;
        yDistStartToRNode = 50;
        aisleLength = shelfLength * 2 + centerXStart;
        aisleWidth = g.shelvesPerAisle * shelfWidth;
        this.Paint += new PaintEventHandler(DrawRectangle);
        shortestNodePath = pathNodes;
        this.CenterToScreen();
        this.DoubleBuffered = true;
        GetOrCreate(g);
    }

    public static OrderSequenceAnalysis GetOrCreate(Graph g)
    {
        if (_instance == null)
            _instance = new OrderSequenceAnalysis(g);

        return _instance;
    }

    private void DrawRectangle(object sender, PaintEventArgs e) {
        Graphics graphics = e.Graphics;

        int firstAisleCol = 0;
        for (int i = 0; i < g.lanes.Count + 1; i++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < g.shelvesPerAisle; y++) {
                    Rectangle rack = new Rectangle(x * shelfLength + shelfLength + i * aisleToAisleDist, y * shelfWidth + shelfWidth, shelfLength, shelfWidth); //(x, y, width, height)

                    graphics.DrawRectangle(bluePen, rack);
                    string text = g.LayoutManager.LayoutMatrix[y, x + firstAisleCol].ToString();
                  
                        SizeF textSize = graphics.MeasureString(text, smallFont);
                        float textX = rack.X + (rack.Width - textSize.Width) / 2;
                        float textY = rack.Y + (rack.Height - textSize.Height) / 2;
                        graphics.DrawString(text, smallFont, Brushes.Black, textX, textY);

                        if (i == 0 && x == 0 && y == g.shelvesPerAisle - 1) {
                        graphics.DrawString(shelfLength.ToString(), smallFont, Brushes.Black, textX - 0.85f * shelfLength, textY);
                        graphics.DrawString(shelfWidth.ToString(), smallFont, Brushes.Black, textX, textY + shelfLength / 2); 
                        } 
                }
            }
            firstAisleCol = firstAisleCol + 2;
        }
        DrawStartAndEndCircle(redPen, graphics, shelfLength, g.shelvesPerAisle);
        DrawPath(graphics);
        DrawNodes(graphics);
        DisplayShortestPathString(graphics);
    }

    public void DrawStartAndEndCircle(Pen redPen, Graphics graphics, int shelfLength, int shelvesPerAisle) {
        int radius = 10;
        centerXStart = shelfLength/ 2;
        centerYStart = shelfLength * (g.shelvesPerAisle + 2);
        string label1 = "Start";
        Brush brush = Brushes.Black;
        SizeF textSize = graphics.MeasureString(label1, smallFont);
        graphics.DrawString(label1, smallFont, brush, centerXStart - textSize.Width / 2, centerYStart - textSize.Height / 2 + 2 * radius);
        graphics.DrawEllipse(redPen, centerXStart - radius, centerYStart - radius, radius * 2, radius * 2);

        centerXEnd = centerXStart + g.aisles * aisleToAisleDist;
        centerYEnd = shelfLength * g.shelvesPerAisle + 2 * shelfLength;
        string label2 = "End";
        Brush brush2 = Brushes.Black;
        SizeF textSize2 = graphics.MeasureString(label2, smallFont);
        graphics.DrawString(label2, smallFont, brush2, centerXEnd - textSize2.Width / 2, centerYEnd - textSize2.Height / 2 + 2 * radius);
        graphics.DrawEllipse(redPen, centerXEnd - radius, centerYEnd - radius, radius * 2, radius * 2);
    }

    public void DrawPath(Graphics graphics) {
        int currX = centerXStart;
        int currY = centerYStart;

        Y_R = centerYStart;
        Y_L = centerYStart - ((g.shelvesPerAisle + 1) * shelfWidth + shelfWidth / 2);
        //xMove = aisleToAisleDist;

        for (int i = 0; i < shortestNodePath.Count - 1; i++) {
            xMove = aisleToAisleDist;
            GraphNode curr = shortestNodePath[i];

            if (curr.nodeType == 'L' && shortestNodePath[i + 1].Neighbors.Count == 0) {
                graphics.DrawLine(redPen, currX, Y_L, currX, Y_R);
                graphics.DrawLine(redPen, currX, Y_R, centerXEnd, Y_R);
            }

            if (curr.nodeType == 'R' && shortestNodePath[i + 1].Neighbors.Count == 0) {
                int yDist = (int)(g.getColPickDist_R(curr) / 2) * shelfLength;

                if (g.getColPickDist_R(curr) / 2 == 0) {
                    yDist = 0;
                    graphics.DrawLine(redPen, currX, Y_R, currX, Y_R - yDist);
                }
                else {
                    graphics.DrawLine(redPen, currX-shelfLength/4, Y_R, currX-shelfLength/4, Y_R - yDist - yDistStartToRNode / 2); //vert test
                                graphics.DrawLine(redPen, currX-shelfLength/4, Y_R - yDist - yDistStartToRNode / 2, currX+shelfLength/4, Y_R - yDist - yDistStartToRNode / 2); //hort TEST
                                graphics.DrawLine(redPen, currX+shelfLength/4, Y_R, currX+shelfLength/4, Y_R - yDist - yDistStartToRNode / 2); //vert TEST          
                }

                graphics.DrawLine(redPen, currX, Y_R, centerXEnd, Y_R);
                if(yDist != 0) {
                    graphics.DrawLine(greyPen, currX-shelfLength/4, Y_R, currX+shelfLength/4, Y_R); //GREY TEST
                }
            }

            GraphNode next = shortestNodePath[i + 1];

            if (curr.nodeType == 'R' && next.nodeType == 'L') {
                if (curr.nodeNbr == 1) { //if the node is the first in the aisle, only move xMove - centerXStart
                    xMove = xMove - centerXStart;
                }

                graphics.DrawLine(redPen, currX, Y_R, currX, Y_L);
                graphics.DrawLine(redPen, currX, Y_L, currX + xMove, Y_L);
                
                currX = currX + xMove;
                currY = Y_L;
            }

            if (curr.nodeType == 'L' && next.nodeType == 'R' && curr.nodeNbr != g.aisles + 1) {
                graphics.DrawLine(redPen, currX, Y_L, currX, Y_R);
                graphics.DrawLine(redPen, currX, Y_R, currX + xMove, Y_R);
                currX = currX + xMove;
                currY = Y_R;
            }

            if (curr.nodeType == 'L' && next.nodeType == 'L' && curr.nodeNbr != g.aisles + 1) {
                int yDist = (int)(g.getColPickDist_L(curr) / 2 - 1) * shelfLength;

                if (g.getColPickDist_R(curr) / 2 == 0) {
                    yDist = 0;
                }

                graphics.DrawLine(redPen, currX-shelfLength/4, Y_L, currX-shelfLength/4, Y_L + yDist); //TEST MED -shelfLength/4
                                graphics.DrawLine(redPen, currX-shelfLength/4, Y_L+yDist, currX+shelfLength/4, Y_L + yDist); //hort TEST
                                graphics.DrawLine(redPen, currX+shelfLength/4, Y_L, currX+shelfLength/4, Y_L + yDist); //vert TEST
                graphics.DrawLine(redPen, currX, Y_L, currX + xMove, Y_L);
                                if(yDist != 0) {
                                graphics.DrawLine(greyPen, currX-shelfLength/4, Y_L, currX+shelfLength/4, Y_L); //hort TEST
                                }
                currX = currX + xMove;
                currY = Y_L;
            }

            if (curr.nodeType == 'R' && next.nodeType == 'R' && curr.nodeNbr != g.aisles + 1) { //FIXA
                int yDist = (int)((g.getColPickDist_R(curr) / 2 - 1) * shelfLength + yDistStartToRNode / 2);

                if (g.getColPickDist_R(curr) / 2 == 0) {
                    yDist = 0;
                }

                //Fix R to R pick if getColPickDist_R == 2.
                if (g.getColPickDist_R(curr) == g.aisleWidth) {
                    yDist = 0;
                }

                if (curr.nodeNbr == 1) { //if the node is the first in the aisle, only move xMove - centerXStart
                    xMove = xMove - centerXStart;
                } 

                graphics.DrawLine(redPen, currX-shelfLength/4, Y_R, currX-shelfLength/4, Y_R - yDist);                     //vert  TEST MED -shelfLength/4
                                graphics.DrawLine(redPen, currX-shelfLength/4, Y_R-yDist, currX+shelfLength/4, Y_R-yDist); //hort TEST
                                graphics.DrawLine(redPen, currX+shelfLength/4, Y_R, currX+shelfLength/4, Y_R - yDist); //vert TEST
                graphics.DrawLine(redPen, currX, Y_R, currX + xMove, Y_R);
                                if(yDist != 0) {
                                graphics.DrawLine(greyPen, currX-shelfLength/4, Y_R, currX+shelfLength/4, Y_R); //hort TEST
                                }
                currX = currX + xMove;
                currY = Y_R;
            }

        }
    }

    public void DrawNodes(Graphics graphics) {
        int currX = centerXStart + xMove;
        for (int i = 0; i < g.LayoutManager.lanes.Count + 1; i++) {
            //draw L-nodes
            String nodeStringNameL = "L" + Convert.ToString(i + 2);
            Point stringPointL = new Point(currX, Y_L);
            graphics.DrawString(nodeStringNameL, normalFont, Brushes.Black, stringPointL);

            //draw R-nodes
            String nodeStringNameR = "R" + Convert.ToString(i + 2);
            Point stringPointR = new Point(currX, Y_R + yDistStartToRNode);
            if (i == 0) {
                graphics.DrawString("R1", normalFont, Brushes.Black, new Point(currX - xMove, Y_R + yDistStartToRNode));
            }
            graphics.DrawString(nodeStringNameR, normalFont, Brushes.Black, stringPointR);

            currX = currX + xMove;
        }
    }

    public void DisplayShortestPathString(Graphics graphics) {
        string shortestPathString = "Shortest path route: ";

        for (int i = 0; i < shortestNodePath.Count; i++) {
            shortestPathString += shortestNodePath[i].Name;

            if (i < shortestNodePath.Count - 1) {
                shortestPathString += " → ";
            }
        }
<<<<<<< HEAD:GUI_solution.cs
        shortestPathString += $" | Total distance: {g.shortestDistance} units";
=======
        shortestPathString += $" | Total distance: {g.shortestDistance}";
>>>>>>> allowManyOrders:GUI/GUI_solution.cs
        graphics.DrawString(shortestPathString, normalFont, Brushes.Black, new Point(50, Y_R + shelfLength / 3));
    }
}