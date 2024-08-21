using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.Threading;

namespace xamarin_zebra_tech
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private static int PERMISSION_REQUEST_CODE = 10;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var btnStart = FindViewById<Button>(Resource.Id.btnStart);
            btnStart.Click += StartProcess;
        }

        private async void StartProcess(object sender, EventArgs eventArgs)
        {
            new Thread(() =>
            {
                Thread.Sleep(500);
                RunOnUiThread(Print);
            }).Start();
        }

        private async void Print()
        {
            try
            {
                string printerMacAddress = "00:22:58:3C:4A:5B";
                var printerHelper = new ZebraPrinterHelper(printerMacAddress);

                printerHelper.Connect();
                printerHelper.PrintText("Hello, Zebra Printer");
                printerHelper.Disconnect();


                //if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
                //{

                //    var listPermissions = new System.Collections.Generic.List<string>();

                //    const string bl = Manifest.Permission.BluetoothAdmin;
                //    if (CheckSelfPermission(bl) == (int)Android.Content.PM.Permission.Granted)
                //    {
                //        Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
                //        PrintData();
                //        return;
                //    }
                //    else
                //    {
                //        listPermissions.Add(Android.Manifest.Permission.BluetoothAdmin);
                //    }

                //    RequestPermissions(listPermissions.ToArray(), PERMISSION_REQUEST_CODE);
                //}
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }


        //private void PrintData()
        //{
        //    Connection connection = new BluetoothConnection("macAddress");
        //    try
        //    {
        //        connection.Open();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}