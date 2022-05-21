using System;
using System.Collections;
using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    public GameObject GridBasePrefab;

    public int UnitCount = 1;
    public GameObject UnitPrefabs;

    private WaitForEndOfFrame _waitEf;

    public GridStatistics GridStats;

    [Serializable]
    public class GridStatistics
    {
        public int MaxX = 16;
        public int MaxY = 1;
        public int MaxZ = 24;

        public float OffsetX = 1;
        public float OffsetY = 1;
        public float OffsetZ = 1;
    }

    private void Start()
    {
        _waitEf = new WaitForEndOfFrame();

        StartCoroutine(InitLevel());
    }

    private IEnumerator InitLevel()
    {
        yield return StartCoroutine(CreateGrid());

        yield return StartCoroutine(CreateUnits());

        yield return StartCoroutine(EnablePlayerInteractions());
    }

    private IEnumerator CreateGrid()
    {
        var  go = Instantiate(GridBasePrefab, Vector3.zero, Quaternion.identity);

        go.GetComponent<GridBase>().InitGrid(GridStats);

        yield return _waitEf;
    }

    private IEnumerator CreateUnits()
    {
        for (var i = 0; i < UnitCount; i++)
        {
            Instantiate(UnitPrefabs, Vector3.zero, Quaternion.identity);
        }

        yield return _waitEf;
    }

    private IEnumerator EnablePlayerInteractions()
    {
        GetComponent<PI.PlayerInteractions>().enabled = true;

        yield return _waitEf;
    }
}
