using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class MenuPlayButton : MonoBehaviour
    {
        public void OnPressed()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}