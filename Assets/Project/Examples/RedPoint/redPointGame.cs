using IFramework;
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

        //public Dot(string path, Text text)
        //{
        //    this.text = text;
        //    SetPath(path);
        //}

        public override void FreshView(int count)
        {
            text.text = count.ToString();
        }
    }

    protected override void Startup()
    {
        this.UseRedTree();
        for (int i = 0; i < keys.Count; i++)
        {
            this.RedTree().ReadPath(keys[i]);
        }
        for (int i = 0; i < btns.Count; i++)
        {
            var index = i;
            var btnsNode = btns[i];
            var key = keys[index];
            var text = btnsNode.GetComponentInChildren<Text>();
            this.RedTree().CreateRedDot<Dot>(key, (op) =>
            {
                op.text = text;
            });
            this.RedTree().CreateRedActiveDot(key, text.gameObject);
            btnsNode.onClick.AddListener(() =>
            {
                this.RedTree().SetCount(key, this.RedTree().GetCount(key) + (add.isOn ? 1 : -1));
            });


        }
    }
    private void OnGUI()
    {
        if (GUILayout.Button("open WIndow", new GUIStyle("button") { fontSize = 40 }, GUILayout.Height(300), GUILayout.Width(300)))
        {
            IFramework.EditorTools.EditorWindowTool.Create("RedPoint");

        }
        this.RedTree().FreshDots();
    }

}
