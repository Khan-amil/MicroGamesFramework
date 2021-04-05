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
        Finished=3,
        Ending=4,
        Over=5
    }

    public enum TimerRule
    {
        Win,
        Loose,
        CustomCheck
    }

    public string _name;
    public string _description;

    public string _sceneName;

    public SceneAssetReference _scene;
    private SceneInstance _sceneInstance;
    public event Action OnGameStarted;
    public event Action<bool> OnGameEnded;

    protected TimerRule _onTimerEnd;
    protected bool _gameLostIfTimeOver;
    private float _timer;
    private float _startTimer;

    public float Timer
    {
        get => _timer;
        private set => _timer = value;
    }

    public float TimerRatio
    {
        get
        {
            if (_startTimer == 0)
                return 0;
            return _timer / _startTimer;
        }
    }

    public GameStatus Status
    {
        get => _status;
        private set => _status = value;
    }

    private GameStatus _status = GameStatus.Init;
    private AsyncOperationHandle<SceneInstance> _loadScene;

    public IEnumerator PrewarmGame()
    {
        _status = GameStatus.Init;
        SceneLoader.Instance.LoadScene(_scene,()=>
        {
            _status = GameStatus.Idle;
            Debug.Log("Game loaded over, "+_status);
        });
        while ( _status != GameStatus.Idle)
        {
            yield return null;
        }
        Debug.Log("Prewarm over, "+_status);
    }

    public async Task<SceneInstance> PrewarmGame2()
    {
        var result= await _scene.PreLoadSceneAsync().Task;
        return result;
    }

    public abstract IEnumerator PreloadGame();

    public void InitGame(int difficultyLevel)
    {
        // _sceneInstance.Activate();
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
        _startTimer = initialTime;
        Debug.Log("Start timer of "+Timer + " ; "+ Time.time);
        while (Timer > 0)
        {
            yield return null;
            Timer -= Time.deltaTime;
        }


        Debug.Log("Time out");

        bool victory;
        //time out
        switch (_onTimerEnd)
        {
            case TimerRule.Win:
                victory = true;
                break;
            case TimerRule.Loose:
                victory = false;
                break;
            case TimerRule.CustomCheck:
                victory = CheckVictoryCondition();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        // Send endgame event
        GameFramework.Feedback(victory);

        EndGame(victory);

    }

    public void WinGame()
    {
        _onTimerEnd = TimerRule.Win;
        Status = GameStatus.Finished;
    }

    public void FailGame()
    {
        _onTimerEnd = TimerRule.Loose;
        Status = GameStatus.Finished;
    }

    protected virtual bool CheckVictoryCondition()
    {
        return false;
    }

    public abstract float GetGameTimer(int difficultyLevel);

    protected abstract void SpecificInitGame(int difficultyLevel);

    public void EndGame(bool gameWon)
    {
        if (Status == GameStatus.Running || Status== GameStatus.Finished)
        {
            Status = GameStatus.Ending;
            SceneLoader.Instance.UnloadScene(()=>OnSceneUnloaded(gameWon));
        }
        else
        {
            Debug.LogError("Can't fail current game, current status mismatch : "+Status);
        }
    }

    private void OnSceneUnloaded(bool gameWon)
    {
        Debug.Log("Scene successfully unloaded");
        SpecificEndGame(gameWon);
        if (OnGameEnded != null)
            OnGameEnded(gameWon);
        Resources.UnloadUnusedAssets();
        Status = GameStatus.Over;
    }

    protected abstract void SpecificEndGame(bool gameWon);

    public void ForceEndGame()
    {
        SceneLoader.Instance.UnloadScene(() =>
        {
            SpecificEndGame(false);
            Resources.UnloadUnusedAssets();
            Status = GameStatus.Over;
        });
    }
}
