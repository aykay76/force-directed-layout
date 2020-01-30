using System;
using System.Collections.Generic;
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
	public class Graph
	{
        public double gravConstant = -10.0;
		public Page1 canvas;
		public List<Node> nodes = new List<Node>();
		public List<Edge> edges = new List<Edge>();
        public Node dragNode = null;
		public Node selectedNode = null;

		public Graph(Page1 canvas)
		{
			this.canvas = canvas;
		}

		public void createSibling()
		{
			if (selectedNode == null)
			{
				Node n = new Node(this, 4.0, canvas.Width / 2.0, canvas.Height / 2.0);
				n.parent = null;
				n.createUI(canvas);
				nodes.Add(n);
				selectedNode = n;
			}
			else
			{
				Node n = new Node(this, selectedNode.mass / 2.0, canvas.Width / 2.0, canvas.Height / 2.0);
				n.parent = selectedNode.parent;
				n.createUI(canvas);
				nodes.Add(n);
				selectedNode = n;

                if (n.parent != null)
                {
                    Edge e = new Edge(canvas, n.parent, n);
                    e.g = this;
                    edges.Add(e);
                }
			}
		}

		public void createChild()
		{
			if (selectedNode == null)
			{
				Node n = new Node(this, 4.0, canvas.Width / 2, canvas.Height / 2);
				n.parent = null;
				n.createUI(canvas);
				nodes.Add(n);
//				selectedNode = n;
			}
			else
			{
				Node n = new Node(this, selectedNode.mass>1.0?selectedNode.mass - 1.0:1.0, canvas.Width / 2, canvas.Height / 2);
				n.parent = selectedNode;
				n.createUI(canvas);
				nodes.Add(n);

				Edge e = new Edge(canvas, n.parent, n);
				e.g = this;
				edges.Add(e);

//				selectedNode = n;
			}
		}

		#region Test Creation Functions
		public void createRings()
        {
            Random r = new Random();
            int n = 0;
            for (int i = 0; i < 8; i++, n++)
            {
				Node node = new Node(this, 1.0 + r.Next(3), 350 + r.Next(100), 250 + r.Next(100));
                node.createUI(canvas);
                nodes.Add(node);
            }

            int e = 0;
            for (int i = 0; i < 8; i++, e++)
            {
                Edge edge = new Edge(canvas, nodes[i], nodes[(i + 1) % 8]);
                edge.g = this;
                edges.Add(edge);
            }

            for (int i = 0; i < 8; i++)
            {
                Node[] subnodes = new Node[5];
                for (int j = 0; j < 5; j++)
                {
					Node node = new Node(this, 1.0, 350 + r.Next(100), 250 + r.Next(100));
                    node.createUI(canvas);
                    nodes.Add(node);
                    subnodes[j] = node;

                    Edge edge = new Edge(canvas, nodes[i], node);
                    edge.g = this;
                    edges.Add(edge);
                }

                //for (int j = 0; j < 4; j++)
                //{
                //    Edge edge = new Edge(canvas, subnodes[j], subnodes[j + 1]);
                //    edge.g = this;
                //    edge.naturalLength = 1.0;
                //    edges.Add(edge);
                //}
            }
        }
        public void createRandom()
		{
			int nNodes = 30;
			int nEdges = 50;
			double W = canvas.Width;
			double H = canvas.Height;

			Random r = new Random();

			for (int i = 0; i < nNodes; i++)
			{
				Node n = new Node(this, 
									1.0 + i / 10, 
									W * 7 / 16 + r.Next((int)W / 8), //W / 4 + r.Next((int)(W / 2)), 
									H * 7 / 16 + r.Next((int)H / 8) //H / 4 + r.Next((int)(H / 2))
									);
				n.createUI(canvas);
				nodes.Add(n);
			}

			for (int i = 0; i < nEdges; i++)
			{
				Node a = nodes[r.Next(nodes.Count)];
				Node b = nodes[r.Next(nodes.Count)];

                if (a == b)
                {
                    i--;
                }
                else
                {
                    bool alreadyExists = false;
                    for (int j = 0; j < i; j++)
                    {
                        Edge edge = edges[j];
                        if ((edge.a == a && edge.b == b) || (edge.a == b && edge.b == a))
                        {
                            i--;
                            alreadyExists = true;
                        }
                    }

                    if (!alreadyExists)
                    {
                        Edge e = new Edge(canvas, a, b);
                        e.g = this;
                        edges.Add(e);
                    }
                }
			}
		}
		#endregion

		#region Force Directed Layout
		public void resetForces()
		{
			foreach (Node n in nodes)
			{
				n.f[0] = 0.0;
				n.f[1] = 0.0;
				n.f[2] = 0.0;
			}
		}
		public void calculateSpringForces()
		{
		    foreach (Edge e in edges)
		    {
			    double[] f = e.getSpringForce();
			    e.a.f[0] += f[0];
			    e.a.f[1] += f[1];
			    e.a.f[2] += f[2];

                e.b.f[0] -= f[0];
			    e.b.f[1] -= f[1];
			    e.b.f[2] -= f[2];
		    }
		}
		public void calculateGravForces()
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				Node a = nodes[i];

				for (int j = i; j < nodes.Count; j++)
				{
					Node b = nodes[j];

					if (b != a)
					{
						double dx = b.x - a.x;
						double dy = b.y - a.y;
						double dz = b.z - a.z;
						double rSquared = dx * dx + dy * dy + dz * dz;

						// F = G*m1*m2/r^2  
						if (rSquared != 0)
						{ 
							double f = gravConstant * (a.mass * b.mass / rSquared);

                            a.f[0] += dx * f;
                            a.f[1] += dy * f;
                            a.f[2] += dz * f;

                            b.f[0] -= dx * f;
							b.f[1] -= dy * f;
							b.f[2] -= dz * f;
						}
					}
				}
			}
		}
		public void moveNodes()
		{
			// move nodes according to forces
			foreach (Node n in nodes)
			{
                if (dragNode != null && dragNode == n)
                {
                    continue;
                }
                else
                {
					if (n.mass < 4.0)
					{
						n.x += n.f[0];
						n.y += n.f[1];
						n.z += n.f[2];
					}
                }

				n.ellipse.SetValue(Canvas.LeftProperty, n.x - n.ellipse.Width / 2);
				n.ellipse.SetValue(Canvas.TopProperty, n.y - n.ellipse.Height / 2);
			}

			foreach (Edge e in edges)
			{
				e.line.X1 = e.a.x;
				e.line.Y1 = e.a.y;
				e.line.X2 = e.b.x;
				e.line.Y2 = e.b.y;
			}
		}
		public bool areConnected(Node a, Node b)
		{
			foreach (Edge e in edges)
			{
				if (e.a.GetHashCode() == a.GetHashCode() && e.b.GetHashCode() == b.GetHashCode() || 
					e.a.GetHashCode() == b.GetHashCode() && e.b.GetHashCode() == a.GetHashCode())
					return true;
			}

			return false;
		}
		#endregion
	}
}
