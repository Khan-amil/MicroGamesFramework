using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor.Timeline;
using UnityEngine;
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

    private void Awake()
    {
        if (_instance != null)
        {
            DestroyImmediate(this);
        }

        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentGames = _debugGames;
        foreach (var game in _currentGames)
        {
            StartCoroutine(game.PrewarmGame());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _currentGame = _currentGames[_currentGameIndex++];
            StartGame();
        }

    }

    void StartGame()
    {
        _currentGame.OnGameEnded += NextGame;
        _ui._currentGame = _currentGame;
        StartCoroutine(StartupGame());
    }

    IEnumerator StartupGame()
    {
        yield return StartCoroutine(_currentGame.PreloadGame());
        _intro.ApplyTitle(_currentGame);
        _director.time = 0;
        _director.Play();
    }

    public void OnIntroFinished()
    {
        _currentGame.InitGame(0);
        StartCoroutine((_currentGame.TimerCountDown(_currentGame.GetGameTimer(0))));
    }

    private void NextGame(bool obj)
    {
        StartCoroutine(FeedbackGameStatus(obj));
    }

    private IEnumerator FeedbackGameStatus(bool gameWon)
    {
        yield return StartCoroutine(_ui.EndGame(gameWon));

        if (_currentGameIndex < _currentGames.Count)
        {
            _currentGame = _currentGames[_currentGameIndex++];
            StartGame();
        }
    }
}
