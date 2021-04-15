using UnityEngine;
using UnityEditor;

/* Example of custom editor GUI for specific fields
 * Shamelessly stolen from Unitys Documentation
 */

[CustomEditor(typeof(GameSceneManager), true)]
public class ScenePickerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var picker = target as GameSceneManager;
        var oldMenuMenuScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(picker.MainMenuScenePath);
        var oldLevelScenePath = AssetDatabase.LoadAssetAtPath<SceneAsset>(picker.LevelScenePath);

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        var newMainMenuScene = EditorGUILayout.ObjectField("MainMenuScene", oldMenuMenuScene, typeof(SceneAsset), false) as SceneAsset;
        var newLevelScene = EditorGUILayout.ObjectField("LevelScene", oldLevelScenePath, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            var newMainMenuPath = AssetDatabase.GetAssetPath(newMainMenuScene);
            var newLevelScenePath = AssetDatabase.GetAssetPath(newLevelScene);
            var mainMenuScenePath = serializedObject.FindProperty("MainMenuScenePath");
            var levelScenePath = serializedObject.FindProperty("LevelScenePath");
            mainMenuScenePath.stringValue = newMainMenuPath;
            levelScenePath.stringValue = newLevelScenePath;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
