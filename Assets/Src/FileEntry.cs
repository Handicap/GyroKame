using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    [System.Serializable]
    public class FileEntry
    {
        [SerializeField] public string type;
        [SerializeField] public string name;
        [SerializeField] public List<FileEntry> contents;

        public override string ToString()
        {
            return name;
        }
    }

    [System.Serializable]
    public class FileEntries
    {
        [SerializeField] public List<FileEntry> contents;
    }
}