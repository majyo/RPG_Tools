using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UIFramework
{
    /// <summary>
    /// UI资源加载器
    /// </summary>
    public class UIResourceLoader : MonoBehaviour
    {
        private static UIResourceLoader instance;
        public static UIResourceLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("UIResourceLoader");
                    instance = go.AddComponent<UIResourceLoader>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        /// <summary>
        /// 同步加载UI预制体
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns>加载的GameObject</returns>
        public GameObject LoadUIPrefab(string path)
        {
            try
            {
                GameObject prefab = Resources.Load<GameObject>(path);
                if (prefab == null)
                {
                    Debug.LogError($"Failed to load UI prefab at path: {path}");
                    return null;
                }
                
                GameObject instance = Instantiate(prefab);
                return instance;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading UI prefab at path {path}: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 异步加载UI预制体
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="onComplete">加载完成回调</param>
        public void LoadUIPrefabAsync(string path, Action<GameObject> onComplete)
        {
            StartCoroutine(LoadUIPrefabCoroutine(path, onComplete));
        }

        /// <summary>
        /// 异步加载协程
        /// </summary>
        private IEnumerator LoadUIPrefabCoroutine(string path, Action<GameObject> onComplete)
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>(path);
            yield return request;

            GameObject prefab = request.asset as GameObject;
            if (prefab == null)
            {
                Debug.LogError($"Failed to load UI prefab at path: {path}");
                onComplete?.Invoke(null);
                yield break;
            }

            GameObject instance = Instantiate(prefab);
            onComplete?.Invoke(instance);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset">要卸载的资源</param>
        public void UnloadAsset(Object asset)
        {
            if (asset != null)
            {
                Resources.UnloadAsset(asset);
            }
        }

        /// <summary>
        /// 卸载未使用的资源
        /// </summary>
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}
