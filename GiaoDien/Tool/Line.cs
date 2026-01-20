using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GiaoDien
{
    internal class Line_ : Tool_
    {
        // không clone prev trong ctor
        public Line_(MainWindow owner) : base(owner) { }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = new Point(e.X - 70, e.Y - 27);
                firstPoint = new Point(e.X - 70, e.Y - 27);
                isDrawing = true;

                // Lấy snapshot khi bắt đầu vẽ
                try { prev?.Dispose(); } catch { }
                prev = (Bitmap)owner.drawZone_data.Clone();
            }
        }
        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing == true)
            {
                lastPoint = new Point(e.X - 70, e.Y - 27);

                // preview từ snapshot
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
            // vẽ kết quả lên prev
            using (Graphics g = Graphics.FromImage(prev))
                g.DrawLine(pen_, firstPoint, lastPoint);

            // commit bằng clone (tách rời owner.drawZone_data và prev)
            var old = owner.drawZone_data;
            owner.drawZone_data = (Bitmap)prev.Clone();
            try { old?.Dispose(); } catch { }

            // cập nhật prev thành snapshot mới tách biệt
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
