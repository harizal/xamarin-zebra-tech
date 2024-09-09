using Android.Bluetooth;
using Android.Content;
using Android.Widget;
using System.Collections.Generic;

namespace xamarin_zebra_tech
{
    public class BluetoothReceiver : BroadcastReceiver
    {
        public BluetoothReceiver()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;
            if (BluetoothDevice.ActionFound.Equals(action))
            {
                var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                var deviceName = device.Name;
                var deviceAddress = device.Address;

                if (!string.IsNullOrEmpty(deviceName) && deviceName.Contains("Zebra"))
                {
                    // Add device to list
                    (context as MainActivity)?._deviceList.Add($"{deviceName} ({deviceAddress})");
                    Toast.MakeText(context, $"Found: {deviceName} ({deviceAddress})", ToastLength.Short).Show();
                }
            }
            else if (BluetoothAdapter.ActionDiscoveryStarted.Equals(action))
            {
                Toast.MakeText(context, "Bluetooth discovery started", ToastLength.Short).Show();
            }
            else if (BluetoothAdapter.ActionDiscoveryFinished.Equals(action))
            {
                Toast.MakeText(context, "Bluetooth discovery finished", ToastLength.Short).Show();
                (context as MainActivity)?.ShowDeviceSelectionDialog();
            }
        }
    }
}