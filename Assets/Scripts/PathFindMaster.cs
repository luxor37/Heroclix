using System.Collections.Generic;
using System.Threading;
using GridMaster;
using UnityEngine;

namespace Pathfinding
{
    public class PathFindMaster : MonoBehaviour
    {
        private static PathFindMaster _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static PathFindMaster GetInstance()
        {
            return _instance;
        }

        public int MaxJobs = 3;

        public delegate void PathFindingJobComplete(List<Node> path);

        private List<PathFinder> _currentJobs;
        private List<PathFinder> _toDoJobs;

        private void Start()
        {
            _currentJobs = new List<PathFinder>();
            _toDoJobs = new List<PathFinder>();
        }

        private void Update()
        {
            var i = 0;

            while (i < _currentJobs.Count)
            {
                if (_currentJobs[i].JobDone)
                {
                    _currentJobs[i].NotifyComplete();
                    _currentJobs.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if (_toDoJobs.Count > 0 && _currentJobs.Count < MaxJobs)
            {
                var job = _toDoJobs[0];
                _toDoJobs.RemoveAt(0);
                _currentJobs.Add(job);

                var jobThread = new Thread(job.FindPath);
                jobThread.Start();
            }
        }

        public void RequestPathfind(Node start, Node target, PathFindingJobComplete completeCallBack)
        {
            var newJob = new PathFinder(start, target, completeCallBack);
            _toDoJobs.Add(newJob);
        }
    }
}