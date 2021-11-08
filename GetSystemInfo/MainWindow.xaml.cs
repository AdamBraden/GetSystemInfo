using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
//Windows WinRT APIs
using Windows.System.Profile;
using Windows.Security.Cryptography;
using Windows.Devices.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using WinRT.Interop;


namespace GetSystemInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaCapture captureManager;

        public MainWindow()
        {
            InitializeComponent();
        }
        private async void SystemInfo_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();

            //Get System Identifier
            var systemId = SystemIdentification.GetSystemIdForPublisher();

            //Get info about current DisplayMonitor
            var devinfoDisplays = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(DisplayMonitor.GetDeviceSelector());

            // Show system id and specific display monitor values
            sb.Append($"\nSystemId: ({systemId.Source})\n {CryptographicBuffer.EncodeToHexString(systemId.Id).ToUpper().Substring(0, 10)}");
            sb.Append("\n\nDisplay Info:\n");
            foreach (var devInfo in devinfoDisplays)
            {
                var displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(devInfo.Id);
                sb.Append($" Name:{displayMonitor.DisplayName}\n DeviceId: {displayMonitor.DeviceId}\n Connector: {displayMonitor.PhysicalConnector}\n Size: {displayMonitor.PhysicalSizeInInches}");
            }

            //// Windows App SDK APIs
            //var displayPower = Microsoft.Windows.System.Power.PowerManager.DisplayStatus;
            //var energySaver = Microsoft.Windows.System.Power.PowerManager.EnergySaverStatus;
            //sb.Append($"\n\n Display Status: {displayPower}");
            //sb.Append($"\n Energy Saver: {energySaver}");

            SystemInfo.Text = sb.ToString();

        }

        private async void TakePicture_Click(object sender, RoutedEventArgs e)
        {

            //Initialize Camera
            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();

            //set the format
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

            // create storage file in local app storage
            StorageFile file = await KnownFolders.CameraRoll.CreateFileAsync("TestPhoto.jpg", CreationCollisionOption.GenerateUniqueName);

            // take photo
            await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);

            // Get photo as a BitmapImage
            _ = new BitmapImage(new Uri(file.Path));

            // imagePreview is a <Image> object defined in XAML
            //imagePreview.Source = bmpImage;
        }

    }
}






////Get Windows Insider Preview OS Flighting info
//var attrNames = new List<string>{ "DeviceFamily", "OSVersionFull", "FlightRing" };
//var attrData = await AnalyticsInfo.GetSystemPropertiesAsync(attrNames);

////Format output
//sb.Append($"\nSystemId: ({systemId.Source})\n {CryptographicBuffer.EncodeToHexString(systemId.Id).ToUpper().Substring(0,10)}\n");
//sb.Append("\nWindows Build Info:");
//foreach (KeyValuePair<string, string> attr in attrData)
//{
//    sb.Append($"\n{attr.Key}={attr.Value}");
//}
