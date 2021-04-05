using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private enum Status
    {
        Idle,
        Loading,
        Loaded,
        Unloading,
    }

    private Status _status;
    private AsyncOperationHandle<SceneInstance> _loadScene;

    private SceneInstance _sceneInstance;

    #region Unity standard singleton

    /// <summary>
    /// Singleton Instance for HexaManager.
    /// </summary>
    private static SceneLoader _instance;

    /// <summary>
    /// Singleton Instance for HexaManager.
    /// </summary>
    public static SceneLoader Instance
    {
        get
        {
            // If we don't have an instance yet, we will search it in the scene
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<SceneLoader>();

            return _instance;
        }
    }

    #endregion

    public void LoadScene(SceneAssetReference scene, Action callback)
    {
        if(_status==Status.Idle)
        {
            _status = Status.Loading;
            _loadScene = scene.PreLoadSceneAsync();
            _loadScene.Completed += s => { OnSceneLoaded(s, callback); };
        }
        else
        {
            Debug.LogError("Can't load, already busy");

        }
    }

    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> obj, Action callback)
    {
        _status = Status.Loaded;
        _sceneInstance = obj.Result;
        SceneManager.SetActiveScene(_sceneInstance.Scene);
        Debug.Log("Scene loaded");
        callback?.Invoke();

    }

    public void UnloadScene(Action callback)
    {
        if (_status == Status.Loaded)
        {
            _status = Status.Unloading;
            Addressables.UnloadSceneAsync(_loadScene).Completed+=o=>
            {
                OnSceneUnloaded(o,callback);
            };
        }
        else
        {
            Debug.LogError("Can't unload, no scene loaded or already unloading");

        }
    }

    private void OnSceneUnloaded(AsyncOperationHandle<SceneInstance> asyncOperationHandle, Action callback)
    {
        _status = Status.Idle;
        callback?.Invoke();
    }
}
