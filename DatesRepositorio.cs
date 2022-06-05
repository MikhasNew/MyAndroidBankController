using EfcToXamarinAndroid.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyAndroidBankController
{
    public static class DatesRepositorio
    {
        public static List<DataItem> DataItems { get; private set; }
        public static int NewDataItemsCount { get; private set; }
        //public static List<DataItem> NewDataItems { get; private set; }
        private static readonly string dbFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private static readonly string fileName = "Cats.db";
        private static readonly string dbFullPath = Path.Combine(dbFolder, fileName);
        public static async Task AddDatas(List<DataItem> dataItems)
        {
            var newDataItems = new List<DataItem>();

            try
            {
                using (var db = new DataItemContext(dbFullPath))
                {
                     await db.Database.MigrateAsync(); //We need to ensure the latest Migration was added. This is different than EnsureDatabaseCreated.
                    DataItems = await db.Cats.ToListAsync();

                    if (DataItems.Count > 0)
                    {
                        foreach (var item in dataItems)
                        {
                            if (!DataItems.Any(x => x.HashId == item.HashId))
                                newDataItems.Add(item);
                        }
                    }
                    else
                        newDataItems = dataItems;
                    if (newDataItems.Count > 0)
                    {
                        NewDataItemsCount = newDataItems.Count;
                        DataItems.AddRange(newDataItems);
                        // await db.Cats.AddRangeAsync(newDataItems);
                        // await db.SaveChangesAsync();

                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public static async Task AddNewDatas(List<DataItem> dataItems)
        {
            var newDataItems = new List<DataItem>();

            try
            {
                if (DataItems.Count > 0)
                {
                    foreach (var item in dataItems)
                    {
                        if (!DataItems.Any(x => x.HashId == item.HashId))
                            newDataItems.Add(item);
                    }
                }
                else
                    newDataItems = dataItems;

                using (var db = new DataItemContext(dbFullPath))
                {
                    await db.Database.MigrateAsync(); //We need to ensure the latest Migration was added. This is different than EnsureDatabaseCreated.

                    if (newDataItems.Count > 0)
                    {
                        // await db.Cats.AddRangeAsync(newDataItems);
                        // await db.SaveChangesAsync();
                        NewDataItemsCount = newDataItems.Count;
                        DataItems.AddRange(newDataItems);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }



        public static List<DataItem> GetPayments(List<DataItem> dataItems)
        {
            return dataItems.Where(x => x.OperacionTyp == OperacionTyps.OPLATA).ToList();
        }
        public static List<DataItem> GetDeposits(List<DataItem> dataItems)
        {
            return dataItems.Where(x => x.OperacionTyp == OperacionTyps.ZACHISLENIE).ToList();
        }
        public static List<DataItem> GetCashs(List<DataItem> dataItems)
        {
            return dataItems.Where(x => x.OperacionTyp == OperacionTyps.NALICHNYE).ToList();
        }

    }
}