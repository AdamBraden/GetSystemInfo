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
using Windows.Graphics.Display;
using Windows.System.Profile;
using Windows.Security.Cryptography;
using Windows.Devices.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using System.Diagnostics;
using System.Windows.Interop;
using Windows.Security.Credentials.UI;

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();


            //Get System Identifier
            var systemId = SystemIdentification.GetSystemIdForPublisher();
            sb.Append($"SystemId ({systemId.Source})\n {CryptographicBuffer.EncodeToHexString(systemId.Id).ToUpper()}");

            var attrNames = new List<string>{ "DeviceFamily", "OSVersionFull", "FlightRing" };
            var attrData = await AnalyticsInfo.GetSystemPropertiesAsync(attrNames);

            foreach (KeyValuePair<string, string> attr in attrData)
            {
                sb.Append($"\n{attr.Key}={attr.Value}");
            }
            SystemInfo.Text = sb.ToString();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();

            var devinfoDisplays = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(DisplayMonitor.GetDeviceSelector());

            foreach (var devInfo in devinfoDisplays)
            {
                var displayMonitor = await DisplayMonitor.FromInterfaceIdAsync(devInfo.Id);
                sb.Append($"{displayMonitor.DisplayName}\n{displayMonitor.DeviceId}\n{displayMonitor.PhysicalConnector}\n{displayMonitor.PhysicalSizeInInches}");
            }
            DisplayInfo.Text = sb.ToString();
            
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            //Initialize Camera
            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();

            // Use Windows Hello - Pin
            // Request the logged on user's consent via fingerprint swipe.
            var userMessage = "Please provide Pin Code verification.";

            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            var consentResult = await UserConsentVerifierInterop.RequestVerificationForWindowAsync(hWnd, userMessage);
            //var consentResult = await UserConsentVerifier.RequestVerificationAsync(userMessage);

            if (consentResult == Windows.Security.Credentials.UI.UserConsentVerificationResult.Verified)
            {
                ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

                // create storage file in local app storage
                StorageFile file = await KnownFolders.CameraRoll.CreateFileAsync("TestPhoto.jpg", CreationCollisionOption.GenerateUniqueName);

                // take photo
                await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);

                // Get photo as a BitmapImage
                BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));

                // imagePreview is a <Image> object defined in XAML
                imagePreview.Source = bmpImage;
            }
            else
            {
                Debug.WriteLine("Pin code error!");
            
            }
        }

    }
}
