using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OverlayPrototype
{
    public partial class Form1 : Form
    {
        int MinScreenX => Screen.AllScreens.Select(s => s.Bounds.X).Min();
        int MinScreenY => Screen.AllScreens.Select(s => s.Bounds.Y).Min();

        int MaxScreenX => Screen.AllScreens.Select(s => s.Bounds.X + s.Bounds.Width).Max();
        int MaxScreenY => Screen.AllScreens.Select(s => s.Bounds.Y + s.Bounds.Height).Max();

        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int WS_EX_TRANSPARENT = 0x00000020;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT;
                return cp;
            }
        }

        public Form1()
        {
            InitializeComponent();
            
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TransparencyKey = Color.Black;
            
            var p = new Point(MinScreenX, MinScreenY);
            var w = MaxScreenX - MinScreenX;
            var h = MaxScreenY - MinScreenY;
            Console.WriteLine($"p={p}, w={w}, h={h}");
            this.Location = p;
            this.Width = w;
            this.Height = h;

            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                DrawGestureStrokes();
            };
            timer.AutoReset = true;
            timer.Start();
        }

        public void DrawGestureStrokes()
        {
            var g = this.CreateGraphics();

            var w = MaxScreenX - MinScreenX;
            var h = MaxScreenY - MinScreenY;
            var rand = new Random();

            var strokes = Enumerable.Range(1, rand.Next(1, 5)).Select(
                s => Enumerable.Range(1, rand.Next(5, 10)).Select(
                    p => new Point(rand.Next(0, w), rand.Next(0, h))));

            using (var buffer = BufferedGraphicsManager.Current.Allocate(g, this.DisplayRectangle))
            {
                foreach (var storke in strokes)
                {
                    Console.WriteLine($"storke: {storke.Count()}");

                    var color = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
                    using (var pen = new Pen(color))
                    {
                        pen.Width = 4.0f;
                        buffer.Graphics.DrawLines(pen, storke.ToArray());
                    }
                }
                buffer.Render(g);
            }
        }
    }
}
