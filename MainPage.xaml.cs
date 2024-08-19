using Camera.MAUI;
//using HomeKit;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace VehicleRecognitionApp
{
    public partial class MainPage : ContentPage
    {
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
            });

        }

        private void Button_clicked(object sender, EventArgs e)
        {
            string fileName = "snapshot.png";
            string mainDir = "/storage/emulated/0/Documents";
            string filePath = Path.Combine(mainDir, fileName);

            cameraView.SaveSnapShot(Camera.MAUI.ImageFormat.PNG, filePath); 
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
