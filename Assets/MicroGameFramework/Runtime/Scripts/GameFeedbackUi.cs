using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFeedbackUi : MonoBehaviour
{

    public float _feedbackDuration = 1;

    public GameObject _wonFeedback;
    public GameObject _lostFeedback;

    public GameObject _timerRoot;
    public TMP_Text _timer;
    public Slider _timerBackground;

    public GameObject _livesRoot;
    
    public List<GameObject> _lives;


    public MicroGameHandler _currentGame;
    // Start is called before the first frame update
    void Start()
    {
        _livesRoot.SetActive(false);
        _wonFeedback.SetActive(false);
        _lostFeedback.SetActive(false);
    }

    private void Update()
    {
        if (_currentGame != null && (_currentGame.Status == MicroGameHandler.GameStatus.Running || _currentGame.Status == MicroGameHandler.GameStatus.Finished) )
        {
            _timerRoot.SetActive(true);
            _timer.text = _currentGame.Timer.ToString("0");
            _timerBackground.value = _currentGame.TimerRatio;
        }
        else
        {
            _timerRoot.SetActive(false);
        }
    }


    public IEnumerator EndGame(bool gameWon)
    {

        _livesRoot.SetActive(true);
        if (gameWon)
        {
            _wonFeedback.SetActive(true);
            _lostFeedback.SetActive(false);
        }
        else
        {
            _wonFeedback.SetActive(false);
            _lostFeedback.SetActive(true);
            _lostFeedback.GetComponent<Animator>().SetTrigger("Lost");
            _lives.Last(x => x.activeInHierarchy).SetActive(false);
        }
        yield return new WaitForSeconds(_feedbackDuration);

        _wonFeedback.SetActive(false);
        _lostFeedback.SetActive(false);
        _livesRoot.SetActive(false);
    }

    public void Begin()
    {
        _livesRoot.SetActive(false);
        _lives.ForEach(x => x.SetActive(true));
    }
}
