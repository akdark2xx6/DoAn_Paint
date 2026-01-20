using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GiaoDien
{
    internal class RRectangle_ : Tool_
    {
        // không clone prev trong ctor
        public RRectangle_(MainWindow owner) : base(owner) { }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = new Point(e.X - 70, e.Y - 27);
                firstPoint = new Point(e.X - 70, e.Y - 27);
                isDrawing = true;

                // snapshot mới khi bắt đầu vẽ
                try { prev?.Dispose(); } catch { }
                prev = (Bitmap)owner.drawZone_data.Clone();
            }
        }
        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing == true)
            {
                lastPoint = new Point(e.X - 70, e.Y - 27);
                Rectangle rec = new Rectangle(Math.Min(firstPoint.X, lastPoint.X), Math.Min(firstPoint.Y, lastPoint.Y),
                                              Math.Abs(firstPoint.X - lastPoint.X), Math.Abs(firstPoint.Y - lastPoint.Y));

                // preview từ snapshot (clone để không chia sẻ instance)
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
            Rectangle rec = new Rectangle(Math.Min(firstPoint.X, lastPoint.X), Math.Min(firstPoint.Y, lastPoint.Y),
                                          Math.Abs(firstPoint.X - lastPoint.X), Math.Abs(firstPoint.Y - lastPoint.Y));

            // vẽ kết quả lên prev (snapshot)
            using (Graphics g = Graphics.FromImage(prev))
            {
                g.DrawRoundedRectangle(pen_, rec, new Size(Math.Max(rec.Width / 5, 1), Math.Max(1, rec.Height / 5)));
            }

            // commit bằng một clone để owner và prev không chia sẻ cùng instance
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
            // Không gán owner.drawZone_data ở đây để tránh giữ tham chiếu shared với prev.
            // MainWindow.OnPaint sẽ vẽ owner.drawZone_data.
        }
    }//

}
