using System;

namespace Trains
{
    [Serializable]
    public class Cargo
    {
        public int Passengers { get; set; } = 10;
        public int Mail { get; set; } = 20;
        public Freight Freight { get; set; } = new(30);
    }
}
