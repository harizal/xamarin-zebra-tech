using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using System;
using System.Collections.Generic;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace xamarin_zebra_tech
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private static int PERMISSION_REQUEST_CODE = 10;
        private static int REQUEST_ENABLE_BT = 1;
        private BluetoothAdapter _bluetoothAdapter;
        private BluetoothReceiver _bluetoothReceiver;
        public List<string> _deviceList = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            RequestBluetoothPermissions();

            var btnStart = FindViewById<Button>(Resource.Id.btnStart);
            btnStart.Click += StartProcess;
        }

        private bool CheckPermission()
        {
            return ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothScan) == Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted;
        }

        private void StartProcess(object sender, EventArgs eventArgs)
        {
            if (CheckPermission())
            {
                if (_bluetoothAdapter == null)
                {
                    Toast.MakeText(this, "Bluetooth is not supported on this device.", ToastLength.Short).Show();
                    return;
                }
                if (!_bluetoothAdapter.IsEnabled)
                {
                    // Prompt user to enable Bluetooth
                    Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                    StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
                }
                else
                    StartScanning();
            }
            else
                Toast.MakeText(this, "Bluetooth permissions denied.", ToastLength.Short).Show();
        }

        private void RequestBluetoothPermissions()
        {
            ActivityCompat.RequestPermissions(this, new[]
            {
                Manifest.Permission.BluetoothScan,
                Manifest.Permission.BluetoothConnect,
                Manifest.Permission.AccessFineLocation
            }, PERMISSION_REQUEST_CODE);
        }

        private void StartScanning()
        {
            try
            {
                _deviceList = new List<string>();
                _bluetoothReceiver = new BluetoothReceiver();
                RegisterReceiver(_bluetoothReceiver, new IntentFilter(BluetoothDevice.ActionFound));
                RegisterReceiver(_bluetoothReceiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted));
                RegisterReceiver(_bluetoothReceiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));

                _bluetoothAdapter.StartDiscovery();
                Toast.MakeText(this, "Scanning for Bluetooth devices...", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public void ShowDeviceSelectionDialog()
        {
            if (_deviceList != null && _deviceList.Count > 0)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Select a Printer");

                var devicesArray = _deviceList.ToArray();

                builder.SetItems(devicesArray, (sender, args) =>
                {
                    // User selected a device from the list
                    var selectedDevice = devicesArray[args.Which];
                    ConnectAndPrint(selectedDevice);
                });

                builder.SetNegativeButton("Cancel", (sender, args) => { /* Handle cancellation */ });

                AlertDialog dialog = builder.Create();
                dialog.Show();
            }
            else
            {
                Toast.MakeText(this, "No devices found", ToastLength.Short).Show();
            }
        }

        private void ConnectAndPrint(string deviceInfo)
        {
            try
            {
                // Extract MAC address from the selected device info
                var deviceAddress = deviceInfo.Split('(')[1].TrimEnd(')');

                // Initialize Zebra printer helper with the selected device's MAC address
                var printerHelper = new ZebraPrinterHelper(deviceAddress);

                // Connect to the printer
                printerHelper.Connect();

                // Print text
                printerHelper.PrintText("Hello, Zebra Printer!");

                // Disconnect after printing
                printerHelper.Disconnect();

                Toast.MakeText(this, "Printed successfully!", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, $"Failed to print: {ex.Message}", ToastLength.Long).Show();
            }
        }


        #region Override        

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterReceiver(_bluetoothReceiver);
            _bluetoothAdapter.CancelDiscovery();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == PERMISSION_REQUEST_CODE)
            {
                bool allPermissionsGranted = true;

                for (int i = 0; i < permissions.Length; i++)
                {
                    if (grantResults[i] != Permission.Granted)
                    {
                        allPermissionsGranted = false;
                        break;
                    }
                }

                if (!allPermissionsGranted)
                {
                    Toast.MakeText(this, "Bluetooth permissions denied", ToastLength.Short).Show();
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == REQUEST_ENABLE_BT)
            {
                if (resultCode != Result.Ok)
                {
                    // User did not enable Bluetooth
                    Toast.MakeText(this, "Bluetooth is required for this application", ToastLength.Short).Show();
                }
            }
        }

        #endregion
    }
}