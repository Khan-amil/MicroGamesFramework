using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DefaultNamespace;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MicroGameCreator : ScriptableWizard
{

    public string _gameName;
    public string _gameDescription;

    public static string _saved;

    [MenuItem("MicroGames/Create New game")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<MicroGameCreator>("Create Game", "Create");
    }

    void OnWizardCreate()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects,NewSceneMode.Additive);
        EditorSceneManager.SaveScene(scene,GetScenePath(_gameName));

        _saved = _gameName;
        EditorPrefs.SetString("MGF_name",_gameName);
        EditorPrefs.SetString("MGF_desc",_gameDescription);
        // Create a file to write to.
        using (StreamWriter sw = File.CreateText("Assets/"+_gameName+".cs"))
        {
            sw.WriteLine(GetBasicClass());
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset( _gameName+".cs", ImportAssetOptions.ForceUpdate );
        Type gameType = Assembly.GetExecutingAssembly().GetType(_gameName);


        var game = CreateInstance(gameType) as MicroGameHandler;
        if (game)
        {
            game._name = _gameName;
            game._description = _gameDescription;
            AssetDatabase.CreateAsset(game,"Assets/"+_gameName+".asset");

        }
    }

    private static string GetScenePath(string name)
    {
        return "Assets/"+name+".unity";
    }

    void OnWizardUpdate()
    {
        helpString = "Please set the color of the light!";
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void CreateAssetWhenReady()
    {
        if (!string.IsNullOrEmpty(EditorPrefs.GetString("MGF_name")))
        {
            if(EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += CreateAssetWhenReady;
                return;
            }

            EditorApplication.delayCall += CreateAssetNow;
        }
    }

    private static void CreateAssetNow()
    {
        string gameName=EditorPrefs.GetString("MGF_name");

        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        Type gameType = null;
        foreach (var assembly in assemblies)
        {
            gameType = assembly.GetType(gameName);
            if(gameType!=null)
                break;
        }

        var game = CreateInstance(gameType) as MicroGameHandler;

        game._name = gameName;
        game._description = EditorPrefs.GetString("MGF_desc");
        game._sceneName = gameName;
        game._scene = new SceneAssetReference(AssetDatabase.AssetPathToGUID(GetScenePath(gameName)));
        AssetDatabase.CreateAsset(game,"Assets/"+gameName+".asset");

        EditorPrefs.SetString("MGF_name","");
        EditorPrefs.SetString("MGF_desc","");
    }

    string GetBasicClass()
    {
        string result = @"
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;
        ";
        result += $"public class {_gameName} : MicroGameHandler";
        result += @"
        {

            public override IEnumerator PreloadGame()
            {
                yield return 0;
            }

            public override float GetGameTimer(int difficultyLevel)
            {
                return 5;
            }

            protected override void SpecificInitGame(int difficultyLevel)
            {

            }

            protected override void SpecificEndGame(bool gameWon)
            {

            }
        }";
        return result;
    }

}
