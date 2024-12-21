using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using System.Windows.Media.Imaging;

using BluetoothManager.Class;
using Windows.UI.Core;

namespace BluetoothManager.Class
{
    class DeviceWatcherHelper
    {

        public DeviceWatcherHelper(
            ObservableCollection<DeviceInformationDisplay> devicesCollection,
            Dispatcher dispatcher)
        {
            this.devicesCollection = devicesCollection;
            this.dispatcher = dispatcher;
        }

        public delegate void DeviceChangedHandler(DeviceWatcher deviceWatcher, string id);
        public event DeviceChangedHandler DeviceChanged;

        public DeviceWatcher DeviceWatcher => deviceWatcher;
        public bool UpdateStatus = true;

        public void StartWatcher(DeviceWatcher deviceWatcher)
        {
            this.deviceWatcher = deviceWatcher;

            // Connect events to update our collection as the watcher report results.
            deviceWatcher.Added += Watcher_DeviceAdded;
            deviceWatcher.Updated += Watcher_DeviceUpdated;
            deviceWatcher.Removed += Watcher_DeviceRemoved;
            deviceWatcher.EnumerationCompleted += Watcher_EnumerationCompleted;
            deviceWatcher.Stopped += Watcher_Stopped;

            deviceWatcher.Start();
        }

        public void StopWatcher()
        {
            if (IsWatcherStarted(deviceWatcher))
            {
                deviceWatcher.Stop();
            }
        }

        public void Reset()
        {
            if (deviceWatcher != null)
            {
                StopWatcher();
                deviceWatcher = null;
            }
        }

        public DeviceWatcher deviceWatcher = null;
        public ObservableCollection<DeviceInformationDisplay> devicesCollection;
        public Dispatcher dispatcher;


        static bool IsWatcherStarted(DeviceWatcher watcher)
        {
            return (watcher.Status == DeviceWatcherStatus.Started) ||
                (watcher.Status == DeviceWatcherStatus.EnumerationCompleted);
        }

        public bool IsWatcherRunning()
        {
            if (deviceWatcher == null)
            {
                return false;
            }

            DeviceWatcherStatus status = deviceWatcher.Status;
            return (status == DeviceWatcherStatus.Started) ||
                (status == DeviceWatcherStatus.EnumerationCompleted) ||
                (status == DeviceWatcherStatus.Stopping);
        }

        public void Watcher_DeviceAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            dispatcher.Invoke(() =>
            {
                if (IsWatcherStarted(sender))
                {
                    devicesCollection.Add(new DeviceInformationDisplay(deviceInfo));
                    RaiseDeviceChanged(sender, deviceInfo.Id);
                }
            });
        }

        public void Watcher_DeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            dispatcher.Invoke(() =>
            {
                if (IsWatcherStarted(sender))
                {
                    foreach (DeviceInformationDisplay deviceInfoDisp in devicesCollection)
                    {
                        if (deviceInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            deviceInfoDisp.Update(deviceInfoUpdate);
                            RaiseDeviceChanged(sender, deviceInfoUpdate.Id);
                            break;
                        }
                    }
                }
            });
        }

        public void Watcher_DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            dispatcher.Invoke(() =>
            {
                if (IsWatcherStarted(sender))
                {
                    foreach (DeviceInformationDisplay deviceInfoDisp in devicesCollection)
                    {
                        if (deviceInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            devicesCollection.Remove(deviceInfoDisp);
                            break;
                        }
                    }

                    RaiseDeviceChanged(sender, deviceInfoUpdate.Id);
                }
            });
        }

        public void Watcher_EnumerationCompleted(DeviceWatcher sender, object obj)
        {
            dispatcher.Invoke(() =>
            {
                RaiseDeviceChanged(sender, string.Empty);
            });
        }

        public void Watcher_Stopped(DeviceWatcher sender, object obj)
        {
            dispatcher.Invoke(() =>
            {
                RaiseDeviceChanged(sender, string.Empty);
            });
        }

        public void RaiseDeviceChanged(DeviceWatcher sender, string id)
        {
            if (UpdateStatus)
            {
                //string message = string.Empty;
                switch (sender.Status)
                {
                    case DeviceWatcherStatus.Started:
                        //message = $"{Devices.Count} devices found.";
                        break;

                    case DeviceWatcherStatus.EnumerationCompleted:
                        //message = $"{Devices.Count} devices found. Enumeration completed. Watching for updates...";
                        break;

                    case DeviceWatcherStatus.Stopped:
                        //message = $"{Devices.Count} devices found. Watcher stopped.";
                        break;

                    case DeviceWatcherStatus.Aborted:
                        //message = $"{Devices.Count} devices found. Watcher aborted.";
                        break;
                }
            }

            DeviceChanged?.Invoke(sender, id);
        }
    }
}
