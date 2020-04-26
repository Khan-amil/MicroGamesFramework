using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFeedbackUi : MonoBehaviour
{

    public float _feedbackDuration = 1;

    public GameObject _wonFeedback;
    public GameObject _lostFeedback;

    public Text _timer;

    public MicroGameHandler _currentGame;
    // Start is called before the first frame update
    void Start()
    {
        _wonFeedback.SetActive(false);
        _lostFeedback.SetActive(false);
    }

    private void Update()
    {
        if (_currentGame != null & _currentGame.Status == MicroGameHandler.GameStatus.Running)
        {
            _timer.gameObject.SetActive(true);
            _timer.text = _currentGame.Timer.ToString("0");
        }
        else
        {
            _timer.gameObject.SetActive(false);
        }
    }


    public IEnumerator EndGame(bool gameWon)
    {
        if (gameWon)
        {
            _wonFeedback.SetActive(true);
            _lostFeedback.SetActive(false);
        }
        else
        {
            _wonFeedback.SetActive(false);
            _lostFeedback.SetActive(true);
        }
        yield return new WaitForSeconds(_feedbackDuration);

        _wonFeedback.SetActive(false);
        _lostFeedback.SetActive(false);
    }
}
