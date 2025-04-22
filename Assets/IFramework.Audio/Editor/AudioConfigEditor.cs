using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using static IFramework.AudioEx.AudioConfig;
namespace IFramework.AudioEx
{
    [UnityEditor.CustomEditor(typeof(AudioConfig))]
    class AudioConfigEditor : Editor
    {
        AudioConfig cfg;
        private ReorderableList list;
        private Tree tree;
        private void OnEnable()
        {
            cfg = (AudioConfig)target;
            list = new ReorderableList(cfg.channels, typeof(string));
            list.drawHeaderCallback = (rect) =>
            {
                GUI.Label(rect, nameof(AudioConfig.channels));
            };
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var src = cfg.channels[index];
                var tmp = EditorGUI.TextField(rect, src);
                if (tmp != src)
                {
                    cfg.channels[index] = tmp;
                }
            };
            list.onAddCallback = (_list) => { cfg.channels.Add(cfg.channels.LastOrDefault()); };
            tree = new Tree(new TreeViewState(), cfg);
            field = new SearchField();
        }
        private class Tree : TreeView
        {
            private AudioConfig cfg;
            public Tree(TreeViewState state, AudioConfig cfg) : base(state)
            {
                this.cfg = cfg;
                this.showAlternatingRowBackgrounds = true;
                this.multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(new MultiColumnHeaderState.Column[] {

                       new MultiColumnHeaderState.Column()
                    {
                      width=50,
                                 maxWidth=50,minWidth=50,

                    },
                    new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.id)),
                           width=80,
                                 maxWidth=80,minWidth=80,
                    },
                    new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.channel)),
                                           width=80,
                                 maxWidth=80,minWidth=80,
                    },
                        new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.clip)),
                                            width=120,
                                 maxWidth=120,minWidth=120,
                    },
                     new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.loop)),
                       width=35,
                       maxWidth=35,minWidth=35,

                    },
                         new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.cover)),
                           width=40,
                       maxWidth=40,minWidth=40,
                    },
                    new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.existTime)),
                       width=80,
                                 maxWidth=80,minWidth=80,
                    },

                    new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.volume)),
                                   width=150,
                                 maxWidth=150,minWidth=150,
                    },
                }));
                this.multiColumnHeader.ResizeToFit();
                Reload();
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                var data = cfg._sounds[args.item.id];
                data.id = EditorGUI.IntField(args.GetCellRect(1), data.id);
                data.channel = EditorGUI.Popup(args.GetCellRect(2), Mathf.Clamp(data.channel, 0, cfg.channels.Count), cfg.channels.ToArray());
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(data.clip);
                var tmp = EditorGUI.ObjectField(args.GetCellRect(3), clip, typeof(AudioClip), false);
                if (tmp != clip)
                {
                    data.clip = AssetDatabase.GetAssetPath(tmp);
                }
                data.loop = EditorGUI.Toggle(args.GetCellRect(4), data.loop);
                data.cover = EditorGUI.Toggle(args.GetCellRect(5), data.cover);
                data.existTime = EditorGUI.IntField(args.GetCellRect(6), data.existTime);

                data.volume = EditorGUI.Slider(args.GetCellRect(7), data.volume, -1, 1);
           
            }
            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                IList<TreeViewItem> list = GetRows() ?? new List<TreeViewItem>();
                list.Clear();
                for (int i = 0; i < cfg._sounds.Count; i++)
                {
                    var sound = cfg._sounds[i];
                    if (string.IsNullOrEmpty(this.searchString) || sound.id.ToString().Contains(this.searchString))
                    {
                        TreeViewItem item = new TreeViewItem()
                        {
                            id = i,
                            depth = 0,
                            displayName = i.ToString(),
                            parent = root,
                        };
                        list.Add(item);
                    }
                }
                return list;
            }
            protected override TreeViewItem BuildRoot() => new TreeViewItem() { depth = -1 };

        }
        private SearchField field;

        public override void OnInspectorGUI()
        {
            this.serializedObject.UpdateIfRequiredOrScript();
            list.DoLayoutList();
            GUILayout.BeginHorizontal();

            string tmp = field.OnToolbarGUI(tree.searchString);
            if (tree.searchString != tmp)
            {
                tree.searchString = tmp;
                tree.Reload();
            }
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                var selection = tree.GetSelection().ToList();
                selection.Sort();
                for (int i = selection.Count - 1; i >= 0; i--)
                {
                    cfg._sounds.RemoveAt(selection[i]);
                }
                tree.Reload();
                tree.SetSelection(new List<int>());
            }
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                cfg._sounds.Add(new SoundData()
                {
                    id = cfg._sounds.Count,
                });
                tree.Reload();
                tree.SetSelection(new List<int>() {
                cfg._sounds.Count-1
                });
            }
            if (GUILayout.Button("Save", GUILayout.Width(50)))
            {
                EditorUtility.SetDirty(this.cfg);
                AssetDatabase.SaveAssets();
            }
            GUILayout.EndHorizontal();
            var rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinHeight(400));
            tree.OnGUI(rect);
            this.serializedObject.ApplyModifiedProperties();

        }



    }

}
