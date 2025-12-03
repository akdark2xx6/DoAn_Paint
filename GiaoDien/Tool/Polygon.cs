using System;
using System.Collections.Generic;
using System.Text;

namespace GiaoDien
{
    internal class Polygon : Tool_
    {
        public Polygon(MainWindow owner) : base(owner) { prev = (Bitmap)owner.drawZone_data.Clone(); }
        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (firstPoint.IsEmpty && lastPoint.IsEmpty)
                {
                    lastPoint = new Point(e.X-70, e.Y-27);
                    firstPoint = new Point(e.X-70, e.Y-27);
                    isDrawing = true;
                }
                else
                {
                    firstPoint = lastPoint;
                    lastPoint = new Point(e.X-70, e.Y-27);
                    isDrawing = true;
                }
            }
        }
        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing == true)
            {
                lastPoint = new Point(e.X-70, e.Y-27);
                owner.drawZone_data = (Bitmap)prev.Clone();

                using (Graphics gPrev = Graphics.FromImage(owner.drawZone_data))
                {
                    gPrev.DrawLine(pen_, firstPoint, lastPoint);
                }
                owner.Invalidate();
            }

        }
        public override void MouseUp(object sender, MouseEventArgs e)
        {
            using (Graphics g = Graphics.FromImage(prev))
                g.DrawLine(pen_, firstPoint, lastPoint);
            owner.Invalidate();
            isDrawing = false;
        }
        public override void OnPaint_(PaintEventArgs e)
        {
        }
    }//
}
