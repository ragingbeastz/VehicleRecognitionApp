using Camera.MAUI;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace VehicleRecognitionApp
{
    public partial class MainPage : ContentPage
    {
        private bool _isCapturing;
        private const int FrameRate = 60; // Target frame rate


        public MainPage()
        {
            InitializeComponent();
            StartPseudoLiveFeed();


        }

        private void StartPseudoLiveFeed()
        {
            _isCapturing = true;

            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / FrameRate), () =>
            {
                CaptureAndDisplaySnapshot();
                return _isCapturing; // Return true to keep the timer running
            });
        }

        private async void CaptureAndDisplaySnapshot()
        {
            try
            {
                // Capture a snapshot from the camera (Assuming the camera is already started)
                ImageSource snapshot = cameraView.GetSnapShot(ImageFormat.PNG);

                if (snapshot != null)
                {
                    // Display the snapshot in the Image control
                    pseudoCameraFeed.Source = snapshot;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing snapshot: {ex.Message}");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _isCapturing = false; // Stop capturing when the page is no longer visible
        }





        private async void CameraView_CamerasLoaded(object sender, EventArgs e)
        {
            if (cameraView.NumCamerasDetected > 0)
            {
                cameraView.Camera = cameraView.Cameras.First();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var result = await cameraView.StartCameraAsync();
                    if (result == CameraResult.Success)
                    {
                        Console.WriteLine("Camera Started");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Unable to start camera.", "OK");
                    }
                });
            }
            else
            {
                await DisplayAlert("Error", "No cameras detected.", "OK");
            }
        }



        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckPermissionsAsync();
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

        /*
        private async void OnCaptureImageClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    var stream = await photo.OpenReadAsync();
                    vehicleImage.Source = ImageSource.FromStream(() => stream);
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Error", "Camera is not supported on this device.", "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Error", "Camera permission is required.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
        }

        
        private async void OnUploadImageClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                if (photo != null)
                {
                    var stream = await photo.OpenReadAsync();
                    vehicleImage.Source = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
        }

        private void OnViewFavoritesClicked(object sender, EventArgs e)
        {
            // Navigate to the Favorites Page (to be implemented)
        }
        */
    }
}
