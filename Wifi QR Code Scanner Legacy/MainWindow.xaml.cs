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
using System.IO;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Aztec;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Timers;
using System.Drawing.Imaging;
using Wifi_QR_Code_Scanner_Legacy.Managers;
using Wifi_QR_Code_Scanner_Legacy.Business;
using System.Windows.Forms;
using System.ComponentModel;

namespace Wifi_QR_Code_Scanner_Legacy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FilterInfoCollection CaptureDevice;
        VideoCaptureDevice FinalFrame;
        WifiConnectionManager wifiConnectionManager;
        BarcodeReader barcodeReader;
        Bitmap cameraFrame;
        ImageUtils.ImageUtils imageUtils;
        int frameCounter = 0;
        bool scanningLocked = false;
        RotateFlipType imageMode = RotateFlipType.RotateNoneFlipNone;
        public MainWindow()
        {
            InitializeComponent();
            wifiConnectionManager = new WifiConnectionManager();
            barcodeReader = new BarcodeReader();
            imageUtils = new ImageUtils.ImageUtils();
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                String translated = Properties.Resources.ResourceManager.GetString("Device " + Device.Name);
                if (translated != null)
                {
                    comboBox1.Items.Add(translated);
                } else
                {
                    comboBox1.Items.Add(Device.Name);
                }
            }
            pictureBox1.Stretch = Stretch.Uniform;
            comboBox1.SelectedIndex = 0;
            FinalFrame = new VideoCaptureDevice();
            System.Windows.Forms.Application.ApplicationExit += new EventHandler(Shutdown);
            Closing += new CancelEventHandler(Window_Closing);
        }

        private void Shutdown(object sender, EventArgs e)
        {
            scanningLocked = true;
            if (FinalFrame != null) {
                FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);
                FinalFrame.SignalToStop();
                FinalFrame = null;
            }
            
            pictureBox1.Source = null;
            cameraFrame = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            scanningLocked = true;
            if (FinalFrame != null)
            {
                FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);
                FinalFrame.SignalToStop();
                FinalFrame = null;
            }
            pictureBox1.Source = null;
            cameraFrame = null;
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (!(this.Dispatcher.HasShutdownStarted||this.Dispatcher.HasShutdownFinished))
            {
                this.Dispatcher.Invoke(() =>
                {
                    frameCounter++;
                    cameraFrame = eventArgs.Frame;
                    cameraFrame.RotateFlip(imageMode);
                    pictureBox1.Source = imageUtils.ToBitmapImage(cameraFrame);

                    //I'm expecting 30fps
                    if (frameCounter > 5 && !scanningLocked)
                    {
                        frameCounter = 0;

                        Result result = barcodeReader.Decode(cameraFrame);
                        if (result != null)
                        {
                            try
                            {
                                string decoded = result.ToString();
                                if (decoded != "")
                                {
                                    scanningLocked = true;
                                    var wifiAPdata = WifiStringParser.parseWifiString(decoded);
                                    if (wifiAPdata != null)
                                    {
                                        DialogResult dr = System.Windows.Forms.MessageBox.Show(wifiAPdata.ToString(),
                                        Properties.Resources.ConnectTitle, MessageBoxButtons.YesNo);
                                        switch (dr)
                                        {
                                            case System.Windows.Forms.DialogResult.Yes:
                                                wifiConnectionManager.Connect(wifiAPdata);
                                                break;
                                            case System.Windows.Forms.DialogResult.No:
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        DialogResult dr = System.Windows.Forms.MessageBox.Show(Properties.Resources.ErrorMessageNoWifi,
                                        Properties.Resources.ErrorTitleNoWifi, MessageBoxButtons.OK);
                                        switch (dr)
                                        {
                                            case System.Windows.Forms.DialogResult.OK:
                                                break;
                                        }
                                    }
                                }
                                scanningLocked = false;
                            }
                            catch (Exception ex)
                            {
                                scanningLocked = false;
                            }
                        }
                    }
                });
            }
        }

        private void Button1_Click_1(object sender, RoutedEventArgs e)
        {
            if (comboBox1.IsEnabled)
            {
                comboBox1.IsEnabled = false;
                scanningLocked = false;
                button1.Content = Properties.Resources.ScanButtonStop;
                FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
                List<VideoCapabilities> availableCapabilities = null;
                try
                {
                    availableCapabilities = FinalFrame.VideoCapabilities.Where(capabilities => capabilities is VideoCapabilities).Select(capabilities => (VideoCapabilities)capabilities).ToList();
                    VideoCapabilities bestVideoResolution = this.findBestResolution(availableCapabilities);
                    if (bestVideoResolution != null)
                    {
                        FinalFrame.VideoResolution = bestVideoResolution;
                    }
                }
                catch (Exception ex)
                {
                    //MessageManager.ShowMessageToUserAsync("No resolutions could be detected, trying default mode.");
                }

                FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
                FinalFrame.Start();
            } else
            {
                comboBox1.IsEnabled = true;
                button1.Content = Properties.Resources.ScanButton;
                Shutdown(null, null);
            }
        }

        private VideoCapabilities findBestResolution(List<VideoCapabilities> videoEncodingProperties)
        {
            if (videoEncodingProperties != null && videoEncodingProperties.Any())
            {
                //we want the highest bitrate, highest fps, with a resolution that is as square as possible, and not too small or too large
                var result = videoEncodingProperties.Where(a => (a.FrameSize.Width >= a.FrameSize.Height))//square or wider
                    .Where(b => b.FrameSize.Width >= 400 && b.FrameSize.Height >= 400)//not too small
                    .Where(c => c.FrameSize.Width <= 800 && c.FrameSize.Height <= 600)//not too large
                    .OrderBy(d => ((double)d.FrameSize.Width) / ((double)d.FrameSize.Height))//order by smallest aspect ratio(most 'square' possible)
                    .ThenBy(e => e.FrameSize.Width)//order by the smallest possible width
                    .First();
                return result;
            }
            return null;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (flipImageCheckbox.IsChecked.Value)
            {
                imageMode = RotateFlipType.RotateNoneFlipX;
            } else
            {
                imageMode = RotateFlipType.RotateNoneFlipNone;
            }
        }
    }
}
