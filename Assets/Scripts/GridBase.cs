using System;
using System.Collections.Generic;
using GridMaster;
using LevelControl;
using UnityEngine;

public class GridBase : MonoBehaviour
{
    public int MaxX;
    public int MaxY;
    public int MaxZ;

    public float OffsetX;
    public float OffsetY;
    public float OffsetZ;

    public Node[,,] Grid;

    public GameObject GridFloorPrefab;

    public Vector3 StartNodePosition;
    public Vector3 EndNodePosition;

    public int EnabledY;
    private readonly List<GameObject> _yCollisions = new List<GameObject>();

    public int Agents;

    private LevelManager _lvlManager;

    public void InitGrid(LevelInitializer.GridStatistics gridStats)
    {
        MaxX = gridStats.MaxX;
        MaxY = gridStats.MaxY;
        MaxZ = gridStats.MaxZ;

        OffsetX = gridStats.OffsetX;
        OffsetY = gridStats.OffsetY;
        OffsetZ = gridStats.OffsetZ;

        _lvlManager = LevelManager.GetInstance();

        CreateGrid();
        CreateMouseCollision();
        CloseAllMouseCollisions();

        _yCollisions[EnabledY].SetActive(true);
    }

    public Node GetNode(int x, int y, int z)
    {
        Node res = null;

        if (x < MaxX && x >= 0 &&
            y < MaxY && y >= 0 &&
            z < MaxZ && z >= 0)
        {
            res = Grid[z, y, z];
        }

        return res;
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        var worldX = worldPosition.x;
        var worldY = worldPosition.y;
        var worldZ = worldPosition.z;

        worldX /= OffsetX;
        worldY /= OffsetY;
        worldZ /= OffsetZ;

        var x = Mathf.RoundToInt(worldX);
        var y = Mathf.RoundToInt(worldY);
        var z = Mathf.RoundToInt(worldZ);

        if (x > MaxX - 1)
            x = MaxX - 1;
        if (y > MaxY - 1)
            y = MaxY - 1;
        if (z > MaxZ - 1)
            z = MaxZ - 1;

        if (x < 0)
            x = 0;
        if (y < 0)
            y = 0;
        if (z < 0)
            z = 0;

        return Grid[x, y, z];
    }

    public Node GetNodeFromVector3(Vector3 pos)
    {
        throw new Exception("NotImplementedYet");
    }

    private void CreateGrid()
    {
        Grid = new Node[MaxX, MaxY, MaxZ];

        for (var i = 0; i < MaxY; i++)
        {
            _lvlManager.LvlObjects.Add(new ObjectsPerFloor());
            _lvlManager.LvlObjects[i].FloorIndex = i;
        }

        for (var x = 0; x < MaxX; x++)
        {
            for (var y = 0; y < MaxY; y++)
            {
                for (var z = 0; z < MaxZ; z++)
                {
                    var posX = x * OffsetX;
                    var posY = y * OffsetY;
                    var posZ = z * OffsetZ;

                    var go = Instantiate(GridFloorPrefab, new Vector3(posX, posY, posZ), Quaternion.identity);

                    go.transform.name = x + "_" + y + "_" + z;
                    go.transform.parent = transform;

                    var node = new Node()
                    {
                        X = x,
                        Y = y,
                        Z = z,
                        WorldObject = go,
                        NodeRef = go.GetComponentInChildren<NodeReferences>(),
                        IsWalkable = false
                    };
                    node.NodeRef.TileRender.enabled = false;

                    var hits = Physics.BoxCastAll(
                        new Vector3(posX, posY, posZ),
                        new Vector3(0.5f, 0.5f, 0.5f),
                        Vector3.up);

                    foreach (var hit in hits)
                    {
                        if (hit.transform.GetComponent<LevelObject>())
                        {
                            var lvlObj = hit.transform.GetComponent<LevelObject>();

                            if (!_lvlManager.LvlObjects[y].Objects.Contains(lvlObj.gameObject))
                            {
                                _lvlManager.LvlObjects[y].Objects.Add(lvlObj.gameObject);
                            }

                            node.NodeRef.TileRender.enabled = true;

                            if (lvlObj.ObjType == LevelObject.LvlObjectType.Obstacle)
                            {
                                node.IsWalkable = false;
                                node.NodeRef.ChangeTileMaterial(1);
                                break;
                            }

                            if (lvlObj.ObjType == LevelObject.LvlObjectType.Floor)
                            {
                                node.IsWalkable = true;
                                node.NodeRef.ChangeTileMaterial(0);
                            }
                        }
                    }

                    Grid[x, y, z] = node;
                }
            }
        }
    }

    private void CreateMouseCollision()
    {
        for (var y = 0; y < MaxY; y++)
        {
            var go = new GameObject()
            {
                transform =
                {
                    name = "Collision for Y " + y
                }
            };

            go.AddComponent<BoxCollider>();
            go.GetComponent<BoxCollider>().size = new Vector3(MaxX * OffsetX, 0.1f, MaxZ * OffsetZ);

            go.transform.position = new Vector3((MaxX * OffsetX) / 2 - OffsetX / 2, y * OffsetY,
                (MaxZ * OffsetZ) / 2 - OffsetZ / 2);

            _yCollisions.Add(go);
        }
    }

    private void CloseAllMouseCollisions()
    {
        foreach (var yCollision in _yCollisions)
        {
            yCollision.SetActive(false);
        }
    }

    private static GridBase _instance;

    public static GridBase GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }
}
