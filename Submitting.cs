using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Firebase.Database;
using Firebase.Database.Query;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Android.Provider;
using System.Net.Mail;
using System.Net;
using Java.Nio;
using System.Threading.Tasks;

namespace Rego_APP
{
    [Activity(Label = "Submitting")]
    public class Submitting : Activity, View.IOnClickListener , IDialogInterfaceOnClickListener
    {
        Bitmap CurrentImage;
        int[] Faces = new int[] { Resource.Id.Smileface1 , Resource.Id.Smileface2 , Resource.Id.Smileface3 , Resource.Id.Smileface4 , Resource.Id.Smileface5};
        public void OnClick(View v)
        {
            if (Faces.Contains<int>(v.Id))
            {
                SmallFaces();
                FindViewById<ImageView>(v.Id).SetPadding(0, 0, 0, 0);
            }
            else if(v.Id == Resource.Id.CameraSelect)
            {
                AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(this);
                //Set title of Alert Dialog    
                alertDialogBuilder.SetTitle("Grap an Image");
                //Set dialog message    
                alertDialogBuilder.SetMessage("How Would you Like to grap your picture");
                alertDialogBuilder.SetCancelable(false);
                alertDialogBuilder.SetPositiveButton("Live Camera" , this);
                alertDialogBuilder.SetNegativeButton("From Gallery" , this);
                AlertDialog alertDialog = alertDialogBuilder.Create();
                alertDialog.Show();



            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == 54) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                FindViewById<ImageView>(Resource.Id.CameraSelect).SetImageURI(uri);
                FindViewById<ImageView>(Resource.Id.CameraSelect).SetPadding(0, 0, 0, 0);
                CurrentImage = GetBitmap(uri);
            }
            if ((requestCode == 14) && (data != null))
            {
                CurrentImage = (Bitmap)data.Extras.Get("data");
                FindViewById<ImageView>(Resource.Id.CameraSelect).SetImageBitmap(CurrentImage);
                FindViewById<ImageView>(Resource.Id.CameraSelect).SetPadding(0, 0, 0, 0);


            }


            }


        void SendEmail(string to , string name , string trackID)
        {
            var fromAddress = new MailAddress("dilipkumarthota815@gmail.com", "REGO App");
            var toAddress = new MailAddress(to, name);
            const string fromPassword = "1991dilipkumar";
            const string subject = "Submit Confirmation";
            string body = $"Hey, We Would Like to Let You Know That We Got Your Submit, Thanks for your Contribuation \n ID:{trackID}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        private Android.Graphics.Bitmap GetBitmap(Android.Net.Uri uriImage)
        {
            Android.Graphics.Bitmap mBitmap = null;
            mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.ContentResolver, uriImage);
            return mBitmap;
        }

        byte[] GetImageBytes(Android.Graphics.Bitmap bitmap)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
                byte[] bitmapData = stream.ToArray();
                return bitmapData;
            }
            catch
            {
                return null;
            }
        }

        int GetRate()
        {
            if (FindViewById<ImageView>(Resource.Id.Smileface1).PaddingEnd == 0)
            {
                return 1;
            }else if (FindViewById<ImageView>(Resource.Id.Smileface2).PaddingEnd == 0)
            {
                return 2;
            }else if (FindViewById<ImageView>(Resource.Id.Smileface3).PaddingEnd == 0)
            {
                return 2;
            }else if (FindViewById<ImageView>(Resource.Id.Smileface4).PaddingEnd == 0)
            {
                return 3;
            }else if (FindViewById<ImageView>(Resource.Id.Smileface5).PaddingEnd == 0)
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Donate);
            SmallFaces();
            FindViewById<ImageView>(Resource.Id.Smileface5).SetPadding(0, 0, 0, 0);

            FindViewById<EditText>(Resource.Id.Comment).TextChanged += Submitting_TextChanged;
            // Create your application here
            SetDataIntoLayout();
        }

        private void Submitting_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            FindViewById<TextView>(Resource.Id.Limitlength).Text = $"{((TextView)sender).Text.ToCharArray().Length}/500";
        }

        private void SetDataIntoLayout()
        {
            FindViewById<FloatingActionButton>(Resource.Id.myFab).Click += Submitting_Click;
            FindViewById<TextView>(Resource.Id.CountryName).Text = "Country : " + Second.Address.country; 
            FindViewById<TextView>(Resource.Id.RegionName).Text = "Region : " + Second.Address.state; 
            FindViewById<TextView>(Resource.Id.CityName).Text = "City : " + Second.Address.city; 
            FindViewById<TextView>(Resource.Id.LocationDonate).Text = $"Latitude: {Second.Address.LATITUDE}\nLongitude: {Second.Address.LONGITUDE}";



            FindViewById<ImageView>(Resource.Id.Smileface1).SetOnClickListener(this);
            FindViewById<ImageView>(Resource.Id.Smileface2).SetOnClickListener(this);
            FindViewById<ImageView>(Resource.Id.Smileface3).SetOnClickListener(this);
            FindViewById<ImageView>(Resource.Id.Smileface4).SetOnClickListener(this);
            FindViewById<ImageView>(Resource.Id.Smileface5).SetOnClickListener(this);
            FindViewById<ImageView>(Resource.Id.CameraSelect).SetOnClickListener(this);

        }

        static FirebaseClient firebase = new FirebaseClient("https://rego-af3bb-default-rtdb.firebaseio.com/");

        private async void Submitting_Click(object sender, EventArgs e)
        {
            try
            {
                ((FloatingActionButton)sender).Enabled = false;
                var s = (DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + (new Random()).Next(0, 84898412)).ToString();
                var d = GetData(s);
                await firebase.Child(s).PutAsync<Data>(d);
                try
                {
                    SendEmail(d.Email, d.Name,s);
                    Snackbar.Make(FindViewById(Resource.Id.coordinatorLayout1), "You Will Get Noticed by an e-Mail Soon, Got your Request so Thanks :)", Snackbar.LengthLong).Show();
                    await Task.Delay(1000);
                    this.Finish();

                }
                catch
                {
                    Snackbar.Make(FindViewById(Resource.Id.coordinatorLayout1), "Error While Sending e-Mail To You but still Got your Request so Thanks :)", Snackbar.LengthLong).Show();
                    await Task.Delay(8000);
                    this.Finish();
                }
            }
            catch (Exception ex)
            {
                Snackbar.Make(FindViewById(Resource.Id.coordinatorLayout1), "Error", Snackbar.LengthLong).Show();
            }
        }


        Data GetData(string s)
        {
            return new Data() {ID =s ,Name = MainActivity.GoogleUser.Name, Email = MainActivity.GoogleUser.Email, Image = GetImageBytes(CurrentImage), Address = Second.Address , Rating = GetRate() , Comment = FindViewById<EditText>(Resource.Id.Comment).Text , Date = DateTime.Now};
        }

        
            void SmallFaces()
        {
            int pad = 15;
            FindViewById<ImageView>(Resource.Id.Smileface1).SetPadding(pad, pad, pad, pad);
            FindViewById<ImageView>(Resource.Id.Smileface2).SetPadding(pad, pad, pad, pad);
            FindViewById<ImageView>(Resource.Id.Smileface3).SetPadding(pad, pad, pad, pad);
            FindViewById<ImageView>(Resource.Id.Smileface4).SetPadding(pad, pad, pad, pad);
            FindViewById<ImageView>(Resource.Id.Smileface5).SetPadding(pad, pad, pad, pad);
            
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            if (which == -2)
            {
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), 54);
            }
            if (which == -1)
            {
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(intent, 14);
            }
        }
    }
}