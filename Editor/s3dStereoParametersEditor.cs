using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dStereoParameters.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dStereoParameters))]
public class s3dStereoParametersEditor : Editor
{
    public static bool foldout1;

    private s3dStereoParameters target;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(170, 30);
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
        if (target.s3dDeviceMan == null)
        {
            target.stereoParamsTouchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Stereo Params Touchpad", "Select s3dTouchpad for Stereo Params"), target.stereoParamsTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            target.interaxialTouchpad = (s3dTouchpad)EditorGUILayout.ObjectField(new GUIContent("Interaxial Touchpad", "Select s3dTouchpad for Interaxial"), target.interaxialTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            target.zeroPrlxTouchpad = (s3dTouchpad)EditorGUILayout.ObjectField(new GUIContent("Zero Prlx Touchpad", "Select s3dTouchpad for Zero Prlx"), target.zeroPrlxTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            target.hitTouchpad = (s3dTouchpad)EditorGUILayout.ObjectField(new GUIContent("H I T Touchpad", "Select s3dTouchpad for H I T"), target.hitTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
        }
        target.saveStereoParamsToDisk = EditorGUILayout.Toggle(new GUIContent("Save 3D Params To Disk", "Save 3D Params To Disk"), target.saveStereoParamsToDisk, new GUILayoutOption[] {});
        target.stereoReadoutText = (s3dGuiText) EditorGUILayout.ObjectField(new GUIContent("Stereo Readout Text", "Select s3dGuiText for Stereo Readout"), target.stereoReadoutText, typeof(s3dGuiText), allowSceneObjects, new GUILayoutOption[] {});
        s3dStereoParametersEditor.foldout1 = EditorGUILayout.Foldout(s3dStereoParametersEditor.foldout1, new GUIContent("3D Params Touchpad Textures", "Select 3D Params Touchpad Textures"));
        if (s3dStereoParametersEditor.foldout1)
        {
            target.showStereoParamsTexture = (Texture2D) EditorGUILayout.ObjectField(new GUIContent("Show Params Texture", "Select Texture for Stereo Params Toggle"), target.showStereoParamsTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
            target.dismissStereoParamsTexture = (Texture2D) EditorGUILayout.ObjectField(new GUIContent("Dismiss Params Texture", "Select Texture for Dismiss Stereo Params Toggle"), target.dismissStereoParamsTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}