using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AutoWinGame", menuName = "MGF/AutoWin")]
public class AutoWinGame : MicroGameHandler
{

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
        Debug.Log("Init autowin");
    }

    protected override void SpecificEndGame(bool gameWon)
    {

        Debug.Log("End autowin");
    }
}
