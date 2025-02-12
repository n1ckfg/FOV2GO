using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dWindow.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dTouchpad))]
public class s3dTouchpadEditor : Editor
{

    private s3dTouchpad target;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
        target.moveLikeJoystick = EditorGUILayout.Toggle(new GUIContent("Move Like Joystick", "Move Graphic With Touch"), target.moveLikeJoystick, new GUILayoutOption[] {});
        target.actLikeJoystick = EditorGUILayout.Toggle(new GUIContent("Act Like Joystick", "Jump to Touch Down Position"), target.actLikeJoystick, new GUILayoutOption[] {});
        target.shortTapTimeMax = EditorGUILayout.Slider(new GUIContent("Short Tap Time Max", "Maximum Touch Time for Short Tap"), (float) target.shortTapTimeMax, 0.1f, 0.5f, new GUILayoutOption[] {});
        target.longTapTimeMax = EditorGUILayout.Slider(new GUIContent("Long Tap Time Max", "Maximum Touch Time for Long Tap"), (float) target.longTapTimeMax, 0.2f, 1f, new GUILayoutOption[] {});
        target.tapDistanceLimit = EditorGUILayout.Slider(new GUIContent("Tap Distance Limit", "Maximum Travel Distance for Tap"), (float) target.tapDistanceLimit, 1f, 20f, new GUILayoutOption[] {});
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}