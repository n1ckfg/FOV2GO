using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dGuiCursor.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dGuiCursor))]
public class s3dGuiCursorEditor : Editor
{
    public bool showFileFields;

    private s3dGuiCursor target;

    public override void OnInspectorGUI()
    {
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        EditorGUIUtility.LookLikeControls(160, 50);
        EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
        target.trackMouseXYPosition = EditorGUILayout.Toggle(new GUIContent("Track Mouse Position", "Follow mouse position [enable for desktop stereo cursor]"), target.trackMouseXYPosition, new GUILayoutOption[] {});
        if (target.trackMouseXYPosition != null)
        {
            EditorGUI.indentLevel = 1;
            target.onlyWhenMouseDown = EditorGUILayout.Toggle(new GUIContent("Track Only When Down", "Follow mouse position only when mouse button down"), target.onlyWhenMouseDown, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        target.useTouchpad = EditorGUILayout.Toggle(new GUIContent("Use Touchpad", "Control via s3dTouchpad"), target.useTouchpad, new GUILayoutOption[] {});
        if (target.useTouchpad != null)
        {
            EditorGUI.indentLevel = 1;
            target.touchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Touchpad", "Assign s3d Touchpad"), target.touchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            target.touchpadSpeed = EditorGUILayout.Vector2Field("Touchpad Speed Factor", target.touchpadSpeed, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        target.clickDistance = EditorGUILayout.Slider(new GUIContent("Maximum Click Distance", "Ignore clicks beyond this distance"), (float) target.clickDistance, 5, 100, new GUILayoutOption[] {});
        target.hidePointer = EditorGUILayout.Toggle(new GUIContent("Hide Pointer", "Hide Default Pointer"), target.hidePointer, new GUILayoutOption[] {});
        target.interactiveLayer = EditorGUILayout.LayerField(new GUIContent("Interactive Layer:", "Layer for clickable objects."), (int) target.interactiveLayer, new GUILayoutOption[] {});
        showFileFields = EditorGUILayout.Foldout(showFileFields, "Textures & Sounds");
        if (showFileFields)
        {
            EditorGUI.indentLevel = 1;
            target.defaultTexture = (Texture) EditorGUILayout.ObjectField(new GUIContent("Default Texture", "Default Texture"), target.defaultTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
            target.clickSound = (AudioClip) EditorGUILayout.ObjectField(new GUIContent("Click Sound", "Sound for click"), target.clickSound, typeof(AudioClip), allowSceneObjects, new GUILayoutOption[] {});
            target.clickTexture = (Texture) EditorGUILayout.ObjectField(new GUIContent("Click Texture", "Texture for clicks"), target.clickTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
            target.pickSound = (AudioClip) EditorGUILayout.ObjectField(new GUIContent("Pick Sound", "Sound for select"), target.pickSound, typeof(AudioClip), allowSceneObjects, new GUILayoutOption[] {});
            target.pickTexture = (Texture) EditorGUILayout.ObjectField(new GUIContent("Pick Texture", "Texture for select"), target.pickTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}