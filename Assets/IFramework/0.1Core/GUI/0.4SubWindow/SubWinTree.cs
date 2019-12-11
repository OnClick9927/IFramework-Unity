/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using IFramework.Serialization;

namespace IFramework.GUITool
{
    public class SubWinTree
    {
        private class Styles
        {
            public static GUIStyle FlowWindow = "window";
            public static GUIStyle SelectRect = "SelectionRect";
            public static GUIStyle DockArea = "LODBlackBox";

        }
        public enum DockType
        {
            Down, Up, Left, Right
        }
        private class TreeRoot : TreeTrunk
        {
            private SubWinTree m_tree;
            protected override SubWinTree tree { get { return m_tree; } }
            public override int depth { get { return 0; } }

            internal TreeRoot(SubWinTree tree) { isRoot = true; this.m_tree = tree; }

            internal override Vector2 CalcMinsize()
            {
                if (isEmpty)
                    return Vector2.zero;
                return base.CalcMinsize();
            }
            internal override void LegalSplit()
            {
                if (isEmpty) return;
                base.LegalSplit();
            }
            public override void CalcPosition(Rect position)
            {
                if (isEmpty) return;
                base.CalcPosition(position);
            }
            internal void Clear()
            {
                trunks.Clear();
                leafs.Clear();
            }
        }
        internal class TreeTrunk : TreeNode
        {
            private const int maxNodeCount = 2;
            private const float ruleRectWidth = 4;
            protected SplitType splitType;
            protected bool isEmpty { get { return trunkCount == 0 && leafCount == 0; } }
            protected int leafCount { get { return leafs.Count; } }
            protected int trunkCount { get { return trunks.Count; } }
            private bool hasChildTrunks { get { return trunks.Count == maxNodeCount; } }
            private TreeTrunk parentTrunk { get { return parent as TreeTrunk; } }

            protected List<TreeLeaf> leafs = new List<TreeLeaf>();
            protected List<TreeTrunk> trunks = new List<TreeTrunk>();

            internal void DockLeaf(TreeLeaf underLeaf, TreeLeaf dockLeaf, DockType dockType)
            {
                if (hasChildTrunks) Log.E("Err Tree");
                else
                {
                    if (leafCount == 0 || leafCount == 1)
                    {
                        dockLeaf.parent = this;
                        switch (dockType)
                        {
                            case DockType.Down:
                                leafs.Add(dockLeaf);
                                splitType = SplitType.Horizontal;
                                break;
                            case DockType.Up:
                                leafs.Insert(0, dockLeaf);
                                splitType = SplitType.Horizontal;
                                break;
                            case DockType.Left:
                                leafs.Insert(0, dockLeaf);
                                splitType = SplitType.Vertical;
                                break;
                            case DockType.Right:
                                leafs.Add(dockLeaf);
                                splitType = SplitType.Vertical;
                                break;
                        }
                    }
                    else
                    {
                        int index = 0;
                        for (; index < leafCount; index++)
                            if (leafs[index] == underLeaf)
                                break;
                        if (index == 2) Log.E("Err Tree");
                        else
                        {
                            trunks.Add(new TreeTrunk() { parent = this });
                            trunks.Add(new TreeTrunk() { parent = this });
                            TreeLeaf leaf0 = leafs[0];
                            TreeLeaf leaf1 = leafs[1];
                            leafs.Clear();
                            DockType orgDockType = DockType.Down;
                            switch (dockType)
                            {
                                case DockType.Down: orgDockType = DockType.Up; break;
                                case DockType.Up: orgDockType = DockType.Down; break;
                                case DockType.Left: orgDockType = DockType.Right; break;
                                case DockType.Right: orgDockType = DockType.Left; break;
                            }
                            if (index == 0)
                            {
                                trunks[0].DockLeaf(leaf0, orgDockType);
                                trunks[0].DockLeaf(dockLeaf, dockType);
                                trunks[1].DockLeaf(leaf1, orgDockType);
                            }
                            else
                            {
                                trunks[0].DockLeaf(leaf0, orgDockType);
                                trunks[1].DockLeaf(dockLeaf, dockType);
                                trunks[1].DockLeaf(leaf1, orgDockType);
                            }
                        }
                    }
                }
            }
            internal void DockLeaf(TreeLeaf leaf, DockType dockType)
            {
                if (hasChildTrunks)
                {
                    trunks[0].DockLeaf(leaf, dockType);
                }
                else if (!hasChildTrunks)
                {
                    if (leafCount == 0 || leafCount == 1)
                    {
                        leaf.parent = this;
                        switch (dockType)
                        {
                            case DockType.Down:
                                leafs.Add(leaf);
                                splitType = SplitType.Horizontal;
                                break;
                            case DockType.Up:
                                leafs.Insert(0, leaf);
                                splitType = SplitType.Horizontal;
                                break;
                            case DockType.Left:
                                leafs.Insert(0, leaf);
                                splitType = SplitType.Vertical;
                                break;
                            case DockType.Right:
                                leafs.Add(leaf);
                                splitType = SplitType.Vertical;
                                break;
                        }
                    }
                    else
                    {
                        trunks.Add(new TreeTrunk() { parent = this });
                        trunks.Add(new TreeTrunk() { parent = this });
                        TreeLeaf leaf0 = leafs[0];
                        TreeLeaf leaf1 = leafs[1];
                        leafs.Clear();
                        switch (dockType)
                        {
                            case DockType.Down:
                            case DockType.Up:
                                splitType = SplitType.Horizontal;
                                break;
                            case DockType.Left:
                            case DockType.Right:
                                splitType = SplitType.Vertical;

                                break;
                        }
                        switch (splitType)
                        {
                            case SplitType.Vertical:
                                trunks[0].DockLeaf(leaf0, DockType.Left);
                                trunks[1].DockLeaf(leaf1, DockType.Right);
                                break;
                            case SplitType.Horizontal:
                                trunks[0].DockLeaf(leaf0, DockType.Up);
                                trunks[1].DockLeaf(leaf1, DockType.Down);
                                break;
                        }
                        trunks[0].DockLeaf(leaf, dockType);
                    }
                }
            }
            internal bool RemoveLeaf(TreeLeaf leaf)
            {
                if (hasChildTrunks)
                {
                    if (trunks[0].RemoveLeaf(leaf))
                        return true;
                    return trunks[1].RemoveLeaf(leaf);
                }
                else
                {
                    if (!leafs.Contains(leaf)) return false;
                    leafs.Remove(leaf);
                    leaf.parent = null;
                    if (leafCount == 1)
                    {
                        if (!isRoot)
                        {
                            int index = 0;
                            TreeTrunk brother = default(TreeTrunk);
                            for (; index < parentTrunk.trunkCount; index++)
                                if (parentTrunk.trunks[index] != this)
                                {
                                    brother = parentTrunk.trunks[index];
                                    break;
                                }
                            if (brother != null && !brother.hasChildTrunks && brother.leafCount == 1)
                            {
                                parentTrunk.trunks.Clear();
                                switch (parentTrunk.splitType)
                                {
                                    case SplitType.Vertical:
                                        if (index == 0)
                                        {
                                            parentTrunk.DockLeaf(brother.leafs[0], DockType.Left);
                                            parentTrunk.DockLeaf(leafs[0], DockType.Right);
                                        }
                                        else
                                        {
                                            parentTrunk.DockLeaf(brother.leafs[0], DockType.Right);
                                            parentTrunk.DockLeaf(leafs[0], DockType.Left);
                                        }
                                        break;
                                    case SplitType.Horizontal:
                                        if (index == 0)
                                        {
                                            parentTrunk.DockLeaf(brother.leafs[0], DockType.Up);
                                            parentTrunk.DockLeaf(leafs[0], DockType.Down);
                                        }
                                        else
                                        {
                                            parentTrunk.DockLeaf(brother.leafs[0], DockType.Down);
                                            parentTrunk.DockLeaf(leafs[0], DockType.Up);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else if (leafCount == 0)
                    {
                        if (!isRoot)
                        {
                            TreeTrunk brother = default(TreeTrunk);
                            for (int i = 0; i < parentTrunk.trunkCount; i++)
                                if (parentTrunk.trunks[i] != this)
                                    brother = parentTrunk.trunks[i];
                            parentTrunk.trunks.Clear();

                            if (brother != null)
                            {
                                if (brother.hasChildTrunks)
                                {
                                    parentTrunk.splitType = brother.splitType;
                                    for (int i = 0; i < brother.trunkCount; i++)
                                    {
                                        brother.trunks[i].parent = parentTrunk;
                                        parentTrunk.trunks.Add(brother.trunks[i]);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < brother.leafCount; i++)
                                    {
                                        TreeLeaf tmpLeaf = brother.leafs[i];
                                        tmpLeaf.parent = null;
                                        switch (brother.splitType)
                                        {
                                            case SplitType.Vertical:
                                                if (i == 0) parentTrunk.DockLeaf(tmpLeaf, DockType.Left);
                                                else if (i == 1) parentTrunk.DockLeaf(tmpLeaf, DockType.Right);
                                                break;
                                            case SplitType.Horizontal:
                                                if (i == 0) parentTrunk.DockLeaf(tmpLeaf, DockType.Up);
                                                else if (i == 1) parentTrunk.DockLeaf(tmpLeaf, DockType.Down);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        //parent = null;
                    }
                    return true;
                }
            }


            protected float splitWidth = 150;
            private Rect splitRect;
            public override void CalcPosition(Rect position)
            {
                base.CalcPosition(position);
                if (!hasChildTrunks && leafCount == 1)
                    leafs[0].CalcPosition(position);
                else
                {
                    Rect[] rs = default(Rect[]);

                    rs = position.Split(splitType,splitWidth, ruleRectWidth, true);
                    splitRect = position.SplitRect(splitType,splitWidth, ruleRectWidth);
                    if (hasChildTrunks)
                    {
                        trunks[0].CalcPosition(rs[0]);
                        trunks[1].CalcPosition(rs[1]);
                    }
                    else
                    {
                        leafs[0].CalcPosition(rs[0]);
                        leafs[1].CalcPosition(rs[1]);
                    }
                }
            }

            internal virtual Vector2 CalcMinsize()
            {
                if (!hasChildTrunks && leafCount == 1)
                    m_minSize = leafs[0].minSize;
                else
                {
                    Vector2 v0 = hasChildTrunks ? trunks[0].CalcMinsize() : leafs[0].minSize;
                    Vector2 v1 = hasChildTrunks ? trunks[1].CalcMinsize() : leafs[1].minSize;
                    switch (splitType)
                    {
                        case SplitType.Vertical:
                            m_minSize = new Vector2(v0.x + v1.x, v0.y > v1.y ? v0.y : v1.y);
                            break;
                        case SplitType.Horizontal:
                            m_minSize = new Vector2(v0.x > v1.x ? v0.x : v1.x, v0.y + v1.y);
                            break;
                    }
                }
                return minSize;
            }
            internal virtual void LegalSplit()
            {
                if (!hasChildTrunks && leafCount == 1) return;
                Vector2 v0 = hasChildTrunks ? trunks[0].minSize : leafs[0].minSize;
                Vector2 v1 = hasChildTrunks ? trunks[1].minSize : leafs[1].minSize;
                float spRangeL = 0;
                float spRangeR = 0;
                switch (splitType)
                {
                    case SplitType.Vertical:
                        spRangeL = v0.x;
                        spRangeR = position.width - v1.x;
                        break;
                    case SplitType.Horizontal:
                        spRangeL = v0.y;
                        spRangeR = position.height - v1.y;
                        break;
                }
                splitWidth = Mathf.Clamp(splitWidth, spRangeL, spRangeR);
                if (hasChildTrunks)
                    for (int i = 0; i < trunkCount; i++)
                        trunks[i].LegalSplit();
            }


            public override void OnGUI()
            {
                if (!tree.isLocked && !tree.IsDargingWindow)
                ResizeSplit();
                if (hasChildTrunks)
                    for (int i = 0; i < trunks.Count; i++)
                        trunks[i].OnGUI();
                else
                    for (int i = 0; i < leafs.Count; i++)
                        leafs[i].OnGUI();
            }
            private bool dragging;
            private static string DragName;
            private void ResizeSplit()
            {
                if (!position.Contains(Event.current.mousePosition)) return;
                if (Event.current.button != 0) return;
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if (splitRect.Contains(Event.current.mousePosition))
                        {
                            tree.IsResizeWindow = true;
                            dragging = true;
                            DragName = uuid;
                        }
                        break;
                    case EventType.MouseDrag:
                        if (dragging && DragName == uuid)
                        {
                            switch (splitType)
                            {
                                case SplitType.Vertical:
                                    splitWidth += Event.current.delta.x;
                                    break;
                                case SplitType.Horizontal:
                                    splitWidth += Event.current.delta.y;
                                    break;
                            }
                            tree.Repaint();
                        }
                        break;
                    case EventType.MouseUp:
                        if (dragging)
                        {
                            DragName = string.Empty;
                            dragging = false;
                            tree.IsResizeWindow = false;
                        }
                        break;
                }
                tree.AddCursorRect(splitRect, splitType);
            }


            public override XmlElement Serialize(XmlDocument doc)
            {
                XmlElement root = base.Serialize(doc);
                SerializeField(root, "SplitType", splitType);
                SerializeField(root, "SplitWidth", splitWidth);

                XmlElement ts = doc.CreateElement("Trunks");
                XmlElement ls = doc.CreateElement("Leafs");
                for (int i = 0; i < trunkCount; i++)
                    ts.AppendChild(trunks[i].Serialize(doc));
                for (int i = 0; i < leafCount; i++)
                    ls.AppendChild(leafs[i].Serialize(doc));
                root.AppendChild(ts);
                root.AppendChild(ls);
                return root;
            }
            public override void DeSerialize(XmlElement root)
            {
                base.DeSerialize(root);
                DeSerializeField(root, "SplitType",ref splitType);
                DeSerializeField(root, "SplitWidth",ref splitWidth);
                XmlNode ts = root.SelectSingleNode("Trunks");
                for (int i = 0; i < ts.ChildNodes.Count; i++)
                {
                    TreeTrunk trunk = new TreeTrunk();
                    trunk.parent = this;
                    trunk.DeSerialize(ts.ChildNodes[i] as XmlElement);
                    trunks.Add(trunk);
                }
                XmlNode ls = root.SelectSingleNode("Leafs");
                for (int i = 0; i < ls.ChildNodes.Count; i++)
                {
                    TreeLeaf leaf = tree.CreateOpenedLeaf(null);
                    leaf.parent = this;
                    leaf.DeSerialize(ls.ChildNodes[i] as XmlElement);
                    leafs.Add(leaf);
                }
            }
        }
        public class TreeLeaf : TreeNode
        {
            private class Styles
            {
                public static GUIStyle DockArea = "dockarea";
                public static GUIStyle TitileContent = "dragtabdropwindow";
                public static GUIStyle CloseBtn = "WinBtnClose";
                public static GUIStyle ContentBG = "IN BigTitle Inner";
                public const float ContentHeight = 17;
                public const float CloseBtnSize = 17;
                public const float minize = 120;
                public const float TitleDragWidth = 100;
                public const float ContentZoomIn = -10;
            }
            public GUIContent titleContent { get; set; }
            private Rect titleRect;
            public event Action<Rect> paintDelegate;
            public override Vector2 minSize
            {
                get { return m_minSize; }
                set
                {
                    m_minSize = value;
                    if (m_minSize.x < Styles.minize) m_minSize.x = Styles.minize;
                    if (m_minSize.y < Styles.minize) m_minSize.y = Styles.minize;
                }
            }
            public string userData;
            internal TreeLeaf(GUIContent title) : base() { m_minSize = new Vector2(Styles.minize, Styles.minize); this.titleContent = title; }

            internal bool IsOverTitle(Vector2 vector2) { return titleRect.Contains(vector2); }

            public override void OnGUI()
            {
                if (tree.isShowTitle )
                {
                    var rs = m_position.HorizontalSplit(Styles.ContentHeight);
                    titleRect = new Rect(rs[0].position, new Vector2(Styles.TitleDragWidth, Styles.ContentHeight));
                    DrawTitle(rs[0]);
                    DrawContent(rs[1]);
                }
                else
                    DrawContent(m_position);
            }
            private void DrawTitle(Rect rect)
            {
                if (Event.current.type == EventType.Repaint)
                        Styles.TitileContent.Draw(rect, titleContent, false, false, false, false);
                Rect r = new Rect(rect.xMax - Styles.CloseBtnSize, rect.y, Styles.CloseBtnSize, Styles.CloseBtnSize);
                if (GUI.Button(r, string.Empty, Styles.CloseBtn))
                    tree.CloseLeaf(this);
            }
            private void DrawContent(Rect rect)
            {
                if (Event.current.type == EventType.Repaint)
                    Styles.ContentBG.Draw(rect, false, false, false, false);

                if (paintDelegate != null)
                {
                    GUI.BeginClip(rect);
                    paintDelegate(new Rect(Vector2.zero,rect.size).Zoom(AnchorType.MiddleCenter, Styles.ContentZoomIn));
                    GUI.EndClip();
                }
            }

            public override XmlElement Serialize(XmlDocument doc)
            {
                XmlElement root = base.Serialize(doc);
                SerializeField(root, "UserData", userData);
                return root;
            }
            public override void DeSerialize(XmlElement root)
            {
                base.DeSerialize(root);
                DeSerializeField(root, "UserData",ref userData);
            }
        }
        public abstract class TreeNode
        {
            protected Vector2 m_minSize;
            protected bool isRoot;
            protected Rect m_position;
            public string uuid;

            internal TreeNode parent { get; set; }
            public Rect position { get { return m_position; } set { m_position = value; } }
            public virtual int depth { get { return parent.depth + 1; } }
            protected virtual SubWinTree tree { get { return parent.tree; } }
            public virtual Vector2 minSize { get { return m_minSize; }set { m_minSize = value; } }

            public TreeNode() { this.uuid = Guid.NewGuid().ToString(); }

            public bool IsOver(Vector2 v) { return m_position.Contains(v); }
            public virtual void CalcPosition(Rect position) { this.m_position = position; }

            public abstract void OnGUI();

            public virtual XmlElement Serialize(XmlDocument doc)
            {
                XmlElement root = doc.CreateElement(GetType().Name);
                SerializeField(root, "uuid", uuid);
                SerializeField(root, "MinSize", m_minSize);
                SerializeField(root, "Position", position);
                SerializeField(root, "IsRoot", isRoot);
                return root;
            }
            public virtual void DeSerialize(XmlElement root)
            {
                DeSerializeField(root, "uuid", ref uuid);
                DeSerializeField(root, "MinSize", ref m_minSize);
                DeSerializeField(root, "Position", ref m_position);
                DeSerializeField(root, "IsRoot", ref isRoot);
            }
        }

        private TreeRoot root;

        public bool isShowTitle=true;
        public bool isLocked = false;
        public Vector2 minSize { get { return root.minSize; } }
        public Rect position { get { return root.position; } }
        public int allLeafCount { get { return allLeafs.Count; } }
        public int closedLeafCount { get { return closedLeafs.Count; } }

        public event Action repaintEve;
        public event Action<Rect, SplitType> drawCursorEve;
        public event Action<TreeLeaf> onCreatLeaf;
        public event Action<TreeLeaf> onDockLeaf;
        public event Action<TreeLeaf> onCloseLeaf;
        public event Action<TreeLeaf> onClearLeaf;
        public event Action onResizeWindow;
        public event Action onEndResizeWindow;
        public event Action onDragWindow;
        public event Action onEndDragWindow;

        public TreeLeaf this[string userData]
        {
            get {
                for (int i = 0; i < allLeafs.Count; i++)
                {
                    if (allLeafs[i].userData== userData)
                    {
                        return allLeafs[i];
                    }
                }
                return null;
            }
        }
        private bool IsLeafOpen(TreeLeaf leaf)
        {
            return !closedLeafs.Contains(leaf) && allLeafs.Contains(leaf);
        }

        public readonly List<TreeLeaf> allLeafs = new List<TreeLeaf>();
        public readonly List<TreeLeaf> closedLeafs = new List<TreeLeaf>();

        private void Repaint()
        {
            if (repaintEve != null)
                repaintEve();
            if (Event.current!=null)
                Event.current.Use();
        }
        private void AddCursorRect(Rect rect, SplitType splitType)
        {
            if (drawCursorEve != null)
                drawCursorEve(rect, splitType);
        }

        public SubWinTree(){ root = new TreeRoot(this);}

        private long index = 0;
        public TreeLeaf CreateLeaf(GUIContent title)
        {
            if (title==null)
                title= new GUIContent("window ".Append((++index).ToString("X3")));
            TreeLeaf leaf = new TreeLeaf(title);
            if (onCreatLeaf != null)
            {
                onCreatLeaf(leaf);
            }
            closedLeafs.Add(leaf);
            allLeafs.Add(leaf);
            return leaf;
        }
        private TreeLeaf CreateOpenedLeaf(GUIContent title)
        {
            TreeLeaf leaf = CreateLeaf(title);
            closedLeafs.Remove(leaf);
            return leaf;
        }

        public void DockLeaf(TreeLeaf leaf, DockType dockType)
        {
            DockLeaf(root, leaf, dockType);
        }
        public void DockLeaf(TreeLeaf underLeaf, TreeLeaf dockLeaf, DockType dockType)
        {
            DockLeaf((underLeaf.parent as TreeTrunk), dockLeaf, dockType);
        }
        private void DockLeaf(TreeTrunk trunk, TreeLeaf leaf, DockType dockType)
        {
            if (!closedLeafs.Contains(leaf))
            {
                Log.E("Err: Have Exist");
            }
            else
            {
                trunk.DockLeaf(leaf, dockType);
                closedLeafs.Remove(leaf);
                if (onDockLeaf != null) onDockLeaf(leaf);
                Repaint();
            }
        }
        public void CloseLeaf(TreeLeaf leaf)
        {
            root.RemoveLeaf(leaf);
            closedLeafs.Add(leaf);
            if (onCloseLeaf != null) onCloseLeaf(leaf);
        }
        public void ClearLeafs()
        {
            index = 0;
            closedLeafs.Clear();
            for (int i = allLeafs.Count-1; i >=0; i--)
            {
                TreeLeaf leaf = allLeafs[i];
                allLeafs.Remove(leaf);
                if (onClearLeaf != null)
                {
                    onClearLeaf(leaf);
                }
            }
            root.Clear();
        }


        public void OnGUI(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
                Styles.DockArea.Draw(position, false, false, false, false);
            if (!isLocked) Resize(position);
            root.OnGUI();
            if (isShowTitle && !isLocked) DragWindow();
        }
        private void Resize(Rect rect)
        {
            Vector2 tmp = root.CalcMinsize();

            root.CalcPosition(rect);
            if (tmp.x > root.position.width || tmp.y > root.position.height)
                rect = rect.Set(root.position.position, new Vector2(
                                                             Math.Max(tmp.x, root.position.width),
                                                             Math.Max(tmp.y, root.position.height)
                                                            ));
            root.LegalSplit();
            root.CalcPosition(rect);
        }
        private bool m_IsResizeWindow;
        public bool IsResizeWindow
        {
            get
            {
                return m_IsResizeWindow;
            }
            set
            {
                m_IsResizeWindow = value;
                if (value)
                {
                    if (onResizeWindow != null) onResizeWindow();
                }
                else
                {
                    if (onEndResizeWindow != null) onEndResizeWindow();
                }
            }
        }
        public bool IsDargingWindow { get { return dragleaf != null; } }
        private TreeLeaf dragleaf { get { return m_dragleaf; }set {

                m_dragleaf = value;
                if (value == null)
                {
                    if (onEndDragWindow != null) onEndDragWindow();
                }
                else
                {
                    if (onDragWindow != null) onDragWindow();
                }
            } }
        private TreeLeaf m_dragleaf;

        private DockType docktype;
        private Rect selectionRect;
        private void DragWindow()
        {
            List<TreeLeaf> OpenLeafs = allLeafs.FindAll((leaf) => { return !closedLeafs.Contains(leaf); });
            Event e = Event.current;
            if (e.button != 0) return;
            if (!position.Contains(e.mousePosition)) return;

            if (e.type == EventType.Repaint && dragleaf != null)
            {
                Styles.FlowWindow.Draw(new Rect(e.mousePosition, Vector2.one * 150), dragleaf.titleContent, false, false, false, false);
                Styles.SelectRect.Draw(selectionRect, false, false, false, false);
            }
            if (e.type == EventType.MouseDrag)
            {
                if (dragleaf == null)
                    dragleaf = OpenLeafs.Find((leaf) => { return leaf.IsOverTitle(e.mousePosition); });
                if (dragleaf == null) return;
                var overLeaf = OpenLeafs.Find((leaf) => { return leaf.IsOver(e.mousePosition); });
                if (overLeaf == null)
                {
                    selectionRect = Rect.zero;
                    return;
                }
                Vector2 v = e.mousePosition;
                Rect r = overLeaf.position;
                float spH = r.height / 3;
                float spW = r.width / 3;
                if (v.y < r.yMin + spH &&
                        v.x > r.xMin + spW &&
                        v.x < r.xMax - spW)
                {
                    selectionRect = new Rect(r.position, new Vector2(r.width, spH));
                    docktype = DockType.Up;
                }
                else if (v.y > r.yMax - spH &&
                            v.x > r.xMin + spW &&
                            v.x < r.xMax - spW)
                {
                    selectionRect = new Rect(new Vector2(r.x, r.yMax - spH),
                                        new Vector2(r.width, spH));
                    docktype = DockType.Down;
                }
                else if (v.x > r.xMax - spW &&
                            v.y < r.yMax - spH &&
                            v.y > r.yMin + spH)
                {
                    selectionRect = new Rect(new Vector2(r.xMax - spW, r.y),
                                         new Vector2(spW, r.height));
                    docktype = DockType.Right;
                }
                else if (v.x < r.xMin + spW &&
                            v.y < r.yMax - spH &&
                            v.y > r.yMin + spH)
                {
                    selectionRect = new Rect(r.position, new Vector2(spW, r.height));
                    docktype = DockType.Left;
                }
                else
                {
                    selectionRect = Rect.zero;
                }
                Repaint();
            }
            else if (e.type == EventType.MouseUp)
            {
                if (dragleaf != null)
                {
                    var overLeaf = OpenLeafs.Find((leaf) => { return leaf.IsOver(e.mousePosition); });
                    if (overLeaf != null && selectionRect != Rect.zero && overLeaf != dragleaf)
                    {
                        root.RemoveLeaf(dragleaf);
                        TreeTrunk node = overLeaf.parent as TreeTrunk;
                        node.DockLeaf(overLeaf, dragleaf, docktype);
                    }
                    dragleaf = null;
                    selectionRect = Rect.zero;
                    Repaint();
                }
            }
        }


        public string Serialize(string name="Temp")
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Tree");
            root.SetAttribute("Name", name);

            SerializeField(root, "IsShowTitle", isShowTitle);
            SerializeField(root, "IsLock", isLocked);

            XmlElement Closed = doc.CreateElement("ClosedLeafs");
            for (int i = 0; i < closedLeafs.Count; i++)
                Closed.AppendChild(closedLeafs[i].Serialize(doc));
            root.AppendChild(Closed);

            root.AppendChild(this.root.Serialize(doc));
            doc.AppendChild(root);
            return doc.InnerXml;
        }
        public XmlElement Serialize(XmlDocument doc,string name)
        {
            XmlElement root = doc.CreateElement("Tree");
            root.SetAttribute("Name", name);

            SerializeField(root, "IsShowTitle", isShowTitle);
            SerializeField(root, "IsLock", isLocked);

            XmlElement Closed = doc.CreateElement("ClosedLeafs");
            for (int i = 0; i < closedLeafs.Count; i++)
                Closed.AppendChild(closedLeafs[i].Serialize(doc));
            root.AppendChild(Closed);

            root.AppendChild(this.root.Serialize(doc));
            return root;
        }

        public void DeSerialize(string xmlStr)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);
            DeSerialize(doc.DocumentElement);
        }
        public void DeSerialize(XmlElement treeEle)
        {
            ClearLeafs();
            DeSerializeField(treeEle, "IsShowTitle",ref isShowTitle);
            DeSerializeField(treeEle, "IsLock",ref isLocked);

            ReadClosedLeafs(treeEle.SelectSingleNode("ClosedLeafs") as XmlElement);
            root.DeSerialize(treeEle.SelectSingleNode("TreeRoot") as XmlElement);
        }
        private void ReadClosedLeafs(XmlElement root)
        {
            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement ele = root.ChildNodes[i] as XmlElement;
                TreeLeaf leaf = CreateLeaf(null);
                leaf.DeSerialize(ele);
                DockLeaf(leaf, DockType.Down);
                CloseLeaf(leaf);
            }
        }

        private static XmlElement SerializeField<T>(XmlElement root, string name, T value)
        {
            try
            {
                XmlElement ele = root.OwnerDocument.CreateElement(name);
                if (value != null)
                {
                    Type type = value.GetType();
                    XmlNode node = root.OwnerDocument.CreateTextNode(StringConvert.ConvertToString(value, type));
                    ele.AppendChild(node);
                    root.AppendChild(ele);
                }
                else if (name != "UserData")
                {
                    Log.W("SubWin    "  + name + "  value is mull,will not Serializate");
                }
                return root;
            }
            catch (Exception)
            {
                throw new Exception(name);
            }

        }
        private static void DeSerializeField<T>(XmlElement root, string name, ref T obj)
        {
            try
            {
                T obj1;
                XmlNode node = root.SelectSingleNode(name);
                if (node != null)
                {
                    StringConvert.TryConvert(node.InnerText, out obj1);
                    obj = obj1;
                }
                else if (name!= "UserData")
                {
                    Log.W( "SubWin    "+name + "   NotFind");
                }
            }
            catch
            {
                throw new Exception( name );
            }
        }
    }

}
