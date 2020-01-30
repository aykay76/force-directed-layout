using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPF
{
	public class Edge
	{
		public Line line;
		public Node a;
		public Node b;
		public Graph g;
		public double stiffness = 0.2;
		public double naturalLength;

        public Edge(Page1 canvas, Node a, Node b)
        {
            this.a = a;
            this.b = b;
            naturalLength = a.mass * b.mass * 5.0;

            line = new Line();
            line.X1 = a.x;
            line.Y1 = a.y;
            line.X2 = b.x;
            line.Y2 = b.y;
            line.Stroke = new SolidColorBrush(Color.FromArgb(32, 0, 0, 128));
            line.SetValue(Canvas.ZIndexProperty, 1);
            canvas.parentCanvas.Children.Add(line);
        }

		public double[] getSpringForce()
		{
			double dx = b.x - a.x;
			double dy = b.y - a.y;
			double dz = b.z - a.z;
			double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
			double f = stiffness * (distance - naturalLength);

			return new double[] { f * dx / distance, f * dy / distance, f * dz / distance };
		}
	}
}
