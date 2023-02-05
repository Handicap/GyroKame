using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class LevelGenerator : MonoBehaviour
    {
        //[SerializeField] private 
        [SerializeField] private GameLink linkPrefab;
        [SerializeField] private GameFile filePrefab;
        [SerializeField] private GameDirectory folderPrefab;
        [SerializeField] private float entryHorizontalDistance = 1f;
        public void GenerateLevel(FileEntries directoryTree)
        {
            Debug.Log("Level generation");
            foreach (var item in directoryTree.contents)
            {
                 var rootDir = CreateDirectory(item, 0);
                rootDir.gameObject.SetActive(true);
                rootDir.ActivateChildren();
            }
        }

        private GameDirectory CreateDirectory(FileEntry entry, float horizontalPosition, GameDirectory parent = null)
        {
            var folder = Instantiate(folderPrefab);
            folder.Initialize(entry, parent, horizontalPosition);
            //Debug.Log("Created folder " + entry, folder);
            //folder.Initialize()
            if (entry.contents != null && entry.contents.Count > 0)
            {
                float leftOffset = -entry.contents.Count / 2 * entryHorizontalDistance;
                for (int i = 0; i < entry.contents.Count; i++)
                {
                    
                    var next = entry.contents[i];
                    float horizontal = leftOffset + folder.transform.position.x + entryHorizontalDistance * i;
                    if (next.type == "directory")
                    {
                        CreateDirectory(next, horizontal, folder);
                    } else if (next.type == "link")
                    {
                        CreateLink(next, horizontal, folder);
                    } else if (next.type == "file")
                    {
                        CreateFile(next, horizontal, folder);
                    }
                    /*
                    switch (next.type)
                    {
                        case "directory":
                            CreateDirectory(next, folder);
                            break;
                        case "file":
                            CreateFile(next, folder);
                            break;
                        case "link":
                            CreateLink(next, folder);
                            break;
                        default:
                            Debug.LogError("Unknown entry type " + entry.type);
                            break;
                    }
                    */
                }
            }
            return folder;
        }

        private void CreateFile(FileEntry entry, float horizontalPosition, GameDirectory parent)
        {
            var file = Instantiate(filePrefab);
            file.Initialize(entry, parent, horizontalPosition);
            Debug.Log("Created file " + entry, file);
            //folder.Initialize()
        }

        private void CreateLink(FileEntry entry, float horizontalPosition, GameDirectory parent)
        {
            var link = Instantiate(filePrefab);
            link.Initialize(entry, parent, horizontalPosition);
            Debug.Log("Created link " + entry, link);
            //folder.Initialize()
        }
    }
}