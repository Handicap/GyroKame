using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GyroKame
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI shitbox;
        [SerializeField] private TMPro.TextMeshProUGUI accelerometerBox;

        private string previousMessage = "";

        private void Start()
        {
            shitbox.text = "";
            Application.logMessageReceived += Application_logMessageReceived;

            if (Accelerometer.current != null)
            {
                InputSystem.EnableDevice(Accelerometer.current);
                Debug.Log("Accelerometer!");

            }
            else
            {
                Debug.LogError("no acc");
            }
        }

        private void Update()
        {
            if (Accelerometer.current.enabled)
            {
                Debug.Log("ACC: " + Accelerometer.current.acceleration.ReadValue().ToString());
            } else
            {
                Debug.LogError("Accelerometer unabled :L");
            }
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (previousMessage != condition)
            {
                shitbox.text += "\n" + condition;
            }
            previousMessage = condition;
        }
    }

}
