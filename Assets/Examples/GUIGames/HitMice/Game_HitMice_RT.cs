/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.14f1
 *Date:           2019-11-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using IFramework.GUITool.LayoutDesign;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using IFramework;
namespace IFramework_Demo.GUIGames
{
    [RequireComponent(typeof(GUICanvasComponet))]
    public class Game_HitMice_RT:MonoBehaviour
	{
        private class BtnMice
        {
            public enum MouseState
            {
                Hide,
                Show,
                Hited
            }
            private Texture2D Show;
            private MouseState mouseState = MouseState.Hide;
            private Button btn;
            public BtnMice(int index, Button btn, Texture2D hited, Texture2D Show, Action hitCallBack)
            {
                this.btn = btn;
                this.Show = Show;
                TrickTrun = index % 2 == 0;
                mouseState = MouseState.Hide;
                btn.color = Color.clear;
                btn.image = null;
                this.btn.OnClick = () =>
                {
                    if (mouseState == MouseState.Show)
                    {
                        mouseState = MouseState.Hited;
                        btn.color = Color.white;
                        btn.image = hited;
                        if (hitCallBack != null)
                        {
                            hitCallBack();
                        }
                    }
                };
            }
            bool TrickTrun;
            public void Trick(float seed)
            {
                TrickTrun = !TrickTrun;
                if (TrickTrun)
                {
                    float rand = UnityEngine.Random.Range(0, 2f);
                    if (rand >= seed)
                    {
                        mouseState = MouseState.Show;
                        btn.image = Show;
                        btn.color = Color.white;
                        ShowMusic();
                    }
                    else
                    {
                        mouseState = MouseState.Hide;
                        btn.color = Color.clear;
                        btn.image = null;
                    }
                }
                else
                {
                    mouseState = MouseState.Hide;
                    btn.color = Color.clear;
                    btn.image = null;
                }

            }
        }
        List<BtnMice> btns = new List<BtnMice>();
        GUICanvasComponet comp { get { return GetComponent<GUICanvasComponet>(); } }
        GUICanvas guiCanvas { get { return comp.guiCanvas; } }
        private void Start()
        {
            btns.Clear();
            TextAsset txt = Resources.Load<TextAsset>("Game_HitMice");
            comp.LoadCanvas(txt);
            guiCanvas.canvasRect = new Rect(Vector2.one * 0, new Vector2(Screen.width, Screen.height));

            Texture2D hited = guiCanvas.Find<ImageLabel>("hited").image;
            Texture2D Show = guiCanvas.Find<ImageLabel>("chutou").image;
            score = 0;
            second = 0;
            guiCanvas.Find<Label>("BG/top/score").text = score.ToString();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Button btn = guiCanvas.Find<Button>(string.Format("BG/{0}/{1}", i + 1, j + 1));
                    btns.Add(new BtnMice(btns.Count, btn, hited, Show, HitCallBack));
                }
            }
            ImageToggle togEasy = guiCanvas.Find<ImageToggle>("lev/lev/easy/check");
            ImageToggle togMid = guiCanvas.Find<ImageToggle>("lev/lev/mid/check");
            ImageToggle togHard = guiCanvas.Find<ImageToggle>("lev/lev/hard/check");
            ImageToggle togVeryHard = guiCanvas.Find<ImageToggle>("lev/lev/veryHard/check");

            togEasy.value = true;
            lev = 1;
            togEasy.onValueChange = (bo) =>
            {
                if (bo)
                {
                    togVeryHard.value = togMid.value = togHard.value = false;
                    lev = 1;
                }
                else
                {
                    if (!togMid.value && !togHard.value && !togVeryHard.value)
                    {
                        togEasy.value = true;
                    }
                }
            };
            togMid.onValueChange = (bo) =>
            {
                if (bo)
                {
                    togVeryHard.value = togEasy.value = togHard.value = false;
                    lev = 2;
                }
                else
                {
                    if (!togEasy.value && !togHard.value && !togVeryHard.value)
                    {
                        togMid.value = true;
                    }
                }
            };
            togHard.onValueChange = (bo) =>
            {
                if (bo)
                {
                    togVeryHard.value = togMid.value = togEasy.value = false;
                    lev = 3;
                }
                else
                {
                    if (!togMid.value && !togEasy.value && !togVeryHard.value)
                    {
                        togHard.value = true;
                    }
                }
            };
            togVeryHard.onValueChange = (bo) =>
            {
                if (bo)
                {
                    togHard.value = togMid.value = togEasy.value = false;
                    lev = 6;
                }
                else
                {
                    if (!togMid.value && !togEasy.value && !togHard.value)
                    {
                        togHard.value = true;
                    }
                }
            };

            GameObject go = new GameObject("niganma");
            go.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            niganma = go.AddComponent<AudioSource>();
            niganma.clip = Resources.Load<AudioClip>("niganma");

            go = new GameObject("jinitaimei");
            go.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            jinitaimei = go.AddComponent<AudioSource>();
            jinitaimei.clip = Resources.Load<AudioClip>("jinitaimei");
        }
        public int lev = 1;
        private int score = 0;
        AudioSource niganma;
        static AudioSource jinitaimei;

        private static void ShowMusic()
        {
            jinitaimei.Stop();
            jinitaimei.Play();
        }
        private void HitCallBack()
        {
            guiCanvas.Find<Label>("BG/top/score").text = (++score).ToString();
            niganma.Stop();
            niganma.Play();
        }
        private void OnDisable()
        {
            DestroyImmediate(niganma.gameObject);
            DestroyImmediate(jinitaimei.gameObject);

        }
        int second;
        private void Update()
        {
            Trick();
            float val = (float)Time.realtimeSinceStartup % 1f;
            guiCanvas.Find<Horizontal>("BG/top").color = new Color(val, val, val, val);
        }
        private void Trick()
        {
            second++;
            second = second % (240 / lev);

            if (second == 0)
            {
                float rand = UnityEngine.Random.Range(0, 7f);
                btns.ForEach((i, b) => {
                    if (i > rand)
                    {
                        b.Trick(0.85f);
                    }
                    else
                    {
                        b.Trick(1.25f);
                    }
                });
            }

        }

    }
}
