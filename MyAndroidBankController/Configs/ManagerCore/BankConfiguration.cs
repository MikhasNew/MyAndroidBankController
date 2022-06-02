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
    public class BankConfiguration
    {
        public string Name { get; set; }
        public string SmsNumber { get; set; }
        public SmsParseRegex SmsParseRegex { get; set; }
    }
}
