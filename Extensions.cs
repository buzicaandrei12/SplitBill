using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitBill
{
    public static class Extensions
    {
        public static string NormalizePercentage(this string value)
        {
            return value.Replace("%", "");
        }
    }
}