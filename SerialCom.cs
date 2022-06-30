using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace My串口调试助手
{
    //串口类
    public class SerialCom
    {
        public static SerialPort com { get; set; } = new SerialPort();
        //串口名字
        public static string comName { get; set; }
        //波特率
        public static int comBaudRate { get; set; }
        //校验位
        public static string comCheckBit { get; set; }
        //数据位
        public static int comDataBit { get; set; }
        //停止位
        public static string comStopBit { get; set; }
        //串口打开状态
        public static bool comStatus { get; set; }
        //数据显示
        public static List<string> comData = new List<string>();
    }

    //串口收发类
    public class MyCom
    {
        //方法：串口打开
        public void openCom()
        {
            if (SerialCom.comStatus == false)
            {
                //端口名
                SerialCom.com.PortName = SerialCom.comName;
                //波特率
                SerialCom.com.BaudRate = SerialCom.comBaudRate;
                //数据位
                SerialCom.com.DataBits = SerialCom.comDataBit;
                //校验位
                if (SerialCom.comCheckBit == "none")
                    SerialCom.com.Parity = System.IO.Ports.Parity.None;
                if (SerialCom.comCheckBit == "odd")
                    SerialCom.com.Parity = System.IO.Ports.Parity.Odd;
                if (SerialCom.comCheckBit == "even")
                    SerialCom.com.Parity = System.IO.Ports.Parity.Even;
                //停止位
                if (SerialCom.comStopBit == "1")
                    SerialCom.com.StopBits = System.IO.Ports.StopBits.One;
                if (SerialCom.comStopBit == "1.5")
                    SerialCom.com.StopBits = System.IO.Ports.StopBits.OnePointFive;
                if (SerialCom.comStopBit == "2")
                    SerialCom.com.StopBits = System.IO.Ports.StopBits.Two;
                //接收、发送数据回车显示
                SerialCom.com.NewLine = "\r\n";
                //启动线程
                Comthread();
            }
            else
            {
                //关闭串口
                SerialCom.comData.Add("关闭串口");
                SerialCom.com.Close();
                SerialCom.comStatus = false;
            }
        }
        //方法：线程读取串口数据
        private void Comthread()
        {
            //打开串口
            SerialCom.com.Open();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //设置编码格式
            SerialCom.com.Encoding = Encoding.GetEncoding("GB2312");
            //创建一个线程
            Thread thread = new Thread(ReadData);
            //设置为后台线程
            thread.IsBackground = true;
            //启动线程
            thread.Start();

        }
        //方法：串口读取
        public void ReadData()
        {
            SerialCom.comData.Add("打开串口");
            SerialCom.comStatus = true;
            while (SerialCom.comStatus)
            {
                //延时50ms
                Thread.Sleep(50);
                try
                {
                    //查询串口中目前保存了多少数据
                    int n = SerialCom.com.BytesToRead;
                    byte[] buffer = new byte[n];
                    SerialCom.com.Read(buffer, 0, n);
                    if (buffer.Length > 0)
                    {
                        string str = Encoding.Default.GetString(buffer);
                        string dateStr = DateTime.Now.ToString().Split(" ")[1];
                        SerialCom.comData.Add(dateStr + "-<  " + str);
                        //SerialCom.comData.Add(str);
                    }
                }
                catch
                {
                    SerialCom.comStatus = false;
                    SerialCom.com.Close();
                }
            }
        }

        //方法：串口写数据
        public void WriteData(byte[] bytes)
        {
            if(SerialCom.comStatus && bytes!=null)
            {
                SerialCom.com.Write(bytes,0, bytes.Length);
            }
        }
    }
}
