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
        private Bitmap drawZone; //Vùng vẽ chính
        [Browsable(false)]
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

        private string currentFilePath = "";// Biến lưu đường dẫn file hiện tại (Mặc định rỗng nghĩa là Untitled)

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
        //
        //private void InitializeHandles()
        //{
        //    handles = new Rectangle[3];
        //    for (int i = 0; i < 3; i++)
        //    {
        //        handles[i] = new Rectangle(); // Khởi tạo rỗng
        //    }
        //    using (SolidBrush handleBrush = new SolidBrush(Color.DarkBlue)) // Màu xanh đậm cho handles
        //    {
        //        foreach (Rectangle handle in handles)
        //        {
        //            g.FillRectangle(handleBrush, handle);//??
        //        }
        //    }
        //}

        private Rectangle rt;
        public Rectangle rt_data { get => rt; }

        public MainWindow()
        {
            InitializeComponent();

            //InitializeHandles();
            drawZone = new Bitmap(826, 440);
            g = Graphics.FromImage(drawZone);
            g.Clear(System.Drawing.Color.White);
            pen.StartCap = LineCap.Round; // Thêm dấu đen mờ ở điểm nối làm đường kẻ mịn hơn
            pen.EndCap = LineCap.Round;
            eraser.StartCap = LineCap.Round;
            eraser.EndCap = LineCap.Round;
            picking(pencilButton);
            using_ = new Pencil(this);
            rt = new Rectangle(70, 27, 826, 440);

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
            textButton.BackColor = SystemColors.ControlLight;
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

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            using_.MouseDown(sender, e);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            using_.MouseMove(sender, e);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            using_.MouseUp(sender, e);
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
        }

        private void textButton_Click(object sender, EventArgs e)
        {
            unpickingAll();
            picking(textButton);
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
            using_ = new Select(this);
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
                    e.Graphics.FillRectangle(Brushes.LightBlue, rec);
            }


        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

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
            if ((e.Location.X - 70) < 0 || (e.Location.Y - 27) < 0|| (e.Location.X) > rt.X + rt.Width || (e.Location.Y) > rt.Y + rt.Height)
                toolStripStatusLabel1.Text = "";
            else
                toolStripStatusLabel1.Text = (e.Location.X - 70).ToString() + ", " + (e.Location.Y - 27).ToString();
            SetCursor(e.Location);
            if (e.Button == MouseButtons.Left && isResize == true)
            {
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
                    //Rectangle rectScreen = new Rectangle(
                    //     selectionRect.X + OFFSET_X,
                    //     selectionRect.Y + OFFSET_Y,
                    //     selectionRect.Width,
                    //     selectionRect.Height
                    // );

                    //using (Pen pen = new Pen(Color.Black, 2))
                    //{
                    //    pen.DashStyle = DashStyle.Dot;
                    //    g.DrawRectangle(pen, rectScreen);
                    //}

                    Invalidate();
                }
            }
            else
                using_.MouseMove(sender, e);

        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (GetActiveHandle(e.Location) != (int)ResizeHandle.None && Select.BackColor == SystemColors.ControlLight)
            {
                isResize = true;
                firstPoint = e.Location;
                activeHandle = GetActiveHandle(e.Location);
                return;
            }
            else
            {
                using_.MouseDown(sender, e);
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
                Bitmap bmp = new Bitmap(img); // Chuyển thành Bitmap để dễ xử lý

                // 2. Chuyển công cụ sang Select
                unpickingAll();
                picking(Select); // Highlight nút Select

                // Khởi tạo công cụ Select mới
                Select selectTool = new Select(this);
                using_ = selectTool;

                // 3. QUAN TRỌNG: Lưu Undo trước khi Paste
                // (Để nếu người dùng không thích có thể Undo mất hình vừa dán)
                SaveHistory();

                // 4. Gọi hàm PasteImage vừa viết bên Select.cs
                // Thay vì vẽ dính xuống nền, ta đưa nó vào chế độ "được chọn"
                selectTool.PasteImage(bmp);
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Trường hợp 1: Chưa từng lưu (Untitled) -> Phải hỏi lưu ở đâu
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                sfd.Title = "Save your drawing";

                if (sfd.ShowDialog() == DialogResult.OK && sfd.FileName != "")
                {
                    // Cập nhật đường dẫn file hiện tại
                    currentFilePath = sfd.FileName;

                    // Cập nhật tiêu đề cửa sổ (Ví dụ: Paint - MyPicture.png)
                    this.Text = "Paint - " + System.IO.Path.GetFileName(currentFilePath);

                    // Thực hiện lưu
                    SaveImage(currentFilePath, sfd.FilterIndex);
                }
            }
            // Trường hợp 2: Đã có file -> Lưu đè luôn không hỏi
            else
            {
                // Mặc định lưu theo đuôi file cũ (đơn giản hóa là lưu PNG hoặc check đuôi)
                // Để an toàn, ta cứ gọi hàm lưu mặc định
                SaveImage(currentFilePath, 1); // 1 là PNG
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
    }
}
