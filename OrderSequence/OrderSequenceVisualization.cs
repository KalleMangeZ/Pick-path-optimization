using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConsoleApp1
{
    public class OrderSequenceVisualization : Form
    {
        private Graph g;
        private OrderSequenceAnalysis a;
        private string orderSequenceString;
        private Font normalFont = new Font("Arial", 12);
        private Pen pen = new Pen(Color.Black, 2);
        private bool visualizationCreated = false; // Guard flag

        public OrderSequenceVisualization(Graph g, OrderSequenceAnalysis a)
        {
            this.g = g;
            this.a = a;

            this.Text = "Order sequence visualized";
            this.Size = new Size(1000, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.AutoScroll = true;
            // Make the virtual canvas big enough (adjust as needed)
            this.AutoScrollMinSize = new Size(
                150 + a.orderSequence.Count * 25,   // width
                100 + g.orders * 35                  // height
            );
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ShowOrderStrings(e.Graphics);
            PrintOrderSequence();
        }

        private void PrintOrderSequence()
        {
            String orderSequenceString_concat = "";
            for(int i = 0; i < a.orderSequence.Count; i++) {
                orderSequenceString_concat += a.orderSequence[i];
            }
            Console.WriteLine(orderSequenceString_concat);
        }

        private void ShowOrderStrings(Graphics graphics)
        {
            graphics.DrawString("Order sequence:",normalFont,Brushes.Black,new Point(50, 25));
            for(int i = 0; i < a.orderSequence.Count; i++) {
                orderSequenceString += a.orderSequence[i] + "   ";
            }

            int rowLength = 32;
            int spaceBetweenOrders = 22;

            for(int order = 1; order < g.orders+1; order++) {
                graphics.DrawString(order.ToString(),normalFont,Brushes.Black,new Point(50, 50+rowLength*order));
                graphics.DrawLine(pen, 
                        109+a.orderStartInSequence[order-1]*spaceBetweenOrders, 50+rowLength*order,
                        109+a.orderEndInSequence[order-1]*spaceBetweenOrders, 50+rowLength*order);

            graphics.DrawString(orderSequenceString,normalFont,Brushes.Black,new Point(100, 50));
            }
        }
    }
}
