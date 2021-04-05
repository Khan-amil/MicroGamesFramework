using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public class GameFramework : MonoBehaviour
{

    #region Unity standard singleton

    /// <summary>
    /// Singleton Instance for HexaManager.
    /// </summary>
    private static GameFramework _instance;

    /// <summary>
    /// Singleton Instance for HexaManager.
    /// </summary>
    public static GameFramework Instance
    {
        get
        {
            // If we don't have an instance yet, we will search it in the scene
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameFramework>();

            return _instance;
        }
    }

    #endregion

    public List<MicroGameHandler> _debugGames;

    private List<MicroGameHandler> _currentGames;

    public int _nbLives=3;
    private MicroGameHandler _currentGame;
    private int _currentGameIndex;
    public MicroGameIntro _intro;
    public GameFeedbackUi _ui;
    public PlayableDirector _director;
    private int _nbGames;
    private bool _allgamesReady;
    private BaseControls _input;

    public BaseControls Input
    {
        get
        {
            if (_input == null)
            {
                _input = new BaseControls();
            }
            return _input;
        }
    }

    public GameObject _titleScreen;

    public static event Action OnGameFailed;

    public static event Action OnGameWon;
    public static event Action OnStartGame;

    public static bool CurrentGameRunning => Instance._currentGame.Status == MicroGameHandler.GameStatus.Running;

    private void Awake()
    {
        if (_input == null)
        {
            _input = new BaseControls();
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        _input.Enable();
        _currentGames = _debugGames;
        _currentGames.Shuffle();
        yield return null;
        yield return null;
        yield return null;
        // _currentGames[0].PrewarmGame();
        // while (_currentGames[0].Status== MicroGameHandler.GameStatus.Init)
        // {
        //     yield return null;
        // }

       // foreach (var game in _currentGames)
       // {
       //     game.PrewarmGame();
       //     while (game.Status== MicroGameHandler.GameStatus.Init)
       //     {
       //          yield return null;
       //     }
       // }

        _allgamesReady = true;
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();


            _director.time = 0;
            _director.Stop();

            if (_currentGame != null)
            {
                _currentGame.OnGameEnded -= NextGame;
                _currentGame.ForceEndGame();
                _currentGame = null;
            }
            _currentGameIndex = 0;
            _titleScreen.SetActive(true);
        }

        if (!_allgamesReady)
        {
            return;
        }

        if (_currentGame == null)
        {
            if (_input.Gameplay.MainAction.triggered)
            {
                Begin();
            }
        }
    }

    public void Begin()
    {
        _titleScreen.SetActive(false);
        _currentGameIndex = 0;
        _currentGame = _currentGames[_currentGameIndex];
        _ui.Begin();
        StartGame();
    }

    void StartGame()
    {
        _currentGame.OnGameEnded += NextGame;
        _ui._currentGame = _currentGame;
        StartCoroutine(StartupGame());
    }


    IEnumerator StartupGame()
    {
        //
        // yield return StartCoroutine(_currentGame.PrewarmGame());
        // yield return StartCoroutine(_currentGame.PreloadGame());
        OnStartGame?.Invoke();
        _intro.ApplyTitle(_currentGame);
        _director.time = 0;
        _director.Play();
        yield return 0;
    }

    public void OnIntroFinished()
    {
        if (_currentGame != null)
        {
            StartCoroutine(DoStartGame());
        }
        // _currentGame.InitGame(0);
        // StartCoroutine((_currentGame.TimerCountDown(_currentGame.GetGameTimer(0))));
    }

    IEnumerator DoStartGame()
    {
        yield return StartCoroutine(_currentGame.PrewarmGame());
        Debug.Log("Prewarm over, "+_currentGame.Status);
        yield return StartCoroutine(_currentGame.PreloadGame());
        Debug.Log("Preload over, "+_currentGame.Status);
        _currentGame.InitGame(0);
        Debug.Log("Game started "+_currentGame.Status + " ; "+ Time.time);
        StartCoroutine((_currentGame.TimerCountDown(_currentGame.GetGameTimer(0))));
    }

    private void NextGame(bool obj)
    {
        // PrewarmNextGame();
        StartCoroutine(FeedbackGameStatus(obj));
    }

    private IEnumerator FeedbackGameStatus(bool gameWon)
    {
        yield return StartCoroutine(_ui.EndGame(gameWon));


        while (_currentGame.Status!=MicroGameHandler.GameStatus.Over)
        {
            yield return null;
        }

        if (!gameWon)
        {
            _nbLives--;
        }



        if (_nbLives>0 &&_currentGameIndex < _currentGames.Count-1)
        {
            _currentGame = _currentGames[++_currentGameIndex];
            Debug.Log("Start new game");
            // yield return StartCoroutine(_currentGame.PrewarmGame());
            // _currentGame.PrewarmGameAsync();
            // while (_currentGame.Status!=MicroGameHandler.GameStatus.Idle)
            // {
            //     yield return null;
            // }
            StartGame();
        }
        else
        {
            _currentGameIndex = 0;
            _titleScreen.SetActive(true);
        }
    }

    public static void FailGame()
    {
        Debug.Log("Fail");
        Instance._currentGame.FailGame();
        OnGameFailed?.Invoke();
    }

    public static void WinGame()
    {
        Debug.Log("Win");
        Instance._currentGame.WinGame();
        OnGameWon?.Invoke();
    }

    public static float GetCurrentTimerRatio()
    {
        return Instance._currentGame.TimerRatio;
    }


    public static void Feedback(bool victory)
    {
        if (Instance._currentGame.Status == MicroGameHandler.GameStatus.Running)
        {
            if (victory)
            {
                OnGameWon?.Invoke();
            }
            else
            {
                OnGameFailed?.Invoke();
            }
        }
    }


}


public static class MyExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n+1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}