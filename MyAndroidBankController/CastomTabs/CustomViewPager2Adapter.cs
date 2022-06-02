using AndroidX.ViewPager2.Adapter;
using AndroidX.Lifecycle;

namespace MyAndroidBankController
{
    public partial class MainActivity
    {
        public class CustomViewPager2Adapter : FragmentStateAdapter
        {
            public CustomViewPager2Adapter(AndroidX.Fragment.App.FragmentManager fragmentManager, Lifecycle lifecycle) : base(fragmentManager, lifecycle)
            {
            }
            public override int ItemCount => 3;
            private AndroidX.Fragment.App.Fragment fragment = new AndroidX.Fragment.App.Fragment();
            public override AndroidX.Fragment.App.Fragment CreateFragment(int position)
            {
                switch (position)
                {
                    case 0:
                        fragment = new ViewPage2Fragment(position, DatesRepositorio.GetPayments());
                        break;
                    case 1:
                        fragment = new ViewPage2Fragment(position, DatesRepositorio.GetDeposits());
                        break;
                    case 2:
                        fragment = new ViewPage2Fragment(position, DatesRepositorio.GetCashs());
                        break;
                }
                return fragment;
            }
        }

    }

}

