using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class MicroGameHandler : ScriptableObject
{

    public enum GameStatus
    {
        Init,
        Running,
        Over
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
    private AsyncOperation _loadScene;

    public IEnumerator PrewarmGame()
    {
        _loadScene = SceneManager.LoadSceneAsync(_sceneName,LoadSceneMode.Additive);
        _loadScene.allowSceneActivation = false;
        while (!_loadScene.isDone)
        {
            yield return null;
        }
    }

    public abstract IEnumerator PreloadGame();

    public void InitGame(int difficultyLevel)
    {
        _loadScene.allowSceneActivation = true;
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
        SceneManager.UnloadSceneAsync(_sceneName);
        SpecificEndGame(gameWon);
        if (OnGameEnded != null) OnGameEnded(gameWon);
    }

    protected abstract void SpecificEndGame(bool gameWon);
}
