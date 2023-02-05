using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GyroKame
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI shitbox;

        [SerializeField] private GameObject accX;
        [SerializeField] private GameObject accY;
        [SerializeField] private GameObject accZ;

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


        //Notes:

        // regular scale: -1.0-1.0
        // scale might jump with PID?
        // x: left-right
        // y: back-forward
        // z: upside-downside
        private void PrintAccelerometer()
        {

            var accValue = Accelerometer.current.acceleration.ReadValue();
            //Debug.Log("ACC: " + accValue.ToString());
            var baseVec = new Vector3(1.0f, 0.0f, 1.0f);
            accX.transform.localScale = baseVec + (Vector3.up * accValue.x);
            accY.transform.localScale = baseVec + (Vector3.up * accValue.y);
            accZ.transform.localScale = baseVec + (Vector3.up * accValue.z);
        }

        private void Update()
        {
            if (Application.isEditor) return;
            if (Accelerometer.current.enabled)
            {
                PrintAccelerometer();
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
