using System;
using System.Collections.Generic;
using System.Text;

namespace GiaoDien
{
    public class pickColor_ : Tool_
    {
        public pickColor_(MainWindow owner) : base(owner) { prev = (Bitmap)owner.drawZone_data.Clone(); }
        public override void MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
        }
        public override void MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing == true)
                owner.pickingColor_data.BackColor = owner.drawZone_data.GetPixel(e.X-70, e.Y-27);
        }
        public override void MouseUp(object sender, MouseEventArgs e)
        {
            owner.pickingColor_data.BackColor = owner.drawZone_data.GetPixel(e.X-70, e.Y-27);
            owner.pen_data.Color = owner.drawZone_data.GetPixel(e.X-70, e.Y-27);
            isDrawing = false;
        }
        public override void OnPaint_(PaintEventArgs e)
        {
        }
    }//
}
