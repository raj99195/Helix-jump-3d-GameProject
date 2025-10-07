using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageCreator))]
public class StageCreatorEditor : Editor
{
    private StageCreator _stageCreator;

    private void OnEnable()
    {
        _stageCreator = (StageCreator) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Random"))
        {
            _stageCreator.GenerateOrRefresh();
        }

        if (GUILayout.Button("Auto Detect"))
        {

        }

//        if (GUILayout.Button("Clear"))
//        {
//            if (_stageCreator.Stage!=null)
//            {
//                _stageCreator.Clear();
//            }
//        }
        EditorGUILayout.EndHorizontal();
    }
}



//[CustomEditor(typeof(LevelSceneGroup))]
//public class LevelSceneGroupEditor : Editor
//{
//    private SerializedProperty _scenesProperty;
//
//    private void OnEnable()
//    {
//        _scenesProperty = serializedObject.FindProperty(LevelSceneGroup.SCENES_FIELD);
//    }
//
//    // ReSharper disable once MethodTooLong
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        _scenesProperty.isExpanded = EditorGUILayout.Foldout(_scenesProperty.isExpanded, _scenesProperty.displayName);
//        if (_scenesProperty.isExpanded)
//        {
//            EditorGUI.indentLevel++;
//            for (int i = 0; i < _scenesProperty.arraySize; i++)
//            {
//                var property = _scenesProperty.GetArrayElementAtIndex(i);
//                var levelProperty = property.FindPropertyRelative(nameof(LevelScene.level));
//                var sceneProperty = property.FindPropertyRelative(nameof(LevelScene.scene));
//                EditorGUILayout.BeginHorizontal();
//                EditorGUILayout.LabelField(levelProperty.intValue.ToString());
//                EditorGUILayout.LabelField(sceneProperty.stringValue);
//                EditorGUILayout.EndHorizontal();
//            }
//
//            EditorGUI.indentLevel--;
//        }
//
//        if (GUILayout.Button("Refresh"))
//        {
//            var levelScenes = EditorBuildSettings.scenes.Select(scene =>
//            {
//                var index = scene.path.LastIndexOf('/');
//                return scene.path.Substring(index + 1, scene.path.Length - index - 1);
//                // ReSharper disable once TooManyChainedReferences
//            }).Where(scene => scene.Contains("Level_")).ToList();
//
//            _scenesProperty.ClearArray();
//            for (var index = 0; index < levelScenes.Count; index++)
//            {
//                var levelScene = levelScenes[index];
//                _scenesProperty.InsertArrayElementAtIndex(index);
//                var property = _scenesProperty.GetArrayElementAtIndex(index);
//
//                var levelProperty = property.FindPropertyRelative(nameof(LevelScene.level));
//                var sceneProperty = property.FindPropertyRelative(nameof(LevelScene.scene));
//
//                var i = levelScene.IndexOf('_');
//                levelProperty.intValue =
//                    int.Parse(levelScene.Substring(i + 1, levelScene.Length - i - 1).Split('.')[0]);
//                sceneProperty.stringValue = levelScene.Split('.')[0];
//            }
//        }
//
//        serializedObject.ApplyModifiedProperties();
//    }
//}