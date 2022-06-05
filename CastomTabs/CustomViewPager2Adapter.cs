using AndroidX.Lifecycle;
using AndroidX.ViewPager2.Adapter;

namespace MyAndroidBankController
{
    public partial class MainActivity
    {
        public class CustomViewPager2Adapter : FragmentStateAdapter
        {
            private AndroidX.Fragment.App.FragmentManager _fragmentManager;
            public CustomViewPager2Adapter(AndroidX.Fragment.App.FragmentManager fragmentManager, Lifecycle lifecycle) : base(fragmentManager, lifecycle)
            {
                _fragmentManager = fragmentManager;
            }
            public override int ItemCount => 3;
            private AndroidX.Fragment.App.Fragment fragment = new AndroidX.Fragment.App.Fragment();
            public override AndroidX.Fragment.App.Fragment CreateFragment(int position)
            {
                switch (position)
                {
                    case 0:
                        fragment = new ViewPage2Fragment(position, DatesRepositorio.GetPayments(DatesRepositorio.DataItems));
                        break;
                    case 1:
                        fragment = new ViewPage2Fragment(position, DatesRepositorio.GetDeposits(DatesRepositorio.DataItems));
                        break;
                    case 2:
                        fragment = new ViewPage2Fragment(position, DatesRepositorio.GetCashs(DatesRepositorio.DataItems));
                        break;
                }
                return fragment;
            }
            public void AddNewItemToFragment(int position)
            {
                //var ft = (ViewPage2Fragment)_fragmentManager.Fragments[position];
                //switch (position)
                //{
                //    case 0:
                //        ft.ListData.AddRange(DatesRepositorio.GetPayments(DatesRepositorio.NewDataItems));
                //        break;
                //    case 1A
                //        ft.ListData.AddRange(DatesRepositorio.GetDeposits(DatesRepositorio.NewDataItems));
                //        break;
                //    case 2:
                //        ft.ListData.AddRange(DatesRepositorio.GetCashs(DatesRepositorio.NewDataItems));
                //        break;
                //}

            }
        }

    }

}

