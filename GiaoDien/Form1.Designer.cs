namespace GiaoDien
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            cutToolStripMenuItem = new ToolStripMenuItem();
            copyToolStripMenuItem = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            panel1 = new Panel();
            TextButton = new Button();
            Select = new Button();
            pickingColor = new Button();
            brushPanel = new Panel();
            label1 = new Label();
            trackBar1 = new TrackBar();
            shapePanel = new Panel();
            roundedRectangleButton = new Button();
            rectangleButton = new Button();
            curveButton = new Button();
            polygonButton = new Button();
            lineButton = new Button();
            ellipseButton = new Button();
            colorButton = new Button();
            pickColorButton = new Button();
            shapeButton = new Button();
            eraserButton = new Button();
            fillButton = new Button();
            pencilButton = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            colorDialog1 = new ColorDialog();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            panel1.SuspendLayout();
            brushPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            shapePanel.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.ControlLight;
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(961, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, toolStripMenuItem1, saveToolStripMenuItem, saveAsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(184, 22);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.O;
            toolStripMenuItem1.Size = new Size(184, 22);
            toolStripMenuItem1.Text = "Open";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(184, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            saveAsToolStripMenuItem.Size = new Size(184, 22);
            saveAsToolStripMenuItem.Text = "Save as";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, undoToolStripMenuItem, redoToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Enabled = false;
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            cutToolStripMenuItem.Size = new Size(144, 22);
            cutToolStripMenuItem.Text = "Cut";
            cutToolStripMenuItem.Click += cutToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Enabled = false;
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            copyToolStripMenuItem.Size = new Size(144, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            pasteToolStripMenuItem.Size = new Size(144, 22);
            pasteToolStripMenuItem.Text = "Paste";
            pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoToolStripMenuItem.Size = new Size(144, 22);
            undoToolStripMenuItem.Text = "Undo";
            undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoToolStripMenuItem.Size = new Size(144, 22);
            redoToolStripMenuItem.Text = "Redo";
            redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel1.BackColor = SystemColors.ControlLight;
            panel1.Controls.Add(TextButton);
            panel1.Controls.Add(Select);
            panel1.Controls.Add(pickingColor);
            panel1.Controls.Add(brushPanel);
            panel1.Controls.Add(shapePanel);
            panel1.Controls.Add(colorButton);
            panel1.Controls.Add(pickColorButton);
            panel1.Controls.Add(shapeButton);
            panel1.Controls.Add(eraserButton);
            panel1.Controls.Add(fillButton);
            panel1.Controls.Add(pencilButton);
            panel1.Location = new Point(0, 27);
            panel1.Name = "panel1";
            panel1.Size = new Size(66, 485);
            panel1.TabIndex = 1;
            // 
            // TextButton
            // 
            TextButton.AllowDrop = true;
            TextButton.BackColor = SystemColors.ControlLight;
            TextButton.FlatStyle = FlatStyle.Flat;
            TextButton.ForeColor = SystemColors.ControlLight;
            TextButton.Image = Properties.Resources.text_fields_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24;
            TextButton.Location = new Point(4, 133);
            TextButton.Name = "TextButton";
            TextButton.Size = new Size(30, 30);
            TextButton.TabIndex = 11;
            TextButton.UseVisualStyleBackColor = false;
            TextButton.Click += TextButton_Click;
            // 
            // Select
            // 
            Select.AllowDrop = true;
            Select.BackColor = SystemColors.ControlLight;
            Select.FlatStyle = FlatStyle.Flat;
            Select.ForeColor = SystemColors.ControlLight;
            Select.Image = (Image)resources.GetObject("Select.Image");
            Select.Location = new Point(3, 67);
            Select.Name = "Select";
            Select.Size = new Size(30, 30);
            Select.TabIndex = 10;
            Select.Tag = "4";
            Select.UseVisualStyleBackColor = false;
            Select.Click += Select_Click;
            // 
            // pickingColor
            // 
            pickingColor.AllowDrop = true;
            pickingColor.BackColor = Color.Black;
            pickingColor.Enabled = false;
            pickingColor.ForeColor = SystemColors.ControlLight;
            pickingColor.Location = new Point(40, 106);
            pickingColor.Name = "pickingColor";
            pickingColor.Size = new Size(20, 20);
            pickingColor.TabIndex = 9;
            pickingColor.UseVisualStyleBackColor = false;
            // 
            // brushPanel
            // 
            brushPanel.BackColor = SystemColors.ControlDark;
            brushPanel.Controls.Add(label1);
            brushPanel.Controls.Add(trackBar1);
            brushPanel.Location = new Point(4, 170);
            brushPanel.Name = "brushPanel";
            brushPanel.Size = new Size(57, 142);
            brushPanel.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 124);
            label1.Name = "label1";
            label1.Size = new Size(27, 15);
            label1.TabIndex = 4;
            label1.Text = "Size";
            // 
            // trackBar1
            // 
            trackBar1.BackColor = SystemColors.ControlLight;
            trackBar1.Location = new Point(6, 6);
            trackBar1.Maximum = 5;
            trackBar1.Name = "trackBar1";
            trackBar1.Orientation = Orientation.Vertical;
            trackBar1.Size = new Size(45, 115);
            trackBar1.TabIndex = 3;
            trackBar1.TickStyle = TickStyle.Both;
            trackBar1.ValueChanged += trackBar1_ValueChanged;
            // 
            // shapePanel
            // 
            shapePanel.BackColor = SystemColors.ControlDark;
            shapePanel.Controls.Add(roundedRectangleButton);
            shapePanel.Controls.Add(rectangleButton);
            shapePanel.Controls.Add(curveButton);
            shapePanel.Controls.Add(polygonButton);
            shapePanel.Controls.Add(lineButton);
            shapePanel.Controls.Add(ellipseButton);
            shapePanel.Location = new Point(4, 326);
            shapePanel.Name = "shapePanel";
            shapePanel.Size = new Size(58, 85);
            shapePanel.TabIndex = 0;
            // 
            // roundedRectangleButton
            // 
            roundedRectangleButton.AllowDrop = true;
            roundedRectangleButton.BackColor = SystemColors.ControlLight;
            roundedRectangleButton.FlatStyle = FlatStyle.Flat;
            roundedRectangleButton.ForeColor = SystemColors.ControlLightLight;
            roundedRectangleButton.Image = Properties.Resources.crop_3_2_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24;
            roundedRectangleButton.Location = new Point(30, 30);
            roundedRectangleButton.Name = "roundedRectangleButton";
            roundedRectangleButton.Size = new Size(25, 25);
            roundedRectangleButton.TabIndex = 14;
            roundedRectangleButton.Tag = "9";
            roundedRectangleButton.UseVisualStyleBackColor = false;
            roundedRectangleButton.Click += roundedRectangleButton_Click;
            // 
            // rectangleButton
            // 
            rectangleButton.AllowDrop = true;
            rectangleButton.BackColor = SystemColors.ControlLight;
            rectangleButton.FlatStyle = FlatStyle.Flat;
            rectangleButton.ForeColor = SystemColors.ControlLight;
            rectangleButton.Image = (Image)resources.GetObject("rectangleButton.Image");
            rectangleButton.Location = new Point(3, 3);
            rectangleButton.Name = "rectangleButton";
            rectangleButton.Size = new Size(25, 25);
            rectangleButton.TabIndex = 9;
            rectangleButton.Tag = "6";
            rectangleButton.UseVisualStyleBackColor = false;
            rectangleButton.Click += rectangleButton_Click;
            // 
            // curveButton
            // 
            curveButton.AllowDrop = true;
            curveButton.BackColor = SystemColors.ControlLight;
            curveButton.FlatStyle = FlatStyle.Flat;
            curveButton.ForeColor = SystemColors.ControlLight;
            curveButton.Image = Properties.Resources.line_curve_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24;
            curveButton.Location = new Point(30, 57);
            curveButton.Name = "curveButton";
            curveButton.Size = new Size(25, 25);
            curveButton.TabIndex = 13;
            curveButton.Tag = "11";
            curveButton.UseVisualStyleBackColor = false;
            curveButton.Click += curveButton_Click;
            // 
            // polygonButton
            // 
            polygonButton.AllowDrop = true;
            polygonButton.BackColor = SystemColors.ControlLight;
            polygonButton.FlatStyle = FlatStyle.Flat;
            polygonButton.ForeColor = SystemColors.ControlLight;
            polygonButton.Image = (Image)resources.GetObject("polygonButton.Image");
            polygonButton.Location = new Point(30, 3);
            polygonButton.Name = "polygonButton";
            polygonButton.Size = new Size(25, 25);
            polygonButton.TabIndex = 10;
            polygonButton.Tag = "7";
            polygonButton.TextAlign = ContentAlignment.TopRight;
            polygonButton.UseVisualStyleBackColor = false;
            polygonButton.Click += polygonButton_Click;
            // 
            // lineButton
            // 
            lineButton.AllowDrop = true;
            lineButton.BackColor = SystemColors.ControlLight;
            lineButton.FlatStyle = FlatStyle.Flat;
            lineButton.ForeColor = SystemColors.ControlLight;
            lineButton.Image = Properties.Resources.icons8_line_24;
            lineButton.Location = new Point(3, 57);
            lineButton.Name = "lineButton";
            lineButton.Size = new Size(25, 25);
            lineButton.TabIndex = 12;
            lineButton.Tag = "10";
            lineButton.UseVisualStyleBackColor = false;
            lineButton.Click += lineButton_Click;
            // 
            // ellipseButton
            // 
            ellipseButton.AllowDrop = true;
            ellipseButton.BackColor = SystemColors.ControlLight;
            ellipseButton.FlatStyle = FlatStyle.Flat;
            ellipseButton.ForeColor = SystemColors.ControlLight;
            ellipseButton.Image = (Image)resources.GetObject("ellipseButton.Image");
            ellipseButton.Location = new Point(3, 30);
            ellipseButton.Name = "ellipseButton";
            ellipseButton.Size = new Size(25, 25);
            ellipseButton.TabIndex = 11;
            ellipseButton.Tag = "8";
            ellipseButton.UseVisualStyleBackColor = false;
            ellipseButton.Click += ellipseButton_Click;
            // 
            // colorButton
            // 
            colorButton.AllowDrop = true;
            colorButton.BackColor = SystemColors.ControlLight;
            colorButton.FlatStyle = FlatStyle.Flat;
            colorButton.ForeColor = SystemColors.ControlLight;
            colorButton.Image = Properties.Resources.palette_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24;
            colorButton.Location = new Point(3, 99);
            colorButton.Name = "colorButton";
            colorButton.Size = new Size(30, 30);
            colorButton.TabIndex = 8;
            colorButton.UseVisualStyleBackColor = false;
            colorButton.Click += colorButton_Click;
            // 
            // pickColorButton
            // 
            pickColorButton.AllowDrop = true;
            pickColorButton.BackColor = SystemColors.ControlLight;
            pickColorButton.FlatStyle = FlatStyle.Flat;
            pickColorButton.ForeColor = SystemColors.ControlLight;
            pickColorButton.Image = Properties.Resources.colorize_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24;
            pickColorButton.Location = new Point(35, 35);
            pickColorButton.Name = "pickColorButton";
            pickColorButton.Size = new Size(30, 30);
            pickColorButton.TabIndex = 6;
            pickColorButton.Tag = "4";
            pickColorButton.UseVisualStyleBackColor = false;
            pickColorButton.Click += pickColorButton_Click;
            // 
            // shapeButton
            // 
            shapeButton.AllowDrop = true;
            shapeButton.BackColor = SystemColors.ControlLight;
            shapeButton.FlatStyle = FlatStyle.Flat;
            shapeButton.ForeColor = SystemColors.ControlLight;
            shapeButton.Image = Properties.Resources.shape_line_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24;
            shapeButton.Location = new Point(34, 67);
            shapeButton.Name = "shapeButton";
            shapeButton.Size = new Size(30, 30);
            shapeButton.TabIndex = 5;
            shapeButton.UseVisualStyleBackColor = false;
            shapeButton.Click += shapeButton_Click;
            // 
            // eraserButton
            // 
            eraserButton.AllowDrop = true;
            eraserButton.BackColor = SystemColors.ControlLight;
            eraserButton.FlatStyle = FlatStyle.Flat;
            eraserButton.ForeColor = SystemColors.ControlLight;
            eraserButton.Image = Properties.Resources.ink_eraser_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24;
            eraserButton.Location = new Point(3, 35);
            eraserButton.Name = "eraserButton";
            eraserButton.Size = new Size(30, 30);
            eraserButton.TabIndex = 2;
            eraserButton.Tag = "3";
            eraserButton.UseVisualStyleBackColor = false;
            eraserButton.Click += eraserButton_Click;
            // 
            // fillButton
            // 
            fillButton.AllowDrop = true;
            fillButton.BackColor = SystemColors.ControlLight;
            fillButton.FlatStyle = FlatStyle.Flat;
            fillButton.ForeColor = SystemColors.ControlLight;
            fillButton.Image = Properties.Resources.format_color_fill_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24;
            fillButton.Location = new Point(35, 3);
            fillButton.Name = "fillButton";
            fillButton.Size = new Size(30, 30);
            fillButton.TabIndex = 1;
            fillButton.Tag = "2";
            fillButton.UseVisualStyleBackColor = false;
            fillButton.Click += fillButton_Click;
            // 
            // pencilButton
            // 
            pencilButton.AllowDrop = true;
            pencilButton.BackColor = SystemColors.ControlLight;
            pencilButton.FlatStyle = FlatStyle.Flat;
            pencilButton.ForeColor = SystemColors.ControlLight;
            pencilButton.Image = Properties.Resources.stylus_24dp_1F1F1F_FILL0_wght400_GRAD0_opsz24;
            pencilButton.Location = new Point(3, 3);
            pencilButton.Name = "pencilButton";
            pencilButton.Size = new Size(30, 30);
            pencilButton.TabIndex = 0;
            pencilButton.Tag = "1";
            pencilButton.UseVisualStyleBackColor = false;
            pencilButton.Click += pencilButton_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = SystemColors.ControlLight;
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 490);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(961, 22);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDark;
            ClientSize = new Size(961, 512);
            Controls.Add(statusStrip1);
            Controls.Add(panel1);
            Controls.Add(menuStrip1);
            DoubleBuffered = true;
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            Name = "MainWindow";
            Text = "Paint";
            FormClosing += MainWindow_FormClosing;
            FormClosed += MainWindow_FormClosed;
            KeyDown += MainWindow_KeyDown;
            MouseDown += MainWindow_MouseDown;
            MouseMove += MainWindow_MouseMove;
            MouseUp += MainWindow_MouseUp;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            brushPanel.ResumeLayout(false);
            brushPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            shapePanel.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private Panel panel1;
        private Button pencilButton;
        private Button fillButton;
        private Button pickColorButton;
        private Button shapeButton;
        private Button eraserButton;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private TrackBar trackBar1;
        private Panel brushPanel;
        private Button colorButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel shapePanel;
        private Button roundedRectangleButton;
        private Button rectangleButton;
        private Button curveButton;
        private Button polygonButton;
        private Button lineButton;
        private Button ellipseButton;
        private Label label1;
        private ColorDialog colorDialog1;
        private Button pickingColor;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem newToolStripMenuItem;
        private Button Select;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Button TextButton;
        private ToolStripMenuItem saveAsToolStripMenuItem;
    }
}
