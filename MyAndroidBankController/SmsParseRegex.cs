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

namespace MyAndroidBankController
{
    public class SmsParseRegex
    {
       public string OperacionTyp { get; set; }
       public string Sum          { get; set; }
       public string Karta        { get; set; }
       public string Date         { get; set; }
       public string Descripton   { get; set; }
       public string Balance      { get; set; }


    }
}