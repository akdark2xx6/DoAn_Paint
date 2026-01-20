using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GiaoDien
{
    internal class Rectangle_ : Tool_
    {
        // prev không clone ở ctor nữa để tránh giữ snapshot cũ lâu dài
        public Rectangle_(MainWindow owner) : base(owner) { }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = new Point(e.X - 70, e.Y - 27);
                firstPoint = new Point(e.X - 70, e.Y - 27);
                isDrawing = true;

                // Lấy snapshot mới ngay khi bắt đầu vẽ
                try { prev?.Dispose(); } catch { }
                prev = (Bitmap)owner.drawZone_data.Clone();
            }
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing == true)
            {
                lastPoint = new Point(e.X - 70, e.Y - 27);

                // Dùng snapshot mới để preview (clone để không chia sẻ instance)
                owner.drawZone_data = (Bitmap)prev.Clone();

                using (Graphics gPrev = Graphics.FromImage(owner.drawZone_data))
                {
                    gPrev.DrawRectangle(pen_, Math.Min(firstPoint.X, lastPoint.X), Math.Min(firstPoint.Y, lastPoint.Y),
                        Math.Abs(firstPoint.X - lastPoint.X), Math.Abs(firstPoint.Y - lastPoint.Y));
                }
                owner.Invalidate();
            }
        }

        public override void MouseUp(object sender, MouseEventArgs e)
        {
            // Vẽ kết quả lên prev (local snapshot)
            using (Graphics g = Graphics.FromImage(prev))
            {
                g.DrawRectangle(pen_, Math.Min(firstPoint.X, lastPoint.X), Math.Min(firstPoint.Y, lastPoint.Y),
                    Math.Abs(firstPoint.X - lastPoint.X), Math.Abs(firstPoint.Y - lastPoint.Y));
            }

            // Commit bằng một clone để owner và prev không chia sẻ cùng instance
            var old = owner.drawZone_data;
            owner.drawZone_data = (Bitmap)prev.Clone();

            try { old?.Dispose(); } catch { }

            // Cập nhật prev thành snapshot của trạng thái hiện tại (tách biệt)
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
