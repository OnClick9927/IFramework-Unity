using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace IFramework.UI.SuperScrollView
{
    public class LoopListView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [System.Serializable]
        public class PrefabConfData
        {
            public string path = null;
            public float padding = 0;
            public float offset = 0;
        }
        public class InitParam
        {
            public float distanceForRecycle0 = 300; //distanceForRecycle0 should be larger than distanceForNew0
            public float distanceForNew0 = 200;
            public float distanceForRecycle1 = 300;//distanceForRecycle1 should be larger than distanceForNew1
            public float distanceForNew1 = 200;
            public float smoothDumpRate = 0.3f;
            public float snapFinishThreshold = 0.01f;
            public float snapVecThreshold = 145;
            public float itemDefaultWithPaddingSize = 20;
        }
        public enum ArrangeType
        {
            TopToBottom,
            BottomToTop,
            LeftToRight,
            RightToLeft,
        }
        public enum CornerEnum
        {
            LeftBottom = 0,
            LeftTop,
            RightTop,
            RightBottom,
        }
        private class ItemPool
        {
            string path;
            float padding = 0;
            float offset = 0;
            RectTransform parent = null;
            LoopListView list;
            Queue<LoopListViewItem> pool = new Queue<LoopListViewItem>();
            Func<string, RectTransform, GameObject> create;
            Action<GameObject> set;
            public ItemPool()
            {

            }
            public void Init(LoopListView list, string path, float padding, float offset, RectTransform parent, Func<string, RectTransform, GameObject> create, Action<GameObject> set)
            {
                this.list = list;
                this.create = create;
                this.set = set;
                this.path = path;
                this.padding = padding;
                this.offset = offset;
                this.parent = parent;
            }
            public LoopListViewItem GetItem()
            {
                LoopListViewItem tItem = null;
                if (pool.Count > 0)
                {
                    tItem = pool.Dequeue();
                    tItem.gameObject.SetActive(true);
                }
                else
                {
                    tItem = CreateItem();
                }
                tItem.padding = padding;
                return tItem;
            }
            public LoopListViewItem CreateItem()
            {
                GameObject go = create?.Invoke(path, parent);
                go.SetActive(true);
                RectTransform rf = go.GetComponent<RectTransform>();
                list.AdjustAnchor(rf);
                list.AdjustPivot(rf);
                rf.localScale = Vector3.one;
                rf.localPosition = Vector3.zero;
                rf.localEulerAngles = Vector3.zero;
                LoopListViewItem tViewItem = go.GetComponent<LoopListViewItem>();
                if (tViewItem == null)
                {
                    tViewItem = go.AddComponent<LoopListViewItem>();
                }
                tViewItem.path = path;
                tViewItem.offset = offset;
                return tViewItem;
            }
            void RecycleItemReal(LoopListViewItem item)
            {
                item.gameObject.SetActive(false);
                set?.Invoke(item.gameObject);
            }
            public void RecycleItem(LoopListViewItem item)
            {
                pool.Enqueue(item);
            }
            public void ClearTmpRecycledItem()
            {
                int count = pool.Count;
                if (count == 0)
                {
                    return;
                }
                for (int i = 0; i < count; ++i)
                {
                    RecycleItemReal(pool.Dequeue());
                }
            }
        }

        private enum SnapStatus
        {
            NoTargetSet = 0,
            TargetHasSet = 1,
            SnapMoving = 2,
            SnapMoveFinish = 3
        }
        private class SnapData
        {
            public SnapStatus mSnapStatus = SnapStatus.NoTargetSet;
            public int mSnapTargetIndex = 0;
            public float mTargetSnapVal = 0;
            public float mCurSnapVal = 0;
            public void Clear()
            {
                mSnapStatus = SnapStatus.NoTargetSet;
            }
        }
        private class ItemSizeGroup
        {

            public float[] mItemSizeArray = null;
            public float[] mItemStartPosArray = null;
            public int mItemCount = 0;
            int mDirtyBeginIndex = ItemPosMgr.mItemMaxCountPerGroup;
            public float mGroupSize = 0;
            public float mGroupStartPos = 0;
            public float mGroupEndPos = 0;
            public int mGroupIndex = 0;
            float mItemDefaultSize = 0;
            public ItemSizeGroup(int index, float itemDefaultSize)
            {
                mGroupIndex = index;
                mItemDefaultSize = itemDefaultSize;
                Init();
            }

            public void Init()
            {
                mItemSizeArray = new float[ItemPosMgr.mItemMaxCountPerGroup];
                if (mItemDefaultSize != 0)
                {
                    for (int i = 0; i < mItemSizeArray.Length; ++i)
                    {
                        mItemSizeArray[i] = mItemDefaultSize;
                    }
                }
                mItemStartPosArray = new float[ItemPosMgr.mItemMaxCountPerGroup];
                mItemStartPosArray[0] = 0;
                mItemCount = ItemPosMgr.mItemMaxCountPerGroup;
                mGroupSize = mItemDefaultSize * mItemSizeArray.Length;
                if (mItemDefaultSize != 0)
                {
                    mDirtyBeginIndex = 0;
                }
                else
                {
                    mDirtyBeginIndex = ItemPosMgr.mItemMaxCountPerGroup;
                }
            }

            public float GetItemStartPos(int index)
            {
                return mGroupStartPos + mItemStartPosArray[index];
            }

            public bool IsDirty
            {
                get
                {
                    return (mDirtyBeginIndex < mItemCount);
                }
            }
            public float SetItemSize(int index, float size)
            {
                float old = mItemSizeArray[index];
                if (old == size)
                {
                    return 0;
                }
                mItemSizeArray[index] = size;
                if (index < mDirtyBeginIndex)
                {
                    mDirtyBeginIndex = index;
                }
                float ds = size - old;
                mGroupSize = mGroupSize + ds;
                return ds;
            }

            public void SetItemCount(int count)
            {
                if (mItemCount == count)
                {
                    return;
                }
                mItemCount = count;
                RecalcGroupSize();
            }

            public void RecalcGroupSize()
            {
                mGroupSize = 0;
                for (int i = 0; i < mItemCount; ++i)
                {
                    mGroupSize += mItemSizeArray[i];
                }
            }

            public int GetItemIndexByPos(float pos)
            {
                if (mItemCount == 0)
                {
                    return -1;
                }
                int low = 0;
                int high = mItemCount - 1;
                while (low <= high)
                {
                    int mid = (low + high) / 2;
                    float startPos = mItemStartPosArray[mid];
                    float endPos = startPos + mItemSizeArray[mid];
                    if (startPos <= pos && endPos >= pos)
                    {
                        return mid;
                    }
                    else if (pos > endPos)
                    {
                        low = mid + 1;
                    }
                    else
                    {
                        high = mid - 1;
                    }
                }
                return -1;
            }

            public void UpdateAllItemStartPos()
            {
                if (mDirtyBeginIndex >= mItemCount)
                {
                    return;
                }
                int startIndex = (mDirtyBeginIndex < 1) ? 1 : mDirtyBeginIndex;
                for (int i = startIndex; i < mItemCount; ++i)
                {
                    mItemStartPosArray[i] = mItemStartPosArray[i - 1] + mItemSizeArray[i - 1];
                }
                mDirtyBeginIndex = mItemCount;
            }
        }

        private class ItemPosMgr
        {
            public const int mItemMaxCountPerGroup = 100;
            List<ItemSizeGroup> mItemSizeGroupList = new List<ItemSizeGroup>();
            int mDirtyBeginIndex = int.MaxValue;
            public float mTotalSize = 0;
            public float mItemDefaultSize = 20;

            public ItemPosMgr(float itemDefaultSize)
            {
                mItemDefaultSize = itemDefaultSize;
            }

            public void SetItemMaxCount(int maxCount)
            {
                mDirtyBeginIndex = 0;
                mTotalSize = 0;
                int st = maxCount % mItemMaxCountPerGroup;
                int lastGroupItemCount = st;
                int needMaxGroupCount = maxCount / mItemMaxCountPerGroup;
                if (st > 0)
                {
                    needMaxGroupCount++;
                }
                else
                {
                    lastGroupItemCount = mItemMaxCountPerGroup;
                }
                int count = mItemSizeGroupList.Count;
                if (count > needMaxGroupCount)
                {
                    int d = count - needMaxGroupCount;
                    mItemSizeGroupList.RemoveRange(needMaxGroupCount, d);
                }
                else if (count < needMaxGroupCount)
                {
                    int d = needMaxGroupCount - count;
                    for (int i = 0; i < d; ++i)
                    {
                        ItemSizeGroup tGroup = new ItemSizeGroup(count + i, mItemDefaultSize);
                        mItemSizeGroupList.Add(tGroup);
                    }
                }
                count = mItemSizeGroupList.Count;
                if (count == 0)
                {
                    return;
                }
                for (int i = 0; i < count - 1; ++i)
                {
                    mItemSizeGroupList[i].SetItemCount(mItemMaxCountPerGroup);
                }
                mItemSizeGroupList[count - 1].SetItemCount(lastGroupItemCount);
                for (int i = 0; i < count; ++i)
                {
                    mTotalSize = mTotalSize + mItemSizeGroupList[i].mGroupSize;
                }

            }

            public void SetItemSize(int itemIndex, float size)
            {
                int groupIndex = itemIndex / mItemMaxCountPerGroup;
                int indexInGroup = itemIndex % mItemMaxCountPerGroup;
                ItemSizeGroup tGroup = mItemSizeGroupList[groupIndex];
                float changedSize = tGroup.SetItemSize(indexInGroup, size);
                if (changedSize != 0f)
                {
                    if (groupIndex < mDirtyBeginIndex)
                    {
                        mDirtyBeginIndex = groupIndex;
                    }
                }
                mTotalSize += changedSize;
            }

            public float GetItemPos(int itemIndex)
            {
                Update(true);
                int groupIndex = itemIndex / mItemMaxCountPerGroup;
                int indexInGroup = itemIndex % mItemMaxCountPerGroup;
                return mItemSizeGroupList[groupIndex].GetItemStartPos(indexInGroup);
            }

            public void GetItemIndexAndPosAtGivenPos(float pos, ref int index, ref float itemPos)
            {
                Update(true);
                index = 0;
                itemPos = 0f;
                int count = mItemSizeGroupList.Count;
                if (count == 0)
                {
                    return;
                }
                ItemSizeGroup hitGroup = null;

                int low = 0;
                int high = count - 1;
                while (low <= high)
                {
                    int mid = (low + high) / 2;
                    ItemSizeGroup tGroup = mItemSizeGroupList[mid];
                    if (tGroup.mGroupStartPos <= pos && tGroup.mGroupEndPos >= pos)
                    {
                        hitGroup = tGroup;
                        break;
                    }
                    else if (pos > tGroup.mGroupEndPos)
                    {
                        low = mid + 1;
                    }
                    else
                    {
                        high = mid - 1;
                    }
                }
                int hitIndex = -1;
                if (hitGroup != null)
                {
                    hitIndex = hitGroup.GetItemIndexByPos(pos - hitGroup.mGroupStartPos);
                }
                else
                {
                    return;
                }
                if (hitIndex < 0)
                {
                    return;
                }
                index = hitIndex + hitGroup.mGroupIndex * mItemMaxCountPerGroup;
                itemPos = hitGroup.GetItemStartPos(hitIndex);
            }

            public void Update(bool updateAll)
            {
                int count = mItemSizeGroupList.Count;
                if (count == 0)
                {
                    return;
                }
                if (mDirtyBeginIndex >= count)
                {
                    return;
                }
                int loopCount = 0;
                for (int i = mDirtyBeginIndex; i < count; ++i)
                {
                    loopCount++;
                    ItemSizeGroup tGroup = mItemSizeGroupList[i];
                    mDirtyBeginIndex++;
                    tGroup.UpdateAllItemStartPos();
                    if (i == 0)
                    {
                        tGroup.mGroupStartPos = 0;
                        tGroup.mGroupEndPos = tGroup.mGroupSize;
                    }
                    else
                    {
                        tGroup.mGroupStartPos = mItemSizeGroupList[i - 1].mGroupEndPos;
                        tGroup.mGroupEndPos = tGroup.mGroupStartPos + tGroup.mGroupSize;
                    }
                    if (!updateAll && loopCount > 1)
                    {
                        return;
                    }

                }
            }

        }
        private Func<LoopListView, int, LoopListViewItem> get;
        private Func<string, RectTransform, GameObject> create;
        private Action<GameObject> set;
        public Action<LoopListView, LoopListViewItem> onSnapFinished = null;
        public Action<LoopListView, LoopListViewItem> onSnapNearestChanged = null;
        public Action onBeginDrag = null;
        public Action onDraging = null;
        public Action onEndDrag = null;
        [SerializeField] List<PrefabConfData> prefabDatas = new List<PrefabConfData>();
        [SerializeField] private ArrangeType _arrangeType = ArrangeType.TopToBottom;
        [SerializeField] bool _snapEnable = false;
        [SerializeField] bool _supportScrollBar = true;
        [SerializeField] Vector2 _viewPortSnapPivot = Vector2.zero;
        [SerializeField] Vector2 _snapPivot = Vector2.zero;

        private Dictionary<string, ItemPool> mItemPoolDict = new Dictionary<string, ItemPool>();
        private List<ItemPool> mItemPoolList = new List<ItemPool>();
        private List<LoopListViewItem> mItemList = new List<LoopListViewItem>();
        private ScrollRect mScrollRect = null;
        private RectTransform mScrollRectTransform = null;
        private float mItemDefaultWithPaddingSize = 20;
        private int mItemTotalCount = 0;
        private bool _vertical = false;
        private Vector3[] mItemWorldCorners = new Vector3[4];
        private Vector3[] mViewPortRectLocalCorners = new Vector3[4];
        private int mCurReadyMinItemIndex = 0;
        private int mCurReadyMaxItemIndex = 0;
        private bool mNeedCheckNextMinItem = true;
        private bool mNeedCheckNextMaxItem = true;
        private ItemPosMgr mItemPosMgr = null;
        private float mDistanceForRecycle0 = 300;
        private float mDistanceForNew0 = 200;
        private float mDistanceForRecycle1 = 300;
        private float mDistanceForNew1 = 200;
        private bool _draging = false;
        private PointerEventData mPointerEventData = null;
        private int mLastItemIndex = 0;
        private float mLastItemPadding = 0;
        private float mSmoothDumpVel = 0;
        private float mSmoothDumpRate = 0.3f;
        private float mSnapFinishThreshold = 0.1f;
        private float mSnapVecThreshold = 145;
        private Vector3 mLastFrameContainerPos = Vector3.zero;
        private int mCurSnapNearestItemIndex = -1;
        private Vector2 mAdjustedVec;
        private bool mNeedAdjustVec = false;
        private int mLeftSnapUpdateExtraCount = 1;
        private UGUIEventListener mScrollBarClickEventListener = null;
        private SnapData mCurSnapData = new SnapData();
        private Vector3 mLastSnapCheckPos = Vector3.zero;
        private bool mListViewInited = false;
        private int mListUpdateCheckFrameCount = 0;


        public ArrangeType arrangeType { get { return _arrangeType; } set { _arrangeType = value; } }
        public bool snapEnable { get { return _snapEnable; } set { _snapEnable = value; } }
        public bool supportScrollBar { get { return _supportScrollBar; } set { _supportScrollBar = value; } }
        public bool vertical => _vertical;
        public int totalCount => mItemTotalCount;
        public RectTransform content => scroll.content;
        public ScrollRect scroll => mScrollRect;
        public bool draging => _draging;
        public int shownItemCount => mItemList.Count;
        public float viewPortSize => _vertical ? scroll.viewport.rect.height : scroll.viewport.rect.width;
        public float viewPortWidth => scroll.viewport.rect.width;
        public float viewPortHeight => scroll.viewport.rect.height;
        public int curSnapNearestItemIndex => mCurSnapNearestItemIndex;

        /*
         All visible items is stored in a List<LoopListViewItem2> , which is named mItemList;
         this method is to get the visible item by the index in visible items list. The parameter index is from 0 to mItemList.Count.
        */
        public LoopListViewItem GetShownItemByIndex(int index)
        {
            int count = mItemList.Count;
            if (index < 0 || index >= count)
            {
                return null;
            }
            return mItemList[index];
        }
        public LoopListViewItem GetShownItemByIndexWithoutCheck(int index)
        {
            return mItemList[index];
        }
        public int GetIndexInShownItemList(LoopListViewItem item)
        {
            if (item == null)
            {
                return -1;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return -1;
            }
            for (int i = 0; i < count; ++i)
            {
                if (mItemList[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }
        public void DoActionForEachShownItem(Action<LoopListViewItem, object> action, object param)
        {
            if (action == null)
            {
                return;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; ++i)
            {
                action(mItemList[i], param);
            }
        }
        public LoopListViewItem NewListViewItem(string path)
        {
            ItemPool pool = GetItemPool(path);
            if (pool == null) return null;
            LoopListViewItem item = pool.GetItem();
            RectTransform rf = item.GetComponent<RectTransform>();
            rf.SetParent(content);
            rf.localScale = Vector3.one;
            rf.localPosition = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            item.list = this;
            return item;
        }
        /*
        For a vertical scrollrect, when a visible item’s height changed at runtime, then this method should be called to let the LoopListView2 component reposition all visible items’ position.
        For a horizontal scrollrect, when a visible item’s width changed at runtime, then this method should be called to let the LoopListView2 component reposition all visible items’ position.
        */
        public void OnItemSizeChanged(int itemIndex)
        {
            LoopListViewItem item = GetShownItemByItemIndex(itemIndex);
            if (item == null)
            {
                return;
            }
            if (_supportScrollBar)
            {
                if (_vertical)
                {
                    SetItemSize(itemIndex, item.rect.rect.height, item.padding);
                }
                else
                {
                    SetItemSize(itemIndex, item.rect.rect.width, item.padding);
                }
            }
            UpdateContentSize();
            UpdateAllShownItemsPos();
        }
        /*
        To update a item by itemIndex.if the itemIndex-th item is not visible, then this method will do nothing.
        Otherwise this method will first call onGetItemByIndex(itemIndex) to get a updated item and then reposition all visible items'position. 
        */
        public void RefreshItemByItemIndex(int itemIndex)
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            if (itemIndex < mItemList[0].index || itemIndex > mItemList[count - 1].index)
            {
                return;
            }
            int firstItemIndex = mItemList[0].index;
            int i = itemIndex - firstItemIndex;
            LoopListViewItem curItem = mItemList[i];
            Vector3 pos = curItem.rect.localPosition;
            RecycleItemTmp(curItem);
            LoopListViewItem newItem = GetNewItemByIndex(itemIndex);
            if (newItem == null)
            {
                RefreshAllShownItemWithFirstIndex(firstItemIndex);
                return;
            }
            mItemList[i] = newItem;
            if (_vertical)
            {
                pos.x = newItem.offset;
            }
            else
            {
                pos.y = newItem.offset;
            }
            newItem.rect.localPosition = pos;
            OnItemSizeChanged(itemIndex);
            ClearAllTmpRecycledItem();
        }
        //snap move will finish at once.
        public void FinishSnapImmediately()
        {
            UpdateSnapMove(true);
        }
        /*
        This method will move the scrollrect content’s position to ( the positon of itemIndex-th item + offset ),
        and in current version the itemIndex is from 0 to MaxInt, offset is from 0 to scrollrect viewport size. 
        */
        public void MovePanelToItemIndex(int itemIndex, float offset)
        {
            mScrollRect.StopMovement();
            mCurSnapData.Clear();
            if (itemIndex < 0 || mItemTotalCount == 0)
            {
                return;
            }
            if (mItemTotalCount > 0 && itemIndex >= mItemTotalCount)
            {
                itemIndex = mItemTotalCount - 1;
            }
            if (offset < 0)
            {
                offset = 0;
            }
            Vector3 pos = Vector3.zero;
            float viewPortSize = this.viewPortSize;
            if (offset > viewPortSize)
            {
                offset = viewPortSize;
            }
            if (_arrangeType == ArrangeType.TopToBottom)
            {
                float containerPos = content.localPosition.y;
                if (containerPos < 0)
                {
                    containerPos = 0;
                }
                pos.y = -containerPos - offset;
            }
            else if (_arrangeType == ArrangeType.BottomToTop)
            {
                float containerPos = content.localPosition.y;
                if (containerPos > 0)
                {
                    containerPos = 0;
                }
                pos.y = -containerPos + offset;
            }
            else if (_arrangeType == ArrangeType.LeftToRight)
            {
                float containerPos = content.localPosition.x;
                if (containerPos > 0)
                {
                    containerPos = 0;
                }
                pos.x = -containerPos + offset;
            }
            else if (_arrangeType == ArrangeType.RightToLeft)
            {
                float containerPos = content.localPosition.x;
                if (containerPos < 0)
                {
                    containerPos = 0;
                }
                pos.x = -containerPos - offset;
            }

            RecycleAllItem();
            LoopListViewItem newItem = GetNewItemByIndex(itemIndex);
            if (newItem == null)
            {
                ClearAllTmpRecycledItem();
                return;
            }
            if (_vertical)
            {
                pos.x = newItem.offset;
            }
            else
            {
                pos.y = newItem.offset;
            }
            newItem.rect.localPosition = pos;
            if (_supportScrollBar)
            {
                if (_vertical)
                {
                    SetItemSize(itemIndex, newItem.rect.rect.height, newItem.padding);
                }
                else
                {
                    SetItemSize(itemIndex, newItem.rect.rect.width, newItem.padding);
                }
            }
            mItemList.Add(newItem);
            UpdateContentSize();
            UpdateListView(viewPortSize + 100, viewPortSize + 100, viewPortSize, viewPortSize);
            AdjustPanelPos();
            ClearAllTmpRecycledItem();
        }
        //update all visible items.
        public void RefreshAllShownItem()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            RefreshAllShownItemWithFirstIndex(mItemList[0].index);
        }
        public void RefreshAllShownItemWithFirstIndex(int firstItemIndex)
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            LoopListViewItem firstItem = mItemList[0];
            Vector3 pos = firstItem.rect.localPosition;
            RecycleAllItem();
            for (int i = 0; i < count; ++i)
            {
                int curIndex = firstItemIndex + i;
                LoopListViewItem newItem = GetNewItemByIndex(curIndex);
                if (newItem == null)
                {
                    break;
                }
                if (_vertical)
                {
                    pos.x = newItem.offset;
                }
                else
                {
                    pos.y = newItem.offset;
                }
                newItem.rect.localPosition = pos;
                if (_supportScrollBar)
                {
                    if (_vertical)
                    {
                        SetItemSize(curIndex, newItem.rect.rect.height, newItem.padding);
                    }
                    else
                    {
                        SetItemSize(curIndex, newItem.rect.rect.width, newItem.padding);
                    }
                }

                mItemList.Add(newItem);
            }
            UpdateContentSize();
            UpdateAllShownItemsPos();
            ClearAllTmpRecycledItem();
        }
        public void RefreshAllShownItemWithFirstIndexAndPos(int firstItemIndex, Vector3 pos)
        {
            RecycleAllItem();
            LoopListViewItem newItem = GetNewItemByIndex(firstItemIndex);
            if (newItem == null)
            {
                return;
            }
            if (_vertical)
            {
                pos.x = newItem.offset;
            }
            else
            {
                pos.y = newItem.offset;
            }
            newItem.rect.localPosition = pos;
            if (_supportScrollBar)
            {
                if (_vertical)
                {
                    SetItemSize(firstItemIndex, newItem.rect.rect.height, newItem.padding);
                }
                else
                {
                    SetItemSize(firstItemIndex, newItem.rect.rect.width, newItem.padding);
                }
            }
            mItemList.Add(newItem);
            UpdateContentSize();
            UpdateAllShownItemsPos();
            UpdateListView(mDistanceForRecycle0, mDistanceForRecycle1, mDistanceForNew0, mDistanceForNew1);
            ClearAllTmpRecycledItem();
        }
        public Vector3 GetItemCornerPosInViewPort(LoopListViewItem item, CornerEnum corner = CornerEnum.LeftBottom)
        {
            item.rect.GetWorldCorners(mItemWorldCorners);
            return scroll.viewport.InverseTransformPoint(mItemWorldCorners[(int)corner]);
        }
        public void UpdateAllShownItemSnapData()
        {
            if (_snapEnable == false)
            {
                return;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            Vector3 pos = content.localPosition;
            LoopListViewItem tViewItem0 = mItemList[0];
            tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
            float start = 0;
            float end = 0;
            float itemSnapCenter = 0;
            float snapCenter = 0;
            if (_arrangeType == ArrangeType.TopToBottom)
            {
                snapCenter = -(1 - _viewPortSnapPivot.y) * scroll.viewport.rect.height;
                Vector3 topPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                start = topPos1.y;
                end = start - tViewItem0.sizeByPadding;
                itemSnapCenter = start - tViewItem0.size * (1 - _snapPivot.y);
                for (int i = 0; i < count; ++i)
                {
                    mItemList[i].distanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
                    if ((i + 1) < count)
                    {
                        start = end;
                        end = end - mItemList[i + 1].sizeByPadding;
                        itemSnapCenter = start - mItemList[i + 1].size * (1 - _snapPivot.y);
                    }
                }
            }
            else if (_arrangeType == ArrangeType.BottomToTop)
            {
                snapCenter = _viewPortSnapPivot.y * scroll.viewport.rect.height;
                Vector3 bottomPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[0]);
                start = bottomPos1.y;
                end = start + tViewItem0.sizeByPadding;
                itemSnapCenter = start + tViewItem0.size * _snapPivot.y;
                for (int i = 0; i < count; ++i)
                {
                    mItemList[i].distanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
                    if ((i + 1) < count)
                    {
                        start = end;
                        end = end + mItemList[i + 1].sizeByPadding;
                        itemSnapCenter = start + mItemList[i + 1].size * _snapPivot.y;
                    }
                }
            }
            else if (_arrangeType == ArrangeType.RightToLeft)
            {
                snapCenter = -(1 - _viewPortSnapPivot.x) * scroll.viewport.rect.width;
                Vector3 rightPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[2]);
                start = rightPos1.x;
                end = start - tViewItem0.sizeByPadding;
                itemSnapCenter = start - tViewItem0.size * (1 - _snapPivot.x);
                for (int i = 0; i < count; ++i)
                {
                    mItemList[i].distanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
                    if ((i + 1) < count)
                    {
                        start = end;
                        end = end - mItemList[i + 1].sizeByPadding;
                        itemSnapCenter = start - mItemList[i + 1].size * (1 - _snapPivot.x);
                    }
                }
            }
            else if (_arrangeType == ArrangeType.LeftToRight)
            {
                snapCenter = _viewPortSnapPivot.x * scroll.viewport.rect.width;
                Vector3 leftPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                start = leftPos1.x;
                end = start + tViewItem0.sizeByPadding;
                itemSnapCenter = start + tViewItem0.size * _snapPivot.x;
                for (int i = 0; i < count; ++i)
                {
                    mItemList[i].distanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
                    if ((i + 1) < count)
                    {
                        start = end;
                        end = end + mItemList[i + 1].sizeByPadding;
                        itemSnapCenter = start + mItemList[i + 1].size * _snapPivot.x;
                    }
                }
            }
        }
        //Clear current snap target and then the LoopScrollView2 will auto snap to the CurSnapNearestItemIndex.
        public void ClearSnapData()
        {
            mCurSnapData.Clear();
        }
        public void SetSnapTargetItemIndex(int itemIndex)
        {
            mCurSnapData.mSnapTargetIndex = itemIndex;
            mCurSnapData.mSnapStatus = SnapStatus.TargetHasSet;
        }
        public void SetItemPrefabConfData(List<PrefabConfData> itemPrefabConfDatas)
        {
            this.prefabDatas = itemPrefabConfDatas;
        }
        public void UpdateListView(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            mListUpdateCheckFrameCount++;
            if (_vertical)
            {
                bool needContinueCheck = true;
                int checkCount = 0;
                int maxCount = 9999;
                while (needContinueCheck)
                {
                    checkCount++;
                    if (checkCount >= maxCount)
                    {
                        Debug.LogError("UpdateListView Vertical while loop " + checkCount + " times! something is wrong!");
                        break;
                    }
                    needContinueCheck = UpdateForVertList(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
                }
            }
            else
            {
                bool needContinueCheck = true;
                int checkCount = 0;
                int maxCount = 9999;
                while (needContinueCheck)
                {
                    checkCount++;
                    if (checkCount >= maxCount)
                    {
                        Debug.LogError("UpdateListView  Horizontal while loop " + checkCount + " times! something is wrong!");
                        break;
                    }
                    needContinueCheck = UpdateForHorizontalList(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
                }
            }

        }
        public void ForceSnapUpdateCheck()
        {
            if (mLeftSnapUpdateExtraCount <= 0)
            {
                mLeftSnapUpdateExtraCount = 1;
            }
        }
        public PrefabConfData GetItemPrefabConfData(string path)
        {
            foreach (PrefabConfData data in prefabDatas)
            {
                if (data.path == null)
                {
                    Debug.LogError("A item prefab is null ");
                    continue;
                }
                if (path == data.path)
                {
                    return data;
                }

            }
            return null;
        }
        public void OnItemPrefabChanged(string path)
        {
            PrefabConfData data = GetItemPrefabConfData(path);
            if (data == null)
            {
                return;
            }
            ItemPool pool = GetItemPool(path);
            if (pool == null) return;
            int firstItemIndex = -1;
            Vector3 pos = Vector3.zero;
            if (mItemList.Count > 0)
            {
                firstItemIndex = mItemList[0].index;
                pos = mItemList[0].rect.localPosition;
            }
            RecycleAllItem();
            ClearAllTmpRecycledItem();
            pool.ClearTmpRecycledItem();
            pool.Init(this, data.path, data.padding, data.offset, content, this.create, this.set);
            if (firstItemIndex >= 0)
            {
                RefreshAllShownItemWithFirstIndexAndPos(firstItemIndex, pos);
            }
        }
        /*
        InitListView method is to initiate the LoopListView2 component. There are 3 parameters:
        itemTotalCount: the total item count in the listview. If this parameter is set -1, then means there are infinite items, and scrollbar would not be supported, and the ItemIndex can be from –MaxInt to +MaxInt. If this parameter is set a value >=0 , then the ItemIndex can only be from 0 to itemTotalCount -1.
        onGetItemByIndex: when a item is getting in the scrollrect viewport, and this Action will be called with the item’ index as a parameter, to let you create the item and update its content.
        */
        public void InitListView(Func<string, RectTransform, GameObject> create, Action<GameObject> set, Func<LoopListView, int, LoopListViewItem> onGetItemByIndex, InitParam initParam = null)
        {
            this.create = create;
            this.set = set;
            if (initParam != null)
            {
                mDistanceForRecycle0 = initParam.distanceForRecycle0;
                mDistanceForNew0 = initParam.distanceForNew0;
                mDistanceForRecycle1 = initParam.distanceForRecycle1;
                mDistanceForNew1 = initParam.distanceForNew1;
                mSmoothDumpRate = initParam.smoothDumpRate;
                mSnapFinishThreshold = initParam.snapFinishThreshold;
                mSnapVecThreshold = initParam.snapVecThreshold;
                mItemDefaultWithPaddingSize = initParam.itemDefaultWithPaddingSize;
            }
            mScrollRect = gameObject.GetComponent<ScrollRect>();
            if (mScrollRect == null)
            {
                Debug.LogError("ListView Init Failed! ScrollRect component not found!");
                return;
            }
            if (mDistanceForRecycle0 <= mDistanceForNew0)
            {
                Debug.LogError("mDistanceForRecycle0 should be bigger than mDistanceForNew0");
            }
            if (mDistanceForRecycle1 <= mDistanceForNew1)
            {
                Debug.LogError("mDistanceForRecycle1 should be bigger than mDistanceForNew1");
            }
            mCurSnapData.Clear();
            mItemPosMgr = new ItemPosMgr(mItemDefaultWithPaddingSize);
            mScrollRectTransform = mScrollRect.GetComponent<RectTransform>();
            scroll.viewport = mScrollRect.viewport;
            if (scroll.viewport == null)
            {
                scroll.viewport = mScrollRectTransform;
            }
            if (mScrollRect.horizontalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport && mScrollRect.horizontalScrollbar != null)
            {
                Debug.LogError("ScrollRect.horizontalScrollbarVisibility cannot be set to AutoHideAndExpandViewport");
            }
            if (mScrollRect.verticalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport && mScrollRect.verticalScrollbar != null)
            {
                Debug.LogError("ScrollRect.verticalScrollbarVisibility cannot be set to AutoHideAndExpandViewport");
            }
            _vertical = (_arrangeType == ArrangeType.TopToBottom || _arrangeType == ArrangeType.BottomToTop);
            mScrollRect.horizontal = !_vertical;
            mScrollRect.vertical = _vertical;
            SetScrollbarListener();
            AdjustPivot(scroll.viewport);
            AdjustAnchor(content);
            AdjustContainerPivot(content);
            get = onGetItemByIndex;
            if (mListViewInited == true)
            {
                Debug.LogError("LoopListView2.InitListView method can be called only once.");
            }
            mListViewInited = true;
            ResetListView();
        }
        public void ResetListView()
        {
            scroll.viewport.GetLocalCorners(mViewPortRectLocalCorners);
            content.localPosition = Vector3.zero;
            ForceSnapUpdateCheck();
        }
        /*
        This method may use to set the item total count of the scrollview at runtime. 
        If this parameter is set -1, then means there are infinite items,
        and scrollbar would not be supported, and the ItemIndex can be from –MaxInt to +MaxInt. 
        If this parameter is set a value >=0 , then the ItemIndex can only be from 0 to itemTotalCount -1.  
        If resetPos is set false, then the scrollrect’s content position will not changed after this method finished.
        */
        public void SetListItemCount(int itemCount, bool resetPos = true)
        {
            if (itemCount == mItemTotalCount)
            {
                return;
            }
            mCurSnapData.Clear();
            mItemTotalCount = itemCount;
            if (mItemTotalCount < 0)
            {
                _supportScrollBar = false;
            }
            if (_supportScrollBar)
            {
                mItemPosMgr.SetItemMaxCount(mItemTotalCount);
            }
            else
            {
                mItemPosMgr.SetItemMaxCount(0);
            }
            if (mItemTotalCount == 0)
            {
                mCurReadyMaxItemIndex = 0;
                mCurReadyMinItemIndex = 0;
                mNeedCheckNextMaxItem = false;
                mNeedCheckNextMinItem = false;
                RecycleAllItem();
                ClearAllTmpRecycledItem();
                UpdateContentSize();
                return;
            }
            mLeftSnapUpdateExtraCount = 1;
            mNeedCheckNextMaxItem = true;
            mNeedCheckNextMinItem = true;
            if (resetPos)
            {
                MovePanelToItemIndex(0, 0);
                return;
            }
            if (mItemList.Count == 0)
            {
                MovePanelToItemIndex(0, 0);
                return;
            }
            int maxItemIndex = mItemTotalCount - 1;
            int lastItemIndex = mItemList[mItemList.Count - 1].index;
            if (lastItemIndex <= maxItemIndex)
            {
                UpdateContentSize();
                UpdateAllShownItemsPos();
                return;
            }
            MovePanelToItemIndex(maxItemIndex, 0);

        }
        //To get the visible item by itemIndex. If the item is not visible, then this method return null.
        public LoopListViewItem GetShownItemByItemIndex(int itemIndex)
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return null;
            }
            if (itemIndex < mItemList[0].index || itemIndex > mItemList[count - 1].index)
            {
                return null;
            }
            int i = itemIndex - mItemList[0].index;
            return mItemList[i];
        }


        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _draging = true;
            CacheDragPointerEventData(eventData);
            mCurSnapData.Clear();
            if (onBeginDrag != null)
            {
                onBeginDrag();
            }
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _draging = false;
            mPointerEventData = null;
            if (onEndDrag != null)
            {
                onEndDrag();
            }
            ForceSnapUpdateCheck();
        }
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            CacheDragPointerEventData(eventData);
            if (onDraging != null)
            {
                onDraging();
            }
        }
        void Update()
        {
            if (mListViewInited == false)
            {
                return;
            }
            if (mNeedAdjustVec)
            {
                mNeedAdjustVec = false;
                if (_vertical)
                {
                    if (mScrollRect.velocity.y * mAdjustedVec.y > 0)
                    {
                        mScrollRect.velocity = mAdjustedVec;
                    }
                }
                else
                {
                    if (mScrollRect.velocity.x * mAdjustedVec.x > 0)
                    {
                        mScrollRect.velocity = mAdjustedVec;
                    }
                }

            }
            if (_supportScrollBar)
            {
                mItemPosMgr.Update(false);
            }
            UpdateSnapMove();
            UpdateListView(mDistanceForRecycle0, mDistanceForRecycle1, mDistanceForNew0, mDistanceForNew1);
            ClearAllTmpRecycledItem();
            mLastFrameContainerPos = content.localPosition;
        }
        private void AdjustPivot(RectTransform rtf)
        {
            Vector2 pivot = rtf.pivot;

            if (_arrangeType == ArrangeType.BottomToTop)
            {
                pivot.y = 0;
            }
            else if (_arrangeType == ArrangeType.TopToBottom)
            {
                pivot.y = 1;
            }
            else if (_arrangeType == ArrangeType.LeftToRight)
            {
                pivot.x = 0;
            }
            else if (_arrangeType == ArrangeType.RightToLeft)
            {
                pivot.x = 1;
            }
            rtf.pivot = pivot;
        }
        private void AdjustAnchor(RectTransform rtf)
        {
            Vector2 anchorMin = rtf.anchorMin;
            Vector2 anchorMax = rtf.anchorMax;
            if (_arrangeType == ArrangeType.BottomToTop)
            {
                anchorMin.y = 0;
                anchorMax.y = 0;
            }
            else if (_arrangeType == ArrangeType.TopToBottom)
            {
                anchorMin.y = 1;
                anchorMax.y = 1;
            }
            else if (_arrangeType == ArrangeType.LeftToRight)
            {
                anchorMin.x = 0;
                anchorMax.x = 0;
            }
            else if (_arrangeType == ArrangeType.RightToLeft)
            {
                anchorMin.x = 1;
                anchorMax.x = 1;
            }
            rtf.anchorMin = anchorMin;
            rtf.anchorMax = anchorMax;
        }
        void AdjustContainerAnchor(RectTransform rtf)
        {
            Vector2 anchorMin = rtf.anchorMin;
            Vector2 anchorMax = rtf.anchorMax;
            if (_arrangeType == ArrangeType.BottomToTop)
            {
                anchorMin.y = 0;
                anchorMax.y = 0;
            }
            else if (_arrangeType == ArrangeType.TopToBottom)
            {
                anchorMin.y = 1;
                anchorMax.y = 1;
            }
            else if (_arrangeType == ArrangeType.LeftToRight)
            {
                anchorMin.x = 0;
                anchorMax.x = 0;
            }
            else if (_arrangeType == ArrangeType.RightToLeft)
            {
                anchorMin.x = 1;
                anchorMax.x = 1;
            }
            rtf.anchorMin = anchorMin;
            rtf.anchorMax = anchorMax;
        }
        void SetScrollbarListener()
        {
            mScrollBarClickEventListener = null;
            Scrollbar curScrollBar = null;
            if (_vertical && mScrollRect.verticalScrollbar != null)
            {
                curScrollBar = mScrollRect.verticalScrollbar;

            }
            if (!_vertical && mScrollRect.horizontalScrollbar != null)
            {
                curScrollBar = mScrollRect.horizontalScrollbar;
            }
            if (curScrollBar == null)
            {
                return;
            }
            UGUIEventListener listener = UGUIEventListener.Get(curScrollBar.gameObject);
            mScrollBarClickEventListener = listener;
            listener.onPointup.AddListener(OnPointerUpInScrollBar);
            listener.onPointDown.AddListener(OnPointerDownInScrollBar);
        }

        void OnPointerDownInScrollBar(GameObject obj, PointerEventData eve)
        {
            mCurSnapData.Clear();
        }

        void OnPointerUpInScrollBar(GameObject obj, PointerEventData eve)
        {
            ForceSnapUpdateCheck();
        }
        void RecycleItemTmp(LoopListViewItem item)
        {
            if (item == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(item.path))
            {
                return;
            }
            ItemPool pool = GetItemPool(item.path);
            if (pool == null) return;
            pool.RecycleItem(item);

        }


        void ClearAllTmpRecycledItem()
        {
            int count = mItemPoolList.Count;
            for (int i = 0; i < count; ++i)
            {
                mItemPoolList[i].ClearTmpRecycledItem();
            }
        }


        void RecycleAllItem()
        {
            foreach (LoopListViewItem item in mItemList)
            {
                RecycleItemTmp(item);
            }
            mItemList.Clear();
        }


        void AdjustContainerPivot(RectTransform rtf)
        {
            Vector2 pivot = rtf.pivot;
            if (_arrangeType == ArrangeType.BottomToTop)
            {
                pivot.y = 0;
            }
            else if (_arrangeType == ArrangeType.TopToBottom)
            {
                pivot.y = 1;
            }
            else if (_arrangeType == ArrangeType.LeftToRight)
            {
                pivot.x = 0;
            }
            else if (_arrangeType == ArrangeType.RightToLeft)
            {
                pivot.x = 1;
            }
            rtf.pivot = pivot;
        }

        ItemPool GetItemPool(string path)
        {
            var data = GetItemPrefabConfData(path);
            if (data == null) return null;
            if (mItemPoolDict.ContainsKey(path))
            {
                return mItemPoolDict[path];
            }
            ItemPool pool = new ItemPool();
            pool.Init(this, data.path, data.padding, data.offset, content, this.create, this.set);
            mItemPoolDict.Add(path, pool);
            mItemPoolList.Add(pool);
            return pool;
        }
        void CacheDragPointerEventData(PointerEventData eventData)
        {
            if (mPointerEventData == null)
            {
                mPointerEventData = new PointerEventData(EventSystem.current);
            }
            mPointerEventData.button = eventData.button;
            mPointerEventData.position = eventData.position;
            mPointerEventData.pointerPressRaycast = eventData.pointerPressRaycast;
            mPointerEventData.pointerCurrentRaycast = eventData.pointerCurrentRaycast;
        }
        LoopListViewItem GetNewItemByIndex(int index)
        {
            if (_supportScrollBar && index < 0)
            {
                return null;
            }
            if (mItemTotalCount > 0 && index >= mItemTotalCount)
            {
                return null;
            }
            LoopListViewItem newItem = get(this, index);
            if (newItem == null)
            {
                return null;
            }
            newItem.index = index;
            newItem.check = mListUpdateCheckFrameCount;
            return newItem;
        }
        void SetItemSize(int itemIndex, float itemSize, float padding)
        {
            mItemPosMgr.SetItemSize(itemIndex, itemSize + padding);
            if (itemIndex >= mLastItemIndex)
            {
                mLastItemIndex = itemIndex;
                mLastItemPadding = padding;
            }
        }
        void GetPlusItemIndexAndPosAtGivenPos(float pos, ref int index, ref float itemPos)
        {
            mItemPosMgr.GetItemIndexAndPosAtGivenPos(pos, ref index, ref itemPos);
        }
        float GetItemPos(int itemIndex)
        {
            return mItemPosMgr.GetItemPos(itemIndex);
        }
        void AdjustPanelPos()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            UpdateAllShownItemsPos();
            float viewPortSize = this.viewPortSize;
            float contentSize = GetContentPanelSize();
            if (_arrangeType == ArrangeType.TopToBottom)
            {
                if (contentSize <= viewPortSize)
                {
                    Vector3 pos = content.localPosition;
                    pos.y = 0;
                    content.localPosition = pos;
                    mItemList[0].rect.localPosition = new Vector3(mItemList[0].offset, 0, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 topPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                if (topPos0.y < mViewPortRectLocalCorners[1].y)
                {
                    Vector3 pos = content.localPosition;
                    pos.y = 0;
                    content.localPosition = pos;
                    mItemList[0].rect.localPosition = new Vector3(mItemList[0].offset, 0, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem tViewItem1 = mItemList[mItemList.Count - 1];
                tViewItem1.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 downPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[0]);
                float d = downPos1.y - mViewPortRectLocalCorners[0].y;
                if (d > 0)
                {
                    Vector3 pos = mItemList[0].rect.localPosition;
                    pos.y = pos.y - d;
                    mItemList[0].rect.localPosition = pos;
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (_arrangeType == ArrangeType.BottomToTop)
            {
                if (contentSize <= viewPortSize)
                {
                    Vector3 pos = content.localPosition;
                    pos.y = 0;
                    content.localPosition = pos;
                    mItemList[0].rect.localPosition = new Vector3(mItemList[0].offset, 0, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 downPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[0]);
                if (downPos0.y > mViewPortRectLocalCorners[0].y)
                {
                    Vector3 pos = content.localPosition;
                    pos.y = 0;
                    content.localPosition = pos;
                    mItemList[0].rect.localPosition = new Vector3(mItemList[0].offset, 0, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem tViewItem1 = mItemList[mItemList.Count - 1];
                tViewItem1.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 topPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                float d = mViewPortRectLocalCorners[1].y - topPos1.y;
                if (d > 0)
                {
                    Vector3 pos = mItemList[0].rect.localPosition;
                    pos.y = pos.y + d;
                    mItemList[0].rect.localPosition = pos;
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (_arrangeType == ArrangeType.LeftToRight)
            {
                if (contentSize <= viewPortSize)
                {
                    Vector3 pos = content.localPosition;
                    pos.x = 0;
                    content.localPosition = pos;
                    mItemList[0].rect.localPosition = new Vector3(0, mItemList[0].offset, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 leftPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                if (leftPos0.x > mViewPortRectLocalCorners[1].x)
                {
                    Vector3 pos = content.localPosition;
                    pos.x = 0;
                    content.localPosition = pos;
                    mItemList[0].rect.localPosition = new Vector3(0, mItemList[0].offset, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem tViewItem1 = mItemList[mItemList.Count - 1];
                tViewItem1.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 rightPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[2]);
                float d = mViewPortRectLocalCorners[2].x - rightPos1.x;
                if (d > 0)
                {
                    Vector3 pos = mItemList[0].rect.localPosition;
                    pos.x = pos.x + d;
                    mItemList[0].rect.localPosition = pos;
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (_arrangeType == ArrangeType.RightToLeft)
            {
                if (contentSize <= viewPortSize)
                {
                    Vector3 pos = content.localPosition;
                    pos.x = 0;
                    content.localPosition = pos;
                    mItemList[0].rect.localPosition = new Vector3(0, mItemList[0].offset, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 rightPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[2]);
                if (rightPos0.x < mViewPortRectLocalCorners[2].x)
                {
                    Vector3 pos = content.localPosition;
                    pos.x = 0;
                    content.localPosition = pos;
                    mItemList[0].rect.localPosition = new Vector3(0, mItemList[0].offset, 0);
                    UpdateAllShownItemsPos();
                    return;
                }
                LoopListViewItem tViewItem1 = mItemList[mItemList.Count - 1];
                tViewItem1.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 leftPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                float d = leftPos1.x - mViewPortRectLocalCorners[1].x;
                if (d > 0)
                {
                    Vector3 pos = mItemList[0].rect.localPosition;
                    pos.x = pos.x - d;
                    mItemList[0].rect.localPosition = pos;
                    UpdateAllShownItemsPos();
                    return;
                }
            }



        }
        //update snap move. if immediate is set true, then the snap move will finish at once.
        void UpdateSnapMove(bool immediate = false)
        {
            if (_snapEnable == false)
            {
                return;
            }
            if (_vertical)
            {
                UpdateSnapVertical(immediate);
            }
            else
            {
                UpdateSnapHorizontal(immediate);
            }
        }
        void UpdateSnapVertical(bool immediate = false)
        {
            if (_snapEnable == false)
            {
                return;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            Vector3 pos = content.localPosition;
            bool needCheck = (pos.y != mLastSnapCheckPos.y);
            mLastSnapCheckPos = pos;
            if (!needCheck)
            {
                if (mLeftSnapUpdateExtraCount > 0)
                {
                    mLeftSnapUpdateExtraCount--;
                    needCheck = true;
                }
            }
            if (needCheck)
            {
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                int curIndex = -1;
                float start = 0;
                float end = 0;
                float itemSnapCenter = 0;
                float curMinDist = float.MaxValue;
                float curDist = 0;
                float curDistAbs = 0;
                float snapCenter = 0;
                if (_arrangeType == ArrangeType.TopToBottom)
                {
                    snapCenter = -(1 - _viewPortSnapPivot.y) * scroll.viewport.rect.height;
                    Vector3 topPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                    start = topPos1.y;
                    end = start - tViewItem0.sizeByPadding;
                    itemSnapCenter = start - tViewItem0.size * (1 - _snapPivot.y);
                    for (int i = 0; i < count; ++i)
                    {
                        curDist = snapCenter - itemSnapCenter;
                        curDistAbs = Mathf.Abs(curDist);
                        if (curDistAbs < curMinDist)
                        {
                            curMinDist = curDistAbs;
                            curIndex = i;
                        }
                        else
                        {
                            break;
                        }

                        if ((i + 1) < count)
                        {
                            start = end;
                            end = end - mItemList[i + 1].sizeByPadding;
                            itemSnapCenter = start - mItemList[i + 1].size * (1 - _snapPivot.y);
                        }
                    }
                }
                else if (_arrangeType == ArrangeType.BottomToTop)
                {
                    snapCenter = _viewPortSnapPivot.y * scroll.viewport.rect.height;
                    Vector3 bottomPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[0]);
                    start = bottomPos1.y;
                    end = start + tViewItem0.sizeByPadding;
                    itemSnapCenter = start + tViewItem0.size * _snapPivot.y;
                    for (int i = 0; i < count; ++i)
                    {
                        curDist = snapCenter - itemSnapCenter;
                        curDistAbs = Mathf.Abs(curDist);
                        if (curDistAbs < curMinDist)
                        {
                            curMinDist = curDistAbs;
                            curIndex = i;
                        }
                        else
                        {
                            break;
                        }

                        if ((i + 1) < count)
                        {
                            start = end;
                            end = end + mItemList[i + 1].sizeByPadding;
                            itemSnapCenter = start + mItemList[i + 1].size * _snapPivot.y;
                        }
                    }
                }

                if (curIndex >= 0)
                {
                    int oldNearestItemIndex = mCurSnapNearestItemIndex;
                    mCurSnapNearestItemIndex = mItemList[curIndex].index;
                    if (mItemList[curIndex].index != oldNearestItemIndex)
                    {
                        if (onSnapNearestChanged != null)
                        {
                            onSnapNearestChanged(this, mItemList[curIndex]);
                        }
                    }
                }
                else
                {
                    mCurSnapNearestItemIndex = -1;
                }
            }
            bool canSnap = true;
            if (mScrollBarClickEventListener != null)
            {
                canSnap = !(mScrollBarClickEventListener.isPress);
            }
            float v = Mathf.Abs(mScrollRect.velocity.y);
            if (canSnap && !_draging && v < mSnapVecThreshold)
            {
                UpdateCurSnapData();
                if (mCurSnapData.mSnapStatus != SnapStatus.SnapMoving)
                {
                    return;
                }
                if (v > 0)
                {
                    mScrollRect.StopMovement();
                }
                float old = mCurSnapData.mCurSnapVal;
                mCurSnapData.mCurSnapVal = Mathf.SmoothDamp(mCurSnapData.mCurSnapVal, mCurSnapData.mTargetSnapVal, ref mSmoothDumpVel, mSmoothDumpRate);
                float dt = mCurSnapData.mCurSnapVal - old;

                if (immediate || Mathf.Abs(mCurSnapData.mTargetSnapVal - mCurSnapData.mCurSnapVal) < mSnapFinishThreshold)
                {
                    pos.y = pos.y + mCurSnapData.mTargetSnapVal - mCurSnapData.mCurSnapVal;
                    mCurSnapData.mSnapStatus = SnapStatus.SnapMoveFinish;
                    if (onSnapFinished != null)
                    {
                        LoopListViewItem targetItem = GetShownItemByItemIndex(mCurSnapNearestItemIndex);
                        if (targetItem != null)
                        {
                            onSnapFinished(this, targetItem);
                        }
                    }
                }
                else
                {
                    pos.y = pos.y + dt;
                }

                if (_arrangeType == ArrangeType.TopToBottom)
                {
                    float maxY = mViewPortRectLocalCorners[0].y + content.rect.height;
                    if (pos.y <= maxY && pos.y >= 0)
                    {
                        content.localPosition = pos;
                    }
                }
                else if (_arrangeType == ArrangeType.BottomToTop)
                {
                    float minY = mViewPortRectLocalCorners[1].y - content.rect.height;
                    if (pos.y >= minY && pos.y <= 0)
                    {
                        content.localPosition = pos;
                    }
                }

            }

        }
        void UpdateCurSnapData()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                mCurSnapData.Clear();
                return;
            }

            if (mCurSnapData.mSnapStatus == SnapStatus.SnapMoving
                || mCurSnapData.mSnapStatus == SnapStatus.SnapMoveFinish)
            {
                return;
            }
            if (mCurSnapData.mSnapStatus == SnapStatus.NoTargetSet)
            {
                LoopListViewItem nearestItem = GetShownItemByItemIndex(mCurSnapNearestItemIndex);
                if (nearestItem == null)
                {
                    return;
                }
                mCurSnapData.mSnapTargetIndex = mCurSnapNearestItemIndex;
                mCurSnapData.mSnapStatus = SnapStatus.TargetHasSet;
            }
            if (mCurSnapData.mSnapStatus == SnapStatus.TargetHasSet)
            {
                LoopListViewItem targetItem = GetShownItemByItemIndex(mCurSnapData.mSnapTargetIndex);
                if (targetItem == null)
                {
                    mCurSnapData.Clear();
                    return;
                }
                UpdateAllShownItemSnapData();
                mCurSnapData.mTargetSnapVal = targetItem.distanceWithViewPortSnapCenter;
                mCurSnapData.mCurSnapVal = 0;
                mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
            }

        }
        void UpdateSnapHorizontal(bool immediate = false)
        {
            if (_snapEnable == false)
            {
                return;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            Vector3 pos = content.localPosition;
            bool needCheck = (pos.x != mLastSnapCheckPos.x);
            mLastSnapCheckPos = pos;
            if (!needCheck)
            {
                if (mLeftSnapUpdateExtraCount > 0)
                {
                    mLeftSnapUpdateExtraCount--;
                    needCheck = true;
                }
            }
            if (needCheck)
            {
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                int curIndex = -1;
                float start = 0;
                float end = 0;
                float itemSnapCenter = 0;
                float curMinDist = float.MaxValue;
                float curDist = 0;
                float curDistAbs = 0;
                float snapCenter = 0;
                if (_arrangeType == ArrangeType.RightToLeft)
                {
                    snapCenter = -(1 - _viewPortSnapPivot.x) * scroll.viewport.rect.width;
                    Vector3 rightPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[2]);
                    start = rightPos1.x;
                    end = start - tViewItem0.sizeByPadding;
                    itemSnapCenter = start - tViewItem0.size * (1 - _snapPivot.x);
                    for (int i = 0; i < count; ++i)
                    {
                        curDist = snapCenter - itemSnapCenter;
                        curDistAbs = Mathf.Abs(curDist);
                        if (curDistAbs < curMinDist)
                        {
                            curMinDist = curDistAbs;
                            curIndex = i;
                        }
                        else
                        {
                            break;
                        }

                        if ((i + 1) < count)
                        {
                            start = end;
                            end = end - mItemList[i + 1].sizeByPadding;
                            itemSnapCenter = start - mItemList[i + 1].size * (1 - _snapPivot.x);
                        }
                    }
                }
                else if (_arrangeType == ArrangeType.LeftToRight)
                {
                    snapCenter = _viewPortSnapPivot.x * scroll.viewport.rect.width;
                    Vector3 leftPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                    start = leftPos1.x;
                    end = start + tViewItem0.sizeByPadding;
                    itemSnapCenter = start + tViewItem0.size * _snapPivot.x;
                    for (int i = 0; i < count; ++i)
                    {
                        curDist = snapCenter - itemSnapCenter;
                        curDistAbs = Mathf.Abs(curDist);
                        if (curDistAbs < curMinDist)
                        {
                            curMinDist = curDistAbs;
                            curIndex = i;
                        }
                        else
                        {
                            break;
                        }

                        if ((i + 1) < count)
                        {
                            start = end;
                            end = end + mItemList[i + 1].sizeByPadding;
                            itemSnapCenter = start + mItemList[i + 1].size * _snapPivot.x;
                        }
                    }
                }


                if (curIndex >= 0)
                {
                    int oldNearestItemIndex = mCurSnapNearestItemIndex;
                    mCurSnapNearestItemIndex = mItemList[curIndex].index;
                    if (mItemList[curIndex].index != oldNearestItemIndex)
                    {
                        if (onSnapNearestChanged != null)
                        {
                            onSnapNearestChanged(this, mItemList[curIndex]);
                        }
                    }
                }
                else
                {
                    mCurSnapNearestItemIndex = -1;
                }
            }
            bool canSnap = true;
            if (mScrollBarClickEventListener != null)
            {
                canSnap = !(mScrollBarClickEventListener.isPress);
            }
            float v = Mathf.Abs(mScrollRect.velocity.x);
            if (canSnap && !_draging && v < mSnapVecThreshold)
            {
                UpdateCurSnapData();
                if (mCurSnapData.mSnapStatus != SnapStatus.SnapMoving)
                {
                    return;
                }
                if (v > 0)
                {
                    mScrollRect.StopMovement();
                }
                float old = mCurSnapData.mCurSnapVal;
                mCurSnapData.mCurSnapVal = Mathf.SmoothDamp(mCurSnapData.mCurSnapVal, mCurSnapData.mTargetSnapVal, ref mSmoothDumpVel, mSmoothDumpRate);
                float dt = mCurSnapData.mCurSnapVal - old;

                if (immediate || Mathf.Abs(mCurSnapData.mTargetSnapVal - mCurSnapData.mCurSnapVal) < mSnapFinishThreshold)
                {
                    pos.x = pos.x + mCurSnapData.mTargetSnapVal - mCurSnapData.mCurSnapVal;
                    mCurSnapData.mSnapStatus = SnapStatus.SnapMoveFinish;
                    if (onSnapFinished != null)
                    {
                        LoopListViewItem targetItem = GetShownItemByItemIndex(mCurSnapNearestItemIndex);
                        if (targetItem != null)
                        {
                            onSnapFinished(this, targetItem);
                        }
                    }
                }
                else
                {
                    pos.x = pos.x + dt;
                }

                if (_arrangeType == ArrangeType.LeftToRight)
                {
                    float minX = mViewPortRectLocalCorners[2].x - content.rect.width;
                    if (pos.x >= minX && pos.x <= 0)
                    {
                        content.localPosition = pos;
                    }
                }
                else if (_arrangeType == ArrangeType.RightToLeft)
                {
                    float maxX = mViewPortRectLocalCorners[1].x + content.rect.width;
                    if (pos.x <= maxX && pos.x >= 0)
                    {
                        content.localPosition = pos;
                    }
                }

            }

        }
        bool UpdateForVertList(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (mItemTotalCount == 0)
            {
                if (mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return false;
            }
            if (_arrangeType == ArrangeType.TopToBottom)
            {
                int itemListCount = mItemList.Count;
                if (itemListCount == 0)
                {
                    float curY = content.localPosition.y;
                    if (curY < 0)
                    {
                        curY = 0;
                    }
                    int index = 0;
                    float pos = -curY;
                    if (_supportScrollBar)
                    {
                        GetPlusItemIndexAndPosAtGivenPos(curY, ref index, ref pos);
                        pos = -pos;
                    }
                    LoopListViewItem newItem = GetNewItemByIndex(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_supportScrollBar)
                    {
                        SetItemSize(index, newItem.rect.rect.height, newItem.padding);
                    }
                    mItemList.Add(newItem);
                    newItem.rect.localPosition = new Vector3(newItem.offset, pos, 0);
                    UpdateContentSize();
                    return true;
                }
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 topPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 downPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[0]);

                if (!_draging && tViewItem0.check != mListUpdateCheckFrameCount
                    && downPos0.y - mViewPortRectLocalCorners[1].y > distanceForRecycle0)
                {
                    mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!_supportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                LoopListViewItem tViewItem1 = mItemList[mItemList.Count - 1];
                tViewItem1.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 topPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 downPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[0]);
                if (!_draging && tViewItem1.check != mListUpdateCheckFrameCount
                    && mViewPortRectLocalCorners[0].y - topPos1.y > distanceForRecycle1)
                {
                    mItemList.RemoveAt(mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!_supportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }



                if (mViewPortRectLocalCorners[0].y - downPos1.y < distanceForNew1)
                {
                    if (tViewItem1.index > mCurReadyMaxItemIndex)
                    {
                        mCurReadyMaxItemIndex = tViewItem1.index;
                        mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.index + 1;
                    if (nIndex <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
                    {
                        LoopListViewItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mCurReadyMaxItemIndex = tViewItem1.index;
                            mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdataItemPos();
                        }
                        else
                        {
                            if (_supportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.rect.rect.height, newItem.padding);
                            }
                            mItemList.Add(newItem);
                            float y = tViewItem1.rect.localPosition.y - tViewItem1.rect.rect.height - tViewItem1.padding;
                            newItem.rect.localPosition = new Vector3(newItem.offset, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();

                            if (nIndex > mCurReadyMaxItemIndex)
                            {
                                mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

                if (topPos0.y - mViewPortRectLocalCorners[1].y < distanceForNew0)
                {
                    if (tViewItem0.index < mCurReadyMinItemIndex)
                    {
                        mCurReadyMinItemIndex = tViewItem0.index;
                        mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.index - 1;
                    if (nIndex >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
                    {
                        LoopListViewItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mCurReadyMinItemIndex = tViewItem0.index;
                            mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (_supportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.rect.rect.height, newItem.padding);
                            }
                            mItemList.Insert(0, newItem);
                            float y = tViewItem0.rect.localPosition.y + newItem.rect.rect.height + newItem.padding;
                            newItem.rect.localPosition = new Vector3(newItem.offset, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex < mCurReadyMinItemIndex)
                            {
                                mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

            }
            else
            {

                if (mItemList.Count == 0)
                {
                    float curY = content.localPosition.y;
                    if (curY > 0)
                    {
                        curY = 0;
                    }
                    int index = 0;
                    float pos = -curY;
                    if (_supportScrollBar)
                    {
                        GetPlusItemIndexAndPosAtGivenPos(-curY, ref index, ref pos);
                    }
                    LoopListViewItem newItem = GetNewItemByIndex(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_supportScrollBar)
                    {
                        SetItemSize(index, newItem.rect.rect.height, newItem.padding);
                    }
                    mItemList.Add(newItem);
                    newItem.rect.localPosition = new Vector3(newItem.offset, pos, 0);
                    UpdateContentSize();
                    return true;
                }
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 topPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 downPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[0]);

                if (!_draging && tViewItem0.check != mListUpdateCheckFrameCount
                    && mViewPortRectLocalCorners[0].y - topPos0.y > distanceForRecycle0)
                {
                    mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!_supportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                LoopListViewItem tViewItem1 = mItemList[mItemList.Count - 1];
                tViewItem1.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 topPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 downPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[0]);
                if (!_draging && tViewItem1.check != mListUpdateCheckFrameCount
                     && downPos1.y - mViewPortRectLocalCorners[1].y > distanceForRecycle1)
                {
                    mItemList.RemoveAt(mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!_supportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                if (topPos1.y - mViewPortRectLocalCorners[1].y < distanceForNew1)
                {
                    if (tViewItem1.index > mCurReadyMaxItemIndex)
                    {
                        mCurReadyMaxItemIndex = tViewItem1.index;
                        mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.index + 1;
                    if (nIndex <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
                    {
                        LoopListViewItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdataItemPos();
                        }
                        else
                        {
                            if (_supportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.rect.rect.height, newItem.padding);
                            }
                            mItemList.Add(newItem);
                            float y = tViewItem1.rect.localPosition.y + tViewItem1.rect.rect.height + tViewItem1.padding;
                            newItem.rect.localPosition = new Vector3(newItem.offset, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex > mCurReadyMaxItemIndex)
                            {
                                mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }


                if (mViewPortRectLocalCorners[0].y - downPos0.y < distanceForNew0)
                {
                    if (tViewItem0.index < mCurReadyMinItemIndex)
                    {
                        mCurReadyMinItemIndex = tViewItem0.index;
                        mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.index - 1;
                    if (nIndex >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
                    {
                        LoopListViewItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mNeedCheckNextMinItem = false;
                            return false;
                        }
                        else
                        {
                            if (_supportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.rect.rect.height, newItem.padding);
                            }
                            mItemList.Insert(0, newItem);
                            float y = tViewItem0.rect.localPosition.y - newItem.rect.rect.height - newItem.padding;
                            newItem.rect.localPosition = new Vector3(newItem.offset, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex < mCurReadyMinItemIndex)
                            {
                                mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }
                }


            }

            return false;

        }
        bool UpdateForHorizontalList(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (mItemTotalCount == 0)
            {
                if (mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return false;
            }
            if (_arrangeType == ArrangeType.LeftToRight)
            {

                if (mItemList.Count == 0)
                {
                    float curX = content.localPosition.x;
                    if (curX > 0)
                    {
                        curX = 0;
                    }
                    int index = 0;
                    float pos = -curX;
                    if (_supportScrollBar)
                    {
                        GetPlusItemIndexAndPosAtGivenPos(-curX, ref index, ref pos);
                    }
                    LoopListViewItem newItem = GetNewItemByIndex(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_supportScrollBar)
                    {
                        SetItemSize(index, newItem.rect.rect.width, newItem.padding);
                    }
                    mItemList.Add(newItem);
                    newItem.rect.localPosition = new Vector3(pos, newItem.offset, 0);
                    UpdateContentSize();
                    return true;
                }
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 leftPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 rightPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[2]);

                if (!_draging && tViewItem0.check != mListUpdateCheckFrameCount
                    && mViewPortRectLocalCorners[1].x - rightPos0.x > distanceForRecycle0)
                {
                    mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!_supportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                LoopListViewItem tViewItem1 = mItemList[mItemList.Count - 1];
                tViewItem1.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 leftPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 rightPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[2]);
                if (!_draging && tViewItem1.check != mListUpdateCheckFrameCount
                    && leftPos1.x - mViewPortRectLocalCorners[2].x > distanceForRecycle1)
                {
                    mItemList.RemoveAt(mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!_supportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }



                if (rightPos1.x - mViewPortRectLocalCorners[2].x < distanceForNew1)
                {
                    if (tViewItem1.index > mCurReadyMaxItemIndex)
                    {
                        mCurReadyMaxItemIndex = tViewItem1.index;
                        mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.index + 1;
                    if (nIndex <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
                    {
                        LoopListViewItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mCurReadyMaxItemIndex = tViewItem1.index;
                            mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdataItemPos();
                        }
                        else
                        {
                            if (_supportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.rect.rect.width, newItem.padding);
                            }
                            mItemList.Add(newItem);
                            float x = tViewItem1.rect.localPosition.x + tViewItem1.rect.rect.width + tViewItem1.padding;
                            newItem.rect.localPosition = new Vector3(x, newItem.offset, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();

                            if (nIndex > mCurReadyMaxItemIndex)
                            {
                                mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

                if (mViewPortRectLocalCorners[1].x - leftPos0.x < distanceForNew0)
                {
                    if (tViewItem0.index < mCurReadyMinItemIndex)
                    {
                        mCurReadyMinItemIndex = tViewItem0.index;
                        mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.index - 1;
                    if (nIndex >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
                    {
                        LoopListViewItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mCurReadyMinItemIndex = tViewItem0.index;
                            mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (_supportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.rect.rect.width, newItem.padding);
                            }
                            mItemList.Insert(0, newItem);
                            float x = tViewItem0.rect.localPosition.x - newItem.rect.rect.width - newItem.padding;
                            newItem.rect.localPosition = new Vector3(x, newItem.offset, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex < mCurReadyMinItemIndex)
                            {
                                mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

            }
            else
            {

                if (mItemList.Count == 0)
                {
                    float curX = content.localPosition.x;
                    if (curX < 0)
                    {
                        curX = 0;
                    }
                    int index = 0;
                    float pos = -curX;
                    if (_supportScrollBar)
                    {
                        GetPlusItemIndexAndPosAtGivenPos(curX, ref index, ref pos);
                        pos = -pos;
                    }
                    LoopListViewItem newItem = GetNewItemByIndex(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (_supportScrollBar)
                    {
                        SetItemSize(index, newItem.rect.rect.width, newItem.padding);
                    }
                    mItemList.Add(newItem);
                    newItem.rect.localPosition = new Vector3(pos, newItem.offset, 0);
                    UpdateContentSize();
                    return true;
                }
                LoopListViewItem tViewItem0 = mItemList[0];
                tViewItem0.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 leftPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 rightPos0 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[2]);

                if (!_draging && tViewItem0.check != mListUpdateCheckFrameCount
                    && leftPos0.x - mViewPortRectLocalCorners[2].x > distanceForRecycle0)
                {
                    mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!_supportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                LoopListViewItem tViewItem1 = mItemList[mItemList.Count - 1];
                tViewItem1.rect.GetWorldCorners(mItemWorldCorners);
                Vector3 leftPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 rightPos1 = scroll.viewport.InverseTransformPoint(mItemWorldCorners[2]);
                if (!_draging && tViewItem1.check != mListUpdateCheckFrameCount
                    && mViewPortRectLocalCorners[1].x - rightPos1.x > distanceForRecycle1)
                {
                    mItemList.RemoveAt(mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!_supportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }



                if (mViewPortRectLocalCorners[1].x - leftPos1.x < distanceForNew1)
                {
                    if (tViewItem1.index > mCurReadyMaxItemIndex)
                    {
                        mCurReadyMaxItemIndex = tViewItem1.index;
                        mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.index + 1;
                    if (nIndex <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
                    {
                        LoopListViewItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mCurReadyMaxItemIndex = tViewItem1.index;
                            mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdataItemPos();
                        }
                        else
                        {
                            if (_supportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.rect.rect.width, newItem.padding);
                            }
                            mItemList.Add(newItem);
                            float x = tViewItem1.rect.localPosition.x - tViewItem1.rect.rect.width - tViewItem1.padding;
                            newItem.rect.localPosition = new Vector3(x, newItem.offset, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();

                            if (nIndex > mCurReadyMaxItemIndex)
                            {
                                mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

                if (rightPos0.x - mViewPortRectLocalCorners[2].x < distanceForNew0)
                {
                    if (tViewItem0.index < mCurReadyMinItemIndex)
                    {
                        mCurReadyMinItemIndex = tViewItem0.index;
                        mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.index - 1;
                    if (nIndex >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
                    {
                        LoopListViewItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mCurReadyMinItemIndex = tViewItem0.index;
                            mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (_supportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.rect.rect.width, newItem.padding);
                            }
                            mItemList.Insert(0, newItem);
                            float x = tViewItem0.rect.localPosition.x + newItem.rect.rect.width + newItem.padding;
                            newItem.rect.localPosition = new Vector3(x, newItem.offset, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex < mCurReadyMinItemIndex)
                            {
                                mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

            }

            return false;

        }
        float GetContentPanelSize()
        {
            if (_supportScrollBar)
            {
                float tTotalSize = mItemPosMgr.mTotalSize > 0 ? (mItemPosMgr.mTotalSize - mLastItemPadding) : 0;
                if (tTotalSize < 0)
                {
                    tTotalSize = 0;
                }
                return tTotalSize;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return 0;
            }
            if (count == 1)
            {
                return mItemList[0].size;
            }
            if (count == 2)
            {
                return mItemList[0].sizeByPadding + mItemList[1].size;
            }
            float s = 0;
            for (int i = 0; i < count - 1; ++i)
            {
                s += mItemList[i].sizeByPadding;
            }
            s += mItemList[count - 1].size;
            return s;
        }
        void CheckIfNeedUpdataItemPos()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            if (_arrangeType == ArrangeType.TopToBottom)
            {
                LoopListViewItem firstItem = mItemList[0];
                LoopListViewItem lastItem = mItemList[mItemList.Count - 1];
                float viewMaxY = GetContentPanelSize();
                if (firstItem.top > 0 || (firstItem.index == mCurReadyMinItemIndex && firstItem.top != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                if ((-lastItem.bottom) > viewMaxY || (lastItem.index == mCurReadyMaxItemIndex && (-lastItem.bottom) != viewMaxY))
                {
                    UpdateAllShownItemsPos();
                    return;
                }

            }
            else if (_arrangeType == ArrangeType.BottomToTop)
            {
                LoopListViewItem firstItem = mItemList[0];
                LoopListViewItem lastItem = mItemList[mItemList.Count - 1];
                float viewMaxY = GetContentPanelSize();
                if (firstItem.bottom < 0 || (firstItem.index == mCurReadyMinItemIndex && firstItem.bottom != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                if (lastItem.top > viewMaxY || (lastItem.index == mCurReadyMaxItemIndex && lastItem.top != viewMaxY))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
            }
            else if (_arrangeType == ArrangeType.LeftToRight)
            {
                LoopListViewItem firstItem = mItemList[0];
                LoopListViewItem lastItem = mItemList[mItemList.Count - 1];
                float viewMaxX = GetContentPanelSize();
                if (firstItem.left < 0 || (firstItem.index == mCurReadyMinItemIndex && firstItem.left != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                if ((lastItem.right) > viewMaxX || (lastItem.index == mCurReadyMaxItemIndex && lastItem.right != viewMaxX))
                {
                    UpdateAllShownItemsPos();
                    return;
                }

            }
            else if (_arrangeType == ArrangeType.RightToLeft)
            {
                LoopListViewItem firstItem = mItemList[0];
                LoopListViewItem lastItem = mItemList[mItemList.Count - 1];
                float viewMaxX = GetContentPanelSize();
                if (firstItem.right > 0 || (firstItem.index == mCurReadyMinItemIndex && firstItem.right != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                if ((-lastItem.left) > viewMaxX || (lastItem.index == mCurReadyMaxItemIndex && (-lastItem.left) != viewMaxX))
                {
                    UpdateAllShownItemsPos();
                    return;
                }

            }

        }
        void UpdateAllShownItemsPos()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }

            mAdjustedVec = (content.localPosition - mLastFrameContainerPos) / Time.deltaTime;

            if (_arrangeType == ArrangeType.TopToBottom)
            {
                float pos = 0;
                if (_supportScrollBar)
                {
                    pos = -GetItemPos(mItemList[0].index);
                }
                float pos1 = mItemList[0].rect.localPosition.y;
                float d = pos - pos1;
                float curY = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopListViewItem item = mItemList[i];
                    item.rect.localPosition = new Vector3(item.offset, curY, 0);
                    curY = curY - item.rect.rect.height - item.padding;
                }
                if (d != 0)
                {
                    Vector2 p = content.localPosition;
                    p.y = p.y - d;
                    content.localPosition = p;
                }

            }
            else if (_arrangeType == ArrangeType.BottomToTop)
            {
                float pos = 0;
                if (_supportScrollBar)
                {
                    pos = GetItemPos(mItemList[0].index);
                }
                float pos1 = mItemList[0].rect.localPosition.y;
                float d = pos - pos1;
                float curY = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopListViewItem item = mItemList[i];
                    item.rect.localPosition = new Vector3(item.offset, curY, 0);
                    curY = curY + item.rect.rect.height + item.padding;
                }
                if (d != 0)
                {
                    Vector3 p = content.localPosition;
                    p.y = p.y - d;
                    content.localPosition = p;
                }
            }
            else if (_arrangeType == ArrangeType.LeftToRight)
            {
                float pos = 0;
                if (_supportScrollBar)
                {
                    pos = GetItemPos(mItemList[0].index);
                }
                float pos1 = mItemList[0].rect.localPosition.x;
                float d = pos - pos1;
                float curX = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopListViewItem item = mItemList[i];
                    item.rect.localPosition = new Vector3(curX, item.offset, 0);
                    curX = curX + item.rect.rect.width + item.padding;
                }
                if (d != 0)
                {
                    Vector3 p = content.localPosition;
                    p.x = p.x - d;
                    content.localPosition = p;
                }

            }
            else if (_arrangeType == ArrangeType.RightToLeft)
            {
                float pos = 0;
                if (_supportScrollBar)
                {
                    pos = -GetItemPos(mItemList[0].index);
                }
                float pos1 = mItemList[0].rect.localPosition.x;
                float d = pos - pos1;
                float curX = pos;
                for (int i = 0; i < count; ++i)
                {
                    LoopListViewItem item = mItemList[i];
                    item.rect.localPosition = new Vector3(curX, item.offset, 0);
                    curX = curX - item.rect.rect.width - item.padding;
                }
                if (d != 0)
                {
                    Vector3 p = content.localPosition;
                    p.x = p.x - d;
                    content.localPosition = p;
                }

            }
            if (_draging)
            {
                mScrollRect.OnBeginDrag(mPointerEventData);
                mScrollRect.Rebuild(CanvasUpdate.PostLayout);
                mScrollRect.velocity = mAdjustedVec;
                mNeedAdjustVec = true;
            }
        }
        void UpdateContentSize()
        {
            float size = GetContentPanelSize();
            if (_vertical)
            {
                if (content.rect.height != size)
                {
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                }
            }
            else
            {
                if (content.rect.width != size)
                {
                    content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                }
            }
        }


    }
}
