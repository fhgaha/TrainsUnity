using UnityEngine.UI;

namespace Trains
{
    public class NamedToggle : Toggle
    {
        public delegate void ToggleChanged(int id, bool value);
        public int Id;
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
