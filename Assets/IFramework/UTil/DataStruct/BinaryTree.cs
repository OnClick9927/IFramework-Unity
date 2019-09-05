/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-06-26
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
   

    public class BinaryTree<T> where T :IComparable
	{
        public class BinaryTreeNode
        {
            public T data;
            public BinaryTreeNode left;
            public BinaryTreeNode right;
        }
        BinaryTreeNode root=new BinaryTreeNode();
        public int maxdepth { get { return root == null ? 0 : GetMaxDepth(root); } }
        public int mindepth { get { return root == null ? 0 : GetMinDepth(root); } }

        public int GetNodeCount(BinaryTreeNode node)
        {
            if (node == null) return 0;
            int left = GetNodeCount(node.left);
            int right = GetNodeCount(node.right);
            return left + right + 1;
        }
        public int GetMaxDepth(BinaryTreeNode node)
        {
            if (node == null) return 0;
            int left = GetMaxDepth(node.left);
            int right = GetMaxDepth(node.right);
            return Math.Max(left, right) + 1;
        }
        public int GetMinDepth(BinaryTreeNode node)
        {
            if (node == null) return int.MaxValue;
            if (node.left == null && node.right == null) return 1;
            return Math.Min(GetMinDepth(node.left), GetMinDepth(node.right)) + 1;
        }
        public int GetLeafCount(BinaryTreeNode node)
        {
            if (node == null) return 0;
            if (node.left == null && node.right == null) return 1;
            return GetLeafCount(node.left) + GetLeafCount(node.right);
        }
        public int GetCountByLevel(BinaryTreeNode node,int lev)
        {
            if (node == null || lev < 1) return 0;
            if (lev == 1) return 1;
            int left = GetCountByLevel(node.left,lev-1);
            int right = GetCountByLevel(node.right,lev-1);
            return left + right;
        }
        public bool Balance(BinaryTreeNode node)
        {
            return MaxDepth(node) != -1;
        }
        public int MaxDepth(BinaryTreeNode node)
        {
            if (node == null) return 0;
            int left = MaxDepth(node.left);
            int right = MaxDepth(node.right);
            if (left == -1 || right == -1 || Math.Abs(left - right) > 1) return -1;
            return Math.Max(left, right) + 1;
        }
       
        public bool IsCompleteTree(BinaryTreeNode node)
        {
            if (node == null) return false;
            Queue<BinaryTreeNode> queue = new Queue<BinaryTreeNode>();
            queue.Enqueue(node);
            bool result = true;
            bool hasNoChild = false;
            while (queue .Count!=0)
            {
                BinaryTreeNode  current = queue.Dequeue();
                if (hasNoChild)
                {
                    if (current.left != null || current.right != null)
                    {
                        result = false;
                        break;
                    }
                }
                else
                {
                    if (current.left != null && current.right != null)
                    {
                        queue.Enqueue(current.left);
                        queue.Enqueue(current.right);
                    }
                    else if (current.left != null && current.right == null)
                    {
                        queue.Enqueue(current.left);
                        hasNoChild = true;

                    }
                    else if (current.left == null && current.right != null)
                    {
                        result = false;
                        break;
                    }
                    else
                    {
                        hasNoChild = true;
                    }
                }

            }
            return result;
        }
        bool IsSameTree(BinaryTreeNode t1, BinaryTreeNode t2)
        {
            if (t1 == null && t2 == null) return true;
            else if (t1 == null || t2 == null) return false;
            if (!t1.data .Equals( t2.data)) return false;
            bool left = IsSameTree(t1.left, t2.left);
            bool right = IsSameTree(t1.right, t2.right);
            return left && right;

        }
        bool IsMirror(BinaryTreeNode t1, BinaryTreeNode t2)
        {
            if (t1 == null && t2 == null) return true;
            if (t1 == null || t2 == null) return false;
            if (!t1.data.Equals(t2.data)) return false;
            return IsMirror(t1.left, t2.right) && IsMirror(t1.right, t2.left);

        }

        public BinaryTreeNode MirrorTree(BinaryTreeNode root)
        {
            if (root == null) return null;
            BinaryTreeNode left = MirrorTree(root.left);
            BinaryTreeNode right = MirrorTree(root.right);
            root.left = right;
            root.right = left;
            return root;
        }


        BinaryTreeNode getLastCommonParent(BinaryTreeNode root, BinaryTreeNode t1, BinaryTreeNode t2)
        {
            if (HasChild(root.left, t1))
            {
                if (HasChild(root.right, t2)) return root;
                return getLastCommonParent(root.left, t1, t2);
            }
            else
            {
                if (HasChild(root.left, t2)) return root;
                return getLastCommonParent(root.right, t1, t2);
            }
        }
        public bool HasChild(BinaryTreeNode root, BinaryTreeNode node)
        {
            if (root == null || node == null) return false;
            if (root == node) return true;
            bool found = HasChild(root.left, node);
            if (!found) found = HasChild(root.right, node);
            return found;
        }


        public List<T> preOrder(BinaryTreeNode root)
        {
            Stack<BinaryTreeNode> stack = new Stack<BinaryTreeNode>();
            List<T> list = new List<T>();
            if (root == null) return list;
            stack.Push(root);
            while (stack.Count!=0)
            {
                BinaryTreeNode node = stack.Pop();
                list.Add(node.data);
                if (node.right != null) stack.Push(node.right);
                if (node.left != null) stack.Push(node.left);
            }
            return list;
        }
        public List<T> inOrder(BinaryTreeNode root)
        {
            List<T> list = new List<T>();
            Stack<BinaryTreeNode> stack = new Stack<BinaryTreeNode>();
            BinaryTreeNode current = root;
            while (current != null || stack.Count !=0)
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = current.left;
                }
                current = stack.Peek();
                stack.Pop();
                list.Add(current.data);
                current = current.right;

            }
            return list;

        }

        public List<T> postOrder(BinaryTreeNode root)
        {
            List<T> list = new List<T>();
            if (root == null)
            {
                return list;
            }
            list.AddRange(postOrder(root.left));
            list.AddRange(postOrder(root.right));
            list.Add(root.data);
            return list;
        }


       public  BinaryTreeNode insertNode(BinaryTreeNode root, BinaryTreeNode node)
        {
            if (root == node) return node;
            BinaryTreeNode tmp = root;
            BinaryTreeNode last = null;
            while (tmp != null)
            {
                last = tmp;
                if (tmp.data.CompareTo(node.data) == 1) tmp = tmp.left;
                else tmp = tmp.right;
            }
            if (last != null)
            {
                if (tmp.data.CompareTo(node.data) == 1) last.left = node;
                else last.right = node;
            }
            return root;
        }
    }
}
