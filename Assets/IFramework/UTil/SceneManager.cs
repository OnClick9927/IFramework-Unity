/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-31
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace IFramework
{
    public class SceneManager:ISingleton
	{
        public enum SceneEventType
        {
            StartLoadAsync,
            LoadProgress,
            LoadComplete,
            StartUnLoadAsync,
            UnLoadProgress,
            UnLoadComplete,
        }
        public class SceneEventArg : IEventArgs
        {
            public LoadSceneMode LoadMode { get; set; }
            public SceneEventType EveType { get; set; }
            public float Progress { get; set; }
            public Scene LoadedScene { get; set; }
            public Scene UnLoadedScene { get; set; }
            public AsyncOperation AsyncOp { get; set; }
            public SceneEventArg() { }
        }
        private static SceneManager Instance { get { return SingletonProperty<SceneManager>.Instance; } }
        private List<Scene> scenes;
        public static string EmptySceneName;
        public static int TotalSceneCount { get { return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; } }
        public static int loadedSceneCount { get { return UnityEngine.SceneManagement.SceneManager.sceneCount; } }
        public static Scene CurrentScene { get { return UnityEngine.SceneManagement.SceneManager.GetActiveScene(); } }
        public static Action<Scene, Scene> OnActiveSceneChange;
        public static Action<Scene, LoadSceneMode> OnLoadScene;
        public static Action<Scene> OnUnLoadScene;

        private SceneManager() { }

        private void ActiveSceneChange(Scene oldScene, Scene newSene)
        {
            if (OnActiveSceneChange != null) OnActiveSceneChange(oldScene, newSene);
        }
        private void SceneUnLoaded(Scene scene)
        {
            scenes.Remove(scene);
            if (OnUnLoadScene != null) OnUnLoadScene(scene);
        }
        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            scenes.Add(scene);
            if (OnLoadScene != null) OnLoadScene(scene, mode);
        }

        void ISingleton.OnSingletonInit()
        {
            scenes = new List<Scene>();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneUnLoaded;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChange;
        }
        public void Dispose()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= SceneUnLoaded;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ActiveSceneChange;
            scenes.Clear();
            scenes = null;
        }


        public static void LoadEmptyScene()
        {
            LoadSync(EmptySceneName, LoadSceneMode.Additive);
        }
        public static void LoadSync(string sceneName, LoadSceneMode mode)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, mode);
        }
        public static void LoadAsyncWithEmpty(string sceneName, LoadSceneMode mode, Action<SceneEventArg> callBack)
        {
            LoadEmptyScene();
            LoadAsync(sceneName, mode, callBack);
        }

        public static void LoadAsync(string sceneName, LoadSceneMode mode ,Action<SceneEventArg> callBack)
        {
            SceneEventArg arg = new SceneEventArg();
            AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
            asyncOp.allowSceneActivation = false;
            arg.LoadMode = mode;
            arg.LoadedScene = GetSceneByName(sceneName);
            arg.EveType = SceneEventType.StartLoadAsync;
            if (callBack != null) callBack(arg);
            arg.EveType = SceneEventType.LoadProgress;
            while (!asyncOp.isDone)
            {
                arg.Progress = asyncOp.progress;
                if (callBack != null) callBack(arg);
            }
            arg.Progress = 1;
            arg.EveType = SceneEventType.LoadComplete;
            if (callBack != null) callBack(arg);
            asyncOp.allowSceneActivation = true;
        }

        public static void UnLoad(Scene scene, Action<SceneEventArg> callBack)
        {
            SceneEventArg arg = new SceneEventArg();

            AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            arg.EveType = SceneEventType.StartUnLoadAsync;
            arg.AsyncOp = asyncOp;
            if (callBack != null) callBack(arg);
            arg.EveType = SceneEventType.UnLoadProgress;
            while (!asyncOp.isDone)
            {
                arg.Progress = asyncOp.progress;
                if (callBack != null) callBack(arg);
            }
            arg.Progress = 1;
            if (callBack != null) callBack(arg);
            arg.EveType = SceneEventType.UnLoadComplete;
        }
        public static void UnLoad(string sceneName, Action<SceneEventArg> callBack)
        {
            UnLoad(GetSceneByName(sceneName), callBack);
        }
        public static Scene[] GetLoadedScenes()
        {
            return Instance.scenes.ToArray();
        }
        public static bool HaveLoad(Scene scene)
        {
            return Instance.scenes.Contains(scene);
        }
        public static bool HaveLoad(string sceneName)
        {
            return HaveLoad(GetSceneByName(sceneName));
        }
        public static bool SetActive(Scene scene)
        {
           return  UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
        }
        public static bool SetActive(string sceneName)
        {
            return UnityEngine.SceneManagement.SceneManager.SetActiveScene(GetSceneByName(sceneName));
        }

        public static Scene GetSceneByName(string name)
        {
            return UnityEngine.SceneManagement.SceneManager.GetSceneByName(name);
        }
        public static Scene GetSceneByPath(string path)
        {
            return UnityEngine.SceneManagement.SceneManager.GetSceneByPath(path);
        }
        public static Scene GetSceneByIndex(int index)
        {
            return UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(index);
        }

        public static int GetBuildIndexByScenePath(string path)
        {
            return SceneUtility.GetBuildIndexByScenePath(path);
        }
        public static string GetScenePathByBuildIndex(int index)
        {
            return SceneUtility.GetScenePathByBuildIndex(index);
        }
    }
}
