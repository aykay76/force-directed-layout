using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Net;
using System.Windows.Shapes;

namespace WPF
{
	public class Node
	{
        public Page1 canvas;
		public Ellipse ellipse;
		public double x, y, z;
		public double mass;
		public string label;
		public Graph graph;
		public Node parent;
		public double[] f = new double[3];
        public bool dragging = false;
        public double offx = 0.0;
        public double offy = 0.0;

        public bool Selected
        {
            set
            {
                if (value)
                {
                    ellipse.Stroke = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    ellipse.Stroke = new SolidColorBrush(Colors.Red);
                }
            }
        }

        public Node(Graph g, double m, double x, double y)
        {
			graph = g;
            mass = m;
			this.x = x;
			this.y = y;
        }

        public void createUI(Page1 parent)
        {
            canvas = parent;

            ellipse = new Ellipse();
            ellipse.Width = mass * 10;
            ellipse.Height = mass * 10;
            ellipse.SetValue(Canvas.LeftProperty, x - ellipse.Width / 2);
            ellipse.SetValue(Canvas.TopProperty, y - ellipse.Height / 2);
            ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            ellipse.SetValue(Canvas.ZIndexProperty, 2);
            canvas.parentCanvas.Children.Add(ellipse);

			ellipse.MouseLeftButtonDown += ellipse_MouseLeftButtonDown;
			ellipse.MouseMove += new MouseEventHandler(ellipse_MouseMove);
			ellipse.MouseLeftButtonUp += ellipse_MouseLeftButtonUp;
        }

        void ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Selected = true;
            dragging = true;
            canvas.Dragging = true;
            graph.dragNode = this;
            if (graph.selectedNode != null && graph.selectedNode != this) graph.selectedNode.Selected = false;
			graph.selectedNode = this;

            Ellipse el = sender as Ellipse;
            el.CaptureMouse();

            Point pos = e.GetPosition(null);
            offx = pos.X - (double)el.GetValue(Canvas.LeftProperty);
            offy = pos.Y - (double)el.GetValue(Canvas.TopProperty);
        }

        void ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            Ellipse el = sender as Ellipse;
            Point pos = e.GetPosition(null);

            if (dragging)
            {
				x = pos.X;
				y = pos.Y;
				
				el.SetValue(Canvas.TopProperty, y - offy);
                el.SetValue(Canvas.LeftProperty, x - offx);
            }
        }

        void ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse el = sender as Ellipse;
            dragging = false;
            graph.dragNode = null;

            canvas.Dragging = false;
            el.ReleaseMouseCapture();
        }
	}
}
