using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelControl
{
    public class LevelManager : MonoBehaviour
    {
        public List<ObjectsPerFloor> LvlObjects = new();

        public static LevelManager Instance;

        public LevelManager GetInstance()
        {
            return Instance;
        }

        private void Awake()
        {
            Instance = this;
        }
    }

    [Serializable]
    public class ObjectsPerFloor
    {
        public int FloorIndex;
        public List<GameObject> Objects = new();
    }
}