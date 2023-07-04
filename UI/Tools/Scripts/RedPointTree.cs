/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.181
 *UnityVersion:   2019.4.22f1c1
 *Date:           2021-12-19
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework.UI
{
	public class RedPointTree
	{
		private class RedPoint
		{
			public string parentKey;
			public string key;
			private event Action<int> onCountChange;
			private int _count;
			public int count { get { return _count; } private set { _count = value; Publish(); } }
			public bool SetCount(int count)
			{
				if (count == _count) return false;
				this.count = count;
				return true;
			}
			public void Bind(Action<int> call)
			{
				onCountChange += call;
				Publish();
			}
			public void UnBind(Action<int> call)
			{
				onCountChange -= call;
			}
			public void Publish()
			{
				onCountChange?.Invoke(_count);
			}
		}
		private Dictionary<string, RedPoint> keyMap = new Dictionary<string, RedPoint>();
		private List<RedPoint> points = new List<RedPoint>();
		public void ReadPoint(string key, char separator)
        {
			var columns = key.Split(separator);
			for (int j = 0; j < columns.Length; j++)
			{
				var _pkey = string.Join(separator.ToString(), columns, 0, j);
				var _key = string.Join(separator.ToString(), columns, 0, j + 1);
				Set(_pkey, _key);
			}
		}
		public void ReadTree(string[] keys, char separator)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				ReadPoint(keys[i], separator);
			}
		}
		public void RemovePoint(string key)
        {
			if (!keyMap.ContainsKey(key)) return;
			points.Remove(keyMap[key]);
			keyMap.Remove(key);
        }
		public void SetCount(string key, int count)
		{
			var dot = Find(key);
			var children = GetChildren(dot);
			if (children != null && children.Count > 0) return;
			if (dot.SetCount(count))
			{
				FreshPrent(dot.parentKey);
			}
		}
		public int GetCount(string key)
        {
			return Find(key).count;
		}
		public string GetParentKey(string key)
        {
			return Find(key).parentKey;
		}
		public void Subscribe(string key, Action<int> callback)
		{
			Find(key).Bind(callback);
		}
		public void UnSubscribe(string key, Action<int> callback)
		{
			Find(key).UnBind(callback);
		}
		public void Fresh(string key)
        {
			Find(key).Publish();
		}
		private void Set(string parentKey, string key)
		{
			if (Find(key) != null) return ;
			RedPoint point = new RedPoint() { parentKey = parentKey, key = key };
			points.Add(point);
			keyMap.Add(point.key, point);
		}
		private RedPoint Find(string key)
		{
            if (keyMap.ContainsKey(key))
            {
			   return keyMap[key];
            }
			return null;
		}
		private List<RedPoint> GetChildren(RedPoint point)
		{
			return points.FindAll((_dot) => { return _dot.parentKey == point.key; });
		}
		private void FreshPrent(string key)
		{
			if (string.IsNullOrEmpty(key)) return;
			var point = Find(key);
			var children = GetChildren(point);
			int sum = 0;
			for (int i = 0; i < children.Count; i++)
			{
				sum += children[i].count;
			}
            if (point.SetCount(sum))
            {
				FreshPrent(point.parentKey);
			}
		}

	}

}
