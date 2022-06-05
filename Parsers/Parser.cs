using EfcToXamarinAndroid.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyAndroidBankController.Parsers
{
    internal class Parser
    {
        public List<BankConfiguration> BankConfigurations { get; }
        public List<DataItem> Data { get; }
        List<Sms> smslist = new List<Sms>();

        public Parser(List<Sms> data, List<BankConfiguration> bankConfigurations)
        {
            BankConfigurations = bankConfigurations;
            Data = new List<DataItem>();
            smslist = data;
            ToDataItems();
        }
        public void ToDataItems()
        {
            // List<string> words = new List<string>();
            foreach (Sms sms in smslist)
            {
                var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.NumberDecimalSeparator = ".";

                var msg = sms.getMsg();

                //Regex wordsRegex = new Regex(BankConfigurations.Where(x=>x.SmsNumber==sms.getAddress()).First().SmsParseRegex);
                var BelarusbankSmsRegex = BankConfigurations.Where(x => x.SmsNumber == sms.getAddress()).First().SmsParseRegex;
                //var smsWords = wordsRegex.Matches(msg);

                OperacionTyps operType;
                //var testRegex = new Regex(@"^[A-Z]{1,}");
                //var testVal = testRegex.Match(msg); 
                if (Enum.TryParse<OperacionTyps>(new Regex(BelarusbankSmsRegex.OperacionTyp).Match(msg).Value.ToString(), out operType))
                {

                    string date = new Regex(BelarusbankSmsRegex.Date).Match(msg).Value;

                    if (date != "")
                    {
                        DateTime dateValue = DateTime.Parse(date);
                        DataItem dataItem = new DataItem(operType, (DateTime)dateValue);

                        dataItem.Sum = float.Parse(new Regex(BelarusbankSmsRegex.Sum).Match(msg).Value, ci);
                        dataItem.Balance = float.Parse(new Regex(BelarusbankSmsRegex.Balance).Match(msg).Value, ci);
                        dataItem.Karta = int.Parse(new Regex(BelarusbankSmsRegex.Karta).Match(msg).Value, ci);
                        dataItem.Descripton = new Regex(BelarusbankSmsRegex.Descripton).Match(msg).Value.ToUpper();
                        Data.Add(dataItem);
                    }
                }
            }
        }
    }
}