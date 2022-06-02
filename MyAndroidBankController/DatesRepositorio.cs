using System;
using System.Linq;
using System.Collections.Generic;

namespace MyAndroidBankController
{
    public static class DatesRepositorio
    {
        public static List<DataItem> DataItems { get; private set; }
        public static void AddDatas(List<DataItem> dataItems)
        {
            DataItems = dataItems;
        }
        public static List<DataItem> GetPayments()
        {
            var payments = DataItems.Where(x => x.OperacionTyp == OperacionTyps.OPLATA).ToList();
           return DataItems.Where(x => x.OperacionTyp == OperacionTyps.OPLATA).ToList();
        }

        public static List<DataItem> GetDeposits()
        {
            return DataItems.Where(x => x.OperacionTyp == OperacionTyps.ZACHISLENIE).ToList();
        }
        public static List<DataItem> GetCashs()
        {
            return DataItems.Where(x => x.OperacionTyp == OperacionTyps.NALICHNYE).ToList();
        }

    }
}