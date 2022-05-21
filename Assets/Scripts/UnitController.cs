using System.Collections.Generic;
using GridMaster;
using UnityEngine;

namespace UnitControl
{
    public class UnitController : MonoBehaviour
    {
        private UnitStates _states;
        private GridBase _grid;

        public Vector3 StartingPosition;

        public Node CurrentNode;

        public bool MovePath;

        private int _indexPath = 0;

        public List<Node> CurrentPath = new List<Node>();
        public List<Node> ShortPath = new List<Node>();

        public float Speed = 5;

        private float _startTime;
        private float _fractLength;
        private Vector3 _startPos;
        private Vector3 _targetPos;
        private bool _updatedPos;

        private void Start()
        {
            _grid = GridBase.GetInstance();
            _states = GetComponent<UnitStates>();
            PlaceOnNodeImmediate(StartingPosition);

            CurrentNode = _grid.GetNodeFromVector3(StartingPosition);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (!MovePath)
                    MovePath = true;
            }

            _states.Move = MovePath;

            if (MovePath)
            {
                if (!_updatedPos)
                {
                    if (_indexPath < ShortPath.Count - 1)
                    {
                        _indexPath++;
                    }
                    else
                    {
                        MovePath = false;
                    }

                    CurrentNode = _grid.NodeFromWorldPosition(transform.position);
                    StartingPosition = CurrentNode.WorldObject.transform.position;
                    _targetPos = ShortPath[_indexPath].WorldObject.transform.position;

                    _fractLength = Vector3.Distance(StartingPosition, _targetPos);
                    _startTime = Time.time;
                    _updatedPos = true;
                }

                var distCover = (Time.time - _startTime) * _states.MovingSpeed;

                if (_fractLength == 0)
                {
                    _fractLength = 0.1f;
                }

                var fracJourney = distCover / _fractLength;

                if (fracJourney > 1)
                {
                    _updatedPos = false;
                }

                var targetPosition = Vector3.Lerp(StartingPosition, _targetPos, fracJourney);

                var dir = targetPosition = StartingPosition;
                dir.y = 0;

                if (dir != Vector3.zero)
                {
                    var targetRotation = Quaternion.LookRotation(dir);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _states.MaxSpeed);
                }

                transform.position = targetPosition;
            }
        }

        public void EvaluatePath()
        {
            var curDirection = Vector3.zero;

            for (var i = 0; i < CurrentPath.Count; i++)
            {
                var nextDirection = new Vector3(
                    CurrentPath[i - 1].X - CurrentPath[i].X,
                    CurrentPath[i - 1].Y - CurrentPath[i].Y,
                    CurrentPath[i - 1].Z -  CurrentPath[i].Z);

                if (nextDirection != curDirection)
                {
                    ShortPath.Add(CurrentPath[i-1]);
                    ShortPath.Add(CurrentPath[i]);
                }

                curDirection = nextDirection;
            }

            ShortPath.Add(CurrentPath[CurrentPath.Count -1]);
        }

        public void ResetMovingVariables()
        {
            _updatedPos = false;
            _indexPath = 0;
            _fractLength = 0;
        }

        public void PlaceOnNodeImmediate(Vector3 nodePos)
        {
            var x = Mathf.CeilToInt(nodePos.x);
            var y = Mathf.CeilToInt(nodePos.y);
            var z = Mathf.CeilToInt(nodePos.z);

            var node = _grid.GetNode(x, y, z);

            transform.position = node.WorldObject.transform.position;
        }
    }
}