using System;
using System.Collections.Generic;
using System.Linq;
using GridMaster;
using UnityEngine;

namespace Pathfinding
{
    public class PathFinder : MonoBehaviour
    {
        private readonly GridBase _gridBase;

        public Node StartPosition;
        public Node EndPosition;

        public volatile bool JobDone = false;

        private readonly PathFindMaster.PathFindingJobComplete _completeCallback;
        private List<Node> _foundPath;

        public PathFinder(Node start, Node target, PathFindMaster.PathFindingJobComplete callback)
        {
            StartPosition = start;
            EndPosition = target;
            _completeCallback = callback;
            _gridBase = GridBase.GetInstance();
        }

        public void FindPath()
        {
            _foundPath = FindPathActual(StartPosition, EndPosition);

            JobDone = true;
        }

        public void NotifyComplete()
        {
            _completeCallback?.Invoke(_foundPath);
        }

        private List<Node> FindPathActual(Node start, Node target)
        {
            var foundPath = new List<Node>();

            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();

            openSet.Add(start);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];

                foreach (var set in openSet.Where(
                    set => set.FCost < currentNode.FCost ||
                     (Math.Abs(set.FCost - currentNode.FCost) <= 0 &&
                      set.HCost < currentNode.HCost)).Where(set => !currentNode.Equals(set)))
                {
                    currentNode = set;
                }

                if (currentNode.Equals(target))
                {
                    foundPath = RetracePath(start, currentNode);
                    break;
                }
                
                foreach (Node neighbor in GetNeighbors(currentNode, true))
                {
                    if (!closedSet.Contains(neighbor))
                    {
                        var newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                        
                        if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                        {
                            neighbor.GCost = newMovementCostToNeighbor;
                            neighbor.HCost = GetDistance(neighbor, target);
                            neighbor.ParentNode = currentNode;
                            if (!openSet.Contains(neighbor))
                            {
                                openSet.Add(neighbor);
                            }
                        }
                    }
                }
            }
            
            return foundPath;
        }
        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.ParentNode;
            }
            
            path.Reverse();

            return path;
        }

        private List<Node> GetNeighbors(Node node, bool getVerticalNeighbors = false)
        {
            var retList = new List<Node>();

            for (var x = -1; x <= 1; x++)
            {
                for (var yIndex = -1; yIndex <= 1; yIndex++)
                {
                    for (var z = -1; z <= 1; z++)
                    {
                        var y = yIndex;
                        
                        if (!getVerticalNeighbors)
                        {
                            y = 0;
                        }

                        if (x == 0 && y == 0 && z == 0)
                        {
                            //000 is the current node
                        }
                        else
                        {
                            var searchPos = new Node()
                            {
                                X = node.X + x,
                                Y = node.Y + y,
                                Z = node.Z + z
                            };

                            var newNode = GetNeighborNode(searchPos, true, node);

                            if (newNode != null)
                            {
                                retList.Add(newNode);
                            }
                        }
                    }
                }
            }

            return retList;

        }

        private Node GetNeighborNode(Node adjPos, bool searchTopDown, Node currentNodePos)
        {
            Node retVal = null;
            
            var node = GetNode(adjPos.X, adjPos.Y, adjPos.Z);
            
            if (node != null && node.IsWalkable)
            {
                retVal = node;
            }
            else if (searchTopDown)
            {
                adjPos.Y -= 1;
                var bottomBlock = GetNode(adjPos.X, adjPos.Y, adjPos.Z);
                
                if (bottomBlock != null && bottomBlock.IsWalkable)
                {
                    retVal = bottomBlock;
                }
                else
                {
                    adjPos.Y += 2;
                    var topBlock = GetNode(adjPos.X, adjPos.Y, adjPos.Z);
                    if (topBlock != null && topBlock.IsWalkable)
                    {
                        retVal = topBlock;
                    }
                }
            }
            
            var originalX = adjPos.X - currentNodePos.X;
            var originalZ = adjPos.Z - currentNodePos.Z;

            if (Mathf.Abs(originalX) == 1 && Mathf.Abs(originalZ) == 1)
            {
                var neighbor1 = GetNode(currentNodePos.X + originalX, currentNodePos.Y, currentNodePos.Z);
                if (neighbor1 == null || !neighbor1.IsWalkable)
                {
                    retVal = null;
                }

                var neighbor2 = GetNode(currentNodePos.X, currentNodePos.Y, currentNodePos.Z + originalZ);
                if (neighbor2 == null || !neighbor2.IsWalkable)
                {
                    retVal = null;
                }
            }
            
            if (retVal != null)
            {
                //Example, do not approach a node from the left
                /*if(node.x > currentNodePos.x) {
                    node = null;
                }*/
            }

            return retVal;
        }

        private Node GetNode(int x, int y, int z)
        {
            Node n;

            lock (_gridBase)
            {
                n = _gridBase.GetNode(x, y, z);
            }
            return n;
        }

        private static int GetDistance(Node posA, Node posB)
        {
            var distX = Mathf.Abs(posA.X - posB.X);
            var distZ = Mathf.Abs(posA.Z - posB.Z);
            var distY = Mathf.Abs(posA.Y - posB.Y);

            if (distX > distZ)
            {
                return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
            }

            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }

    }
}