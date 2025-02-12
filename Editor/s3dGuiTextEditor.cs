using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dGuiText.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dGuiText))]
public class s3dGuiTextEditor : Editor
{
    public static bool foldout1;
    public static bool foldout2;
    public static bool foldout3;

    private s3dGuiText target;

    public override void OnInspectorGUI()
    {
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        EditorGUIUtility.LookLikeControls(150, 50);
        s3dGuiTextEditor.foldout1 = EditorGUILayout.Foldout(s3dGuiTextEditor.foldout1, "Text");
        if (s3dGuiTextEditor.foldout1)
        {
            EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
            EditorGUILayout.LabelField(new GUIContent("Initial Text", "Enter text to display at scene start"), "", new GUILayoutOption[] {GUILayout.MaxWidth(75)});
            target.initString = EditorGUILayout.TextArea(target.initString, new GUILayoutOption[] {GUILayout.MinHeight(50)});
            if (GUILayout.Button("Enter", new GUILayoutOption[] {GUILayout.MaxWidth(50)}))
            {
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();
            target.beginVisible = EditorGUILayout.Toggle(new GUIContent("Begin Visible", "Display text at start"), target.beginVisible, new GUILayoutOption[] {});
            target.timeToDisplay = EditorGUILayout.Slider(new GUIContent("Time to Display (sec)", "Enter 0 to keep visible indefinitely"), (float) target.timeToDisplay, 0, 30, new GUILayoutOption[] {});
            EditorGUILayout.EndVertical();
        }
        s3dGuiTextEditor.foldout2 = EditorGUILayout.Foldout(s3dGuiTextEditor.foldout2, "Style");
        if (s3dGuiTextEditor.foldout2)
        {
            EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
            target.TextColor = EditorGUILayout.ColorField(new GUIContent("Text Color", "Specify text color"), target.TextColor, new GUILayoutOption[] {});
            target.shadowsOn = EditorGUILayout.Toggle(new GUIContent("Shadow", "Add drop shadow to text"), target.shadowsOn, new GUILayoutOption[] {});
            if (target.shadowsOn != null)
            {
                target.ShadowColor = EditorGUILayout.ColorField(new GUIContent("Shadow Color", "Specify shadow color"), target.ShadowColor, new GUILayoutOption[] {});
                target.shadowOffset = EditorGUILayout.Slider(new GUIContent("Shadow Offset", "Specify shadow offset"), (float) target.shadowOffset, 1, 20, new GUILayoutOption[] {});
            }
            EditorGUILayout.EndVertical();
        }
        s3dGuiTextEditor.foldout3 = EditorGUILayout.Foldout(s3dGuiTextEditor.foldout3, "Distance");
        if (s3dGuiTextEditor.foldout3)
        {
            EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
            target.objectDistance = EditorGUILayout.Slider(new GUIContent("Initial Object Distance (M)", "Distance in meters at start"), (float) target.objectDistance, 0.25f, 20, new GUILayoutOption[] {});
            target.keepCloser = EditorGUILayout.Toggle(new GUIContent("Track Depth", "Use dynamic depth to keep text closer than scene objects"), target.keepCloser, new GUILayoutOption[] {});
            if (target.keepCloser != null)
            {
                float distMin = (float) target.minimumDistance;
                float distMax = (float) target.maximumDistance;
                EditorGUILayout.MinMaxSlider(ref distMin, ref distMax, 0.25f, 20f, new GUILayoutOption[] {});
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
                EditorGUILayout.LabelField(new GUIContent("Min Distance (M) " + (Mathf.Round((float) (((int) target.minimumDistance) * 10)) / 10), "Minimum allowed distance"), "", new GUILayoutOption[] {});
                EditorGUILayout.LabelField(new GUIContent("Max Distance (M) " + (Mathf.Round((float) (((int) target.maximumDistance) * 10)) / 10), "Maximum allowed distance"), "", new GUILayoutOption[] {});
                EditorGUILayout.EndHorizontal();
                target.minimumDistance = distMin;
                target.maximumDistance = distMax;
                target.nearPadding = EditorGUILayout.Slider(new GUIContent("Near Padding (mm)", "Padding between text and nearest object behind"), (float) target.nearPadding, 0.5f, 20, new GUILayoutOption[] {});
                target.lagTime = EditorGUILayout.Slider(new GUIContent("Smooth Depth Changes", "Smooth out sudden shifts in depth"), (float) target.lagTime, 0, 50, new GUILayoutOption[] {});
                target.trackMouseXYPosition = EditorGUILayout.Toggle(new GUIContent("Track Mouse Position", "Text follows mouse position"), target.trackMouseXYPosition, new GUILayoutOption[] {});
                target.onlyWhenMouseDown = EditorGUILayout.Toggle(new GUIContent("Track Only When Down", "Text follows mouse position only when mouse button down"), target.onlyWhenMouseDown, new GUILayoutOption[] {});
            }
            EditorGUILayout.EndVertical();
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    static s3dGuiTextEditor()
    {
        s3dGuiTextEditor.foldout1 = true;
        s3dGuiTextEditor.foldout2 = true;
        s3dGuiTextEditor.foldout3 = true;
    }

}