using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging.Effects;
using System.Windows.Forms;
using GiaoDien.Tool;
using Microsoft.VisualBasic.Devices;
using System.Text.RegularExpressions;

namespace GiaoDien
{
    public partial class MainWindow : Form
    {
        private Pen pen = new Pen(System.Drawing.Color.Black, 1); // Thiết lập bút (brushes, color)
        public Pen pen_data { get => pen; }
        private Pen eraser = new Pen(System.Drawing.Color.White, 3);// Pen dành cho chế độ xóa
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Pen eraser_data { get => eraser; set => eraser = value; }
        public Bitmap drawZone; //Vùng vẽ chính
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private string currentFilePath = "";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Bitmap drawZone_data { get => drawZone; set => drawZone = value; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private Graphics g;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Button pickingColor_data { get => pickingColor; set => pickingColor = value; }

        private bool isResize = false;

        Tool_ using_;
        Point firstPoint;

        private Stack<Bitmap> undoStack = new Stack<Bitmap>();
        private Stack<Bitmap> redoStack = new Stack<Bitmap>();
        //
        private const int HANDLE_SIZE = 7; // Kích thước của mỗi điểm điều khiển
        private int activeHandle = -1;     // -1: Không có handle nào được kéo, 0-7: index của handle đang kéo
        private Rectangle[] handles = new Rectangle[3];       //// Mảng chứa các hình chữ nhật cho 8 handle
        private enum ResizeHandle
        {
            None = -1,
            MiddleRight = 0,
            BottomCenter = 1,
            BottomRight = 2,
        }

        // Context menu fields (khởi tạo trong ctor nhưng không gán this.ContextMenuStrip)
        private ContextMenuStrip ctxMenu;
        private ToolStripMenuItem copyItem;
        private ToolStripMenuItem cutItem;
        private ToolStripMenuItem pasteItem;

        public void CutCopy_Unenable()
        {
            cutToolStripMenuItem.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
        }
        public void CutCopy_Enable()
        {
            cutToolStripMenuItem.Enabled = true;
            copyToolStripMenuItem.Enabled = true;
        }

        private Rectangle rt;
        public Rectangle rt_data { get => rt; }

        private bool isDirty = false; // true nếu có thay đổi chưa được lưu

        public MainWindow()
        {
            InitializeComponent();

            // Khởi tạo vùng vẽ
            drawZone = new Bitmap(826, 440);
            g = Graphics.FromImage(drawZone);
            g.Clear(System.Drawing.Color.White);

            // Setup bút vẽ
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            eraser.StartCap = LineCap.Round;
            eraser.EndCap = LineCap.Round;

            // Mặc định chọn bút chì
            picking(pencilButton);
            using_ = new Pencil(this);
            rt = new Rectangle(70, 27, 826, 440);

            // --- TẠO MENU CHUỘT PHẢI NHƯNG KHÔNG GÁN this.ContextMenuStrip ---
            ctxMenu = new ContextMenuStrip();
            cutItem = new ToolStripMenuItem("Cut", null, cutToolStripMenuItem_Click);
            copyItem = new ToolStripMenuItem("Copy", null, copyToolStripMenuItem_Click);
            pasteItem = new ToolStripMenuItem("Paste", null, pasteToolStripMenuItem_Click);

            copyItem.ShortcutKeyDisplayString = "Ctrl+C";
            cutItem.ShortcutKeyDisplayString = "Ctrl+X";
            pasteItem.ShortcutKeyDisplayString = "Ctrl+V";

            ctxMenu.Items.Add(cutItem);
            ctxMenu.Items.Add(copyItem);
            ctxMenu.Items.Add(pasteItem);

            // LƯU Ý: không gán this.ContextMenuStrip = ctxMenu; để menu không xuất hiện mặc định ở mọi vị trí

            // --- PHẦN 2: LOGIC QUẢN LÝ FILE & ĐÓNG FORM (Code từ Server) ---
            // Đăng ký sự kiện đóng cửa sổ để hỏi lưu nếu cần
            this.FormClosing -= MainWindow_FormClosing;
            this.FormClosing += MainWindow_FormClosing;

            // Đặt tiêu đề cửa sổ cho các tài liệu "Untitled"
            if (string.IsNullOrEmpty(currentFilePath))
            {
                this.Text = GetNextUntitledTitle();
            }
            else
            {
                this.Text = "Paint - " + System.IO.Path.GetFileName(currentFilePath);
            }
        }

        private void picking(Button pick) // Hightlight button đang sử dụng
        {
            pick.BackColor = SystemColors.ActiveCaption;
        }
        private void unpickingAll() // Unhightlight toàn bộ button
        {
            eraserButton.BackColor = SystemColors.ControlLight;
            pencilButton.BackColor = SystemColors.ControlLight;
            fillButton.BackColor = SystemColors.ControlLight;
            pickColorButton.BackColor = SystemColors.ControlLight;
            shapeButton.BackColor = SystemColors.ControlLight;
            rectangleButton.BackColor = SystemColors.ControlLight;
            ellipseButton.BackColor = SystemColors.ControlLight;
            roundedRectangleButton.BackColor = SystemColors.ControlLight;
            polygonButton.BackColor = SystemColors.ControlLight;
            lineButton.BackColor = SystemColors.ControlLight;
            curveButton.BackColor = SystemColors.ControlLight;
            Select.BackColor = SystemColors.ControlLight;
            TextButton.BackColor = SystemColors.ControlLight;
            Invalidate();
        }

        public void SaveHistory()
        {
            // Lưu bản sao của drawZone vào undoStack
            undoStack.Push((Bitmap)drawZone.Clone());

            // Khi vẽ nét mới thì redoStack phải bị xóa sạch
            redoStack.Clear();

            // Đánh dấu có thay đổi chưa được lưu
            isDirty = true;
        }

        private void shapeButton_Click(object sender, EventArgs e)
        {
            if (shapePanel.Visible == true)
            {
                shapePanel.Visible = false;
                return;
            }
            shapePanel.Visible = true;
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            colorDialog1 = new ColorDialog();
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pickingColor.BackColor = colorDialog1.Color;
                pen.Color = colorDialog1.Color;
            }

        }

        // helper to switch tools and cancel any active edit in current tool
        private void SetTool(Tool_ newTool)
        {
            try { using_?.CancelEdit(); } catch { }
            using_ = newTool;
        }

        private void pencilButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(pencilButton);
            SetTool(new Pencil(this));
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            pen.Width = 1 + trackBar1.Value * 2;
            eraser.Width = pen.Width * 3;
        }

        private void pickColorButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(pickColorButton);
            SetTool(new pickColor_(this));
        }

        private void eraserButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(eraserButton);
            SetTool(new Eraser(this));
        }

        private void rectangleButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(rectangleButton);
            SetTool(new Rectangle_(this));
        }

        private void fillButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(fillButton);
            SetTool(new FillColor(this));
        }

        private void polygonButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(polygonButton);
            using_ = new Polygon(this);
        }

        private void ellipseButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(ellipseButton);
            using_ = new Ellipse_(this);
        }

        private void roundedRectangleButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(roundedRectangleButton);
            using_ = new RRectangle_(this);
        }

        private void lineButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(lineButton);
            using_ = new Line_(this);
        }

        private void curveButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(curveButton);
            using_ = new Curve_(this);
        }

        private void Select_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(Select);
            // Use SetTool so CancelEdit is called on previous tool (fixes lingering toolbar)
            SetTool(new GiaoDien.Select(this));
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(drawZone, 70, 27);
            handles[2] = new Rectangle(rt.Location.X + rt.Width, rt.Location.Y + rt.Height, HANDLE_SIZE, HANDLE_SIZE);
            handles[1] = new Rectangle(rt.Location.X + rt.Width / 2, rt.Location.Y + rt.Height, HANDLE_SIZE, HANDLE_SIZE);
            handles[0] = new Rectangle(rt.Location.X + rt.Width, rt.Location.Y + rt.Height / 2, HANDLE_SIZE, HANDLE_SIZE);
            using_.OnPaint_(e);
            if (Select.BackColor == SystemColors.ControlLight)
            {
                foreach (Rectangle rec in handles)
                    e.Graphics.FillRectangle(Brushes.DarkBlue, rec);
            }
            if (isResize)
            {
                Pen pen_ = new Pen(Color.Black, 2);
                pen_.DashStyle = DashStyle.Dot;
                e.Graphics.DrawRectangle(pen, rt);
            }
        }

        private int GetActiveHandle(Point mouseLocation)
        {
            for (int i = 0; i < handles.Length; i++)
            {
                if (handles[i].Contains(mouseLocation))
                {
                    return i;
                }
            }
            return (int)ResizeHandle.None;
        }
        private void SetCursor(Point mouseLocation)
        {
            int handle = GetActiveHandle(mouseLocation);
            switch ((ResizeHandle)handle)
            {
                case ResizeHandle.BottomRight:
                    Cursor = Cursors.SizeNWSE;
                    break;

                case ResizeHandle.BottomCenter:
                    Cursor = Cursors.SizeNS;
                    break;
                case ResizeHandle.MiddleRight:
                    Cursor = Cursors.SizeWE;
                    break;
                default:
                    Cursor = Cursors.Default;
                    break;

            }

        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Location.X - 70) < 0 || (e.Location.Y - 27) < 0 || (e.Location.X) > rt.X + rt.Width || (e.Location.Y) > rt.Y + rt.Height)
                toolStripStatusLabel1.Text = "";
            else
                toolStripStatusLabel1.Text = (e.Location.X - 70).ToString() + ", " + (e.Location.Y - 27).ToString();
            SetCursor(e.Location);

            const int MIN_SIZE = 1;

            if (e.Button == MouseButtons.Left && isResize == true)
            {
                int dx = e.X - firstPoint.X;
                int dy = e.Y - firstPoint.Y;

                int oldWidth = rt.Width;
                int oldHeight = rt.Height;

                switch ((ResizeHandle)activeHandle)
                {
                    case ResizeHandle.MiddleRight:
                        {
                            int desiredW = oldWidth + dx;
                            int newW = Math.Max(desiredW, MIN_SIZE);
                            rt.Width = newW;

                            int consumedX = newW - oldWidth;
                            firstPoint.X += consumedX;
                        }
                        break;

                    case ResizeHandle.BottomCenter:
                        {
                            int desiredH = oldHeight + dy;
                            int newH = Math.Max(desiredH, MIN_SIZE);
                            rt.Height = newH;

                            int consumedY = newH - oldHeight;
                            firstPoint.Y += consumedY;
                        }
                        break;

                    case ResizeHandle.BottomRight:
                        {
                            int desiredW = oldWidth + dx;
                            int desiredH = oldHeight + dy;
                            int newW = Math.Max(desiredW, MIN_SIZE);
                            int newH = Math.Max(desiredH, MIN_SIZE);

                            rt.Width = newW;
                            rt.Height = newH;

                            int consumedX = newW - oldWidth;
                            int consumedY = newH - oldHeight;
                            firstPoint.X += consumedX;
                            firstPoint.Y += consumedY;
                        }
                        break;
                }

                Invalidate();
            }
            else
                using_.MouseMove(sender, e);

        }


        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            // CHỈ XỬ LÝ KHI NHẤN CHUỘT TRÁI
            if (e.Button == MouseButtons.Left)
            {
                SaveHistory(); // Lưu Undo

                // Kiểm tra logic Resize khung hình
                if (GetActiveHandle(e.Location) != (int)ResizeHandle.None && Select.BackColor == SystemColors.ControlLight)
                {
                    isResize = true;
                    firstPoint = e.Location;
                    activeHandle = GetActiveHandle(e.Location);
                    return;
                }
                else
                {
                    // Nếu không phải resize thì mới cho công cụ vẽ hoạt động
                    using_.MouseDown(sender, e);
                }
            }
        }
        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            // Xử lý click phải: chỉ show menu khi click vào vùng chọn
            if (e.Button == MouseButtons.Right)
            {
                bool shouldShow = false;
                bool hasSelection = false;

                if (using_ is Select sel)
                {
                    hasSelection = sel.HasSelection;
                    if (sel.IsPointInsideSelection(e.Location))
                        shouldShow = true;
                }

                // cập nhật trạng thái các menu item trước khi hiện
                cutItem.Enabled = copyItem.Enabled = hasSelection;
                pasteItem.Enabled = Clipboard.ContainsImage();

                if (shouldShow)
                {
                    // Hiện menu tại vị trí chuột (relative to form)
                    ctxMenu.Show(this, e.Location);
                }

                // Không tiếp tục xử lý cho click phải
                return;
            }

            if (isResize == true)
            {
                isResize = false;
                Bitmap add = new Bitmap(rt.Width, rt.Height);

                using (Graphics g = Graphics.FromImage(add))
                {
                    g.Clear(Color.White);
                    g.DrawImage(drawZone, 0, 0);
                }
                drawZone = add;

                Invalidate();
                return;
            }
            else using_.MouseUp(sender, e);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using_.Copy();
        }

        // Sự kiện nút Cut
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using_.Cut();
        }

        // Sự kiện nút Paste
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                Image img = Clipboard.GetImage();
                Bitmap bmp = new Bitmap(img);

                Select selectTool = new Select(this);
                // Use SetTool so previous tool's CancelEdit() runs
                SetTool(selectTool);

                SaveHistory();
                selectTool.PasteImage(bmp);
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TRƯỜNG HỢP 1: Chưa lưu bao giờ (Untitled) -> Phải hỏi lưu ở đâu
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                sfd.Title = "Lưu bản vẽ";

                if (sfd.ShowDialog() == DialogResult.OK && sfd.FileName != "")
                {
                    // Thực hiện lưu (Dùng hàm SaveImage mới của server để kiểm tra lỗi)
                    bool ok = SaveImage(sfd.FileName, sfd.FilterIndex);
                    if (ok)
                    {
                        currentFilePath = sfd.FileName;
                        this.Text = "Paint - " + System.IO.Path.GetFileName(currentFilePath);
                    }
                }
            }
            // TRƯỜNG HỢP 2: Đã có file rồi -> Lưu đè luôn (Quick Save)
            else
            {
                bool ok = SaveImage(currentFilePath, 1); // 1 là PNG (hàm tự chọn theo đuôi)
                if (!ok)
                {
                    // Nếu lưu thất bại, có thể báo hoặc giữ nguyên isDirty = true
                }
            }
        }

        // Hàm phụ trợ để lưu file cho gọn code
        private bool SaveImage(string path, int formatIndex)
        {
            try
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;

                // Nếu path có đuôi .jpg thì đổi format, v.v...
                if (path.EndsWith(".jpg") || path.EndsWith(".jpeg")) format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (path.EndsWith(".bmp")) format = System.Drawing.Imaging.ImageFormat.Bmp;

                drawZone.Save(path, format);

                // Nếu lưu thành công, đánh dấu không còn thay đổi
                isDirty = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu file: " + ex.Message);
                return false;
            }
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                // Push current state to redo
                redoStack.Push((Bitmap)drawZone.Clone());

                // Pop previous state and set drawZone
                Bitmap old = undoStack.Pop();

                // Dispose current drawZone to avoid leaking and to avoid stale references
                try { drawZone?.Dispose(); } catch { }
                drawZone = (Bitmap)old.Clone();
                old.Dispose();

                // If current tool holds transient bitmaps (Select), cancel its edit so it won't later overwrite drawZone
                try { using_?.CancelEdit(); } catch { }

                Invalidate();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                // Push current state to undo
                undoStack.Push((Bitmap)drawZone.Clone());

                Bitmap future = redoStack.Pop();

                try { drawZone?.Dispose(); } catch { }
                drawZone = (Bitmap)future.Clone();
                future.Dispose();

                // Cancel transient edits in current tool so no stale clones overwrite drawZone
                try { using_?.CancelEdit(); } catch { }

                Invalidate();
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainWindow newTab = new MainWindow();
            newTab.Show();
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            using_.key_Keydown(sender, e);
        }

        private void TextButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(TextButton);
            using_ = new Text_(this);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn ảnh để mở trong ứng dụng";
                ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string path = ofd.FileName;
                try
                {
                    // Tải ảnh từ file, sao chép vào Bitmap mới (để file không bị khóa)
                    using (Image img = Image.FromFile(path))
                    {
                        Bitmap newBitmap = new Bitmap(img);

                        if (g != null)
                        {
                            g.Dispose();
                            g = null;
                        }
                        if (drawZone != null)
                        {
                            drawZone.Dispose();
                            drawZone = null;
                        }

                        drawZone = newBitmap;
                        g = Graphics.FromImage(drawZone);

                        rt = new Rectangle(rt.X, rt.Y, drawZone.Width, drawZone.Height);
                        currentFilePath = path;
                        this.Text = "Paint - " + System.IO.Path.GetFileName(path);
                        isDirty = false;
                    }
                    Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể tải ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GetNextUntitledTitle()
        {
            var used = new HashSet<int>();
            var regex = new Regex(@"^Paint - Untitled(\d*)$");

            foreach (Form f in Application.OpenForms)
            {
                var text = f.Text ?? string.Empty;
                var m = regex.Match(text);
                if (m.Success)
                {
                    var numPart = m.Groups[1].Value;
                    int n = 0;
                    if (!string.IsNullOrEmpty(numPart))
                    {
                        if (!int.TryParse(numPart, out n))
                            continue;
                    }
                    used.Add(n);
                }
            }

            int i = 0;
            while (used.Contains(i)) i++;

            return i == 0 ? "Paint - Untitled" : $"Paint - Untitled{i}";
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isDirty)
                return; // Không có thay đổi => đóng ngay

            string name = string.IsNullOrEmpty(currentFilePath) ? this.Text : System.IO.Path.GetFileName(currentFilePath);
            var result = MessageBox.Show($"Bạn có muốn lưu thay đổi cho {name}?", "Lưu thay đổi", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (result == DialogResult.No)
            {
                return;
            }

            // Người dùng chọn Yes -> lưu
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                sfd.Title = "Lưu ảnh";

                if (sfd.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(sfd.FileName))
                {
                    e.Cancel = true;
                    return;
                }

                bool ok = SaveImage(sfd.FileName, 1);
                if (ok)
                {
                    currentFilePath = sfd.FileName;
                    this.Text = "Paint - " + System.IO.Path.GetFileName(currentFilePath);
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
            else
            {
                bool ok = SaveImage(currentFilePath, 1);
                if (!ok)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Save As";
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap Image|*.bmp";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (!string.IsNullOrEmpty(currentFilePath))
                    sfd.FileName = System.IO.Path.GetFileName(currentFilePath);

                if (sfd.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(sfd.FileName))
                    return;

                try
                {
                    string path = sfd.FileName;
                    System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                    if (path.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) ||
                        path.EndsWith(".jpeg", System.StringComparison.OrdinalIgnoreCase))
                    {
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                    }
                    else if (path.EndsWith(".bmp", System.StringComparison.OrdinalIgnoreCase))
                    {
                        format = System.Drawing.Imaging.ImageFormat.Bmp;
                    }

                    drawZone.Save(path, format);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}