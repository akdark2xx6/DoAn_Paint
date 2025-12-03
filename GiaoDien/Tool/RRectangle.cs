using System;
using System.Collections.Generic;
using System.Text;

namespace GiaoDien
{
    internal class RRectangle_ : Tool_
    {
        public RRectangle_(MainWindow owner) : base(owner) { prev = (Bitmap)owner.drawZone_data.Clone(); }
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
                    g.DrawRoundedRectangle(pen_, rec, new Size(Math.Max(rec.Width / 5, 1), Math.Max(1, rec.Height / 5)));
                }
                owner.Invalidate();
            }

        }
        public override void MouseUp(object sender, MouseEventArgs e)
        {
            Rectangle rec = new Rectangle(Math.Min(firstPoint.X, lastPoint.X), Math.Min(firstPoint.Y, lastPoint.Y), Math.Abs(firstPoint.X - lastPoint.X), Math.Abs(firstPoint.Y - lastPoint.Y));
            using (Graphics g = Graphics.FromImage(prev))
            {
                g.DrawRoundedRectangle(pen_, rec, new Size(Math.Max(rec.Width / 5, 1), Math.Max(1, rec.Height / 5)));
            }
            owner.drawZone_data = prev;
            owner.Invalidate();
            isDrawing = false;
        }
        public override void OnPaint_(PaintEventArgs e)
        {
            if (isDrawing == true)
                e.Graphics.DrawImage(prev, 70, 27);
            else
            {
                owner.drawZone_data = prev;
                e.Graphics.DrawImage(owner.drawZone_data, 70, 27);
            }
        }
    }//

}
