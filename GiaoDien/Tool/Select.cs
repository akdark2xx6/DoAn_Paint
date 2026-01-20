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

        public bool HasSelection => selectionRect.Width > 0 && selectionRect.Height > 0;

        public bool IsPointInsideSelection(Point formPoint)
        {
            Point mouse = new Point(formPoint.X - OFFSET_X, formPoint.Y - OFFSET_Y);
            return HasSelection && selectionRect.Contains(mouse);
        }

        private void clone_current_bitmap_safe_dispose()
        {
            if (clone_currentBitmap != null)
            {
                try { clone_currentBitmap.Dispose(); } catch { }
                clone_currentBitmap = null;
            }
        }

        public override void CancelEdit()
        {
            try { isChoosingBitmap?.Dispose(); } catch { }
            isChoosingBitmap = null;
            clone_current_bitmap_safe_dispose();
            whiteRect = Rectangle.Empty;
            selectionRect = Rectangle.Empty;
            isSelecting = false;
            activeHandle = (int)ResizeHandle.None;
            owner.CutCopy_Unenable();
            owner.Invalidate();
        }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (owner.drawZone_data == null) return;

            Point mouse = new Point(e.X - OFFSET_X, e.Y - OFFSET_Y);

            int maxW = owner.rt_data.Width;
            int maxH = owner.rt_data.Height;
            mouse.X = Math.Max(0, Math.Min(mouse.X, maxW - 1));
            mouse.Y = Math.Max(0, Math.Min(mouse.Y, maxH - 1));

            activeHandle = GetActiveHandle(e.Location);

            if (selectionRect.Width > 0 && selectionRect.Height > 0 && !selectionRect.Contains(mouse) && activeHandle == (int)ResizeHandle.None)
            {
                try { isChoosingBitmap?.Dispose(); } catch { }
                isChoosingBitmap = null;
                clone_current_bitmap_safe_dispose();
                whiteRect = Rectangle.Empty;
                selectionRect = Rectangle.Empty;
                isSelecting = false;
                owner.Cursor = Cursors.Default;
                owner.CutCopy_Unenable();
                owner.Invalidate();
                return;
            }

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

            Point mouseForm = e.Location;
            Point mouse = new Point(mouseForm.X - OFFSET_X, mouseForm.Y - OFFSET_Y);
            int maxW = owner.rt_data.Width;
            int maxH = owner.rt_data.Height;

            UpdateHandles();
            SetCursor(mouseForm);

            if (e.Button == MouseButtons.Left)
            {
                if (isSelecting)
                {
                    // Clamp mouse vào trong drawZone trước khi tính toán vùng chọn
                    mouse.X = Math.Clamp(mouse.X, 0, maxW);
                    mouse.Y = Math.Clamp(mouse.Y, 0, maxH);

                    int x = Math.Min(firstPoint.X, mouse.X);
                    int y = Math.Min(firstPoint.Y, mouse.Y);
                    int w = Math.Abs(mouse.X - firstPoint.X);
                    int h = Math.Abs(mouse.Y - firstPoint.Y);

                    // đảm bảo không vượt biên (sau khi mouse đã bị clamp thì các công thức này an toàn)
                    x = Math.Max(0, x);
                    y = Math.Max(0, y);
                    w = Math.Min(w, Math.Max(0, maxW - x));
                    h = Math.Min(h, Math.Max(0, maxH - y));

                    selectionRect = new Rectangle(x, y, w, h);
                    owner.Invalidate();
                }
                else if (activeHandle != (int)ResizeHandle.None && isChoosingBitmap != null)
                {
                    int left = selectionRect.Left;
                    int top = selectionRect.Top;
                    int right = selectionRect.Right;
                    int bottom = selectionRect.Bottom;

                    if (activeHandle == (int)ResizeHandle.Move)
                    {
                        int dx = mouse.X - firstPoint.X;
                        int dy = mouse.Y - firstPoint.Y;

                        dx = Math.Max(-left, Math.Min(dx, maxW - right));
                        dy = Math.Max(-top, Math.Min(dy, maxH - bottom));

                        left += dx; right += dx;
                        top += dy; bottom += dy;
                        firstPoint = new Point(firstPoint.X + dx, firstPoint.Y + dy);
                    }
                    else
                    {
                        bool L = (activeHandle == 0 || activeHandle == 3 || activeHandle == 5);
                        bool R = (activeHandle == 2 || activeHandle == 4 || activeHandle == 7);
                        bool T = (activeHandle == 0 || activeHandle == 1 || activeHandle == 2);
                        bool B = (activeHandle == 5 || activeHandle == 6 || activeHandle == 7);

                        // Khi mouse vượt ra ngoài biên, chỉ clamp cạnh đang kéo vào biên.
                        if (L)
                        {
                            if (mouse.X < 0)
                            {
                                left = 0;
                            }
                            else
                            {
                                left = Math.Max(0, Math.Min(mouse.X, right - 1));
                            }
                        }
                        if (R)
                        {
                            if (mouse.X > maxW)
                            {
                                right = maxW;
                            }
                            else
                            {
                                right = Math.Max(left + 1, Math.Min(mouse.X, maxW));
                            }
                        }
                        if (T)
                        {
                            if (mouse.Y < 0)
                            {
                                top = 0;
                            }
                            else
                            {
                                top = Math.Max(0, Math.Min(mouse.Y, bottom - 1));
                            }
                        }
                        if (B)
                        {
                            if (mouse.Y > maxH)
                            {
                                bottom = maxH;
                            }
                            else
                            {
                                bottom = Math.Max(top + 1, Math.Min(mouse.Y, maxH));
                            }
                        }
                    }

                    // final clamp & apply
                    selectionRect = Rectangle.FromLTRB(
                        Math.Max(0, left), Math.Max(0, top),
                        Math.Min(maxW, right), Math.Min(maxH, bottom));

                    if (clone_currentBitmap != null)
                    {
                        Bitmap temp = (Bitmap)clone_currentBitmap.Clone();
                        using (Graphics g = Graphics.FromImage(temp))
                        {
                            if (whiteRect != Rectangle.Empty) g.FillRectangle(Brushes.White, whiteRect);
                            g.DrawImage(isChoosingBitmap, selectionRect);
                        }
                        var old = owner.drawZone_data;
                        owner.drawZone_data = temp;
                        try { old?.Dispose(); } catch { }
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
                    g.DrawImage(owner.drawZone_data, new Rectangle(0, 0, selectionRect.Width, selectionRect.Height), selectionRect, GraphicsUnit.Pixel);
                }

                clone_current_bitmap_safe_dispose();
                clone_currentBitmap = (Bitmap)owner.drawZone_data.Clone();

                owner.CutCopy_Enable();
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
                Rectangle rectScreen = new Rectangle(selectionRect.X + OFFSET_X, selectionRect.Y + OFFSET_Y, selectionRect.Width, selectionRect.Height);
                using (Pen pen = new Pen(Color.Black, 1.5f))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(pen, rectScreen);
                }

                UpdateHandles();
                foreach (Rectangle r in handles)
                {
                    g.FillRectangle(Brushes.DarkBlue, new Rectangle(r.X + OFFSET_X, r.Y + OFFSET_Y, r.Width, r.Height));
                }
            }
        }

        private void UpdateHandles()
        {
            int x = selectionRect.X, y = selectionRect.Y, w = selectionRect.Width, h = selectionRect.Height;
            int midX = x + w / 2, midY = y + h / 2;
            int s = HANDLE_SIZE, h_s = s / 2;

            handles[0] = new Rectangle(x - h_s, y - h_s, s, s);
            handles[1] = new Rectangle(midX - h_s, y - h_s, s, s);
            handles[2] = new Rectangle(x + w - h_s, y - h_s, s, s);
            handles[3] = new Rectangle(x - h_s, midY - h_s, s, s);
            handles[4] = new Rectangle(x + w - h_s, midY - h_s, s, s);
            handles[5] = new Rectangle(x - h_s, y + h - h_s, s, s);
            handles[6] = new Rectangle(midX - h_s, y + h - h_s, s, s);
            handles[7] = new Rectangle(x + w - h_s, y + h - h_s, s, s);
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
            int handle = GetActiveHandle(mouseForm);
            switch ((ResizeHandle)handle)
            {
                case ResizeHandle.TopLeft: case ResizeHandle.BottomRight: owner.Cursor = Cursors.SizeNWSE; break;
                case ResizeHandle.TopRight: case ResizeHandle.BottomLeft: owner.Cursor = Cursors.SizeNESW; break;
                case ResizeHandle.TopCenter: case ResizeHandle.BottomCenter: owner.Cursor = Cursors.SizeNS; break;
                case ResizeHandle.MiddleLeft: case ResizeHandle.MiddleRight: owner.Cursor = Cursors.SizeWE; break;
                case ResizeHandle.None:
                    Point mouse = new Point(mouseForm.X - OFFSET_X, mouseForm.Y - OFFSET_Y);
                    owner.Cursor = selectionRect.Contains(mouse) ? Cursors.SizeAll : Cursors.Default;
                    break;
            }
        }

        public override void Copy() { if (isChoosingBitmap != null) Clipboard.SetImage(isChoosingBitmap); }

        public override void Cut()
        {
            if (isChoosingBitmap != null)
            {
                Copy();
                using (Graphics g = Graphics.FromImage(owner.drawZone_data)) g.FillRectangle(Brushes.White, selectionRect);
                try { isChoosingBitmap.Dispose(); } catch { }
                isChoosingBitmap = null;
                selectionRect = Rectangle.Empty;
                clone_current_bitmap_safe_dispose();
                owner.CutCopy_Unenable();
                owner.Invalidate();
            }
        }

        public void PasteImage(Bitmap pastedImg)
        {
            try { isChoosingBitmap?.Dispose(); } catch { }
            isChoosingBitmap = new Bitmap(pastedImg);

            selectionRect = new Rectangle(0, 0, pastedImg.Width, pastedImg.Height);

            clone_current_bitmap_safe_dispose();
            clone_currentBitmap = (Bitmap)owner.drawZone_data.Clone();

            Bitmap temp = (Bitmap)clone_currentBitmap.Clone();
            using (Graphics g = Graphics.FromImage(temp))
            {
                if (whiteRect != Rectangle.Empty) g.FillRectangle(Brushes.White, whiteRect);
                g.DrawImage(isChoosingBitmap, selectionRect);
            }
            var old = owner.drawZone_data;
            owner.drawZone_data = temp;
            try { old?.Dispose(); } catch { }

            whiteRect = Rectangle.Empty;

            owner.CutCopy_Enable();

            owner.Invalidate();
        }

        public override void key_Keydown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) && isChoosingBitmap != null)
            {
                using (Graphics g = Graphics.FromImage(owner.drawZone_data)) g.FillRectangle(Brushes.White, selectionRect);
                try { isChoosingBitmap.Dispose(); } catch { }
                isChoosingBitmap = null;
                selectionRect = Rectangle.Empty;
                clone_current_bitmap_safe_dispose();
                owner.CutCopy_Unenable();
                owner.Invalidate();
            }
        }
    }
}