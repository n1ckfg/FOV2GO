using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dGuiTexture.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dGuiTexture))]
public class s3dGuiTextureEditor : Editor
{
    public bool showFileFields;

    private s3dGuiTexture target;

    public override void OnInspectorGUI()
    {
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        EditorGUIUtility.LookLikeControls(160, 50);
        EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
        target.beginVisible = EditorGUILayout.Toggle(new GUIContent("Begin Visible", "Display texture at start"), target.beginVisible, new GUILayoutOption[] {});
        target.timeToDisplay = EditorGUILayout.Slider(new GUIContent("Time to Display (sec)", "Enter 0 to keep visible indefinitely"), (float) target.timeToDisplay, 0, 30, new GUILayoutOption[] {});
        target.objectDistance = EditorGUILayout.Slider(new GUIContent("Initial Object Distance (M)", "Distance in meters at start"), (float) target.objectDistance, 0.25f, 20, new GUILayoutOption[] {});
        target.keepCloser = EditorGUILayout.Toggle(new GUIContent("Track Depth", "Use dynamic depth to keep texture closer than scene objects"), target.keepCloser, new GUILayoutOption[] {});
        if (target.keepCloser != null)
        {
            float distMin = Mathf.Clamp((float) target.minimumDistance, 0.01f, 100);
            float distMax = Mathf.Clamp((float) target.maximumDistance, 0.01f, 100);
            EditorGUILayout.MinMaxSlider(ref distMin, ref distMax, 0.01f, 100f, new GUILayoutOption[] {});
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
            EditorGUILayout.LabelField(new GUIContent("Min Distance (M) " + (Mathf.Round((float) (((int) target.minimumDistance) * 100)) / 100), "Minimum allowed distance"), "", new GUILayoutOption[] {});
            EditorGUILayout.LabelField(new GUIContent("Max Distance (M) " + (Mathf.Round((float) (((int) target.maximumDistance) * 10)) / 10), "Maximum allowed distance"), "", new GUILayoutOption[] {});
            EditorGUILayout.EndHorizontal();
            target.minimumDistance = Mathf.Clamp(distMin, 0.01f, 100);
            target.maximumDistance = Mathf.Clamp(distMax, 0.01f, 100);
            target.nearPadding = EditorGUILayout.Slider(new GUIContent("Near Padding (mm)", "Padding between texture and nearest object behind"), (float) target.nearPadding, 0, 20, new GUILayoutOption[] {});
            target.lagTime = EditorGUILayout.Slider(new GUIContent("Smooth Depth Changes", "Smooth out sudden shifts in depth"), (float) target.lagTime, 0, 50, new GUILayoutOption[] {});
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}