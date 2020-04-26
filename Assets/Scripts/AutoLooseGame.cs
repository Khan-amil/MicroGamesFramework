using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AutoLooseGame", menuName = "MGF/AutoLoose")]
public class AutoLooseGame : MicroGameHandler
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
        Debug.Log("Init autoloose");
    }

    protected override void SpecificEndGame(bool gameWon)
    {

        Debug.Log("End autoloose");
    }
}
