using System;
using System.Collections.Generic;
using System.Text;

namespace GiaoDien
{
    public class Eraser : Tool_
    {
        public Eraser(MainWindow owner) : base(owner) { pen_ = owner.eraser_data; }
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
                firstPoint = lastPoint;
                lastPoint = new Point(e.X-70, e.Y-27);
                using (Graphics g = Graphics.FromImage(owner.drawZone_data))
                {
                    g.DrawLine(pen_, firstPoint, lastPoint);
                }
                owner.Invalidate();
            }
        }
        public override void MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
            owner.Invalidate();
        }
        public override void OnPaint_(PaintEventArgs e)
        {

        }
    }//
}
