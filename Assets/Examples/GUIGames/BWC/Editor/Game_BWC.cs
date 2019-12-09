/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.14f1
 *Date:           2019-10-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.GUITool.LayoutDesign;
using System;
using System.Xml;
using UnityEditor;
using UnityEngine;
namespace IFramework_Demo.GUIGames
{
    [EditorWindowCache("IFramework_Demo.GUIGames.黑白棋")]
	public class Game_BWC:EditorWindow
	{
        public enum Turn
        {
            black,white
        }
        private class Map
        {
            public void Reset()
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        ps[i, j].Reset();
                    }
                }
            }
            public class Point
            {
                public void Reset()
                {
                    state = PointState.Nothing;
                    btn.enable = true;
                     btn.image = null;
                }
                public enum PointState
                {
                    Nothing,ShowWhite,ShowBlack
                }
                public PointState State
                {
                    get { return state; }
                    set
                    {
                        state = value;
                        if (value == PointState.ShowBlack)
                        {
                            btn.enable = false;
                            btn.image = BC;
                               
                        }
                        else if (value == PointState.ShowWhite)
                        {
                            btn.enable = false;
                            btn.image = WC;

                        }
                        //else
                        //{
                        //    btn.enable = true;
                        //    btn.image = null;
                        //}
                    }
                }
                private PointState state;
                public Vector2Int position;
                public Button btn;
                public Point(Button Btn,Action<Point> btnCallBack, Vector2Int position)
                {
                    this.btn = Btn;
                    state  = PointState.Nothing;  
                    this.position = position;
                    Btn.OnClick = () =>
                    {
                        if (State== PointState.Nothing)
                        {
                            if (Trun == Turn.black)
                                State = PointState.ShowBlack;
                            else
                                State = PointState.ShowWhite;
                            if (btnCallBack != null) btnCallBack(this);
                        }

                    };
                }
            }

            private void CheckLeft(Point p)
            {
                Vector2Int pos = p.position;
                Point friend=null;
                for (int j = pos.y-1; j >= 0; j--)
                {
                    var tmp = ps[pos.x, j];
                    if (tmp.State == Point.PointState.Nothing) return;
                    if (tmp.State==p.State)
                    {
                        friend = tmp;
                        break;
                    }
                }
                if (friend!=null)
                    for (int j = friend.position.y; j < pos.y; j++)
                        ps[pos.x, j].State = p.State;
            }
            private void CheckRight(Point p)
            {
                Vector2Int pos = p.position;
                Point friend = null;
                for (int j = pos.y+1; j < 10; j++)
                {
                    var tmp = ps[pos.x, j];
                    if (tmp.State == Point.PointState.Nothing) return;
                    if (tmp.State == p.State)
                    {
                        friend = tmp;
                        break;
                    }
                }
                if (friend != null)
                    for (int j = friend.position.y; j > pos.y; j--)
                        ps[pos.x, j].State = p.State;
            }
            private void CheckUp(Point p)
            {
                Vector2Int pos = p.position;
                Point friend = null;
                for (int i = pos.x-1; i >= 0; i--)
                {
                    var tmp = ps[i,pos.y];
                    if (tmp.State == Point.PointState.Nothing) return;
                    if (tmp.State == p.State)
                    {
                        friend = tmp;
                        break;
                    }
                }
                if (friend != null)
                    for (int i = friend.position.x; i < pos.x; i++)
                        ps[i, pos.y].State = p.State;
            }
            private void CheckDown(Point p)
            {
                Vector2Int pos = p.position;
                Point friend = null;
                for (int i = pos.x+1; i < 10; i++)
                {
                    var tmp = ps[i, pos.y];
                    if (tmp.State == Point.PointState.Nothing) return;
                    if (tmp.State == p.State)
                    {
                        friend = tmp;
                        break;
                    }
                }
                if (friend != null)
                    for (int i = friend.position.x; i > pos.x; i--)
                        ps[i, pos.y].State = p.State;
            }

            private void CheckFinish()
            {
                int wcount = 0, bcount = 0;
                for (int i = 0; i < ps.GetLength(0); i++)
                {
                    for (int j = 0; j < ps.GetLength(1); j++)
                    {
                        Point tmp = ps[i, j];
                        switch (tmp.State)
                        {
                            //case Point.PointState.Nothing:
                            //    return;
                            case Point.PointState.ShowWhite:
                                wcount++;
                                break;
                            case Point.PointState.ShowBlack:
                                bcount++;
                                break;

                        }
                    }
                }
                wCount.text = wcount.ToString();
                bCount.text = bcount.ToString();
                if (wcount+bcount==100)
                    ShowResult(wcount > bcount);

            }
            public Point[,] ps=new Point[10,10];
            
            public Map(Area area)
            {
                Action<Point> del = (point) =>
                {
                    PlayCheck();
                    CheckLeft(point);
                    CheckRight(point);
                    CheckUp(point);
                    CheckDown(point);
                    if (Trun == Turn.white)
                        Trun = Turn.black;
                    else
                        Trun = Turn.white;
                    CheckFinish();
                };
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Button btn = area.Find<Button>(string.Format("{0}/{1}", i + 1, j + 1));
                        Point p = new Point(btn, del, new Vector2Int(i, j));
                        ps[i,j]=p;
                    }
                }
            }

        }

        private static Turn Trun { get { return t; }set { t = value;
                if (value == Turn.black)
                    guiCanvas.Find<ImageLabel>("top/ImageLabel").image = BC;
                else
                    guiCanvas.Find<ImageLabel>("top/ImageLabel").image = WC;
            } }
        private static Turn t;
       static GUICanvas guiCanvas;
       static Texture2D WC, BC;
       static Map map;
        static ImageArea jiesuan;
        static Label wCount, bCount;
        private void OnEnable()
        {
            this.maxSize = new Vector2(600, 600);

            XmlDocument doc = new XmlDocument();
            string path = "Assets/Examples/GUIGames/BWC/Editor/Game_BWC.xml";
            doc.Load(path);
            guiCanvas = new GUICanvas();
            guiCanvas.DeSerialize(doc.DocumentElement);
            WC = Resources.Load<Texture2D>("bai");
            BC = Resources.Load<Texture2D>("hei");
            map = new Map(guiCanvas.Find<Area>("map"));
            wCount= guiCanvas.Find<Label>("top/count/w/count");
            bCount = guiCanvas.Find<Label>("top/count/b/count");
            jiesuan = guiCanvas.Find<ImageArea>("jiesuan");


            GameObject go = new GameObject("BG");
            go.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            BG = go.AddComponent<AudioSource>();
            BG.clip = Resources.Load<AudioClip>("BG");
            BG.Play();
            BG.loop = true;

            go = new GameObject("sound");
            go.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            Sound = go.AddComponent<AudioSource>();
        }

        private void OnDisable()
        {
            DestroyImmediate(BG.gameObject);
            DestroyImmediate(Sound.gameObject);

        }

        AudioSource BG;
        static AudioSource Sound;
        public static void PlayCheck()
        {
            Sound.clip = Resources.Load<AudioClip>("c");
            Sound.Stop();
            Sound.Play();

        }
        public static void PlayWin()
        {
            Sound.clip = Resources.Load<AudioClip>("win");
            Sound.Stop();
            Sound.Play();
        }
        private static void ShowResult(bool iswhiteWin)
        {
            PlayWin();
            jiesuan.active = true;
            jiesuan.Find<ImageLabel>("ImageHorizontal/image").image = iswhiteWin ? WC : BC;
            jiesuan.Find<Button>("ImageHorizontal/right/OK").OnClick = () =>
            {
                jiesuan.active = false;


                wCount.text = "0";
                bCount.text = "0";
                map.Reset();
            };
        }
        private void OnGUI()
        {
            float val = (float)EditorApplication.timeSinceStartup % 1f;
            guiCanvas.Find<Horizontal>("top").color = new Color(val, val, val, val);
            guiCanvas.canvasRect = new Rect(Vector2.zero, position.size);
            guiCanvas.OnGUI();
            Repaint();
        }
       
    }
}
