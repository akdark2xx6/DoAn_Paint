using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GiaoDien
{
    internal class Curve_ : Tool_
    {
        private bool isCurving = false;
        private Point preFirstPoint;
        private Point preLastPoint;

        // không clone prev trong ctor
        public Curve_(MainWindow owner) : base(owner) { }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = new Point(e.X - 70, e.Y - 27);
                firstPoint = new Point(e.X - 70, e.Y - 27);
                isDrawing = true;

                // Chỉ tạo snapshot khi bắt đầu phase 1 (khởi tạo đường)
                // Nếu đang ở phase 2 (isCurving == true) thì không override prev,
                // để preview phase 2 được vẽ trên nền ban đầu (không chứa đường thẳng phase 1).
                if (!isCurving)
                {
                    try { prev?.Dispose(); } catch { }
                    prev = (Bitmap)owner.drawZone_data.Clone();
                }
            }
        }
        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            lastPoint = new Point(e.X - 70, e.Y - 27);

            // Luôn restore từ snapshot trước khi vẽ preview để không để lại artifact
            owner.drawZone_data = (Bitmap)prev.Clone();

            if (!isCurving)
            {
                // phase 1: preview đường thẳng
                using (Graphics gPrev = Graphics.FromImage(owner.drawZone_data))
                {
                    gPrev.DrawLine(pen_, firstPoint, lastPoint);
                }

                // lưu điểm tham chiếu cho phase 2 (khi mouse up)
                preFirstPoint = firstPoint;
                preLastPoint = lastPoint;
            }
            else
            {
                // phase 2: preview curve (vẽ trên nền ban đầu prev)
                using (Graphics gPrev = Graphics.FromImage(owner.drawZone_data))
                {
                    // dùng mảng điểm để DrawCurve
                    Point[] pts = new Point[] { preFirstPoint, lastPoint, preLastPoint };
                    gPrev.DrawCurve(pen_, pts);
                }
            }

            owner.Invalidate();
        }
        public override void MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            if (!isCurving)
            {
                // Kết thúc phase 1: lưu điểm tham chiếu và chuyển sang phase 2.
                // KHÔNG vẽ/commit đường thẳng lên prev hay owner.drawZone_data ở đây,
                // để khi chỉnh cong (phase 2) preview không còn đường thẳng ban đầu.
                preFirstPoint = firstPoint;
                preLastPoint = lastPoint;

                isDrawing = false;
                isCurving = true;

                owner.Invalidate();
            }
            else
            {
                // Phase 2: commit curve cuối cùng lên prev (snapshot) rồi gán lên owner.drawZone_data
                using (Graphics g = Graphics.FromImage(prev))
                {
                    Point[] pts = new Point[] { preFirstPoint, lastPoint, preLastPoint };
                    g.DrawCurve(pen_, pts);
                }

                var old = owner.drawZone_data;
                owner.drawZone_data = (Bitmap)prev.Clone();
                try { old?.Dispose(); } catch { }

                // cập nhật prev thành snapshot mới tách biệt
                try { prev.Dispose(); } catch { }
                prev = (Bitmap)owner.drawZone_data.Clone();

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
