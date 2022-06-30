using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using System.IO;

namespace My串口调试助手
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //实例化一个定时器
        DispatcherTimer dtimer = new DispatcherTimer();
        //实例化一个串口类
        private MyCom mCom = new MyCom();
        //日志功能是否开启
        private bool openLog = false;
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //开启定时器
            //设置定时间隔为100ms
            dtimer.Interval = TimeSpan.FromMilliseconds(100);
            //注册定时中断事件
            dtimer.Tick += new EventHandler(timer_Tick);
            dtimer.Start();

            //搜索串口
            string[] ports = SerialPort.GetPortNames();
            //添加到串口列表
            this.portsName.ItemsSource = ports;
            //设置默认选项，索引方式
            this.portsName.SelectedIndex = 3;

            //设置默认波特率
            string[] baudRateList = new string[] { "300", "600", "1200", "2400", "4800", "9600", "19200", "38400", "43000", "56000", "57600", "115200" };
            this.baudRate.ItemsSource = baudRateList;
            this.baudRate.SelectedItem = "9600";

            //添加选项--7位数据位
            this.dataBit.Items.Add(7);
            this.dataBit.Items.Add(8);
            this.dataBit.SelectedIndex = 1;
            //MessageBox.Show(this.dataBit.Items.ToString());

            //设置默认停止位
            this.stopBit.SelectedIndex = 0;

            //设置默认校验位
            this.checkBit.SelectedIndex = 0;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if(SerialCom.comData.Count > 0)
            {
                string str = SerialCom.comData[0] + "\n";
                revData.Text += str;
                //移除索引0处的数据，后面的数据会重新排序
                SerialCom.comData.RemoveAt(0);
                //滚动到最后一行
                revData.ScrollToEnd();
                if (SerialCom.comStatus)
                {
                    open.Content = "关闭串口";
                }
                else
                {
                    open.Content = "打开串口";
                }
                //若开启了日志
                if (openLog)
                {
                    WriteLog(str);
                }
            }
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            //搜索串口
            string[] ports = SerialPort.GetPortNames();
            //添加到串口列表
            this.portsName.ItemsSource = ports;
            //设置默认选项
            this.portsName.SelectedIndex = 0;
        }


        private void open_Click(object sender, RoutedEventArgs e)
        {
            SerialCom.comName = portsName.Text;
            SerialCom.comBaudRate = int.Parse(baudRate.Text);
            SerialCom.comDataBit = int.Parse(dataBit.Text);
            SerialCom.comCheckBit = checkBit.Text;
            SerialCom.comStopBit = stopBit.Text;
            mCom.openCom();
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            mCom.WriteData(Encoding.UTF8.GetBytes(sendData.Text));
            string str = DateTime.Now.ToString().Split(" ")[1];
            string logStr = str + "->  " + sendData.Text;
            SerialCom.comData.Add(logStr);
            //若开启了日志
            if (openLog)
            {
                //WriteLog(logStr);
            }


        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("calc.exe");
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("mspaint.exe");
        }

        //开始日志
        private void log_Click(object sender, RoutedEventArgs e)
        {
            openLog = !openLog;
            if (openLog)
            {
                log.Content = "关闭日志";
            }
            else
            {
                log.Content = "开启日志";
            }
        }

        //写日志
        private void WriteLog(string str)
        {
            // 获取当前文件夹
            string currentDirName = System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine(currentDirName);
            // 创建一个子文件夹名称，并添加到当前文件夹下
            string pathString = System.IO.Path.Combine(currentDirName, "log");
            // 创建子文件夹
            System.IO.Directory.CreateDirectory(pathString);

            // 创建文件名
            string fileName = "log.txt";
            // 再次使用 Combine 方法把文件名添加到路径
            pathString = System.IO.Path.Combine(pathString, fileName);

            // 写
            using (StreamWriter sw = File.AppendText(pathString))
            {
                sw.WriteLine(str);

            }
        }
    }

}
