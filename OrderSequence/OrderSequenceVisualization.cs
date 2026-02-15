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
            orderSequenceString = string.Join("   ", a.orderSequence);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(
                this.AutoScrollPosition.X,
                this.AutoScrollPosition.Y
            );
            base.OnPaint(e);
            ShowOrderStrings(e.Graphics);
            //PrintOrderSequence();
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
            graphics.DrawString("Order sequence:", normalFont, Brushes.Black, new Point(50, 25));
            int startX = 100;
            int ySequence = 50;
            int rowLength = 32;

            List<float> sequencePositions = new List<float>();

            float currentX = startX;

            for (int i = 0; i < a.orderSequence.Count; i++)
            {
                string text = a.orderSequence[i].ToString();

                sequencePositions.Add(currentX);

                graphics.DrawString(text, normalFont, Brushes.Black, currentX, ySequence);

                SizeF size = graphics.MeasureString(text + "   ", normalFont);
                currentX += size.Width;
            }

            // Draw order lines
            for (int i = 0; i < a.orders.Count; i++)
            {
                Order orderObj = a.orders[i];

                int y = 50 + rowLength * orderObj.orderNumber;

                graphics.DrawString(orderObj.orderNumber.ToString(),
                                    normalFont, Brushes.Black, 50, y);

                int startIndex = orderObj.orderStart;
                int endIndex   = orderObj.orderEnd;

                float x1 = sequencePositions[startIndex] + 10;
                float x2 = sequencePositions[endIndex] +
                    graphics.MeasureString(
                        a.orderSequence[endIndex].ToString(),
                        normalFont).Width - 5;

                graphics.DrawLine(pen, x1, y + 10, x2, y + 10);
            }

        }

    }
}
