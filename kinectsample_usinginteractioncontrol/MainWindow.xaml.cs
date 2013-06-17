using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace kinectsample_usinginteractioncontrol
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        KinectSensorChooser sensorChooser;
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensorChooser = new KinectSensorChooser(); 
            sensorChooser.KinectChanged +=sensorChooser_KinectChanged;
            sensorChooserUI.KinectSensorChooser = sensorChooser;
            sensorChooser.Start(); 
        }

        KinectSensor sensor; 
        void sensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
            if (e.NewSensor == null)
            {
                Title = "找不到 Kinect感應器";
                return; 
            }
            else
                Title = "感應器狀態 : " + e.NewSensor.Status;

            //底程式碼僅針對 Kinect for Windows感應器(NewSensor)做處理
            //Kinect for Xbox(OldSensor)需另外處理
            if (e.NewSensor.Status == KinectStatus.Connected)
            {
                sensor = e.NewSensor;
                SensorInit();
            }
        }

        private WriteableBitmap _ColorImageBitmap;
        private Int32Rect _ColorImageBitmapRect;
        private int _ColorImageStride;
        void SensorInit()
        {
            SetupSerialPort();

            #region 彩色攝影機相關參數與物件初始化
            ColorImageStream colorStream = sensor.ColorStream;
            sensor.ColorStream.Enable();
            _ColorImageBitmap = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight, 96, 96,
                                                    PixelFormats.Bgr32, null);
            _ColorImageBitmapRect = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
            _ColorImageStride = colorStream.FrameWidth * colorStream.FrameBytesPerPixel;
            colorframe.Source = _ColorImageBitmap;
            sensor.ColorFrameReady += sensor_ColorFrameReady;            
            #endregion
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame == null)
                    return;

                byte[] pixelData = new byte[frame.PixelDataLength];
                frame.CopyPixelDataTo(pixelData);
                _ColorImageBitmap.WritePixels(_ColorImageBitmapRect, pixelData, _ColorImageStride, 0);
            }
        }

        SerialPort arduinoboard;
        void SetupSerialPort()
        {
            try
            {
                arduinoboard = new SerialPort();
                arduinoboard.PortName = "COM3";
                arduinoboard.BaudRate = 9600;
                arduinoboard.StopBits = StopBits.One;
                arduinoboard.Parity = Parity.None;
                arduinoboard.DataBits = 8;
                arduinoboard.DataReceived += arduinoboard_DataReceived;
                arduinoboard.Open();
            }
            catch
            {
                MessageBox.Show("與序列阜連結失敗");
                Application.Current.Shutdown();
            }
        }

        void arduinoboard_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = arduinoboard.ReadLine().Trim();
            Debug.WriteLine(data);
            if (data == "<UP>")
                sensor.ElevationAngle += 5;
            else if (data == "<DOWN>")
                sensor.ElevationAngle -= 5;
            else if (data == "<RESET>")
                sensor.ElevationAngle = 0 ;

            Thread.Sleep(1000);
        }
    }
}
