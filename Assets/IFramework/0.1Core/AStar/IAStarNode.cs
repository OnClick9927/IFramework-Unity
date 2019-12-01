/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-01-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    public enum AStarNodeType
    {
        Wall, Walkable
    }
    public interface IAStarNode
    {
        AStarNodeType NodeType { get; }
        IAStarNode ParentNode { get; }
        float G { get; }
        float H { get; }
        float F { get; }
        void SetDefaultParent(IAStarNode node);
        float TryUpdateFCost(float g, float h, IAStarNode trySetNode);
        void Reset();
    }
    public class AStarNode2X : IAStarNode
    {
        private AStarNodeType nodeType;
        private IAStarNode parentNode;
        private float g;
        private float h;
        public AStarNodeType NodeType { get { return nodeType; } }
        public IAStarNode ParentNode { get { return parentNode; } }
        public float G { get { return g; } }
        public float H { get { return h; } }
        public float F { get { return g + h; ; } }
        private int m_x;
        private int m_y;
        public int Y { get { return m_y; } }
        public int X { get { return m_x; } }
        public AStarNode2X(int mapPosX, int mapPosY, AStarNodeType nodeType)
        {
            this.m_x = mapPosX;
            this.m_y = mapPosY;
            this.nodeType = nodeType;
            g = h = float.MaxValue / 2;
        }

        public void SetDefaultParent(IAStarNode node)
        {
            this.parentNode = node;
        }

        public float TryUpdateFCost(float g, float h, IAStarNode trySetNode)
        {
            if (g + h < F)
            {
                this.g = g;
                this.h = h;
                this.parentNode = trySetNode;
            }
            return F;
        }

        public void Reset()
        {
            g = h = float.MaxValue / 2;
            parentNode = null;
        }


    }

    public class AStarNode3X : IAStarNode
    {
        private AStarNodeType nodeType;
        private IAStarNode parentNode;
        private float g;
        private float h;
        public AStarNodeType NodeType { get { return nodeType; } }
        public IAStarNode ParentNode { get { return parentNode; } }
        public float G { get { return g; } }
        public float H { get { return h; } }
        public float F { get { return g + h; ; } }
        private int m_X;
        private int m_Y;
        private int m_Z;
        public int Z { get { return m_Z; } }
        public int Y { get { return m_Y; } }
        public int X { get { return m_X; } }
        public AStarNode3X(int mapPosX, int mapPosY, int mapPosZ, AStarNodeType nodeType)
        {
            this.m_X = mapPosX;
            this.m_Y = mapPosY;
            this.m_Z = mapPosZ;
            g = h = float.MaxValue / 2;
            this.nodeType = nodeType;
        }

        public void SetDefaultParent(IAStarNode node)
        {
            this.parentNode = node;
        }

        public float TryUpdateFCost(float g, float h, IAStarNode trySetNode)
        {
            if (g + h < F)
            {
                this.g = g;
                this.h = h;
                this.parentNode = trySetNode;
            }
            return F;
        }

        public void Reset()
        {
            g = h = float.MaxValue / 2;
            parentNode = null;
        }
    }

}
