using System;
using UnityEngine;

namespace GridMaster
{
    [Serializable]
    public class Node : MonoBehaviour
    {
        public int X;
        public int Y;
        public int Z;

        public float HCost;
        public float GCost;

        public float FCost => HCost + GCost;

        public Node ParentNode;
        public bool IsWalkable = true;

        public GameObject WorldObject;
        public NodeReferences NodeRef;

        public NodeTypeEnum NodeType;
        public enum NodeTypeEnum
        {
            Ground,
            Air
        }
    }
}