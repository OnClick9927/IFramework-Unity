/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.198
 *UnityVersion:   2019.4.22f1c1
 *Date:           2021-12-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IFramework.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class LoopScrollRect : MonoBehaviour
    {
        private class ItemPool
        {
            private Func<GameObject> create;
            private Stack<GameObject> pool;

            public ItemPool(Func<GameObject> create)
            {
                this.create = create;
                pool = new Stack<GameObject>();
            }

            public GameObject Get()
            {
                GameObject go;
                if (pool.Count > 0)
                {
                    go = pool.Pop();
                }
                else
                {
                    go = create.Invoke();
                }

                go.SetActive(true);
                return go;
            }
            public void Set(GameObject go)
            {
                if (go == null) return;
                if (pool.Contains(go)) return;
                go.gameObject.SetActive(false);
                pool.Push(go);
            }

            public void Clear()
            {
                while (pool.Count > 0)
                {
                    GameObject.Destroy(pool.Pop());
                }
            }
        }
        private class CellDataPool
        {
            private Stack<CellData> pool;

            public CellDataPool()
            {
                pool = new Stack<CellData>();
            }

            public CellData Get(int index)
            {
                CellData go;
                if (pool.Count > 0)
                {
                    go = pool.Pop();
                }
                else
                {
                    go = new CellData();
                }

                go.index = index;
                return go;
            }
            public void Set(CellData go)
            {
                if (go == null) return;
                if (pool.Contains(go)) return;
                go.gameObject = null;
                go.index = -1;
                pool.Push(go);
            }
        }
        private class CellData
        {
            public Vector3 localPosition;
            public GameObject gameObject;
            public int index;
        }
        public enum Direction
        {
            Horizontal,
            Vertical
        }
        private ItemPool pool;
        private CellDataPool dataPool;
        private List<CellData> datas;
        private Vector2 cellSize;
        private ScrollRect scroll;
        private RectTransform content;
        private RectTransform viewport;
        private Action<GameObject, int> freshItem;
        public GameObject item;
        public int count;
        public Direction direction = Direction.Vertical;
        public int columCount = 1;
        public Vector2 spacing = Vector2.zero;


        private void Awake()
        {
            if (!Application.isPlaying) return;
            this.scroll = GetComponent<ScrollRect>();
            content = scroll.content;
            viewport = scroll.viewport;
            if (viewport == null)
                viewport = scroll.GetComponent<RectTransform>();
            scroll.horizontal = direction == Direction.Horizontal;
            scroll.vertical = direction == Direction.Vertical;
            datas = new List<CellData>();
            dataPool = new CellDataPool();
            pool = new ItemPool(CreateNewItem);
            Rect contentRect = content.rect;
            content.pivot = new Vector2(0f, 1f);
            CheckAnchor(content);
            scroll.onValueChanged.AddListener(OnValueChanged);
            SetItem(item);
            SetCount(count);
        }

        private GameObject CreateNewItem()
        {
            if (item == null) return null;
            var cell = Instantiate(item) as GameObject;
            cell.transform.SetParent(item.transform.parent);
            cell.transform.localScale = Vector3.one;
            return cell;
        }
        private void CheckAnchor(RectTransform rectTrans)
        {
            if (direction == Direction.Vertical)
            {
                if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                         (rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(1, 1))))
                {
                    rectTrans.anchorMin = new Vector2(0, 1);
                    rectTrans.anchorMax = new Vector2(1, 1);
                }
            }
            else
            {
                if (!((rectTrans.anchorMin == new Vector2(0, 1) && rectTrans.anchorMax == new Vector2(0, 1)) ||
                         (rectTrans.anchorMin == new Vector2(0, 0) && rectTrans.anchorMax == new Vector2(0, 1))))
                {
                    rectTrans.anchorMin = new Vector2(0, 0);
                    rectTrans.anchorMax = new Vector2(0, 1);
                }
            }
        }

        public void SetFresh(Action<GameObject, int> freshItem)
        {
            this.freshItem = freshItem;
            if (datas == null || item == null) return;
            for (int i = 0, length = datas.Count; i < length; i++)
            {
                CellData data = datas[i];
                if (data.gameObject != null)
                {
                    if (!IsOutRange(data.localPosition))
                    {
                        FreshItem(data);
                    }
                }
            }
        }

        public void ScrollTo(int index)
        {
            if (item == null || datas == null) return;
            index = Mathf.Clamp(index, 0, count);
            var data = datas[index];
            if (direction == Direction.Horizontal)
            {
                scroll.horizontalNormalizedPosition = data.localPosition.x / content.rect.width;
            }
            else
            {
                scroll.verticalNormalizedPosition = data.localPosition.y / content.rect.height;
            }
        }
        public void SetItem(GameObject item)
        {
            if (item == null) return;
            this.item = item;
            pool.Set(item);
            RectTransform cellRectTrans = item.GetComponent<RectTransform>();
            cellRectTrans.pivot = new Vector2(0f, 1f);
            CheckAnchor(cellRectTrans);
            cellRectTrans.anchoredPosition = Vector2.zero;
            cellSize = cellRectTrans.rect.size;
        }
        private bool IsOutRange(Vector3 pos)
        {
            float rangePos = direction == Direction.Vertical ? pos.y : pos.x;
            Vector3 listP = content.anchoredPosition;
            if (direction == Direction.Vertical)
            {
                if (rangePos + listP.y > cellSize.y || rangePos + listP.y < -viewport.rect.height)
                {
                    return true;
                }
            }
            else
            {
                if (rangePos + listP.x < -cellSize.x || rangePos + listP.x > viewport.rect.width)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetCount(int count)
        {
            if (item == null) return;
            for (int i = 0; i < datas.Count; i++)
            {
                pool.Set(datas[i].gameObject);
                dataPool.Set(datas[i]);
            }
            datas.Clear();
            if (direction == Direction.Vertical)
            {
                float contentSize = (spacing.y + cellSize.y) * Mathf.CeilToInt((float)count / columCount);
                contentSize = contentSize < viewport.rect.height ? viewport.rect.height : contentSize;
                content.sizeDelta = new Vector2(content.sizeDelta.x, contentSize);
                if (count != this.count)
                {
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
                }
            }
            else
            {
                float contentSize = (spacing.x + cellSize.x) * Mathf.CeilToInt((float)count / columCount);
                contentSize = contentSize < viewport.rect.width ? viewport.rect.width : contentSize;
                content.sizeDelta = new Vector2(contentSize, content.sizeDelta.y);
                if (count != this.count)
                {
                    content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
                }
            }
            for (int i = 0; i < count; i++)
            {
                CellData data = dataPool.Get(i);
                datas.Add(data);

                if (direction == Direction.Vertical)
                {
                    float pos = cellSize.y * Mathf.FloorToInt(i / columCount) + spacing.y * Mathf.FloorToInt(i / columCount);
                    float rowPos = cellSize.x * (i % columCount) + spacing.x * (i % columCount);
                    data.localPosition = new Vector3(rowPos, -pos, 0);
                }
                else
                {
                    float pos = cellSize.x * Mathf.FloorToInt(i / columCount) + spacing.x * Mathf.FloorToInt(i / columCount);
                    float rowPos = cellSize.y * (i % columCount) + spacing.y * (i % columCount);
                    data.localPosition = new Vector3(pos, -rowPos, 0);
                }
                if (!IsOutRange(data.localPosition))
                {
                    FreshItem(data);
                }

            }
            this.count = count;
        }

        private Queue<CellData> scrollInRange = new Queue<CellData>();
        private void OnValueChanged(Vector2 value)
        {
            if (item == null) return;

            if (datas == null)
                return;

            for (int i = 0, length = count; i < length; i++)
            {
                CellData data = datas[i];
                GameObject obj = data.gameObject;
                Vector3 pos = data.localPosition;
                if (IsOutRange(data.localPosition))
                {
                    if (obj != null)
                    {
                        pool.Set(obj);
                        datas[i].gameObject = null;
                    }
                }
                else
                {
                    scrollInRange.Enqueue(data);
                }
            }
            while (scrollInRange.Count > 0)
            {
                CellData data = scrollInRange.Dequeue();
                if (data.gameObject == null)
                {
                    FreshItem(data);
                }
            }

        }

        protected void OnDestroy()
        {
            SetCount(0);
            pool.Clear();
        }

        private void FreshItem(CellData data)
        {
            if (data.gameObject == null)
            {
                data.gameObject = pool.Get();
            }
            data.gameObject.transform.localPosition = data.localPosition;
            if (this.freshItem != null)
            {
                this.freshItem.Invoke(data.gameObject, data.index);
            }
        }
    }
}
