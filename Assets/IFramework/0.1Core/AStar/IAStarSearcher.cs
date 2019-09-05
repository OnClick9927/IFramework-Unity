/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-01-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework
{
    public interface IAStarSearcher<Node,Map>where Node:IAStarNode where Map:IAstarMap<Node>
    {
        Node[] Search(Node start, Node end);
        void LoadMap(Map map);
    }
    public class AStarSeacher<Node, Map> : IAStarSearcher<Node, Map> where Node : IAStarNode where Map : IAstarMap<Node>
    {
        private Map map;
        private List<Node> openList;
        private List<Node> closeList;

        public void LoadMap(Map map)
        {
            this.map = map;
            openList = new List<Node>();
            closeList = new List<Node>();
            pathNodes = new List<Node>();
        }

        private List<Node> pathNodes;
        private Node curNode;
        private Node[] neighborNodes;



        public Node[] Search(Node start, Node end)
        {
            map.Reset();
            openList.Clear();
            closeList.Clear();
            pathNodes.Clear();
            start.TryUpdateFCost(0, map.GetHCost(start, end), null);
            curNode = start;
            while (!curNode .Equals( end))
            {
                neighborNodes = map.GetNeighborNodes(curNode);
                for (int i = 0; i < neighborNodes.Length; i++)
                {
                    if (!openList.Contains(neighborNodes[i]) && !closeList.Contains(neighborNodes[i]))
                    {
                        openList.Add(neighborNodes[i]);
                        neighborNodes[i].SetDefaultParent(curNode);
                    }
                    neighborNodes[i].TryUpdateFCost(curNode.G + map.GetHCost(curNode, neighborNodes[i]), map.GetHCost(neighborNodes[i], end), curNode);
                }
                float smallestF = float.MaxValue;
                for (int i = 0; i < openList.Count; i++)
                {
                    if (smallestF > openList[i].F)
                    {
                        smallestF = openList[i].F;
                        curNode = openList[i];
                    }
                }
                closeList.Add(curNode);
                openList.Remove(curNode);
                if (openList.Count == 0)
                {
                    return null;
                }
            }
            while (!curNode.ParentNode.Equals(start))
            {
                pathNodes.Insert(0, curNode);
                curNode = (Node)curNode.ParentNode ;
            }
            pathNodes.Insert(0, curNode);
            return pathNodes.ToArray();
        }

        
    }


}
