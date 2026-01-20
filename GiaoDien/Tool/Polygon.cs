using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GiaoDien
{
    internal class Polygon : Tool_
    {
        // không clone prev trong ctor
        public Polygon(MainWindow owner) : base(owner) { }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (firstPoint.IsEmpty && lastPoint.IsEmpty)
                {
                    lastPoint = new Point(e.X - 70, e.Y - 27);
                    firstPoint = new Point(e.X - 70, e.Y - 27);
                    isDrawing = true;

                    // snapshot khi bắt đầu polygon (lần đầu)
                    try { prev?.Dispose(); } catch { }
                    prev = (Bitmap)owner.drawZone_data.Clone();
                }
                else
                {
                    firstPoint = lastPoint;
                    lastPoint = new Point(e.X - 70, e.Y - 27);
                    isDrawing = true;
                }
            }
        }
        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing == true)
            {
                lastPoint = new Point(e.X - 70, e.Y - 27);
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
            // Vẽ kết quả lên prev
            using (Graphics g = Graphics.FromImage(prev))
                g.DrawLine(pen_, firstPoint, lastPoint);

            // Commit bằng clone
            var old = owner.drawZone_data;
            owner.drawZone_data = (Bitmap)prev.Clone();
            try { old?.Dispose(); } catch { }

            // cập nhật prev thành snapshot mới
            try { prev.Dispose(); } catch { }
            prev = (Bitmap)owner.drawZone_data.Clone();

            owner.Invalidate();
            isDrawing = false;
        }
        public override void OnPaint_(PaintEventArgs e)
        {
        }
    }//
}
