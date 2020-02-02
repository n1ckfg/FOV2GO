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
        bool allowSceneObjects = !EditorUtility.IsPersistent(this.target);
        EditorGUIUtility.LookLikeControls(160, 50);
        EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
        this.target.trackMouseXYPosition = EditorGUILayout.Toggle(new GUIContent("Track Mouse Position", "Follow mouse position [enable for desktop stereo cursor]"), this.target.trackMouseXYPosition, new GUILayoutOption[] {});
        if (this.target.trackMouseXYPosition != null)
        {
            EditorGUI.indentLevel = 1;
            this.target.onlyWhenMouseDown = EditorGUILayout.Toggle(new GUIContent("Track Only When Down", "Follow mouse position only when mouse button down"), this.target.onlyWhenMouseDown, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        this.target.useTouchpad = EditorGUILayout.Toggle(new GUIContent("Use Touchpad", "Control via s3dTouchpad"), this.target.useTouchpad, new GUILayoutOption[] {});
        if (this.target.useTouchpad != null)
        {
            EditorGUI.indentLevel = 1;
            this.target.touchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Touchpad", "Assign s3d Touchpad"), this.target.touchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            this.target.touchpadSpeed = EditorGUILayout.Vector2Field("Touchpad Speed Factor", this.target.touchpadSpeed, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        this.target.clickDistance = EditorGUILayout.Slider(new GUIContent("Maximum Click Distance", "Ignore clicks beyond this distance"), (float) this.target.clickDistance, 5, 100, new GUILayoutOption[] {});
        this.target.hidePointer = EditorGUILayout.Toggle(new GUIContent("Hide Pointer", "Hide Default Pointer"), this.target.hidePointer, new GUILayoutOption[] {});
        this.target.interactiveLayer = EditorGUILayout.LayerField(new GUIContent("Interactive Layer:", "Layer for clickable objects."), (int) this.target.interactiveLayer, new GUILayoutOption[] {});
        this.showFileFields = EditorGUILayout.Foldout(this.showFileFields, "Textures & Sounds");
        if (this.showFileFields)
        {
            EditorGUI.indentLevel = 1;
            this.target.defaultTexture = (Texture) EditorGUILayout.ObjectField(new GUIContent("Default Texture", "Default Texture"), this.target.defaultTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
            this.target.clickSound = (AudioClip) EditorGUILayout.ObjectField(new GUIContent("Click Sound", "Sound for click"), this.target.clickSound, typeof(AudioClip), allowSceneObjects, new GUILayoutOption[] {});
            this.target.clickTexture = (Texture) EditorGUILayout.ObjectField(new GUIContent("Click Texture", "Texture for clicks"), this.target.clickTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
            this.target.pickSound = (AudioClip) EditorGUILayout.ObjectField(new GUIContent("Pick Sound", "Sound for select"), this.target.pickSound, typeof(AudioClip), allowSceneObjects, new GUILayoutOption[] {});
            this.target.pickTexture = (Texture) EditorGUILayout.ObjectField(new GUIContent("Pick Texture", "Texture for select"), this.target.pickTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(this.target);
        }
    }

}