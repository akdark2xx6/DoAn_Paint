using GiaoDien.Tool;
using Microsoft.VisualBasic.Devices;
using System.ComponentModel;  
using System.Drawing;        
using System.Drawing.Drawing2D;
using System.Drawing.Imaging.Effects;
using System.Windows.Forms;

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

            // --- THÊM ĐOẠN NÀY ĐỂ TẠO MENU CHUỘT PHẢI ---
            ContextMenuStrip ctxMenu = new ContextMenuStrip();

            ToolStripMenuItem copyItem = new ToolStripMenuItem("Copy", null, copyToolStripMenuItem_Click);
            ToolStripMenuItem cutItem = new ToolStripMenuItem("Cut", null, cutToolStripMenuItem_Click);
            ToolStripMenuItem pasteItem = new ToolStripMenuItem("Paste", null, pasteToolStripMenuItem_Click);

            copyItem.ShortcutKeyDisplayString = "Ctrl+C";
            cutItem.ShortcutKeyDisplayString = "Ctrl+X";
            pasteItem.ShortcutKeyDisplayString = "Ctrl+V";

            ctxMenu.Items.Add(cutItem);
            ctxMenu.Items.Add(copyItem);
            ctxMenu.Items.Add(pasteItem);

            // Gán menu cho Form (Vì bạn vẽ trực tiếp lên Form)
            this.ContextMenuStrip = ctxMenu;
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
            Invalidate();
        }

        public void SaveHistory()
        {
            // Lưu bản sao của drawZone vào undoStack
            // Lưu ý: Phải dùng Clone() để tạo bản sao, nếu không nó sẽ lưu tham chiếu
            undoStack.Push((Bitmap)drawZone.Clone());

            // Khi vẽ nét mới thì redoStack phải bị xóa sạch (logic chuẩn của Undo)
            redoStack.Clear();
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

        private void pencilButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(pencilButton);
            using_ = new Pencil(this);
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
            using_ = new pickColor_(this);
        }

        private void eraserButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(eraserButton);
            using_ = new Eraser(this);
        }

        private void rectangleButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(rectangleButton);
            using_ = new Rectangle_(this);
        }

        private void fillButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(fillButton);
            using_ = new FillColor(this);
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
            using_ = new GiaoDien.Select(this);
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
            if (e.Button == MouseButtons.Left && isResize == true)
            {

                int dx = e.X - firstPoint.X;
                int dy = e.Y - firstPoint.Y;
                firstPoint = e.Location;
                switch ((ResizeHandle)activeHandle)
                {
                    case ResizeHandle.MiddleRight:
                        rt.Width += dx;
                        break;

                    case ResizeHandle.BottomCenter:
                        rt.Height += dy;
                        break;
                    case ResizeHandle.BottomRight:
                        rt.Width += dx; rt.Height += dy;
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

                // Kiểm tra logic Resize khung hình (Code gốc của bạn)
                // Lưu ý: Select ở đây là cái nút Button màu xám
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
                // 1. Lấy ảnh từ Clipboard
                Image img = Clipboard.GetImage();
                Bitmap bmp = new Bitmap(img);

                // 2. Chuyển ngay sang công cụ Select (quan trọng)
                // Reset công cụ cũ
                using_ = null;

                // Tạo công cụ Select mới
                Select selectTool = new Select(this);
                using_ = selectTool;

                // 3. Lưu Undo trước khi dán (để nếu dán nhầm còn undo được)
                SaveHistory();

                // 4. Gọi hàm PasteImage vừa viết bên Select.cs
                // Ảnh sẽ hiện lên ở dạng "được chọn" để bạn kéo đi chỗ khác
                selectTool.PasteImage(bmp);

                // (Tùy chọn) Highlight nút Select trên giao diện nếu bạn muốn
                // picking(selectBtn); 
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
                    // Cập nhật đường dẫn file vào biến nhớ
                    currentFilePath = sfd.FileName;

                    // Cập nhật tiêu đề cửa sổ (VD: Paint - HinhAnh.png)
                    this.Text = "Paint - " + System.IO.Path.GetFileName(currentFilePath);

                    // Thực hiện lưu
                    SaveImageToDisk(currentFilePath, sfd.FilterIndex);
                }
            }
            // TRƯỜNG HỢP 2: Đã có file rồi -> Lưu đè luôn (Quick Save)
            else
            {
                // Lưu đè vào đường dẫn cũ (mặc định dùng định dạng PNG hoặc tự check)
                SaveImageToDisk(currentFilePath, 1);
            }
        }

        // Hàm phụ trợ để code gọn hơn
        private void SaveImageToDisk(string path, int filterIndex)
        {
            try
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;

                // Kiểm tra đuôi file để chọn định dạng đúng
                string ext = System.IO.Path.GetExtension(path).ToLower();
                if (ext == ".jpg" || ext == ".jpeg") format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (ext == ".bmp") format = System.Drawing.Imaging.ImageFormat.Bmp;

                // Nếu lưu lần đầu qua Dialog thì ưu tiên Filter người dùng chọn
                else if (filterIndex == 2) format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (filterIndex == 3) format = System.Drawing.Imaging.ImageFormat.Bmp;

                drawZone.Save(path, format);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu file: " + ex.Message);
            }
        }

        // Hàm phụ trợ để lưu file cho gọn code
        private void SaveImage(string path, int formatIndex)
        {
            try
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;

                // Nếu path có đuôi .jpg thì đổi format, v.v...
                if (path.EndsWith(".jpg") || path.EndsWith(".jpeg")) format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (path.EndsWith(".bmp")) format = System.Drawing.Imaging.ImageFormat.Bmp;

                // Nếu đang dùng công cụ (ví dụ đang kéo vùng chọn), cần chốt hình xuống nền trước khi lưu
                // (Tùy chọn: bạn có thể thêm using_.Finish() nếu muốn)

                drawZone.Save(path, format);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu file: " + ex.Message);
            }
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                // 1. Đẩy hiện tại sang Redo
                redoStack.Push((Bitmap)drawZone.Clone());

                // 2. Lấy cái cũ từ Undo ra
                Bitmap old = undoStack.Pop();

                // 3. Gán lại vào drawZone
                drawZone = (Bitmap)old.Clone();
                // 4. Dọn dẹp RAM (Quan trọng với ảnh bitmap)
                old.Dispose();
                Invalidate();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                // 1. Đẩy hiện tại sang Undo
                undoStack.Push((Bitmap)drawZone.Clone());

                // 2. Lấy từ Redo ra
                Bitmap future = redoStack.Pop();

                // 3. Gán lại
                drawZone = (Bitmap)future.Clone();
                future.Dispose();
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
            using_ = new GiaoDien.Text(this);
        }


    }
}
