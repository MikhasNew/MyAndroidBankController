using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Tabs;
using MyAndroidBankController.Configs.ManagerCore;
using MyAndroidBankController.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyAndroidBankController
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public partial class MainActivity : AppCompatActivity
    {
        Stopwatch timer = new Stopwatch();
       

        // private static List<string> fragmentTitles;
        private static int[] tabIcons;
        // private static List<DataAdapter> dataAdapters;

        TabLayout tabLayout;
        ViewPager2 pager;
        CustomViewPager2Adapter adapter;
        private static List<BankConfiguration> smsFilters = new List<BankConfiguration>();

        #region Stock
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            timer.Start();
            #region Stock
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClickAsync;
            #endregion

            #region ConfigManager
            ConfigurationManager configManager = ConfigurationManager.ConfigManager;
            var configuration = configManager.JSONConfiguration;
            #endregion

            //#region ShowDataFromDb
            //await DatesRepositorio.AddDatas();
            //#endregion

            #region Castom Tab
            // dataAdapters = new List<DataAdapter>();

            tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.InlineLabel = true;
            tabLayout.TabGravity = 0;
            //var layoutParams = tabLayout.LayoutParameters as LinearLayout.LayoutParams;
            //layoutParams.Gravity = GravityFlags.Fill;
            //layoutParams.Width = LinearLayout.LayoutParams.MatchParent;
            //tabLayout.LayoutParameters = layoutParams;

            pager = FindViewById<ViewPager2>(Resource.Id.pager);


            tabLayout.TabSelected += (object sender, TabLayout.TabSelectedEventArgs e) =>
            {
                var tab = e.Tab;
                var layout = tab.View;
                var layoutParams = layout.LayoutParameters as LinearLayout.LayoutParams;
                tab.SetTabLabelVisibility(TabLayout.TabLabelVisibilityLabeled);
                //layoutParams.Weight = 3f;
                layoutParams.Width = LinearLayout.LayoutParams.WrapContent;
                layout.LayoutParameters = layoutParams;
            };
            tabLayout.TabUnselected += (object sender, TabLayout.TabUnselectedEventArgs e) =>
            {
                var tab = e.Tab;
                var layout = tab.View;
                tab.SetTabLabelVisibility(TabLayout.TabLabelVisibilityUnlabeled);
                // layoutParams.Width = LinearLayout.LayoutParams.WrapContent;
            };


            adapter = new CustomViewPager2Adapter(this.SupportFragmentManager, this.Lifecycle);
            tabIcons = new int[]{
            Resource.Drawable.abc_ic_clear_material,
            Resource.Drawable.abc_ic_clear_material,
            Resource.Drawable.abc_ic_clear_material
            };
            pager.Adapter = adapter;

            new TabLayoutMediator(tabLayout, pager, new CustomStrategy()).Attach();
            adapter.NotifyDataSetChanged();
            //var fragment1 = (ViewPage2Fragment)SupportFragmentManager.FindFragmentById(0);
            //var kjh = fragment1.ListData;
            #endregion

            #region ReadSmS
            smsFilters.AddRange(configuration.Banks); //This operation took 5420
            Console.WriteLine($"This operation took {timer.ElapsedMilliseconds}");
            timer.Restart();
            List<Sms> lst = await GetAllSmsAsync(smsFilters);// This operation took 1356
            Console.WriteLine($"This operation took {timer.ElapsedMilliseconds}");
            timer.Restart();
            await ParseSmsToDbAsync(lst, configuration.Banks);//This operation took 56

            #endregion
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

        private async void FabOnClickAsync(object sender, EventArgs eventArgs)
        {
            //#region ConfigManager
            //ConfigurationManager configManager = ConfigurationManager.ConfigManager;
            //var configuration = configManager.JSONConfiguration;
            //#endregion

            //#region ReadSmS
            //smsFilters.AddRange(configuration.Banks);
            //List<Sms> lst = await GetAllSmsAsync(smsFilters);
            //#endregion
            //await ParseSmsToDbAsync(lst, configuration.Banks);
            //adapter.AddNewItemToFragment(0);

            //adapter.NotifyDataSetChanged();

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion

        #region SmS
        public async Task<List<Sms>> GetAllSmsAsync(List<BankConfiguration> adressFilter)
        {
            List<Sms> lstSms = new List<Sms>();
            Sms objSms = new Sms();
            Android.Net.Uri message = Android.Net.Uri.Parse("content://sms/");

            using (var c = ContentResolver.Query(message, null, null, null, null))
            {
                //StartManagingCursor(c);
                int totalSMS = c.Count;

                if (c.MoveToFirst())
                {
                    for (int i = 0; i < totalSMS; i++)
                    {
                        if (adressFilter.Select(x => x.SmsNumber).Contains(c.GetString(c.GetColumnIndexOrThrow("address"))))
                        {
                            objSms = new Sms();
                            objSms.setId(c.GetString(c.GetColumnIndexOrThrow("_id")));
                            objSms.setAddress(c.GetString(c.GetColumnIndexOrThrow("address")));
                            objSms.setMsg(c.GetString(c.GetColumnIndexOrThrow("body")));
                            objSms.setReadState(c.GetString(c.GetColumnIndex("read")));
                            objSms.setTime(c.GetString(c.GetColumnIndexOrThrow("date")));
                            if (c.GetString(c.GetColumnIndexOrThrow("type")).Contains("1"))
                            {
                                objSms.setFolderName("inbox");
                            }
                            else
                            {
                                objSms.setFolderName("sent");
                            }
                            lstSms.Add(objSms);
                        }
                        c.MoveToNext();
                    }
                }
                else
                {
                    lstSms.Add(new Sms());
                    return lstSms;
                }
                c.Close();
                return lstSms;
            }
        }
        public async Task ParseSmsToDbAsync(List<Sms> smsList, List<BankConfiguration> bankConfigurations)
        {
            Console.WriteLine($"This operation took {timer.ElapsedMilliseconds}");
            timer.Restart();
            Parser parserBelarusbank = new Parser(smsList, bankConfigurations);//This operation took 3558
            Console.WriteLine($"This operation took {timer.ElapsedMilliseconds}");
            timer.Restart();
            await DatesRepositorio.AddDatas(parserBelarusbank.Data);//This operation took 10825
            Console.WriteLine($"This operation took {timer.ElapsedMilliseconds}");
            timer.Restart();


        }

    }
    #endregion
}

