/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-01-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
    public interface IAstarMap<T>where T:IAStarNode
    {
        bool WalkSideways { get; set; }
        T[] GetNeighborNodes(T node);
        float GetHCost(T start, T end);
        void Reset();
    }
    public class AStarMap2X : IAstarMap<AStarNode2X>
    {
        private List<AStarNode2X> neighborNodes;
        private AStarNode2X[,] map;
        public AStarNode2X this[int x,int y]
        {
            get { return map[x, y]; }
        }
        private int len, wid;
        private bool walkSideways;
        public bool WalkSideways { get { return walkSideways; }set { walkSideways = value; } }

        public AStarMap2X()
        {
            neighborNodes = new List<AStarNode2X>(8);

        }
        public float GetHCost(AStarNode2X start, AStarNode2X end)
        {
            return (float)Math.Sqrt((start.X - end.X) * (start.X - end.X) + (start.Y - end.Y) * (start.Y - end.Y));
        }

        public AStarNode2X[] GetNeighborNodes(AStarNode2X node)
        {
            neighborNodes.Clear();
            if ( walkSideways)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (node.X - 1 + i >= 0 && node.X - 1 + i < len)
                        {
                            if (node.Y - 1 + j >= 0 && node.Y - 1 + j < wid)
                            {
                                if (j==1 && i == 1) continue;
                                if (map[node.X - 1 + i, node.Y - 1 + j].NodeType == AStarNodeType.Walkable)
                                {
                                    neighborNodes.Add(map[node.X - 1 + i, node.Y - 1 + j]);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (node.X - 1 + i >= 0 && node.X - 1 + i < len)
                        {
                            if (node.Y - 1 + j >= 0 && node.Y - 1 + j < wid)
                            {
                                if ((i - 1) * (j - 1)  == 0 && i!=j)
                                {
                                    if (map[node.X - 1 + i, node.Y - 1 + j].NodeType == AStarNodeType.Walkable)
                                    {
                                        neighborNodes.Add(map[node.X - 1 + i, node.Y - 1 + j]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return neighborNodes.ToArray();
        }
        public void ReadMap<T>(Func<T,AStarNodeType> func,T[,] arr,bool walkSideways=true)
        {
            this.walkSideways = walkSideways;
            map = new AStarNode2X[arr.GetLength(0), arr.GetLength(1)];
            this.wid = map.GetLength(1);
            this.len = map.GetLength(0);
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < wid; j++)
                {
                    map[i, j] = new AStarNode2X(i, j, func(arr[i, j]));
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < wid; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    map[i, j].Reset();
                }
            }
        }
    }
    public class AStarMap3X : IAstarMap<AStarNode3X>
    {
        private bool walkSideways;
        public bool WalkSideways { get { return walkSideways; } set { walkSideways = value; } }
        private List<AStarNode3X> neighborNodes;
        private AStarNode3X[,,] map;
        private int len, wid, hei;
        public AStarMap3X()
        {
            neighborNodes = new List<AStarNode3X>(27 - 1);
        }
        public AStarNode3X this[int x, int y,int z]
        {
            get { return map[x, y,z]; }
        }
        public void ReadMap<T>(Func<T, AStarNodeType> func, T[,,] arr,bool walkSideways= true)
        {

            this.walkSideways = walkSideways;

            map = new  AStarNode3X[arr.GetLength(0), arr.GetLength(1),arr.GetLength(2)];
            this.wid = map.GetLength(1);
            this.len = map.GetLength(0);
            this.hei = map.GetLength(2);

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < wid; j++)
                {
                    for (int k = 0; k < hei; k++)
                    {
                        map[i, j,k] = new AStarNode3X(i, j,k, func(arr[i, j,k]));

                    }
                }
            }
        }

        public float GetHCost(AStarNode3X start, AStarNode3X end)
        {
            return (float)Math.Sqrt((start.X - end.X) * (start.X - end.X) + (start.Y - end.Y) * (start.Y - end.Y) + (start.Z - end.Z) * (start.Z - end.Z));
        }

        public AStarNode3X[] GetNeighborNodes(AStarNode3X node)
        {
            neighborNodes.Clear();
            if (walkSideways)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {

                            if (node.X - 1 + i >= 0 && node.X - 1 + i < len)
                            {
                                if (node.Y - 1 + j >= 0 && node.Y - 1 + j < wid)
                                {
                                    if (node.Z - 1 + k >= 0 && node.Z - 1 + k < hei)
                                    {
                                        if (i==1 && j==1 && k==1) continue;
                                        if (map[node.X - 1 + i, node.Y - 1 + j, node.Z - 1 + k].NodeType == AStarNodeType.Walkable)
                                        {
                                            neighborNodes.Add(map[node.X - 1 + i, node.Y - 1 + j, node.Z - 1 + k]);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {

                            if (node.X - 1 + i >= 0 && node.X - 1 + i < len)
                            {
                                if (node.Y - 1 + j >= 0 && node.Y - 1 + j < wid)
                                {
                                    if (node.Z - 1 + k >= 0 && node.Z - 1 + k < hei)
                                    {
                                        if ((i-1)*(j-1)*(k-1)==0 && i != j && j!=k)
                                        {
                                            if (map[node.X - 1 + i, node.Y - 1 + j, node.Z - 1 + k].NodeType == AStarNodeType.Walkable)
                                            {
                                                neighborNodes.Add(map[node.X - 1 + i, node.Y - 1 + j, node.Z - 1 + k]);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

            }
            return neighborNodes.ToArray();
        }

        public void Reset()
        {
            for (int i = 0; i < wid; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    for (int k = 0; k < hei; k++)
                    {
                        map[i, j, k].Reset();
                    }
                }
            }
        }
    }

}
