using System;

namespace Trains
{
    [Serializable]
    public class Freight
    {
        public int WoodAmnt { get; set; }
        public Freight(int woodAmnt)
        {
            WoodAmnt = woodAmnt;
        }
    }
}