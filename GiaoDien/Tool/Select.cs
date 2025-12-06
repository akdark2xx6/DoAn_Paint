using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GiaoDien
{
    internal class Select : Tool_
    {
        private Bitmap? isChoosingBitmap;
        private Bitmap? clone_currentBitmap;
        private Rectangle selectionRect;
        private bool isSelecting = false;
        private Rectangle whiteRect;

        private const int HANDLE_SIZE = 6;
        private int activeHandle = -1;
        private Rectangle[] handles = new Rectangle[8];

        // OFFSET của drawZone trên Form
        private const int OFFSET_X = 70;
        private const int OFFSET_Y = 27;

        private enum ResizeHandle
        {
            None = -1,
            TopLeft = 0,
            TopCenter = 1,
            TopRight = 2,
            MiddleLeft = 3,
            MiddleRight = 4,
            BottomLeft = 5,
            BottomCenter = 6,
            BottomRight = 7,
            Move = 8
        }

        public Select(MainWindow owner) : base(owner)
        {
            selectionRect = new Rectangle();
        }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (owner.drawZone_data == null) return;

            // Chuyển tọa độ chuột từ Form → Bitmap
            Point mouse = new Point(e.X - OFFSET_X, e.Y - OFFSET_Y);

            activeHandle = GetActiveHandle(e.Location);

            if (activeHandle != (int)ResizeHandle.None)
            {
                firstPoint = mouse;
                isSelecting = false;
                return;
            }

            if (selectionRect.Contains(mouse))
            {
                activeHandle = (int)ResizeHandle.Move;
                firstPoint = mouse;
                isSelecting = false;
                return;
            }

            firstPoint = mouse;
            isSelecting = true;
            selectionRect = new Rectangle(mouse.X, mouse.Y, 0, 0);
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (owner.drawZone_data == null) return;

            // Mouse form tọa độ thật
            Point mouseForm = e.Location;

            // Mouse trong tọa độ bitmap
            Point mouse = new Point(mouseForm.X - OFFSET_X, mouseForm.Y - OFFSET_Y);

            UpdateHandles();
            SetCursor(mouseForm);

            if (e.Button == MouseButtons.Left)
            {
                if (isSelecting)
                {
                    int x = Math.Min(firstPoint.X, mouse.X);
                    int y = Math.Min(firstPoint.Y, mouse.Y);
                    int w = Math.Abs(mouse.X - firstPoint.X);
                    int h = Math.Abs(mouse.Y - firstPoint.Y);
                    if (x + w > owner.rt_data.Width)
                    {
                        w = owner.rt_data.X - x;
                    }
                    if (y + h > owner.rt_data.Height)
                    {
                        h = owner.rt_data.Y - y;
                    }
                    if (w > 0 && h > 0)
                        selectionRect = new Rectangle(x, y, w, h);

                    owner.Invalidate();
                }
                else if (activeHandle != (int)ResizeHandle.None && isChoosingBitmap != null)
                {
                    int dx = mouse.X - firstPoint.X;
                    int dy = mouse.Y - firstPoint.Y;
                    Rectangle newRect = selectionRect;

                    owner.drawZone_data = (Bitmap)clone_currentBitmap.Clone();
                    using (Graphics g = Graphics.FromImage(owner.drawZone_data))
                    {
                        g.FillRectangle(Brushes.White, whiteRect);
                    }

                    if (activeHandle == (int)ResizeHandle.Move)
                    {
                        newRect.X += dx;
                        newRect.Y += dy;
                    }
                    else
                    {
                        switch ((ResizeHandle)activeHandle)
                        {
                            case ResizeHandle.TopLeft:
                                newRect.X += dx; newRect.Y += dy;
                                newRect.Width -= dx; newRect.Height -= dy;
                                break;
                            case ResizeHandle.TopCenter:
                                newRect.Y += dy; newRect.Height -= dy;
                                break;
                            case ResizeHandle.TopRight:
                                newRect.Y += dy; newRect.Height -= dy;
                                newRect.Width += dx;
                                break;
                            case ResizeHandle.MiddleLeft:
                                newRect.X += dx; newRect.Width -= dx;
                                break;
                            case ResizeHandle.MiddleRight:
                                newRect.Width += dx;
                                break;
                            case ResizeHandle.BottomLeft:
                                newRect.X += dx; newRect.Width -= dx;
                                newRect.Height += dy;
                                break;
                            case ResizeHandle.BottomCenter:
                                newRect.Height += dy;
                                break;
                            case ResizeHandle.BottomRight:
                                newRect.Width += dx; newRect.Height += dy;
                                break;
                        }
                    }

                    if (newRect.Width > 0 && newRect.Height > 0)
                        selectionRect = newRect;

                    firstPoint = mouse;

                    using (Graphics g = Graphics.FromImage(owner.drawZone_data))
                    {
                        g.DrawImage(isChoosingBitmap, selectionRect);
                    }

                    owner.Invalidate();
                }
            }
        }

        public override void MouseUp(object sender, MouseEventArgs e)
        {
            if (isSelecting && selectionRect.Width > 0 && selectionRect.Height > 0)
            {
                whiteRect = selectionRect;

                isChoosingBitmap = new Bitmap(selectionRect.Width, selectionRect.Height);
                using (Graphics g = Graphics.FromImage(isChoosingBitmap))
                {
                    Rectangle src = new Rectangle(selectionRect.X, selectionRect.Y, selectionRect.Width, selectionRect.Height);
                    Rectangle dest = new Rectangle(0, 0, selectionRect.Width, selectionRect.Height);
                    g.DrawImage(owner.drawZone_data, dest, src, GraphicsUnit.Pixel);
                }

                clone_currentBitmap = (Bitmap)owner.drawZone_data.Clone();
            }

            isSelecting = false;
            activeHandle = (int)ResizeHandle.None;
            owner.Cursor = Cursors.Default;
            owner.Invalidate();
        }

        public override void OnPaint_(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (owner.drawZone_data != null)
                g.DrawImage(owner.drawZone_data, OFFSET_X, OFFSET_Y);

            if (selectionRect.Width > 0 && selectionRect.Height > 0)
            {
                Rectangle rectScreen = new Rectangle(
                    selectionRect.X + OFFSET_X,
                    selectionRect.Y + OFFSET_Y,
                    selectionRect.Width,
                    selectionRect.Height
                );

                using (Pen pen = new Pen(Color.Black, 2))
                {
                    pen.DashStyle = DashStyle.Dot;
                    g.DrawRectangle(pen, rectScreen);
                }

                UpdateHandles();
                using (SolidBrush brush = new SolidBrush(Color.DarkBlue))
                {
                    foreach (Rectangle r in handles)
                    {
                        Rectangle rr = new Rectangle(
                            r.X + OFFSET_X,
                            r.Y + OFFSET_Y,
                            r.Width,
                            r.Height
                        );
                        g.FillRectangle(brush, rr);
                    }
                }
            }
        }

        private void UpdateHandles()
        {
            int half = HANDLE_SIZE / 2;
            handles[0] = new Rectangle(selectionRect.X - half, selectionRect.Y - half, HANDLE_SIZE, HANDLE_SIZE);
            handles[1] = new Rectangle(selectionRect.X + selectionRect.Width / 2 - half, selectionRect.Y - half, HANDLE_SIZE, HANDLE_SIZE);
            handles[2] = new Rectangle(selectionRect.Right - half, selectionRect.Y - half, HANDLE_SIZE, HANDLE_SIZE);
            handles[3] = new Rectangle(selectionRect.X - half, selectionRect.Y + selectionRect.Height / 2 - half, HANDLE_SIZE, HANDLE_SIZE);
            handles[4] = new Rectangle(selectionRect.Right - half, selectionRect.Y + selectionRect.Height / 2 - half, HANDLE_SIZE, HANDLE_SIZE);
            handles[5] = new Rectangle(selectionRect.X - half, selectionRect.Bottom - half, HANDLE_SIZE, HANDLE_SIZE);
            handles[6] = new Rectangle(selectionRect.X + selectionRect.Width / 2 - half, selectionRect.Bottom - half, HANDLE_SIZE, HANDLE_SIZE);
            handles[7] = new Rectangle(selectionRect.Right - half, selectionRect.Bottom - half, HANDLE_SIZE, HANDLE_SIZE);
        }

        private int GetActiveHandle(Point mouseForm)
        {
            Point mouse = new Point(mouseForm.X - OFFSET_X, mouseForm.Y - OFFSET_Y);

            for (int i = 0; i < handles.Length; i++)
                if (handles[i].Contains(mouse)) return i;

            return (int)ResizeHandle.None;
        }

        private void SetCursor(Point mouseForm)
        {
            Point mouse = new Point(mouseForm.X - OFFSET_X, mouseForm.Y - OFFSET_Y);

            int handle = GetActiveHandle(mouseForm);

            switch ((ResizeHandle)handle)
            {
                case ResizeHandle.TopLeft:
                case ResizeHandle.BottomRight:
                    owner.Cursor = Cursors.SizeNWSE; break;

                case ResizeHandle.TopRight:
                case ResizeHandle.BottomLeft:
                    owner.Cursor = Cursors.SizeNESW; break;

                case ResizeHandle.TopCenter:
                case ResizeHandle.BottomCenter:
                    owner.Cursor = Cursors.SizeNS; break;

                case ResizeHandle.MiddleLeft:
                case ResizeHandle.MiddleRight:
                    owner.Cursor = Cursors.SizeWE; break;

                case ResizeHandle.None:
                    owner.Cursor = selectionRect.Contains(mouse) ? Cursors.SizeAll : Cursors.Default;
                    break;
            }
        }
        public override void Copy()
        {
            // isChoosingBitmap là biến bạn đã tạo để lưu vùng đang chọn
            if (isChoosingBitmap != null)
            {
                Clipboard.SetImage(isChoosingBitmap);
            }
        }

        public override void Cut()
        {
            if (isChoosingBitmap != null)
            {
                // 1. Copy trước
                Clipboard.SetImage(isChoosingBitmap);

                // 2. Xóa vùng đang chọn (Tô màu trắng lên drawZone gốc)
                // Lưu ý: selectionRect là biến private, bạn cần đổi nó thành protected hoặc dùng biến nội bộ
                // Ở đây mình giả định bạn đã có selectionRect
                using (Graphics g = Graphics.FromImage(owner.drawZone_data))
                {
                    g.FillRectangle(Brushes.White, selectionRect);
                }

                // 3. Xóa vùng chọn ảo đi
                isChoosingBitmap = null;
                selectionRect = Rectangle.Empty;
                owner.Invalidate();
            }
        }
        public void PasteImage(Bitmap pastedImg)
        {
            // 1. Gán ảnh từ Clipboard vào biến ảnh đang chọn
            isChoosingBitmap = pastedImg;

            // 2. Thiết lập vùng chọn nằm ở góc trái trên (0,0) với kích thước bằng ảnh
            selectionRect = new Rectangle(0, 0, pastedImg.Width, pastedImg.Height);

            // 3. Chụp lại màn hình hiện tại làm nền (Background)
            // Lưu ý: Lúc này chưa cắt gì cả nên nền vẫn nguyên vẹn
            clone_currentBitmap = (Bitmap)owner.drawZone_data.Clone();

            // 4. Đặt vùng trắng (vùng cắt) là Rỗng
            // (Vì đây là ảnh dán vào, không phải cắt từ nền ra nên không để lại lỗ trắng)
            whiteRect = Rectangle.Empty;

            // 5. Vẽ lại để hiển thị ảnh và khung chọn
            owner.Invalidate();
        }
    }
}
