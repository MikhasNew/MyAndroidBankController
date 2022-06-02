using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager2.Widget;
using System.Collections.Generic;
using Google.Android.Material.Tabs;
using Google.Android.Material.Snackbar;
using Google.Android.Material.FloatingActionButton;
using Android.Provider;
using Android.Content;
using MyAndroidBankController.Parsers;
using MyAndroidBankController.Configs.ManagerCore;
using System.Linq;
using Android.Widget;

namespace MyAndroidBankController
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public partial class MainActivity : AppCompatActivity
    {


        private static List<string> fragmentTitles;
        private static int[] tabIcons;
        // private static List<DataAdapter> dataAdapters;

        TabLayout tabLayout;
        ViewPager2 pager;
        private static List<BankConfiguration> smsFilters = new List<BankConfiguration>();

        #region Stock
        protected override void OnCreate(Bundle savedInstanceState)
        {
            #region Stock
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
            #endregion

            #region ConfigManager
            ConfigurationManager configManager = ConfigurationManager.ConfigManager;
            var configuration = configManager.JSONConfiguration;
            #endregion

            #region ReadSmS
            smsFilters.AddRange(configuration.Banks);
            List<Sms> lst = GetAllSms(smsFilters);
            ParseSmsToDb(lst, configuration.Banks);
            #endregion

            #region Castom Tab
           // dataAdapters = new List<DataAdapter>();

            tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.InlineLabel = true;
            var layoutParams = tabLayout.LayoutParameters as LinearLayout.LayoutParams;
            layoutParams.Gravity = GravityFlags.Start;
            layoutParams.Width = LinearLayout.LayoutParams.MatchParent;
            tabLayout.LayoutParameters = layoutParams;

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
                layoutParams.Width = LinearLayout.LayoutParams.WrapContent;
            };


                CustomViewPager2Adapter adapter = new CustomViewPager2Adapter(this.SupportFragmentManager, this.Lifecycle);
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

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion

        #region SmS
        public List<Sms> GetAllSms(List<BankConfiguration> adressFilter)
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
                        if (adressFilter.Select(x=>x.SmsNumber).Contains(c.GetString(c.GetColumnIndexOrThrow("address"))))
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
        public static void ParseSmsToDb(List<Sms> smsList, List<BankConfiguration> bankConfigurations)
        {
            Parser parserBelarusbank = new Parser(smsList, bankConfigurations);
            DatesRepositorio.AddDatas(parserBelarusbank.Data);
        }

    }
    #endregion
}

