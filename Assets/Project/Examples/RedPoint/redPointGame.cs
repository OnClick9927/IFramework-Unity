using IFramework;
using IFramework.RedPoint;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class redPointGame : Game
{
    public Toggle add;
    public List<Button> btns = new List<Button>();
    public List<string> keys = new List<string>() {
    "11",
    "11/22",
    "11/33",
    };
    private class Dot : RedDot
    {
        public Text text;

        public Dot(string path, Text text)
        {
            this.text = text;
            SetPath(path);
        }

        public override void FreshView(int count)
        {
            text.text = count.ToString();
        }
    }
    public override void Init()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            RedTree.ReadPath(keys[i]);
        }
        for (int i = 0; i < btns.Count; i++)
        {
            var index = i;
            var btnsNode = btns[i];
            var key = keys[index];
            var text = btnsNode.GetComponentInChildren<Text>();
            new Dot(key, text);
            new RedActiveDot(key, text.gameObject);
            btnsNode.onClick.AddListener(() =>
            {
                RedTree.SetCount(key, RedTree.GetCount(key) + (add.isOn ? 1 : -1));
            });


        }

    }

    public override void Startup()
    {

    }
    private void OnGUI()
    {
        if (GUILayout.Button("open WIndow", new GUIStyle("button") { fontSize = 40 }, GUILayout.Height(300), GUILayout.Width(300)))
        {
            IFramework.EditorTools.EditorWindowTool.Create("RedPoint");

        }
    }

}
