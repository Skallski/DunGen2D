#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    private DungeonGenerator dungeonGenerator;

    private bool generationParametersSelected;
    private float useRandomWalkChecked;

    private void Awake()
    {
        dungeonGenerator = (DungeonGenerator) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        // displays random walk data
        if (EditorGUILayout.BeginFadeGroup(dungeonGenerator.useRandomWalk ? 1 : 0))
        {
            EditorGUI.indentLevel++;
            
            dungeonGenerator.iterations = EditorGUILayout.IntSlider(
                new GUIContent("Iterations", "Number of algorithm iterations"),
                dungeonGenerator.iterations, 1, 100);

            dungeonGenerator.steps = EditorGUILayout.IntSlider(
                new GUIContent("Steps", "Number of random walk steps that algorithm performs during each iteration"), 
                dungeonGenerator.steps, 1, 50);
            
            dungeonGenerator.startRandomlyEachIteration = EditorGUILayout.Toggle(
                new GUIContent("Start randomly each iteration", "Should the position be random during each iteration?" +
                                                                "\n" + "YES : more tight passes" +
                                                                "\n" + "NO : more island shape"),
                dungeonGenerator.startRandomlyEachIteration);
            
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUILayout.Space();
        
        // displays buttons
        if (GUILayout.Button("Generate")) 
            dungeonGenerator.GenerateDungeon(); // generate dungeon when button is pressed

        if (GUILayout.Button("Clear")) 
            dungeonGenerator.ClearDungeon(); // clear generated dungeon when button is pressed
    }

}

#endif