using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<ObjectsPerFloor> LvlObjects = new List<ObjectsPerFloor>();

    private static LevelManager _instance;

    public static LevelManager GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }
}

[Serializable]
public class ObjectsPerFloor
{
    public int FloorIndex;
    public List<GameObject> Objects = new List<GameObject>();
}
