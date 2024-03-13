using UnityEngine;

namespace Trains
{
    public interface IFootCargoDestination
    {
        public Cargo Supply { get; set; }
        //public Cargo Consumption { get; set; }
        public Transform transform { get; }
        public void OnFootCargoCame(FootCargo footCargo);
    }

    public interface IProfitBuilding
    {
        public Cargo Supply { get; set; }
        public Cargo Demand { get; set; }
        public MeshRenderer Visual { get; }
        public GameObject gameObject { get; }

        //public void SendGoodsToStation(StationCargoHandler sch);
    }
}