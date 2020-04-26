using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "PressZGame", menuName = "MGF/PressZ")]
public class PressZGame : MicroGameHandler
{
    private bool _win;

    private void Update()
    {
        if (Status == GameStatus.Running)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _win = true;
            }

        }
    }

    protected override bool CheckVictoryCondition()
    {
        return _win;
    }

    public override IEnumerator PreloadGame()
    {
        yield return new WaitForSeconds(2);
    }

    public override float GetGameTimer(int difficultyLevel)
    {
        return 3;
    }

    protected override void SpecificInitGame(int difficultyLevel)
    {
        Debug.Log("Init z");
    }

    protected override void SpecificEndGame(bool gameWon)
    {

        Debug.Log("End z");
    }
}
