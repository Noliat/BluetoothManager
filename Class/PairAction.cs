﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Devices.Enumeration;
using Windows.Security.Credentials;
using Windows.UI.Core;

namespace BluetoothManager.Class
{
    internal class PairAction
    {
        private async void PairingRequestedHandler(
            DeviceInformationCustomPairing sender,
            DevicePairingRequestedEventArgs args)
        {
            switch (args.PairingKind)
            {
                case DevicePairingKinds.ConfirmOnly:
                    // Windows itself will pop the confirmation dialog as part of "consent" if this is running on Desktop or Mobile
                    // If this is an App for 'Windows IoT Core' where there is no Windows Consent UX, you may want to provide your own confirmation.
                    args.Accept();
                    break;

                case DevicePairingKinds.DisplayPin:
                    // We just show the PIN on this side. The ceremony is actually completed when the user enters the PIN
                    // on the target device. We automatically accept here since we can't really "cancel" the operation
                    // from this side.
                    args.Accept();

                    // No need for a deferral since we don't need any decision from the user
                    await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ShowPairingPanel(
                            "Please enter this PIN on the device you are pairing with: " + args.Pin,
                            args.PairingKind);

                    });
                    break;

                case DevicePairingKinds.ProvidePin:
                    // A PIN may be shown on the target device and the user needs to enter the matching PIN on
                    // this Windows device. Get a deferral so we can perform the async request to the user.
                    var collectPinDeferral = args.GetDeferral();

                    await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        string pin = await GetPinFromUserAsync();
                        if (!string.IsNullOrEmpty(pin))
                        {
                            args.Accept(pin);
                        }

                        collectPinDeferral.Complete();
                    });
                    break;

                case DevicePairingKinds.ProvidePasswordCredential:
                    var collectCredentialDeferral = args.GetDeferral();
                    await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        var credential = await GetPasswordCredentialFromUserAsync();
                        if (credential != null)
                        {
                            args.AcceptWithPasswordCredential(credential);
                        }
                        collectCredentialDeferral.Complete();
                    });
                    break;

                case DevicePairingKinds.ConfirmPinMatch:
                    // We show the PIN here and the user responds with whether the PIN matches what they see
                    // on the target device. Response comes back and we set it on the PinComparePairingRequestedData
                    // then complete the deferral.
                    var displayMessageDeferral = args.GetDeferral();

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        bool accept = await GetUserConfirmationAsync(args.Pin);
                        if (accept)
                        {
                            args.Accept();
                        }

                        displayMessageDeferral.Complete();
                    });
                    break;
            }
        }

        private void ShowPairingPanel(string text, DevicePairingKinds pairingKind)
        {
            pairingPanel.Visibility = Visibility.Collapsed;
            pinEntryTextBox.Visibility = Visibility.Collapsed;
            okButton.Visibility = Visibility.Collapsed;
            usernameEntryTextBox.Visibility = Visibility.Collapsed;
            passwordEntryTextBox.Visibility = Visibility.Collapsed;
            verifyButton.Visibility = Visibility.Collapsed;
            yesButton.Visibility = Visibility.Collapsed;
            noButton.Visibility = Visibility.Collapsed;
            pairingTextBlock.Text = text;

            switch (pairingKind)
            {
                case DevicePairingKinds.ConfirmOnly:
                case DevicePairingKinds.DisplayPin:
                    // Don't need any buttons
                    break;
                case DevicePairingKinds.ProvidePin:
                    pinEntryTextBox.Text = "";
                    pinEntryTextBox.Visibility = Visibility.Visible;
                    okButton.Visibility = Visibility.Visible;
                    break;
                case DevicePairingKinds.ConfirmPinMatch:
                    yesButton.Visibility = Visibility.Visible;
                    noButton.Visibility = Visibility.Visible;
                    break;
                case DevicePairingKinds.ProvidePasswordCredential:
                    usernameEntryTextBox.Text = "";
                    passwordEntryTextBox.Text = "";
                    passwordEntryTextBox.Visibility = Visibility.Visible;
                    usernameEntryTextBox.Visibility = Visibility.Visible;
                    verifyButton.Visibility = Visibility.Visible;
                    break;
            }

            pairingPanel.Visibility = Visibility.Visible;
        }

        private void HidePairingPanel()
        {
            pairingPanel.Visibility = Visibility.Collapsed;
            pairingTextBlock.Text = "";
        }

        private async Task<string> GetPinFromUserAsync()
        {
            HidePairingPanel();
            CompleteProvidePinTask(); // Abandon any previous pin request.

            ShowPairingPanel(
                "Please enter the PIN shown on the device you're pairing with",
                DevicePairingKinds.ProvidePin);

            providePinTaskSrc = new TaskCompletionSource<string>();

            return await providePinTaskSrc.Task;
        }

        // If pin is not provided, then any pending pairing request is abandoned.
        private void CompleteProvidePinTask(string pin = null)
        {
            if (providePinTaskSrc != null)
            {
                providePinTaskSrc.SetResult(pin);
                providePinTaskSrc = null;
            }
        }

        private async Task<PasswordCredential> GetPasswordCredentialFromUserAsync()
        {
            HidePairingPanel();
            CompletePasswordCredential(); // Abandon any previous pin request.

            ShowPairingPanel(
                "Please enter the username and password",
                DevicePairingKinds.ProvidePasswordCredential);

            providePasswordCredential = new TaskCompletionSource<PasswordCredential>();

            return await providePasswordCredential.Task;
        }

        private void CompletePasswordCredential(string username = null, string password = null)
        {
            if (providePasswordCredential != null)
            {
                if (String.IsNullOrEmpty(username))
                {
                    providePasswordCredential.SetResult(null);
                }
                else
                {
                    providePasswordCredential.SetResult(new PasswordCredential() { UserName = username, Password = password });
                }
                providePasswordCredential = null;
            }
        }

        private async Task<bool> GetUserConfirmationAsync(string pin)
        {
            HidePairingPanel();
            CompleteConfirmPinTask(false); // Abandon any previous request.

            ShowPairingPanel(
                "Does the following PIN match the one shown on the device you are pairing?: " + pin,
                DevicePairingKinds.ConfirmPinMatch);

            confirmPinTaskSrc = new TaskCompletionSource<bool>();

            return await confirmPinTaskSrc.Task;
        }

        // If pin is not provided, then any pending pairing request is abandoned.
        private void CompleteConfirmPinTask(bool accept)
        {
            if (confirmPinTaskSrc != null)
            {
                confirmPinTaskSrc.SetResult(accept);
                confirmPinTaskSrc = null;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            // OK button is only used for the ProvidePin scenario
            CompleteProvidePinTask(pinEntryTextBox.Text);
            HidePairingPanel();
        }

        private void verifyButton_Click(object sender, RoutedEventArgs e)
        {
            // verify button is only used for the ProvidePin scenario
            CompletePasswordCredential(usernameEntryTextBox.Text, passwordEntryTextBox.Text);
            HidePairingPanel();
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            CompleteConfirmPinTask(true);
            HidePairingPanel();
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            CompleteConfirmPinTask(false);
            HidePairingPanel();
        }

        private DevicePairingKinds GetSelectedCeremonies()
        {
            DevicePairingKinds ceremonySelection = DevicePairingKinds.None;
            if (confirmOnlyOption.IsChecked.Value) ceremonySelection |= DevicePairingKinds.ConfirmOnly;
            if (displayPinOption.IsChecked.Value) ceremonySelection |= DevicePairingKinds.DisplayPin;
            if (providePinOption.IsChecked.Value) ceremonySelection |= DevicePairingKinds.ProvidePin;
            if (confirmPinMatchOption.IsChecked.Value) ceremonySelection |= DevicePairingKinds.ConfirmPinMatch;
            if (passwordCredentialOption.IsChecked.Value) ceremonySelection |= DevicePairingKinds.ProvidePasswordCredential;
            return ceremonySelection;
        }
    }
}
