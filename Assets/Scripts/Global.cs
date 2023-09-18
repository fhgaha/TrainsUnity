using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Global : MonoBehaviour
    {
        public static Global Instance { get; private set; }

        public float DriveDistance { get; private set; } = 1f;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                throw new System.Exception($"Attempting to create instance of {this.GetType()} signleton when such instance already exists");
                //Destroy(gameObject);
            }

            // Теперь нам нужно указать, чтобы объект не уничтожался
            // при переходе на другую сцену игры
            DontDestroyOnLoad(gameObject);

            // И запускаем собственно инициализатор
            InitializeManager();
        }

        // Метод инициализации менеджера
        private void InitializeManager()
        {
            /* TODO: Здесь мы будем проводить инициализацию */
        }
    }
}
