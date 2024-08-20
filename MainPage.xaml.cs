using Camera.MAUI;
//using HomeKit;
using Microsoft.Maui.Controls;
//using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VehicleRecognitionApp
{
    public partial class MainPage : ContentPage
    {
        private bool _isCapturing;
        private const int FrameRate = 30; // Target frame rate

        public MainPage()
        {
            InitializeComponent();
            _ = CheckPermissionsAsync();
            cameraView.CamerasLoaded += CameraView_CamerasLoaded;

        }

        private async void CameraView_CamerasLoaded(object sender, EventArgs e)
        {
            cameraView.Camera = cameraView.Cameras.First();

            _ = MainThread.InvokeOnMainThreadAsync(async () =>
            {
                _ = await cameraView.StopCameraAsync();
                _ = await cameraView.StartCameraAsync();

                StartPseudoLiveFeed();
            });

        }

        private void Button_clicked(object sender, EventArgs e)
        {
            string fileName = "snapshot.png";
            string mainDir = "/storage/emulated/0/Documents";
            string filePath = Path.Combine(mainDir, fileName);

            cameraView.SaveSnapShot(Camera.MAUI.ImageFormat.PNG, filePath); 
        }

        private void StartPseudoLiveFeed()
        {
            _isCapturing = true;


            // Start capturing frames at the specified frame rate
            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / FrameRate), () =>
            {
                CaptureAndDisplaySnapshot();
                return _isCapturing; // Return true to keep the timer running
            });
        }

        private async void CaptureAndDisplaySnapshot()
        {
            

            string fileName = "snapshot.png";
            string mainDir = "/storage/emulated/0/Documents";
            string filePath = Path.Combine(mainDir, fileName);

            if (File.Exists(@filePath))
            {
                File.Delete(@filePath);
            }

            Debug.WriteLine(filePath);


            cameraView.SaveSnapShot(Camera.MAUI.ImageFormat.PNG, filePath);

            _ = MainThread.InvokeOnMainThreadAsync(async () =>
            {
                pseudoCameraFeed.Source = ImageSource.FromFile(filePath);
            });
        }










        private async Task CheckPermissionsAsync()
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                // Handle the scenario when the permission is not granted
                await DisplayAlert("Permissions Denied", "Unable to access camera.", "OK");
                return;
            }

            var micStatus = await Permissions.RequestAsync<Permissions.Microphone>();
            if (micStatus != PermissionStatus.Granted)
            {
                // Handle the scenario when the permission is not granted
                await DisplayAlert("Permissions Denied", "Unable to access microphone.", "OK");
                return;
            }
        }

    }
}
