using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App.User.LocationInfo.Services;
using Java.Util;
using MarcTron.Plugin;
using Plugin.Geolocator;
using Xamarin.Essentials;

namespace Rego_APP
{
    [Activity(Label = "Second")]
    public class Second : Activity, View.IOnClickListener
    {
        public static LocationInfo Address;
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.CameraImage || v.Id == Resource.Id.CameraText) //Go To Next
            {
                if (string.IsNullOrEmpty(Address.address))
                {
                    GetLocation();
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Location Is not Available");
                    alert.SetMessage("Please Wait till We Know Your location");
                    alert.SetCancelable(false);

                    alert.SetPositiveButton("Ok", (senderAlert, args) =>
                    {
                        
                    });
                    Dialog dialog = alert.Create();
                    dialog.SetCancelable(false);
                    dialog.Show();
                }
                else
                {
                    StartActivity(new Intent(this, typeof(Submitting)));
                }
            }

            else if(v.Id == Resource.Id.AdsImage || v.Id == Resource.Id.AdsText) //Watch Ad
            {
                CrossMTAdmob.Current.ShowRewardedVideo();
            }
        }
        public bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable && CrossGeolocator.Current.IsGeolocationEnabled;
            
        }

        public void OpenSettings()
        {
            LocationManager LM = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);


            if (LM.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.MultipleTask);
                StartActivityForResult(intent,36);
            }
            else
            {
                //this is handled in the PCL
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SecondActivity);
            
            CrossMTAdmob.Current.LoadRewardedVideo("ca-app-pub-7480570357579844/1223919627");
            CrossMTAdmob.Current.ShowRewardedVideo();

            GetLocation();
            StartInilize();
            // Create your application here
        }

        public LocationInfo GetAddress(Context context, double LATITUDE, double LONGITUDE)
        {
            LocationInfo a = new LocationInfo();
            //Set Address
            try
            {
                Geocoder geocoder = new Geocoder(context, Java.Util.Locale.GetDefault(Java.Util.Locale.Category.Display));
                var addresses = geocoder.GetFromLocation(LATITUDE, LONGITUDE, 1);
                if (addresses != null && addresses.Count > 0)
                {



                    a.address = addresses[0].GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                    a.city = addresses[0].Locality;
                    a.state = addresses[0].AdminArea;
                    a.country = addresses[0].CountryName;
                    a.LATITUDE = LATITUDE;
                    a.LONGITUDE = LONGITUDE;
                    
                    
                   

                }
            }
            catch 
            {
             
            }
            return a;
        }

      
    

        private async void GetLocation()
        {
            
           try
            {
                if (!IsLocationAvailable())
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Location Is not Available");
                    alert.SetMessage("Please Turn On Location");
                    alert.SetCancelable(false);

                    alert.SetPositiveButton("Ok", (senderAlert, args) =>
                    {
                        OpenSettings();
                    });

                    alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                    {
                        this.Finish();
                    });

                    Dialog dialog = alert.Create();
                    dialog.SetCancelable(false);
                    dialog.Show();
                    
                    
                }
                var location = await Geolocation.GetLastKnownLocationAsync();
                Address = GetAddress(this, location.Latitude, location.Longitude);
                FindViewById<TextView>(Resource.Id.LocationText).Text = $"{Address.country}, {Address.state}, {Address.city}";

            }
            catch
            {
                try
                {
                    //var UserInfo = await TrackingService.GetBasicLocatioInfoAsync();
                    //Address = new LocationInfo() { LATITUDE = UserInfo.Location.Latitude, LONGITUDE = UserInfo.Location.Longitude, city = UserInfo.City, country = UserInfo.CountryName, state = UserInfo.Region };
                    //FindViewById<TextView>(Resource.Id.LocationText).Text = $"{Address.country}, {Address.state}, {Address.city}";

                    FindViewById<TextView>(Resource.Id.LocationText).Text = "N/A";
                }
                catch
                {
                    FindViewById<TextView>(Resource.Id.LocationText).Text = "N/A";
                }
               

            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if(requestCode == 36)
            {
                GetLocation();
            }
        }

        private void StartInilize()
        {
            FindViewById<ImageView>(Resource.Id.CameraImage).SetOnClickListener(this);
            FindViewById<ImageView>(Resource.Id.CameraText).SetOnClickListener(this);
            FindViewById<ImageView>(Resource.Id.AdsImage).SetOnClickListener(this);
            FindViewById<ImageView>(Resource.Id.AdsText).SetOnClickListener(this);
        }
    }
}