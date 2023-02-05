using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace GyroKame
{
    public class JsonAnalyzer : MonoBehaviour
    {
        private FileEntries files;
        [SerializeField] private LevelGenerator generator;
        [SerializeField] private TextAsset text;

        void Start()
        {
            /*
            Debug.Log("Combining path");
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, "files.json");
            Debug.Log("Reading json from " + path);
            string output = System.IO.File.ReadAllText(path);
            */

            Debug.Log("Json read");
            //var entries = JsonConvert.DeserializeObject<List<FileEntry>>(output);
            files = JsonUtility.FromJson<FileEntries>(text.text);
            Debug.Log("Json converted");
            if (generator != null && files.contents.Count > 0)
            {
                generator.GenerateLevel(files);
            } else
            {
                Debug.LogError("Failed level creation");
            }
        }
    }
}