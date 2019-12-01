/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-10-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.GUITool.LayoutDesign;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace IFramework_Demo.GUIGames
{
    [EditorWindowCache("IFramework_Demo.GUIGames.Game_2048")]
	public class Game2048:EditorWindow
	{
        GUICanvas guiCanvas;
        private int[,] arr = new int[4, 4];
        private int Score = 0;

        private void OnEnable()
        {
            XmlDocument doc = new XmlDocument();
            string path= "Assets/Examples/GUIGames/2048/Resources/Game_2048.xml";
            doc.Load(path);
            guiCanvas = new GUICanvas();
            guiCanvas.DeSerialize(doc.DocumentElement);
            guiCanvas.Find<Button>("ImageArea/up/up").OnClick = UpClick;
            guiCanvas.Find<Button>("ImageArea/down/left").OnClick = LeftClick;
            guiCanvas.Find<Button>("ImageArea/down/down").OnClick = DownClick;
            guiCanvas.Find<Button>("ImageArea/down/right").OnClick = RigntClick;
            guiCanvas.Find<Button>("btn/reset").OnClick = Reset;
            Reset();
        }

        private void Reset()
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    arr[i, j] = 0;
            Score = 0;
            RandomCreate2();
            RandomCreate2();
        }

        private bool RandomCreate2()
        {
            List<Vector2Int> pos = new List<Vector2Int>();
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (arr[i, j] == 0)
                        pos.Add(new Vector2Int(i, j));
            if (pos.Count == 0) return false;
           int index= Random.Range(0, pos.Count);
            arr[pos[index].x, pos[index].y] = 2;
            return true;
        }
        private void gainScore(int score)
        {
            Score += score;
        }
        private const int size = 4; 


        private void LeftClick()
        {
            MoveLeft(arr);
            RandomCreate2();
        }
        private void RigntClick()
        {
            MoveRight(arr);
            RandomCreate2();
        }
        private void DownClick()
        {
            MoveDown(arr);
            RandomCreate2();
        }
        private void UpClick()
        {
            MoveUp(arr);
            RandomCreate2();
        }


        private  void MoveUp(int[,] resaveArr)
        {
            int[] temp = new int[size];

            for (int i = 0; i < resaveArr.GetLength(1); i++)
            {
                for (int j = 0; j < resaveArr.GetLength(0); j++)
                {
                    temp[j] = resaveArr[j, i];
                }
                AddLine(temp);
                for (int k = 0; k < temp.Length; k++)
                {
                    resaveArr[k, i] = temp[k];
                }
            }

        }
        //下移函数
        private  void MoveDown(int[,] resaveArr)
        {
            int[] temp = new int[size];

            for (int i = 0; i < resaveArr.GetLength(1); i++)
            {
                for (int j = 0; j < resaveArr.GetLength(0); j++)
                {
                    temp[j] = resaveArr[size-1 - j, i];
                }
                AddLine(temp);
                for (int k = 0; k < temp.Length; k++)
                {
                    resaveArr[size-1 - k, i] = temp[k];
                }
            }

        }
        //左移函数
        private  void MoveLeft(int[,] resaveArr)
        {
            int[] temp = new int[size];
            for (int i = 0; i < resaveArr.GetLength(0); i++)
            {
                for (int j = 0; j < resaveArr.GetLength(1); j++)
                {
                    temp[j] = resaveArr[i, j];
                }
                 AddLine(temp);
                for (int k = 0; k < temp.Length; k++)
                {
                    resaveArr[i, k] = temp[k];
                }
            }

        }
        //右移函数
        private  void MoveRight(int[,] resaveArr)
        {
            int[] temp = new int[size];
            for (int i = 0; i < resaveArr.GetLength(0); i++)
            {
                for (int j = 0; j < resaveArr.GetLength(1); j++)
                {
                    temp[j] = resaveArr[i, size-1 - j];
                }
                AddLine(temp);
                for (int k = 0; k < temp.Length; k++)
                {
                    resaveArr[i, size-1 - k] = temp[k];
                }
            }
        }
        //排序函数,将为0的值靠后,非0的值提前
        private  void RemvoeZero(int[] arr)
        {
            int index = 0;
            int[] arr0 = new int[size];
            foreach (int each in arr)
            {
                if (each != 0) arr0[index++] = each;
            }
            arr0.CopyTo(arr, 0);
        }
        //数组中相邻的同样的数相加的方法
        private void AddLine(int[] arr)
        {
            RemvoeZero(arr);
            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (arr[i] != 0 && arr[i] == arr[i + 1])
                {
                    arr[i] += arr[i + 1];
                    arr[i + 1] = 0;
                    gainScore(arr[i] / 2);
                }
            }
            RemvoeZero(arr);
        }

        private void FreshView()
        {
            guiCanvas.Find<Label>("Horizontal/score").text = Score.ToString();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int val = arr[i, j];
                    if (val != 0)
                        guiCanvas.Find<Label>(string.Format("map/{0}/{1}", i + 1, j + 1)).text = val.ToString();
                    else
                        guiCanvas.Find<Label>(string.Format("map/{0}/{1}", i + 1, j + 1)).text = "";
                }
            }
        }
        private void OnGUI()
        {
            FreshView();
            guiCanvas.OnGUI();

            float val = (float)EditorApplication.timeSinceStartup % 1f;
            guiCanvas.Find<Horizontal>("Horizontal").color = new Color(val, val,val, val);
            guiCanvas.Find<Button>("btn/reset").rotateAngle = ((float)EditorApplication.timeSinceStartup *20)% 360;
            Repaint();
        }

      
    }
}
