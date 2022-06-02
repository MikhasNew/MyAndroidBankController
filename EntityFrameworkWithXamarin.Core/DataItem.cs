using System;

namespace EntityFrameworkWithXamarin.Core
{

    public class DataItem
    {
        public int Id { get; set; }
        private long HashId { get; set; }
        private OperacionTyps OperacionTyps { get; set; }
        public float Balance { get; set; }
        public float Sum { get; set; }
        public string Title { get; set; }
        public string Descripton { get; set; }
        private DateTime Date { get; set; }
        public DataItem(OperacionTyps operacionTyps, DateTime dateTime )
        {
            //not overflov to 16/11/3169 09:46:40
            HashId =(long)((int)operacionTyps * 1000000000000000000)+ dateTime.Ticks;
        }
        public override string ToString()
        {
            return Title + " por " + Descripton;
        }


    }
}
