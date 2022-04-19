using System.Drawing.Imaging;

namespace Project1._0
{
    public partial class Ve : Form
    {
        public Ve()
        {
            InitializeComponent();
            this.Width = 900;
            this.Height = 700;
            bm = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm;
        }

        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point Px, Py;
        Pen P = new Pen(Color.Black, 1);
        int index;
        Pen E = new Pen(Color.White, 10);
        int x, y, sX, sY, cX, cY;
        ColorDialog cd = new ColorDialog();
        Color new_color;

        private void btnLine_Click(object sender, EventArgs e)
        {
            index = 5;
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            index = 4;
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            index = 3;
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if(paint)
            {
                // Ellipse
                if (index == 3)
                {
                    g.DrawEllipse(P, cX, cY, sX, sY);
                }

                // Rectangle
                if (index == 4)
                {
                    g.DrawRectangle(P, cX, cY, sX, sY);
                }

                if (index == 5)
                {
                    g.DrawLine(P, cX, cY, x, y);
                }
            }    
        }

        private void btnPencil_Click(object sender, EventArgs e)
        {
            index = 1;
        }

        private void colorPicker_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = set_point(colorPicker, e.Location);
            pickColor.BackColor = ((Bitmap)colorPicker.Image).GetPixel(point.X, point.Y);
            new_color = pickColor.BackColor;
            P.Color = pickColor.BackColor;
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            index = 7;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            new_color = cd.Color;
            pickColor.BackColor = cd.Color;
            P.Color = cd.Color;
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if(index == 7)
            {
                Point point = set_point(pic,e.Location);
                Fill(bm, point.X, point.Y, new_color);
            }    
        }

        private void btsave_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Image(*.jpg)|*.jpg|(*.*|*.*";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width, pic.Height), bm.PixelFormat);
                btm.Save(sfd.FileName,ImageFormat.Jpeg);
            }    
        }

        private void btclear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pic.Image = bm;

        }

        // Pic
        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            Py= e.Location;
            cX = e.X;
            cY = e.Y;
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if(paint)
            {
                // Pen
                if(index == 1)
                {
                    Px = e.Location;
                    g.DrawLine(P, Px, Py);
                    Py = Px;
                }
                // Erase
                if (index == 2)
                {
                    Px = e.Location;
                    g.DrawLine(E, Px, Py);
                    Py = Px;
                }
            }
            pic.Refresh();

            x = e.X;
            y = e.Y;
            sX = e.X - cX;
            sY = e.Y - cY;
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;

            sX = x - cX;
            sY = y - cY;

            // Ellipse
            if(index == 3)
            {
                g.DrawEllipse(P,cX,cY,sX,sY);
            }

            // Rectangle
            if (index == 4)
            {
                g.DrawRectangle(P, cX, cY, sX, sY);
            }

            if (index == 5)
            {
                g.DrawLine(P, cX, cY, x, y);
            }
        }

        static Point set_point(PictureBox pb, Point pt)
        {
            float Px = 1f * pb.Image.Width / pb.Width;
            float Py = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X*Px),(int)(pt.Y*Py));
        }

        private void validate(Bitmap bm, Stack<Point> sp, int x, int y, Color o_color, Color n_color)
        {
            Color cx = bm.GetPixel(x, y);
            if(cx == o_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, n_color);
            }    
        }

        public void Fill(Bitmap bm, int x, int y, Color n_color)
        {
            Color o_color = bm.GetPixel(x, y);
            Stack<Point> picel = new Stack<Point>();
            picel.Push(new Point(x, y));
            bm.SetPixel(x, y, n_color);
            if (o_color == n_color) return;

            while(picel.Count > 0)
            {
                Point pt = picel.Pop();
                if(pt.X>0 && pt.Y>0 && pt.X<bm.Width-1 && pt.Y<bm.Height-1)
                    {
                    validate(bm,picel,pt.X-1,pt.Y,o_color,n_color);
                    validate(bm, picel, pt.X, pt.Y-1, o_color, n_color);
                    validate(bm, picel, pt.X+1, pt.Y, o_color, n_color);
                    validate(bm, picel, pt.X, pt.Y+1, o_color, n_color);
                }
            }    
        }
    }
}