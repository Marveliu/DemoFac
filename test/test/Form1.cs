using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //开始时窗体最小化，截屏后再还原
            Screen s = Screen.PrimaryScreen;
            Rectangle r = s.Bounds;
            int iWidth = r.Width;
            int iHeight = r.Height;
            //创建一个和屏幕一样大的bitmap
            Image img = new Bitmap(iWidth, iHeight);
            //从一个继承自image类的对象中创建Graphics对象
            Graphics g = Graphics.FromImage(img);
            //抓取全屏幕
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(iWidth, iHeight));
            img.Save("E:\\1.jpeg");

            //以下是截取一定范围的图片
            //Image myImage = new Bitmap(300, 200);
            //Graphics g1= Graphics.FromImage(myImage);
            //g1.CopyFromScreen(new Point(Cursor.Position.X - 150, Cursor.Position.Y - 25), new Point(0, 0), new Size(300, 200));
            //IntPtr dc1 = g1.GetHdc();
            //g1.ReleaseHdc(dc1);
            //myImage.Save("E:\\2.jpeg");
        }
    }
}
