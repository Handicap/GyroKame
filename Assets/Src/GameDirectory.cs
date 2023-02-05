using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class GameDirectory : GameEntry
    {
        [SerializeField] private List<GameEntry> children = new List<GameEntry>();

        public List<GameEntry> Children { get => children; }

        public void AddToChildren(GameEntry entry)
        {
            Children.Add(entry);
        }

        public void ActivateChildren()
        {
            foreach (var item in Children)
            {
                item.gameObject.SetActive(true);
                item.MakeVisible();
            }
        }

        public override void DestroyBlock()
        {
            base.DestroyBlock();
            ActivateChildren();
        }
    }
}