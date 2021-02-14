using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class GameFramework : MonoBehaviour
{
    private static GameFramework _instance;

    public static GameFramework Instance => _instance;

    public List<MicroGameHandler> _debugGames;

    private List<MicroGameHandler> _currentGames;

    private MicroGameHandler _currentGame;
    private int _currentGameIndex;
    public MicroGameIntro _intro;
    public GameFeedbackUi _ui;
    public PlayableDirector _director;
    private int _nbGames;
    private bool _allgamesReady;
    private BaseControls _input;

    private void Awake()
    {
        if (_instance != null)
        {
            DestroyImmediate(this);
        }

        _input = new BaseControls();
        _instance = this;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        _input.Enable();
        _currentGames = _debugGames;
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
        if (!_allgamesReady)
        {
            return;
        }

        if (_currentGame == null)
        {
            if (_input.Gameplay.MainAction.triggered)
            {
                _currentGame = _currentGames[0];
                StartGame();
            }
        }
    }

    void StartGame()
    {
        _currentGame.OnGameEnded += NextGame;
        _ui._currentGame = _currentGame;
        StartCoroutine(StartupGame());
    }

    private async void PrewarmNextGame()
    {
        if (_currentGameIndex < _currentGames.Count)
        {
            _currentGames[_currentGameIndex+1].PrewarmGameAsync();
        }
    }

    IEnumerator StartupGame()
    {
        //
        // yield return StartCoroutine(_currentGame.PrewarmGame());
        // yield return StartCoroutine(_currentGame.PreloadGame());
        _intro.ApplyTitle(_currentGame);
        _director.time = 0;
        _director.Play();
        yield return 0;
    }

    public void OnIntroFinished()
    {
        StartCoroutine(DoStartGame());
        // _currentGame.InitGame(0);
        // StartCoroutine((_currentGame.TimerCountDown(_currentGame.GetGameTimer(0))));
    }

    IEnumerator DoStartGame()
    {
        yield return StartCoroutine(_currentGame.PrewarmGame());
        yield return StartCoroutine(_currentGame.PreloadGame());
        _currentGame.InitGame(0);
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

        if (_currentGameIndex < _currentGames.Count)
        {
            _currentGame = _currentGames[_currentGameIndex++];
            yield return StartCoroutine(_currentGame.PrewarmGame());
            // _currentGame.PrewarmGameAsync();
            while (_currentGame.Status!=MicroGameHandler.GameStatus.Idle)
            {
                yield return null;
            }
            StartGame();
        }
    }

    public static void FailGame()
    {
        Debug.Log("Fail");
        Instance._currentGame.EndGame(false);
    }

    public static void WinGame()
    {
        Debug.Log("Win");
        Instance._currentGame.EndGame(true);
    }


}