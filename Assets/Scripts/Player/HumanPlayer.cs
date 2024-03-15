using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public enum PlayerState { None, BuildingRoads, BuildingStations }

    public interface IPlayer
    {
        public static EventHandler<PlayerEventArgs> OnMoneyBalanceChanged;
        public int Id { get; set; }
        public Color Color { get; set; }
        public decimal MoneyBalance { get; set; } 
        public decimal AddProfitForDeliveredCargo(decimal money);
    }

    public class PlayerEventArgs: EventArgs
    {
        public decimal MoneyBalance;
    }

    public class HumanPlayer : MonoBehaviour, IPlayer
    {
        public int Id { get; set; }
        public Color Color { get; set; } = Color.blue;
        public decimal MoneyBalance { get; set; } = 1000;

        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private Camera cam;

        private PlayerState state;

        private void Awake()
        {
            Id = GetInstanceID();
        }

        private void Start()
        {
            EventManager.Instance.OnBuildRailPressed += OnBuildRailPressed;
            //TODO temporary, probably stick main player to current camera
            Global.Instance.MainPlayer = this;

            state = PlayerState.None;
            rb.Configure(this, cam);
            rb.gameObject.SetActive(false);
            sb.Configure(this);

            IPlayer.OnMoneyBalanceChanged?.Invoke(this, new PlayerEventArgs { MoneyBalance = MoneyBalance });
        }

        private void OnBuildRailPressed(object sender, Toggle toggle)
        {
            if (toggle.isOn)
            {
                if (state == PlayerState.BuildingRoads) return;

                rb.gameObject.SetActive(true);
                state = PlayerState.BuildingRoads;
            }
            else
            {
                if (state == PlayerState.None) return;

                rb.gameObject.SetActive(false);
                state = PlayerState.None;
            }
        }

        //public decimal AddProfitForDeliveredCargo(Cargo cargo)
        //{
        //    var worth = cargo.GetWorthValue();
        //    MoneyBalance += worth;
        //    return worth;
        //}

        public decimal AddProfitForDeliveredCargo(decimal money)
        {
            MoneyBalance += money;
            IPlayer.OnMoneyBalanceChanged?.Invoke(this, new PlayerEventArgs { MoneyBalance = MoneyBalance });
            return money;
        }

        //private void Update()
        //{
        //    if (!rb.gameObject.activeInHierarchy) return;

        //    rb.Tick();
        //}
    }
}
