using System;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public class Cargo
    {
        public static Cargo Empty => new() { PassengersAmnt = 0, MailAmnt = 0, Freight = new Freight(0) };

        public static int PassengersMaxAmnt => 15;
        public static int MailMaxAmnt => 20;
        public static int WoodMaxAmnt => 50;

        [field: SerializeField] public int PassengersAmnt { get; set; } = 10;
        [field: SerializeField] public int MailAmnt { get; set; } = 20;
        [field: SerializeField] public Freight Freight { get; set; } = new(30);

        public void Add(Cargo toAdd)
        {
            PassengersAmnt += toAdd.PassengersAmnt;
            MailAmnt += toAdd.MailAmnt;
            Freight.WoodAmnt += toAdd.Freight.WoodAmnt;
        }

        public void Erase()
        {
            PassengersAmnt = 0;
            MailAmnt = 0;
            Freight.WoodAmnt = 0;
        }

        public decimal GetWorthValue()
        {
            decimal worth = 0;
            worth += Prices.PassengerPrice * PassengersAmnt;
            worth += Prices.MailPrice * MailAmnt;
            worth += Prices.Wood * Freight.WoodAmnt;
            return worth;
        }
    }
}
