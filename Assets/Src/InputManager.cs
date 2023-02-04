using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI shitbox;
        private void Start()
        {
            Application.logMessageReceived += Application_logMessageReceived;

            if (UnityEngine.InputSystem.Accelerometer.current == null)
            {
                Debug.LogError("no acc");
            } else
            {
                Debug.Log("Accelerometer!");
            }
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            shitbox.text += "\n" + condition;
        }
    }

}
