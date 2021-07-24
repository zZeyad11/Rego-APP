using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Gms;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Gms.Tasks;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api;
using Firebase.Auth;
using Firebase;
using Android.Content;
using MarcTron.Plugin;
using Android.Gms.Ads;
using Xamarin.Facebook;
using System.Collections.Generic;
using Plugin.FacebookClient;

namespace Rego_APP
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity , View.IOnClickListener
    {


        GoogleManager g;
        public static GoogleUser GoogleUser;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FacebookClientManager.Initialize(this);
            MobileAds.Initialize(ApplicationContext);
            FacebookSdk.SdkInitialize(this.ApplicationContext);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_maininvert);

            Intilize();


        }

        private void Intilize()
        {
            g = new GoogleManager();
            GoogleUser = new GoogleUser();
            FindViewById<ImageView>(Resource.Id.GoogleSignBtn).SetOnClickListener(this);
            FindViewById<ImageView>(Resource.Id.FacebookSignInBtn).SetOnClickListener(this);
           

        }

     

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

      
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async void OnClick(View v)
        {
            if (v.Id == Resource.Id.GoogleSignBtn)
            {
                g.Login(onLoginComplete, this);
            }
            else if(v.Id == Resource.Id.FacebookSignInBtn)
            {
              var t =   await CrossFacebookClient.Current.RequestUserDataAsync(new string[] { "email", "first_name", "gender", "last_name", "birthday" }, new string[] { "email", "user_birthday" });
                
            }

        }

        private void onLoginComplete(GoogleUser arg1, string arg2)
        {
            GoogleUser = arg1;
            StartActivity(new Intent(this, typeof(Second)));
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                FacebookClientManager.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == 1)
                {
                    GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                    GoogleManager.Instance.OnAuthCompleted(result);
                }
            }
            catch
            {

            }
        }


      
    }
}
