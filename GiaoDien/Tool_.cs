using System;
using System.Collections.Generic;
using System.Text;

namespace GiaoDien
{
    public abstract class Tool_
    {
        protected bool isDrawing = false;
        protected MainWindow owner;
        protected Point firstPoint;
        protected Point lastPoint;
        protected Pen pen_;
        protected Bitmap prev;
        public Tool_(MainWindow owner)
        {
            this.owner = owner;
            pen_ = owner.pen_data;
        }
        public abstract void MouseDown(object sender, MouseEventArgs e);
        public abstract void MouseMove(object sender, MouseEventArgs e);
        public abstract void MouseUp(object sender, MouseEventArgs e);

        public abstract void OnPaint_(PaintEventArgs e);
        public virtual void Copy() { }
        public virtual void Cut() { }
        public virtual void Paste(Bitmap clipboardImage) { } // Paste nhận vào ảnh từ clipboard
    }
}
