using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dGyroCam.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dGyroCam))]
public class s3dGyroCamEditor : Editor
{

    private s3dGyroCam target;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
        target.touchRotatesHeading = EditorGUILayout.Toggle(new GUIContent("Touch Rotates Heading", "Horizontal Swipe Rotates Heading"), target.touchRotatesHeading, new GUILayoutOption[] {});
        target.setZeroToNorth = EditorGUILayout.Toggle(new GUIContent("Set Zero Heading to North", "Face Compass North on Startup"), target.setZeroToNorth, new GUILayoutOption[] {});
        target.checkForAutoRotation = EditorGUILayout.Toggle(new GUIContent("Check For Auto Rotation", "Leave off unless Auto Rotation is on"), target.checkForAutoRotation, new GUILayoutOption[] {});
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}