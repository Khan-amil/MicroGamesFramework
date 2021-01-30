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

public abstract class MicroGameHandler : ScriptableObject
{

    public enum GameStatus
    {
        Init=0,
        Idle=1,
        Running=2,
        Over=3
    }

    public enum TimerRule
    {
        AutoWin,
        AutoFail,
        CustomCheck
    }

    public string _name;
    public string _description;

    public string _sceneName;

    public SceneAssetReference _scene;
    private SceneInstance _sceneInstance;
    public event Action OnGameStarted;
    public event Action<bool> OnGameEnded;

    public TimerRule _onTimerEnd;
    protected bool _gameLostIfTimeOver;
    private float _timer;

    public float Timer
    {
        get => _timer;
        private set => _timer = value;
    }

    public GameStatus Status
    {
        get => _status;
        private set => _status = value;
    }

    private GameStatus _status = GameStatus.Init;
    private AsyncOperationHandle<SceneInstance> _loadScene;

    public async void PrewarmGame()
    {
        Debug.Log("Prewarm start");
        // _sceneInstance = await PrewarmGame2();
        _sceneInstance = await _scene.PreLoadSceneAsync().Task;
        await Task.Delay(500);
        Debug.Log("Prewarm over");
        _status = GameStatus.Idle;
    }

    // public IEnumerator PrewarmGame()
    // {
    //     _loadScene = _scene.PreLoadSceneAsync();
    //     while (! _loadScene.IsDone)
    //     {
    //         yield return null;
    //     }
    //
    //     _sceneInstance = _loadScene.Result;
    //      _status = GameStatus.Idle;
    // }

    public async Task<SceneInstance> PrewarmGame2()
    {
        var result= await _scene.PreLoadSceneAsync().Task;
        return result;
    }

    public abstract IEnumerator PreloadGame();

    public void InitGame(int difficultyLevel)
    {
        _sceneInstance.Activate();
        SpecificInitGame(difficultyLevel);
        // var time = GetGameTimer(difficultyLevel);

        Status = GameStatus.Running;
        // StartCoroutine(TimerCountDown(time));
        if (OnGameStarted != null)
        {
            OnGameStarted();
        }
    }

    public IEnumerator TimerCountDown(float initialTime)
    {
        Timer = initialTime;
        while (Timer > 0 && Status == GameStatus.Running)
        {
            yield return null;
            Timer -= Time.deltaTime;
        }

        if (Timer <= 0)
        {
            //time out
            switch (_onTimerEnd)
            {
                case TimerRule.AutoWin:
                    EndGame(true);
                    break;
                case TimerRule.AutoFail:
                    EndGame(false);
                    break;
                case TimerRule.CustomCheck:
                    EndGame(CheckVictoryCondition());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    protected virtual bool CheckVictoryCondition()
    {
        return false;
    }

    public abstract float GetGameTimer(int difficultyLevel);

    protected abstract void SpecificInitGame(int difficultyLevel);

    public void EndGame(bool gameWon)
    {
        Addressables.UnloadSceneAsync(_sceneInstance).Completed+=OnSceneUnloaded;
        SpecificEndGame(gameWon);
        if (OnGameEnded != null) OnGameEnded(gameWon);
    }

    private void OnSceneUnloaded(AsyncOperationHandle<SceneInstance> obj)
    {

        Resources.UnloadUnusedAssets();
        Status = GameStatus.Over;
    }

    protected abstract void SpecificEndGame(bool gameWon);
}
