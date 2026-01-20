using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace GiaoDien.Tool
{
    public class Text_ : Tool_
    {
        private MainWindow main;
        private TextBox txtInput;
        private Point startPoint;
        private Rectangle rectPreview;
        private bool isDragging = false;

        // toolbar controls (shown while editing)
        private Panel toolbar;
        private ComboBox cbFontFamily;
        private ComboBox cbFontSize;
        private Button btnBold;
        private Button btnItalic;
        private Button btnUnderline;

        // current style state
        private FontStyle currentStyle = FontStyle.Regular;
        private string currentFontFamily = "Arial";
        private float currentFontSize = 12f;

        // When user closes the textbox by clicking outside, suppress creating a new box
        // for the remainder of that mouse cycle (MouseDown/MouseMove/MouseUp).
        private bool suppressCreateUntilMouseUp = false;

        public Text_(MainWindow main) : base(main)
        {
            this.main = main;
            txtInput = main.Controls.Find("txtInput", true).FirstOrDefault() as TextBox;

            if (txtInput == null)
            {
                txtInput = new TextBox();
                txtInput.Name = "txtInput";
                txtInput.Multiline = true;
                txtInput.BorderStyle = BorderStyle.None;
                txtInput.Visible = false;
                txtInput.BackColor = Color.White;

                // xử lý phím trong TextBox overlay
                txtInput.KeyDown += TxtInput_KeyDown;

                // cập nhật khung nét đứt khi nội dung thay đổi
                txtInput.TextChanged += (s, e) => main.Invalidate();

                main.Controls.Add(txtInput);
                txtInput.BringToFront();

                // đảm bảo: khi TextBox ẩn/hiện thì toolbar đồng bộ (ẩn toolbar khi textbox ẩn)
                txtInput.VisibleChanged += (s, e) =>
                {
                    if (!txtInput.Visible)
                        HideToolbar();
                };
            }
            else
            {
                // nếu TextBox đã tồn tại từ trước, vẫn cần bắt VisibleChanged để đồng bộ toolbar
                txtInput.VisibleChanged -= TxtInput_VisibleChanged_Internal;
                txtInput.VisibleChanged += TxtInput_VisibleChanged_Internal;
            }

            CreateToolbar();
        }

        // separate handler so we can unsubscribe if needed
        private void TxtInput_VisibleChanged_Internal(object? sender, EventArgs e)
        {
            if (!txtInput.Visible)
                HideToolbar();
        }

        private void CreateToolbar()
        {
            // tăng chiều rộng toolbar để đủ chỗ hiển thị tất cả control
            toolbar = new Panel
            {
                Size = new Size(360, 28),
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle
            };

            // font family
            cbFontFamily = new ComboBox { Left = 4, Top = 3, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            try
            {
                InstalledFontCollection ifc = new InstalledFontCollection();
                foreach (var f in ifc.Families.Take(40)) cbFontFamily.Items.Add(f.Name);
            }
            catch
            {
                cbFontFamily.Items.Add("Arial");
            }
            if (!cbFontFamily.Items.Contains(currentFontFamily)) cbFontFamily.Items.Insert(0, currentFontFamily);
            cbFontFamily.SelectedItem = currentFontFamily;
            cbFontFamily.SelectedIndexChanged += (s, e) =>
            {
                if (cbFontFamily.SelectedItem != null)
                {
                    currentFontFamily = cbFontFamily.SelectedItem.ToString();
                    ApplyFontToTextBox();
                }
            };

            // font size
            cbFontSize = new ComboBox { Left = cbFontFamily.Right + 6, Top = 3, Width = 60, DropDownStyle = ComboBoxStyle.DropDownList };
            float[] sizes = { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32 };
            foreach (var s in sizes) cbFontSize.Items.Add(s.ToString());
            cbFontSize.SelectedItem = currentFontSize.ToString();
            cbFontSize.SelectedIndexChanged += (s, e) =>
            {
                if (float.TryParse(cbFontSize.SelectedItem?.ToString(), out var fs))
                {
                    currentFontSize = fs;
                    ApplyFontToTextBox();
                }
            };

            // style buttons: Bold, Italic, Underline (underline đặt sau Italic)
            int btnLeft = cbFontSize.Right + 6;
            btnBold = new Button { Text = "B", Left = btnLeft, Top = 2, Width = 28, Height = 22, Font = new Font("Arial", 9, FontStyle.Bold) };
            btnLeft = btnBold.Right + 4;
            btnItalic = new Button { Text = "I", Left = btnLeft, Top = 2, Width = 28, Height = 22, Font = new Font("Arial", 9, FontStyle.Italic) };
            btnLeft = btnItalic.Right + 4;
            btnUnderline = new Button { Text = "U", Left = btnLeft, Top = 2, Width = 28, Height = 22, Font = new Font("Arial", 9, FontStyle.Underline) };

            btnBold.Click += (s, e) => { ToggleStyle(FontStyle.Bold); };
            btnItalic.Click += (s, e) => { ToggleStyle(FontStyle.Italic); };
            btnUnderline.Click += (s, e) => { ToggleStyle(FontStyle.Underline); };

            toolbar.Controls.Add(cbFontFamily);
            toolbar.Controls.Add(cbFontSize);
            toolbar.Controls.Add(btnBold);
            toolbar.Controls.Add(btnItalic);
            toolbar.Controls.Add(btnUnderline);

            // Add toolbar to form but keep hidden until editing
            main.Controls.Add(toolbar);
            toolbar.BringToFront();
        }

        private void ToggleStyle(FontStyle flag)
        {
            if ((currentStyle & flag) == flag) currentStyle &= ~flag;
            else currentStyle |= flag;
            ApplyFontToTextBox();
            UpdateStyleButtonStates();
        }

        private void UpdateStyleButtonStates()
        {
            btnBold.BackColor = (currentStyle & FontStyle.Bold) == FontStyle.Bold ? SystemColors.ActiveCaption : SystemColors.Control;
            btnItalic.BackColor = (currentStyle & FontStyle.Italic) == FontStyle.Italic ? SystemColors.ActiveCaption : SystemColors.Control;
            btnUnderline.BackColor = (currentStyle & FontStyle.Underline) == FontStyle.Underline ? SystemColors.ActiveCaption : SystemColors.Control;
        }

        private void ApplyFontToTextBox()
        {
            try
            {
                txtInput.Font = new Font(currentFontFamily, currentFontSize, currentStyle);
            }
            catch
            {
                // fallback
                txtInput.Font = new Font("Arial", currentFontSize, currentStyle);
            }
            // Ensure textbox height at least one line
            txtInput.Height = Math.Max(txtInput.Height, txtInput.Font.Height + 4);
            PositionToolbar(); // recalc toolbar so it does not overlap textbox
            main.Invalidate();
        }

        private void ShowToolbar()
        {
            toolbar.Visible = true;
            UpdateStyleButtonStates();
            PositionToolbar();
        }

        private void HideToolbar()
        {
            if (toolbar != null && toolbar.Visible)
            {
                toolbar.Visible = false;
                main.Invalidate();
            }
        }

        private void PositionToolbar()
        {
            if (toolbar == null || txtInput == null || !txtInput.Visible) return;

            var clientW = main.ClientSize.Width;
            var clientH = main.ClientSize.Height;
            var margin = 6;
            var txtRect = new Rectangle(txtInput.Left, txtInput.Top, txtInput.Width, txtInput.Height);

            // Try positions in order: right, left, top, bottom — pick first that fits and does not intersect textbox.
            Rectangle tryRect;

            // RIGHT
            int tx = txtInput.Right + margin;
            int ty = txtInput.Top;
            ty = Math.Min(ty, clientH - toolbar.Height - margin);
            tryRect = new Rectangle(tx, ty, toolbar.Width, toolbar.Height);
            if (tx + toolbar.Width <= clientW && !tryRect.IntersectsWith(txtRect))
            {
                toolbar.Location = new Point(tx, ty);
                toolbar.BringToFront();
                return;
            }

            // LEFT
            tx = txtInput.Left - toolbar.Width - margin;
            ty = txtInput.Top;
            ty = Math.Min(ty, clientH - toolbar.Height - margin);
            tryRect = new Rectangle(tx, ty, toolbar.Width, toolbar.Height);
            if (tx >= 0 && !tryRect.IntersectsWith(txtRect))
            {
                toolbar.Location = new Point(tx, ty);
                toolbar.BringToFront();
                return;
            }

            // TOP
            tx = txtInput.Left;
            ty = txtInput.Top - toolbar.Height - margin;
            tx = Math.Min(Math.Max(0, tx), clientW - toolbar.Width - margin);
            tryRect = new Rectangle(tx, ty, toolbar.Width, toolbar.Height);
            if (ty >= 0 && !tryRect.IntersectsWith(txtRect))
            {
                toolbar.Location = new Point(tx, ty);
                toolbar.BringToFront();
                return;
            }

            // BOTTOM
            tx = txtInput.Left;
            ty = txtInput.Bottom + margin;
            tx = Math.Min(Math.Max(0, tx), clientW - toolbar.Width - margin);
            tryRect = new Rectangle(tx, ty, toolbar.Width, toolbar.Height);
            if (ty + toolbar.Height <= clientH && !tryRect.IntersectsWith(txtRect))
            {
                toolbar.Location = new Point(tx, ty);
                toolbar.BringToFront();
                return;
            }

            // Fallback: position to the right but adjust vertical so it doesn't fully cover center of textbox
            tx = Math.Min(txtInput.Right + margin, Math.Max(0, clientW - toolbar.Width - margin));
            ty = Math.Min(txtInput.Top, Math.Max(0, clientH - toolbar.Height - margin));
            toolbar.Location = new Point(tx, ty);
            toolbar.BringToFront();
        }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // nếu click vào toolbar, bỏ qua (không commit/không tạo)
                if (toolbar != null && toolbar.Visible && toolbar.Bounds.Contains(e.Location))
                    return;

                // Nếu đang có TextBox hiển thị
                if (txtInput.Visible)
                {
                    // nếu click bên trong textbox thì không commit; nếu click ra ngoài thì chốt/hủy và DỪNG (KHÔNG tạo box mới ngay)
                    Rectangle tbRect = new Rectangle(txtInput.Left, txtInput.Top, txtInput.Width, txtInput.Height);
                    if (tbRect.Contains(e.Location))
                    {
                        // click bên trong textbox -> cho phép tương tác với textbox (do nothing)
                        return;
                    }
                    else
                    {
                        // click ra ngoài: luôn ẩn textbox và toolbar.
                        // Nếu có chữ thì chốt xuống bitmap, ngược lại chỉ ẩn.
                        if (!string.IsNullOrWhiteSpace(txtInput.Text))
                            DrawTextToBitmap();
                        else
                            txtInput.Visible = false;

                        // ensure toolbar also hidden and form regains focus
                        HideToolbar();
                        main.Focus();
                        main.Invalidate();

                        // Suppress creating a new box until this mouse cycle ends
                        suppressCreateUntilMouseUp = true;

                        // stop here: DO NOT start a new selection on the same click
                        return;
                    }
                }

                // If suppressed (previous click closed textbox), ignore start of new drag for this mouse cycle.
                if (suppressCreateUntilMouseUp)
                {
                    // Clear suppression on this new MouseDown and do nothing.
                    // The following MouseUp will also clear if needed.
                    suppressCreateUntilMouseUp = false;
                    return;
                }

                // Nếu không có textbox đang hiển thị -> bắt đầu kéo để tạo khung
                isDragging = true;
                startPoint = e.Location;
                rectPreview = new Rectangle(e.Location, new Size(0, 0));
                txtInput.Visible = false;
                HideToolbar();
            }
        }

        public override void MouseMove(object sender, MouseEventArgs e)
        {
            // If user closed textbox with MouseDown earlier in the same cycle, ignore moves.
            if (suppressCreateUntilMouseUp) return;

            if (isDragging)
            {
                // Cập nhật kích thước khung nháp theo tay kéo của người dùng
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int width = Math.Abs(startPoint.X - e.X);
                int height = Math.Abs(startPoint.Y - e.Y);

                // Raw rectangle in client coordinates
                Rectangle rawRect = new Rectangle(x, y, width, height);

                // Clamp/Intersect the preview to the drawZone (main.rt_data)
                if (main != null)
                {
                    Rectangle drawZoneRect = main.rt_data;
                    rectPreview = Rectangle.Intersect(rawRect, drawZoneRect);
                    // If intersection is empty, keep rectPreview empty (no preview outside drawZone)
                    if (rectPreview.Width <= 0 || rectPreview.Height <= 0)
                        rectPreview = Rectangle.Empty;
                }
                else
                {
                    rectPreview = rawRect;
                }

                main.Invalidate();
            }
        }

        public override void MouseUp(object sender, MouseEventArgs e)
        {
            // If suppression active, clear it and do nothing (prevents creating a box from the same cycle).
            if (suppressCreateUntilMouseUp)
            {
                suppressCreateUntilMouseUp = false;
                return;
            }

            if (isDragging)
            {
                isDragging = false;

                var drawZoneRect = main?.rt_data ?? Rectangle.Empty;

                // Nếu kéo quá nhỏ thì tạo một khung mặc định (but ensure it fits inside drawZone)
                if (rectPreview.Width < 5 || rectPreview.Height < 5)
                {
                    // If user dragged but ended outside drawZone (rectPreview empty), create a default rect near startPoint clamped to drawZone.
                    int defaultW = 150;
                    int defaultH = 30;

                    if (drawZoneRect != Rectangle.Empty)
                    {
                        int left = Math.Min(Math.Max(startPoint.X, drawZoneRect.Left), Math.Max(drawZoneRect.Right - defaultW, drawZoneRect.Left));
                        int top = Math.Min(Math.Max(startPoint.Y, drawZoneRect.Top), Math.Max(drawZoneRect.Bottom - defaultH, drawZoneRect.Top));
                        int w = Math.Min(defaultW, drawZoneRect.Width);
                        int h = Math.Min(defaultH, drawZoneRect.Height);
                        rectPreview = new Rectangle(left, top, w, h);
                    }
                    else
                    {
                        rectPreview.Size = new Size(150, 30);
                    }
                }

                // Ensure rectPreview fully inside drawZoneRect
                if (drawZoneRect != Rectangle.Empty)
                {
                    // Clamp size
                    if (rectPreview.Width > drawZoneRect.Width) rectPreview.Width = drawZoneRect.Width;
                    if (rectPreview.Height > drawZoneRect.Height) rectPreview.Height = drawZoneRect.Height;

                    // Clamp position
                    if (rectPreview.Left < drawZoneRect.Left) rectPreview.X = drawZoneRect.Left;
                    if (rectPreview.Top < drawZoneRect.Top) rectPreview.Y = drawZoneRect.Top;
                    if (rectPreview.Right > drawZoneRect.Right) rectPreview.X = drawZoneRect.Right - rectPreview.Width;
                    if (rectPreview.Bottom > drawZoneRect.Bottom) rectPreview.Y = drawZoneRect.Bottom - rectPreview.Height;
                }

                // Thiết lập TextBox vào vùng đã kéo (tọa độ form/client)
                txtInput.Location = rectPreview.Location;
                txtInput.Size = rectPreview.Size;

                // set default font from current values (guard)
                if (txtInput?.Font != null)
                {
                    currentFontFamily = txtInput.Font.FontFamily?.Name ?? currentFontFamily;
                    currentFontSize = txtInput.Font.Size;
                    currentStyle = txtInput.Font.Style;
                }
                ApplyFontToTextBox();

                txtInput.ForeColor = main.pen_data.Color;
                txtInput.Text = "";
                txtInput.Visible = true;
                txtInput.Focus();

                ShowToolbar();

                main.Invalidate();
            }
        }

        public override void OnPaint_(PaintEventArgs e)
        {
            // Vẽ khung nét đứt khi đang kéo HOẶC khi đang gõ
            if (isDragging || (txtInput != null && txtInput.Visible))
            {
                Rectangle drawRect = isDragging ? rectPreview : new Rectangle(txtInput.Left - 1, txtInput.Top - 1, txtInput.Width + 2, txtInput.Height + 2);

                using (Pen dashedPen = new Pen(Color.Blue, 1))
                {
                    dashedPen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawRectangle(dashedPen, drawRect);
                }
            }
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter không kèm Ctrl => xuống dòng. Nếu không đủ chỗ thì mở rộng TextBox chiều cao
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                // thêm newline vào TextBox (mặc định behavior của TextBox là chèn newline khi Multiline = true)
                // nhưng phím Enter bị KeyDown, so handle manually to ensure behavior
                int selStart = txtInput.SelectionStart;
                txtInput.Text = txtInput.Text.Insert(selStart, Environment.NewLine);
                txtInput.SelectionStart = selStart + Environment.NewLine.Length;

                // nếu không đủ chỗ hiển thị dòng mới, tăng chiều cao TextBox (và di chuyển toolbar)
                int neededLines = txtInput.GetLineFromCharIndex(txtInput.TextLength) + 1;
                int neededHeight = neededLines * txtInput.Font.Height + 6;
                if (neededHeight > txtInput.Height)
                {
                    int maxAllowed = main.drawZone_data != null ? main.drawZone_data.Height - (txtInput.Top - main.rt_data.Y) : neededHeight;
                    txtInput.Height = Math.Min(maxAllowed, neededHeight);
                    PositionToolbar();
                }

                e.SuppressKeyPress = true;
                main.Invalidate();
                return;
            }

            // Ctrl + Enter để lưu kết quả
            if (e.KeyCode == Keys.Enter && e.Control)
            {
                DrawTextToBitmap();
                HideToolbar();
                e.SuppressKeyPress = true;
                return;
            }

            // Esc để hủy
            if (e.KeyCode == Keys.Escape)
            {
                txtInput.Visible = false;
                HideToolbar();
                main.Invalidate();
                e.SuppressKeyPress = true;
                return;
            }
        }

        // Ensure CancelEdit properly hides textbox and toolbar when tool is deactivated
        public override void CancelEdit()
        {
            try
            {
                txtInput.Visible = false;
            }
            catch { }
            HideToolbar();
            suppressCreateUntilMouseUp = false;
            isDragging = false;
            main.Invalidate();
        }

        public void DrawTextToBitmap()
        {
            if (!string.IsNullOrWhiteSpace(txtInput.Text))
            {
                main.SaveHistory();
                using (Graphics gImg = Graphics.FromImage(main.drawZone_data))
                {
                    gImg.TextRenderingHint = TextRenderingHint.AntiAlias;

                    // Chuyển tọa độ Form sang tọa độ drawZone (rt_data là vùng draw offset)
                    int x = txtInput.Left - main.rt_data.X;
                    int y = txtInput.Top - main.rt_data.Y;

                    using (Brush brush = new SolidBrush(txtInput.ForeColor))
                    {
                        // Vẽ toàn bộ văn bản (bao gồm cả các dòng đã Enter)
                        RectangleF layout = new RectangleF(x, y, txtInput.Width, txtInput.Height);
                        gImg.DrawString(txtInput.Text, txtInput.Font, brush, layout);
                    }
                }
            }
            txtInput.Visible = false;
            HideToolbar();
            main.Invalidate();
        }
    }
}