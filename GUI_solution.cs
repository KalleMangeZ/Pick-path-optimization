namespace ConsoleApp1;

using System;
using System.Drawing;
using System.Windows.Forms;

public class GUI_solution : Form { //döpa om till GUI
    Graph g;
    private List<GraphNode> shortestNodePath { get; set; }
    private int centerXStart, centerYStart, centerXEnd, centerYEnd;
    private int xMove, Y_R, Y_L;
    private int shelfLength, shelfWidth;
    private int aisleLength;
    private int aisleWidth;
    private int aisleToAisleDist;
    private int yDistStartToRNode;
    private Pen pen, pen2;

    public GUI_solution(Graph g, List<GraphNode> pathNodes) {
        this.g = g;
        this.Text = "Warehouse Pick Locations";
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

    }

    private void DrawRectangle(object sender, PaintEventArgs e) {
        Graphics graphics = e.Graphics;
        pen = new Pen(Color.Blue, 2);
        pen2 = new Pen(Color.Red, 2);

        int firstAisleCol = 0;
        for (int i = 0; i < g.lanes.Count + 1; i++) {
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < g.shelvesPerAisle; y++) {
                    Rectangle rack = new Rectangle(x * shelfLength + shelfLength + i * aisleToAisleDist, y * shelfWidth + shelfWidth, shelfLength, shelfWidth); //(x, y, width, height)

                    graphics.DrawRectangle(pen, rack);
                    string text = g.LayoutManager.LayoutMatrix[y, x + firstAisleCol].ToString();
                    using (Font font = new Font("Arial", 8))
                    {
                        SizeF textSize = graphics.MeasureString(text, font);
                        float textX = rack.X + (rack.Width - textSize.Width) / 2;
                        float textY = rack.Y + (rack.Height - textSize.Height) / 2;
                        graphics.DrawString(text, font, Brushes.Black, textX, textY);

                        if (i == 0 && x == 0 && y == g.shelvesPerAisle - 1) {
                            TextBox scaleShelfWidth = new TextBox();
                            scaleShelfWidth.Location = new Point((int)textX - (int)(0.85 * shelfLength), (int)textY);
                            scaleShelfWidth.ReadOnly = true;
                            scaleShelfWidth.Width = 20;
                            scaleShelfWidth.Height = 20;
                            scaleShelfWidth.Text = Convert.ToString(shelfLength);
                            this.Controls.Add(scaleShelfWidth);

                            TextBox scaleShelfLength = new TextBox();
                            scaleShelfLength.Location = new Point((int)textX, (int)textY + (int)(shelfLength / 2));
                            scaleShelfLength.ReadOnly = true;
                            scaleShelfLength.Width = 20;
                            scaleShelfLength.Height = 20;
                            scaleShelfLength.Text = Convert.ToString(shelfWidth);
                            this.Controls.Add(scaleShelfLength);


                        }

                    } 
                }
            }
            firstAisleCol = firstAisleCol + 2;
        }
        DrawStartAndEndCircle(pen2, graphics, shelfLength, g.shelvesPerAisle);
        DrawPath(graphics);
        DrawNodes(graphics);
        DisplayShortestPathString(graphics);
    }

    public void DrawStartAndEndCircle(Pen pen2, Graphics graphics, int shelfLength, int shelvesPerAisle) {
        int radius = 10;
        centerXStart = shelfLength/ 2;
        centerYStart = shelfLength * (g.shelvesPerAisle + 2);
        string label1 = "Start";
        Font font = new Font("Arial", 7);
        Brush brush = Brushes.Black;
        SizeF textSize = graphics.MeasureString(label1, font);
        graphics.DrawString(label1, font, brush, centerXStart - textSize.Width / 2, centerYStart - textSize.Height / 2 + 2 * radius);
        graphics.DrawEllipse(pen2, centerXStart - radius, centerYStart - radius, radius * 2, radius * 2);

        centerXEnd = centerXStart + g.aisles * aisleToAisleDist - 2 * shelfLength + xMove;
        centerYEnd = shelfLength * g.shelvesPerAisle + 2 * shelfLength;
        string label2 = "End";
        Font font2 = new Font("Arial", 7);
        Brush brush2 = Brushes.Black;
        SizeF textSize2 = graphics.MeasureString(label2, font2);
        graphics.DrawString(label2, font2, brush2, centerXEnd - textSize2.Width / 2, centerYEnd - textSize2.Height / 2 + 2 * radius);
        graphics.DrawEllipse(pen2, centerXEnd - radius, centerYEnd - radius, radius * 2, radius * 2);

    }

    public void DrawPath(Graphics graphics) {
        int currX = centerXStart;
        int currY = centerYStart;

        Y_R = centerYStart;
        Y_L = centerYStart - ((g.shelvesPerAisle+1) * shelfWidth + shelfWidth / 2);
        //xMove = aisleToAisleDist;

        for (int i = 0; i < shortestNodePath.Count - 1; i++) {
            xMove = aisleToAisleDist;
            GraphNode curr = shortestNodePath[i];
            
            if (curr.nodeType == 'L' && shortestNodePath[i + 1].Neighbors.Count == 0) {
                graphics.DrawLine(pen2, currX, Y_L, currX, Y_R);
                graphics.DrawLine(pen2, currX, Y_R, centerXEnd, Y_R);  
            }
            
            if (curr.nodeType == 'R' && shortestNodePath[i + 1].Neighbors.Count == 0) {
                int yDist = (int)(g.getColPickDist_R(curr)/2) * shelfLength;

                if (g.getColPickDist_R(curr) / 2 == 0) {
                    yDist = 0;
                    graphics.DrawLine(pen2, currX, Y_R, currX, Y_R - yDist);
                } else {
                    graphics.DrawLine(pen2, currX, Y_R, currX, Y_R - yDist - yDistStartToRNode / 2);
                }

                graphics.DrawLine(pen2, currX, Y_R, centerXEnd, Y_R);
            }
            
            GraphNode next = shortestNodePath[i + 1];
            
            if (curr.nodeType == 'R' && next.nodeType == 'L') {
                if (curr.nodeNbr == 1) { //if the node is the first in the aisle, only move xMove - centerXStart
                    xMove = xMove - centerXStart;
                }

                graphics.DrawLine(pen2, currX, Y_R, currX, Y_L);
                graphics.DrawLine(pen2, currX, Y_L, currX + xMove, Y_L);
                currX = currX + xMove;
                currY = Y_L;
            }

            if (curr.nodeType == 'L' && next.nodeType == 'R' && curr.nodeNbr != g.aisles+1) {
                graphics.DrawLine(pen2, currX, Y_L, currX, Y_R);
                graphics.DrawLine(pen2, currX, Y_R, currX + xMove, Y_R);
                currX = currX + xMove;
                currY = Y_R;
            }

            if (curr.nodeType == 'L' && next.nodeType == 'L' && curr.nodeNbr != g.aisles+1) {
                int yDist = (int)(g.getColPickDist_L(curr) / 2 - 1) * shelfLength;
                
                if(g.getColPickDist_R(curr)/2 == 0) {
                    yDist = 0;
                }
                
                graphics.DrawLine(pen2, currX, Y_L, currX, Y_L + yDist);
                graphics.DrawLine(pen2, currX, Y_L, currX + xMove, Y_L);
                currX = currX + xMove;
                currY = Y_L;
            }
            
            if (curr.nodeType == 'R' && next.nodeType == 'R' && curr.nodeNbr != g.aisles+1) { //FIXA
                int yDist = (int)((g.getColPickDist_R(curr) / 2 - 1) * shelfLength + yDistStartToRNode / 2);
                
                if(g.getColPickDist_R(curr)/2 == 0) {
                    yDist = 0;
                }

                if (curr.nodeNbr == 1) { //if the node is the first in the aisle, only move xMove - centerXStart
                    xMove = xMove - centerXStart;
                } 
                
                graphics.DrawLine(pen2, currX, Y_R, currX, Y_R - yDist);
                graphics.DrawLine(pen2, currX, Y_R, currX + xMove, Y_R);
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
            graphics.DrawString(nodeStringNameL, new Font("Arial", 10), Brushes.Black, stringPointL);

            //draw R-nodes
            String nodeStringNameR = "R" + Convert.ToString(i + 2);
            Point stringPointR = new Point(currX, Y_R + yDistStartToRNode);
            if (i == 0) {
                graphics.DrawString("R1", new Font("Arial", 10), Brushes.Black, new Point(currX - xMove, Y_R + yDistStartToRNode));
            }
            graphics.DrawString(nodeStringNameR, new Font("Arial", 10), Brushes.Black, stringPointR);

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
        graphics.DrawString(shortestPathString, new Font("Arial", 10), Brushes.Black, new Point(50, Y_R+shelfLength/3));
    }
}