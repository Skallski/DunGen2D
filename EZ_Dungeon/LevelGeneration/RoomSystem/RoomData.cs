using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(RoomData), true), CanEditMultipleObjects]
public class RoomDataEditor : Editor
{
    private RoomData roomData;

    private SerializedProperty canSpawnEnemies;
    
    private void OnEnable()
    {
        roomData = (RoomData) target;
        canSpawnEnemies = serializedObject.FindProperty("canSpawnEnemies");
    }

    public override void OnInspectorGUI()
    {
        if (roomData == null) return;
        
        // default "script" object field
        EditorGUI.BeginDisabledGroup(true); 
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ScriptableObject)target), GetType(), false);
        EditorGUILayout.Space();
        EditorGUI.EndDisabledGroup();
        
        serializedObject.Update();
        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnableInteriorObjects"));
        
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(canSpawnEnemies);
        if (EditorGUILayout.BeginFadeGroup(canSpawnEnemies.boolValue ? 1 : 0))
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnableEnemies"));
        }
        EditorGUILayout.EndFadeGroup();

        // apply modified properties and repaint
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
        
        if (GUI.changed)
            InternalEditorUtility.RepaintAllViews();
    }
}
#endif

[CreateAssetMenu(fileName = "New RoomData", menuName = "RoomData")]
public sealed class RoomData : ScriptableObject
{
    [SerializeField] private List<InteriorObject> spawnableInteriorObjects;
    public List<InteriorObject> SpawnableInteriorObjects => spawnableInteriorObjects;

    [SerializeField] private bool canSpawnEnemies = true;
    public bool CanSpawnEnemies => canSpawnEnemies;
    
    [SerializeField] private List<EnemyToSpawn> spawnableEnemies;
    public List<EnemyToSpawn> SpawnableEnemies => spawnableEnemies;
}