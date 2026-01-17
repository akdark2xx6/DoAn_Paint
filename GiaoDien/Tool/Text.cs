using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace GiaoDien
{
    internal class Text : Tool_
    {
        private MainWindow main;
        private TextBox txtInput;
        public Font CurrentFont { get; set; } = new Font("Arial", 12);

        public Text(MainWindow main) : base(main)
        {
            this.main = main;

            // Khởi tạo TextBox nếu chưa có trên Form
            txtInput = main.Controls.Find("txtInput", true).FirstOrDefault() as TextBox;
            if (txtInput == null)
            {
                txtInput = new TextBox();
                txtInput.Name = "txtInput";
                txtInput.Multiline = true;
                txtInput.BorderStyle = BorderStyle.FixedSingle;
                txtInput.Visible = false;

                // Gán sự kiện nhấn phím để kết thúc nhập liệu
                txtInput.KeyDown += TxtInput_KeyDown;

                main.Controls.Add(txtInput);
                txtInput.BringToFront();
            }
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Nhấn Enter (không giữ Shift) để vẽ chữ xuống nền
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                DrawTextToBitmap();
                e.SuppressKeyPress = true;
            }
            // Nhấn Esc để hủy
            else if (e.KeyCode == Keys.Escape)
            {
                txtInput.Visible = false;
            }
        }

        public override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Nếu đang gõ ở chỗ khác thì chốt chữ đó xuống trước
                if (txtInput.Visible && !string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    DrawTextToBitmap();
                }

                // Hiển thị khung nhập tại vị trí click
                txtInput.Location = e.Location;
                txtInput.Font = CurrentFont;
                txtInput.ForeColor = main.pen_data.Color; // Lấy màu từ bút hiện tại
                txtInput.Size = new Size(150, CurrentFont.Height + 10);
                txtInput.Text = "";
                txtInput.Visible = true;
                txtInput.Focus();
            }
        }

        public void DrawTextToBitmap()
        {
            if (!string.IsNullOrWhiteSpace(txtInput.Text))
            {
                // Lưu lịch sử (Undo) trước khi thay đổi drawZone
                main.SaveHistory();

                using (Graphics gImg = Graphics.FromImage(main.drawZone_data))
                {
                    // Giúp chữ mịn hơn
                    gImg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    // Tọa độ vẽ = Tọa độ TextBox trên Form - Tọa độ gốc của vùng vẽ (70, 27)
                    int x = txtInput.Left - main.rt_data.X;
                    int y = txtInput.Top - main.rt_data.Y;

                    using (Brush brush = new SolidBrush(txtInput.ForeColor))
                    {
                        gImg.DrawString(txtInput.Text, txtInput.Font, brush, new Point(x, y));
                    }
                }
                main.Invalidate(); // Yêu cầu Form vẽ lại
            }
            txtInput.Visible = false;
        }
        public override void OnPaint_(PaintEventArgs e)
        {}

        // Các hàm này bắt buộc phải override từ Tool_
        public override void MouseMove(object sender, MouseEventArgs e) { }
        public override void MouseUp(object sender, MouseEventArgs e) { }
    }
}