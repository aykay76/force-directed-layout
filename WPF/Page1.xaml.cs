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
using System.Threading;

namespace WPF
{
    public partial class Page1 : Window
	{
        Timer timer = null;
        int frame = 0;
		Graph g;
        private bool dragging = false;
        public bool Dragging
        {
            get { return dragging; }
            set
            {
                dragging = value;
            }
        }

        public void timer_tick(object state)
        {
            Console.WriteLine("timer tick " + frame++);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    g.resetForces();
                    g.calculateSpringForces();
                    g.calculateGravForces();
                    g.moveNodes();
                }
            }));
        }

		public void Page_Loaded(object o, EventArgs e)
		{
			// Required to initialize variables
			InitializeComponent();

			g = new Graph(this);
			g.createRings();
//            g.createRandom();

            timer = new Timer(new TimerCallback(timer_tick), null, 0, 40);

			createSibling.MouseLeftButtonUp += createSibling_MouseLeftButtonUp;
			createChild.MouseLeftButtonUp += createChild_MouseLeftButtonUp;

            //WebApplication.Current.RegisterScriptableObject("basic", this);
        }

		void createSibling_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			g.createSibling();
		}

		void createChild_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			g.createChild();
		}
    
        public void OnMouseWheel(int x, int y, int delta)
        {
            ScaleTransform st = RenderTransform as ScaleTransform;
            if (st == null)
            {
                st = new ScaleTransform();
                RenderTransform = st;
            }

            st.CenterX = x;
            st.CenterY = y;

            if (delta > 0)
            {
                st.ScaleX += 0.1;
                st.ScaleY += 0.1;
            }
            else
            {
                st.ScaleX -= 0.1;
                st.ScaleY -= 0.1;
            }
        }
    }
}
