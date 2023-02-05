using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    [System.Serializable]
    public class FileEntry
    {
        [SerializeField] private string type;
        [SerializeField] private string name;
        [SerializeField] private List<FileEntry> contents;

        public override string ToString()
        {
            return name;
        }
    }

    [System.Serializable]
    public class FileEntries
    {
        [SerializeField] private List<FileEntry> contents;
    }
}