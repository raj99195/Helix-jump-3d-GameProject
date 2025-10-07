using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

[CustomEditor(typeof(LevelCreator),true)]
public class LevelCreatorEditor : Editor {
    private LevelCreator _levelCreator;

    private void OnEnable()
    {
        _levelCreator = (LevelCreator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

//        EditorGUILayout.BeginHorizontal();
//        if (GUILayout.Button("Refresh"))
//        {
//            var stageCreators = _levelCreator.GetComponentsInChildren<StageCreator>();
//          
//            for (var i = 0; i < stageCreators.Length; i++)
//            {
//                var stageCreator = stageCreators[i];
//                stageCreator.transform.position = _levelCreator.transform.position +
//                                                        Vector3.up *
//                                                        (_levelCreator.StartOffset + i * _levelCreator.Space);
//            }
//
//            _levelCreator.EndPoint.transform.position = _levelCreator.transform.position +
//                                                        Vector3.up *
//                                                        (_levelCreator.StartOffset + stageCreators.Length * _levelCreator.Space);
//
//            var scale = _levelCreator.Rod.localScale;
//            scale.y = _levelCreator.StartOffset + _levelCreator.Space * stageCreators.Length+_levelCreator.EndOffset;
//            _levelCreator.Rod.localScale = scale;
//
//        }

        if (GUILayout.Button("Refresh"))
        {
            var stages = _levelCreator.GetComponentsInChildren<Stage>();

//            foreach (var stageCreator in stageCreators)
//            {
//                if (stageCreator.Stage == null)
//                {
//                    stageCreator.GenerateOrRefresh();
//                }
//            }

            for (var i = 0; i < stages.Length; i++)
            {
                var stage = stages[i];
                stage.transform.position = _levelCreator.transform.position -
                                                  Vector3.up *
                                                  (_levelCreator.StartOffset + i * _levelCreator.Space);
            }


            _levelCreator.EndPoint.transform.position = _levelCreator.transform.position -
                                                                    Vector3.up *
                                                                    (_levelCreator.StartOffset + stages.Length * _levelCreator.Space);

            var scale = _levelCreator.Rod.localScale;
            scale.y = _levelCreator.StartOffset + _levelCreator.Space * stages.Length + _levelCreator.EndOffset;
            _levelCreator.Rod.localScale = scale;

            Undo.RecordObject(_levelCreator,"Refreshed");

            
        }

        //        if (GUILayout.Button("Clear"))
        //        {
        //            var stageCreators = _levelCreator.GetComponentsInChildren<StageCreator>();
        //
        //            foreach (var stageCreator in stageCreators)
        //            {
        //                if (stageCreator.Stage != null)
        //                {
        //                    stageCreator.Clear();
        //                }
        //            }
        //        }

        //        EditorGUILayout.EndHorizontal();
    }
}

public static class EditorManager
{

    [MenuItem("MyGames/Clear Prefs")]
    public static void ClearPrefs()
    {
        PrefManager.Clear();
    }

}

public class PreProcessBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;
    // ReSharper disable once IdentifierTypo
    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Preprocessor");
    }
}