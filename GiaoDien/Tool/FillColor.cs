using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace GiaoDien.Tool
{
    internal class FillColor: Tool_
    {
        public FillColor(MainWindow owner) : base(owner) { prev = (Bitmap)owner.drawZone_data.Clone(); }
        public override void MouseDown(object sender, MouseEventArgs e)
        {

        }
        public override void MouseMove(object sender, MouseEventArgs e)
        {

        }
        

        public override void MouseUp(object sender, MouseEventArgs e)
        {
            FloodFill(owner.drawZone_data,new Point(e.X-70, e.Y-27) , owner.pen_data.Color);
        }
        public override void OnPaint_(PaintEventArgs e)
        {
        }
        private void FloodFill(Bitmap bmp, Point pt, Color newColor)
        {
            // Guard: ensure bitmap and point are valid
            if (bmp == null) return;
            if (pt.X < 0 || pt.X >= bmp.Width || pt.Y < 0 || pt.Y >= bmp.Height) return;

            Color targetColor;
            try
            {
                targetColor = bmp.GetPixel(pt.X, pt.Y);
            }
            catch
            {
                // If GetPixel still fails for any reason, abort safely
                return;
            }

            if (targetColor.ToArgb() == newColor.ToArgb())
                return;

            int w = bmp.Width;
            int h = bmp.Height;

            // Lock bitmap để truy cập byte trực tiếp
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int stride = data.Stride;
            IntPtr scan0 = data.Scan0;

            unsafe
            {
                byte* p = (byte*)scan0.ToPointer();

                // Lấy màu target thành 4 byte
                byte tr = targetColor.R;
                byte tg = targetColor.G;
                byte tb = targetColor.B;
                byte ta = targetColor.A;

                byte nr = newColor.R;
                byte ng = newColor.G;
                byte nb = newColor.B;
                byte na = newColor.A;

                Queue<Point> q = new Queue<Point>();
                q.Enqueue(pt);

                while (q.Count > 0)
                {
                    Point n = q.Dequeue();
                    int x = n.X;
                    int y = n.Y;

                    if (x < 0 || x >= w || y < 0 || y >= h)
                        continue;

                    byte* px = p + y * stride + x * 4;

                    // Kiểm tra màu hiện tại có phải targetColor không
                    if (px[0] == tb && px[1] == tg && px[2] == tr && px[3] == ta)
                    {
                        // Set màu mới
                        px[0] = nb;
                        px[1] = ng;
                        px[2] = nr;
                        px[3] = na;

                        // 8 hướng như code cũ của m
                        q.Enqueue(new Point(x + 1, y));
                        q.Enqueue(new Point(x - 1, y));
                        q.Enqueue(new Point(x, y + 1));
                        q.Enqueue(new Point(x, y - 1));
                        q.Enqueue(new Point(x - 1, y + 1));
                        q.Enqueue(new Point(x - 1, y - 1));
                        q.Enqueue(new Point(x + 1, y + 1));
                        q.Enqueue(new Point(x + 1, y - 1));
                    }
                }
            }

            bmp.UnlockBits(data);
            owner.Invalidate();
        }

    }
}
