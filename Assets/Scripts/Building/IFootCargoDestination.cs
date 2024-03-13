using UnityEngine;

namespace Trains
{
    public interface IFootCargoDestination
    {
        public Cargo Supply { get; set; }
        public Transform transform { get; }
    }
}