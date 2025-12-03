using System;
using System.Collections.Generic;
using System.Text;

namespace GiaoDien
{
    internal class Curve_ : Tool_
    {
        private bool isCurving = false;
        private Point preFirstPoint;
        private Point preLastPoint;
        public Curve_(MainWindow owner) : base(owner) { prev = (Bitmap)owner.drawZone_data.Clone(); }
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
                if (isCurving == false)
                {
                    lastPoint = new Point(e.X-70, e.Y-27);
                    owner.drawZone_data = (Bitmap)prev.Clone();
                    using (Graphics gPrev = Graphics.FromImage(owner.drawZone_data))
                    {
                        gPrev.DrawLine(pen_, firstPoint, lastPoint);
                    }
                    preFirstPoint = firstPoint;
                    preLastPoint = lastPoint;
                    owner.Invalidate();
                }
                else
                {
                    lastPoint = new Point(e.X-70, e.Y-27);
                    owner.drawZone_data = (Bitmap)prev.Clone();

                    using (Graphics gPrev = Graphics.FromImage(owner.drawZone_data))
                    {
                        gPrev.DrawCurve(pen_, preFirstPoint, lastPoint, preLastPoint);
                    }
                    owner.Invalidate();
                }
            }

        }
        public override void MouseUp(object sender, MouseEventArgs e)
        {
            if (isCurving == false)
            {
                using (Graphics g = Graphics.FromImage(owner.drawZone_data))
                    g.DrawCurve(pen_, firstPoint, lastPoint);

                owner.Invalidate();
                isDrawing = false;
                isCurving = true;
            }
            else
            {
                using (Graphics g = Graphics.FromImage(prev))
                    g.DrawCurve(pen_, preFirstPoint, lastPoint, preLastPoint);
                owner.drawZone_data = prev;
                owner.Invalidate();
                isDrawing = false;
                isCurving = false;
            }
        }
        public override void OnPaint_(PaintEventArgs e)
        {

        }
    }//
}
