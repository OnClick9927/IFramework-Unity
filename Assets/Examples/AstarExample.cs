/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.Astar;

namespace IFramework_Demo
{
    public class AstarExample:UnityEngine.MonoBehaviour
    {
        void Start()
        {

            int[,] arr = new int[10, 10]
            {
              { 1,1,1,1,1,1,0,0,1,1},
              { 1,1,1,1,1,1,0,0,1,1},
              { 1,1,1,1,1,1,0,0,1,1},
              { 1,1,1,1,1,1,0,0,1,1},
              { 1,1,1,1,1,1,0,0,1,1},
              { 0,0,0,0,0,1,0,0,1,1},
              { 0,0,0,0,0,1,0,0,1,1},
              { 1,1,1,1,1,1,1,1,1,1},
              { 1,1,1,1,1,0,1,1,1,1},
              { 1,1,1,1,1,0,1,1,1,1},
            };
            AStarMap2X map = new AStarMap2X();
            map.ReadMap((val) => 
            {
                if (val == 1)
                    return AStarNodeType.Walkable;
                return AStarNodeType.Wall;
            },arr,false);
            AStarSeacher<AStarNode2X, AStarMap2X> sear = new AStarSeacher<AStarNode2X, AStarMap2X>();
            sear.LoadMap(map);
            try
            {
                AStarNode2X[] result = sear.Search(map[0, 0], map[3, 8]);
                foreach (var item in result)
                {
                    Log.L(item.X + " " + item.Y);
                }
            }
            catch (System.Exception)
            {
                Log.E("none");
            }
        }
    }
}
