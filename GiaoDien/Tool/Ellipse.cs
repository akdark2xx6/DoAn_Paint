using System;
using System.Collections.Generic;
using System.Text;

namespace GiaoDien
{
    internal class Ellipse_ : Tool_
    {
        public Ellipse_(MainWindow owner) : base(owner) { prev = (Bitmap)owner.drawZone_data.Clone(); }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = new Point(e.X-70, e.Y-27);
                firstPoint = new Point(e.X-70, e.Y-27);
                isDrawing = true;
            }
        }
        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing == true)
            {
                lastPoint = new Point(e.X-70, e.Y-27);
                Rectangle rec = new Rectangle(Math.Min(firstPoint.X, lastPoint.X), Math.Min(firstPoint.Y, lastPoint.Y), Math.Abs(firstPoint.X - lastPoint.X), Math.Abs(firstPoint.Y - lastPoint.Y));
                owner.drawZone_data = (Bitmap)prev.Clone();
                using (Graphics g = Graphics.FromImage(owner.drawZone_data))
                {
                    g.DrawEllipse(pen_, rec);
                }
                owner.Invalidate();
            }

        }
        public override void MouseUp(object sender, MouseEventArgs e)
        {
            Rectangle rec = new Rectangle(Math.Min(firstPoint.X, lastPoint.X), Math.Min(firstPoint.Y, lastPoint.Y), Math.Abs(firstPoint.X - lastPoint.X), Math.Abs(firstPoint.Y - lastPoint.Y));
            using (Graphics g = Graphics.FromImage(prev))
            {
                g.DrawEllipse(pen_, rec);
            }
            owner.drawZone_data = prev;
            owner.Invalidate();
            isDrawing = false;
        }
        public override void OnPaint_(PaintEventArgs e)
        {
 
        }
    }
}
