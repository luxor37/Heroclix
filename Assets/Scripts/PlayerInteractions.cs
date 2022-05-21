using System.Collections.Generic;
using GridMaster;
using Pathfinding;
using UnitControl;
using UnityEngine;

namespace PI
{
    public class PlayerInteractions : MonoBehaviour
    {
        public UnitController ActiveUnit;

        public bool HasPath;
        public bool HoldPath;

        private PathFindMaster _pathfinder;
        private GridBase _grid;
        private Node _prevNode;

        public bool VisualizePath;
        public GameObject LineGo;
        private LineRenderer _line;

        private void Start()
        {
            _grid = GridBase.GetInstance();
            _pathfinder = PathFindMaster.GetInstance();
        }

        private void Update()
        {
            if (ActiveUnit)
            {
                if (!HasPath)
                {
                    var startNode = _grid.NodeFromWorldPosition(ActiveUnit.transform.position);
                    var targetNode = FindNodeFromMousePosition();

                    if (targetNode != null && startNode != null)
                    {
                        if (_prevNode != targetNode && !HoldPath)
                        {
                            _pathfinder.RequestPathfind(startNode, targetNode, PopulatePathOfActiveUnit);
                            _prevNode = targetNode;
                            HasPath = true;
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                HoldPath = !HoldPath;
            }

            if (ActiveUnit.ShortPath.Count < 1)
                HoldPath = false;

            if (VisualizePath)
            {
                if (_line == null)
                {
                    var go = Instantiate(LineGo, transform.position, Quaternion.identity);
                    _line = go.GetComponent<LineRenderer>();
                }
                else
                {
                    _line.positionCount = ActiveUnit.ShortPath.Count;
                    for (var i = 0; i < ActiveUnit.ShortPath.Count; i++)
                    {
                        _line.SetPosition(i, ActiveUnit.ShortPath[i].WorldObject.transform.position);
                    }
                }
            }
        }

        private Node FindNodeFromMousePosition()
        {
            Node res = null;

            var ray = Camera.main.ScreenPointToRay((Input.mousePosition));

            if (Physics.Raycast(ray, out var hit, 200))
            {
                res = _grid.NodeFromWorldPosition(hit.point);
            }

            return res;
        }

        private void PopulatePathOfActiveUnit(List<Node> nodes)
        {
            ActiveUnit.CurrentPath.Clear();
            ActiveUnit.ShortPath.Clear();

            ActiveUnit.CurrentPath.Add(_grid.NodeFromWorldPosition(ActiveUnit.transform.position));

            foreach (var node in nodes)
            {
                ActiveUnit.CurrentPath.Add(node);
            }

            ActiveUnit.EvaluatePath();
            ActiveUnit.ResetMovingVariables();
            HasPath = false;
        }
    }
}