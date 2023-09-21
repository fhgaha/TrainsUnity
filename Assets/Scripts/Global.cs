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

            // ������ ��� ����� �������, ����� ������ �� �����������
            // ��� �������� �� ������ ����� ����
            DontDestroyOnLoad(gameObject);

            // � ��������� ���������� �������������
            InitializeManager();
        }

        // ����� ������������� ���������
        private void InitializeManager()
        {
            /* TODO: ����� �� ����� ��������� ������������� */
        }
    }
}
