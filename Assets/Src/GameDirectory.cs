using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class GameDirectory : GameEntry
    {
        [SerializeField] private List<GameEntry> children = new List<GameEntry>();

        public void AddToChildren(GameEntry entry)
        {
            children.Add(entry);
        }

        public void ActivateChildren()
        {
            foreach (var item in children)
            {
                item.gameObject.SetActive(true);
            }
        }

        public override void DestroyBlock()
        {
            base.DestroyBlock();
            ActivateChildren();
        }
    }
}