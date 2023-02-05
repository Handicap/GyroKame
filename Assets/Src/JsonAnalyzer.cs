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

        void Start()
        {
            string path = "./Assets/files.json";
            string output = System.IO.File.ReadAllText(path);

            //var entries = JsonConvert.DeserializeObject<List<FileEntry>>(output);
            files = JsonUtility.FromJson<FileEntries>(output);
            Debug.Log("Json read");
            if (generator != null)
            {
                generator.GenerateLevel(files);
            }
        }
    }
}