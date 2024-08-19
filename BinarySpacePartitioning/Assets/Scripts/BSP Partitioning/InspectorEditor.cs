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
    SerializedProperty publicSpaceDoorNumOnWall;
    SerializedProperty plazaRadius;
    SerializedProperty polygon;
    SerializedProperty maxIterations;
    SerializedProperty entranceSize;
    SerializedProperty material;
    SerializedProperty wallVertical;
    SerializedProperty wallHorizontal;
    SerializedProperty gridSize;

    void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        dungeonWidth = serializedObject.FindProperty("dungeonWidth");
        dungeonHeight = serializedObject.FindProperty("dungeonHeight");
        roomWidthMin = serializedObject.FindProperty("roomWidthMin");
        roomHeightMin = serializedObject.FindProperty("roomHeightMin");
        publicSpaceWidth = serializedObject.FindProperty("publicSpaceWidth");
        publicSpaceHeight = serializedObject.FindProperty("publicSpaceHeight");
        publicSpaceDoorNumOnWall = serializedObject.FindProperty("publicSpaceDoorNumOnWall");
        plazaRadius = serializedObject.FindProperty("plazaRadius");
        polygon = serializedObject.FindProperty("polygon");
        maxIterations = serializedObject.FindProperty("maxIterations");
        entranceSize = serializedObject.FindProperty("entranceSize");
        material = serializedObject.FindProperty("material");
        wallVertical = serializedObject.FindProperty("wallVertical");
        wallHorizontal = serializedObject.FindProperty("wallHorizontal");
        gridSize = serializedObject.FindProperty("gridSize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(type);
        EditorGUILayout.PropertyField(dungeonWidth);
        EditorGUILayout.PropertyField(dungeonHeight);
        EditorGUILayout.PropertyField(roomWidthMin);
        EditorGUILayout.PropertyField(roomHeightMin);

        // type이 none인 경우에만 비활성화
        using (new EditorGUI.DisabledScope(type.enumValueIndex == (int)PUBLICSPACE.none))
        {
            EditorGUILayout.PropertyField(publicSpaceWidth);
            EditorGUILayout.PropertyField(publicSpaceHeight);
            EditorGUILayout.PropertyField(publicSpaceDoorNumOnWall);
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
       
        EditorGUILayout.PropertyField(entranceSize);
        EditorGUILayout.PropertyField(gridSize);
        EditorGUILayout.PropertyField(material);
        EditorGUILayout.PropertyField(wallHorizontal);
        EditorGUILayout.PropertyField(wallVertical);

        serializedObject.ApplyModifiedProperties();
    }
}


