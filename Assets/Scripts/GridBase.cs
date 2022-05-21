using System.Collections.Generic;
using LevelControl;
using UnityEditor.Experimental.GraphView;
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

    public int enabledY;
    private List<GameObject> _yCollisions = new();

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

        _lvlManager = new LevelManager().GetInstance();

        CreateGrid();
        CreateMouseCollision();
        CloseAllMouseCollisions();

        _yCollisions[enabledY].SetActive(true);
    }

    void Update()
    {
       
    }

    public Node GetNode(int x, int y, int z)
    {
        Node res = null;

        if (x < MaxX && x >= 0 &&
            y < MaxY && y >= 0 &&
            z < MaxZ && z >= 0)
        {

        }
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
}
