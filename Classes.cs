using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Rego_APP
{
    [Serializable]
    class Data
    {
        public string Name;
        public string Email;
        public string Comment;
        public byte[] Image;
        public int Rating;
        public DateTime Date;

        public LocationInfo Address;
    }
    [Serializable]
    public class LocationInfo
    {
        public String address; // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
        public String city;
        public String state;
        public String country;
        public double LATITUDE;
        public double LONGITUDE;


        public LocationInfo(Object a = null)
        {
            address = "";
            city = "";
            state = "";
            country = "";
            LATITUDE = 0;
            LONGITUDE = 0;
        }
    }
}