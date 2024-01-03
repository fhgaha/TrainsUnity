using UnityEngine.UI;

namespace Trains
{
    public class NamedToggle : Toggle
    {
        private int id;

        public int Id
        {
            get => id;
            set => id = value;
        }      //when is this Id set?
        public delegate void ToggleChanged(int id, bool value);
        public event ToggleChanged OnToggleChanged;
        //private Text label;

        protected override void Start()
        {
            base.Start();
            //label = GetComponentInChildren<Text>();
            onValueChanged.AddListener((bool value) => OnToggleChanged?.Invoke(Id, value));
        }
    }
}
