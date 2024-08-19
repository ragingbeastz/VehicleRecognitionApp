using Camera.MAUI;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
//using CoreVideo;

namespace VehicleRecognitionApp
{
    public partial class MainPage : ContentPage
    {
        private bool _isCapturing;
        private const int FrameRate = 5; // Target frame rate

        public MainPage()
        {
            InitializeComponent();
           // CheckPermissionsAsync();
            cameraView.CamerasLoaded += CameraView_CamerasLoaded;
        }

        private async void CameraView_CamerasLoaded(object sender, EventArgs e)
        {
            if (cameraView.NumCamerasDetected > 0)
            {
                cameraView.Camera = cameraView.Cameras.FirstOrDefault();

                if (await cameraView.StartCameraAsync(new Size(1280, 720)) == CameraResult.Success)
                {
                    Console.WriteLine("Camera started successfully.");
                    //playing = true;
                    StartPseudoLiveFeed();
                }

                else
                {
                    await DisplayAlert("Error", "Unable to start camera.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No cameras detected.", "OK");
            }
        }

        private async Task DisplayCameraFeedTest()
        {
            // Display the camera feed for 2 seconds to test it
            Console.WriteLine("Displaying camera feed test...");
            await Task.Delay(2000); // Delay to allow camera feed to display
            Console.WriteLine("Camera feed test complete.");
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
            try
            {
                Console.WriteLine("Attempting to capture a snapshot...");

                ImageSource snapshot = cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);

                if (snapshot != null)
                {
                    pseudoCameraFeed.Source = snapshot;
                    Console.WriteLine("Snapshot captured and displayed.");
                }
                else
                {
                    Console.WriteLine("Snapshot capture failed; snapshot is null.");
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
