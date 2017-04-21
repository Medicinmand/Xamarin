﻿using SQLite;
using System;
using System.Diagnostics;
using VVS.Database;
using VVS.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace VVS.Layout
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewMeter : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Replacement _job;
        //use 1 for old meter, 2 for new meter.
        private int oldOrNew = 0;
        private Meter oldMeter;        

        public NewMeter(Replacement currentJob, int oldOrNew)
        {
            if (currentJob == null)
                throw new ArgumentNullException(nameof(currentJob));
            InitializeComponent();
            _job = currentJob;
            this.oldOrNew = oldOrNew;
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            if(oldOrNew == 1)
            {
                
            }
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            var scanPage = new ZXingScannerPage();
            await Navigation.PushAsync(scanPage);

            scanPage.OnScanResult += (result) =>
            {
                // Stop scanning
                scanPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync();
                    await DisplayAlert("Scanned Barcode", result.Text, "OK");
                    if (String.IsNullOrWhiteSpace(BarcodeField.Text))
                    {
                        BarcodeField.Text = result.Text;
                        BarcodeField.IsEnabled = false;                        
                    }
                    else
                    {
                        BarcodeField2.Text = result.Text;
                        BarcodeField2.IsEnabled = false;
                    }
                });
            };
        }

        private async void Onsave(object sender, EventArgs e)
        {
            // Validate serialnumber
            string serialNo1Raw = BarcodeField.Text;
            string serialNo2Raw = BarcodeField2.Text;
            string enterSN = "Vær venlig at indtast serienumre i begge felter";
            CheckNullorWhiteSpace(serialNo1Raw, enterSN);
            serialNo1Raw = serialNo1Raw.ToLower().Trim();
            CheckNullorWhiteSpace(serialNo2Raw, enterSN);
            serialNo2Raw = serialNo2Raw.ToLower().Trim();

            int serialNo1 = -1;
            Int32.TryParse(serialNo1Raw, out serialNo1);
            int serialNo2 = -1;
            Int32.TryParse(serialNo2Raw, out serialNo2);

            if (serialNo2 != serialNo1)
            {
                await DisplayAlert("Error", "Serienumre skal være identiske", "OK");
                return;
            }
            //validate Consumption
            string consumption1Str = Consumption.Text;
            string consumption2Str = Consumption2.Text;
            string enterConsumption = "Vær venlig at indtast forbrug i begge felter";
            CheckNullorWhiteSpace(consumption1Str, enterConsumption);
            consumption1Str = consumption1Str.ToLower().Trim();
            CheckNullorWhiteSpace(consumption2Str, enterConsumption);
            consumption2Str = consumption2Str.ToLower().Trim();

            int consumption1 = -1;
            Int32.TryParse(consumption1Str, out consumption1);
            int consumption2 = -1;
            Int32.TryParse(consumption2Str, out consumption2);

            if (consumption1 != consumption2)
            {
                await DisplayAlert("Error", "Forbrugs tallene skal være identiske", "OK");
                return;
            }

            string comment = Comment.Text;
            
            //TODO add picture path from camera.

            //construct Meter
            var newMeterData = new Meter(serialNo1, consumption1, "picPath", comment);

            //save meter to DB and update Job.
            try
            {
                await _connection.InsertAsync(newMeterData);

                _job.Status =4;
                _job.NewMeterId = newMeterData.SerialNumber;
                await _connection.UpdateAsync(_job);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRRORR" + ex.Message);
            }

            await Navigation.PopAsync();        
        }

        private async void CheckNullorWhiteSpace(string text, string errorMessage)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                await DisplayAlert("Error", errorMessage, "OK");
                return;
            }            
        }
    }
}
