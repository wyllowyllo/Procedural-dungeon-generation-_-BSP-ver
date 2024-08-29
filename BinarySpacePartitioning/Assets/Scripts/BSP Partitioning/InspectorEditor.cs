using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonCreator))]
public class DungeonCreatorEditor : Editor
{
    SerializedProperty type;
    SerializedProperty dungeonWidth;
    SerializedProperty dungeonHeight;
    SerializedProperty roomWidthMin;
    SerializedProperty roomHeightMin;
    SerializedProperty publicSpaceWidth;
    SerializedProperty publicSpaceHeight;
    SerializedProperty plazaRadius;
    SerializedProperty polygon;
    SerializedProperty maxIterations;
    SerializedProperty material;
    SerializedProperty tileObj;
    SerializedProperty wallVertical;
    SerializedProperty wallHorizontal;
    SerializedProperty entranceVertical;
    SerializedProperty entranceHorizontal;
    SerializedProperty playerObj;
    SerializedProperty unitSize;

    void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        dungeonWidth = serializedObject.FindProperty("dungeonWidth");
        dungeonHeight = serializedObject.FindProperty("dungeonHeight");
        roomWidthMin = serializedObject.FindProperty("roomWidthMin");
        roomHeightMin = serializedObject.FindProperty("roomHeightMin");
        publicSpaceWidth = serializedObject.FindProperty("publicSpaceWidth");
        publicSpaceHeight = serializedObject.FindProperty("publicSpaceHeight");
        plazaRadius = serializedObject.FindProperty("plazaRadius");
        polygon = serializedObject.FindProperty("polygon");
        maxIterations = serializedObject.FindProperty("maxIterations");
        material = serializedObject.FindProperty("material");
        tileObj = serializedObject.FindProperty("tileObj");
        wallVertical = serializedObject.FindProperty("wallVertical");
        wallHorizontal = serializedObject.FindProperty("wallHorizontal");
        entranceVertical = serializedObject.FindProperty("entranceVertical");
        entranceHorizontal = serializedObject.FindProperty("entranceHorizontal");
        playerObj = serializedObject.FindProperty("playerObj");
        unitSize = serializedObject.FindProperty("unitSize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(type);
        EditorGUILayout.PropertyField(unitSize);
        EditorGUILayout.PropertyField(dungeonWidth);
        EditorGUILayout.PropertyField(dungeonHeight);
        EditorGUILayout.PropertyField(roomWidthMin);
        EditorGUILayout.PropertyField(roomHeightMin);

        // type이 none인 경우에만 비활성화
        using (new EditorGUI.DisabledScope(type.enumValueIndex == (int)PUBLICSPACE.none))
        {
            EditorGUILayout.PropertyField(publicSpaceWidth);
            EditorGUILayout.PropertyField(publicSpaceHeight);
           
        }

        // type이 plaza가 아닌 경우 비활성화
        using (new EditorGUI.DisabledScope(type.enumValueIndex != (int)PUBLICSPACE.plaza))
        {
            EditorGUILayout.PropertyField(plazaRadius);
            EditorGUILayout.PropertyField(polygon);
        }

        // type이 none이 아닌 경우 비활성화
        using (new EditorGUI.DisabledScope(type.enumValueIndex != (int)PUBLICSPACE.none))
        {
            EditorGUILayout.PropertyField(maxIterations);
        }
       
        EditorGUILayout.PropertyField(material);
        EditorGUILayout.PropertyField(tileObj);
        EditorGUILayout.PropertyField(wallHorizontal);
        EditorGUILayout.PropertyField(wallVertical);
        EditorGUILayout.PropertyField(entranceHorizontal);
        EditorGUILayout.PropertyField(entranceVertical);
        EditorGUILayout.PropertyField(playerObj);

        serializedObject.ApplyModifiedProperties();
    }
}


