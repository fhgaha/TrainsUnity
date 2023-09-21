using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Global : MonoBehaviour
    {
        public static Global Instance { get; private set; }

        public float DriveDistance { get; private set; } = 1f;

        //void Awake()
        //{
        //    if (instance == null)
        //    {
        //        DontDestroyOnLoad(gameObject);
        //        instance = this;
        //    }
        //    else
        //        if (instance != this)
        //        Destroy(gameObject);
        //}

        private void Start()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance == this)
            {
                throw new System.Exception($"Attempting to create instance of {this.GetType()} signleton when such instance already exists");
                //Destroy(gameObject);
            }

            InitializeManager();
        }

        // Метод инициализации менеджера
        private void InitializeManager()
        {
            /* TODO: Здесь мы будем проводить инициализацию */
        }
    }
}
