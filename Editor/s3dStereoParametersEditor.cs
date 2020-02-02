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
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(170, 30);
        bool allowSceneObjects = !EditorUtility.IsPersistent(this.target);
        EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
        if (this.target.s3dDeviceMan == null)
        {
            this.target.stereoParamsTouchpad = EditorGUILayout.ObjectField(new GUIContent("Stereo Params Touchpad", "Select s3dTouchpad for Stereo Params"), this.target.stereoParamsTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            this.target.interaxialTouchpad = EditorGUILayout.ObjectField(new GUIContent("Interaxial Touchpad", "Select s3dTouchpad for Interaxial"), this.target.interaxialTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            this.target.zeroPrlxTouchpad = EditorGUILayout.ObjectField(new GUIContent("Zero Prlx Touchpad", "Select s3dTouchpad for Zero Prlx"), this.target.zeroPrlxTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            this.target.hitTouchpad = EditorGUILayout.ObjectField(new GUIContent("H I T Touchpad", "Select s3dTouchpad for H I T"), this.target.hitTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
        }
        this.target.saveStereoParamsToDisk = EditorGUILayout.Toggle(new GUIContent("Save 3D Params To Disk", "Save 3D Params To Disk"), this.target.saveStereoParamsToDisk, new GUILayoutOption[] {});
        this.target.stereoReadoutText = EditorGUILayout.ObjectField(new GUIContent("Stereo Readout Text", "Select s3dGuiText for Stereo Readout"), this.target.stereoReadoutText, typeof(s3dGuiText), allowSceneObjects, new GUILayoutOption[] {});
        s3dStereoParametersEditor.foldout1 = EditorGUILayout.Foldout(s3dStereoParametersEditor.foldout1, new GUIContent("3D Params Touchpad Textures", "Select 3D Params Touchpad Textures"));
        if (s3dStereoParametersEditor.foldout1)
        {
            this.target.showStereoParamsTexture = EditorGUILayout.ObjectField(new GUIContent("Show Params Texture", "Select Texture for Stereo Params Toggle"), this.target.showStereoParamsTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
            this.target.dismissStereoParamsTexture = EditorGUILayout.ObjectField(new GUIContent("Dismiss Params Texture", "Select Texture for Dismiss Stereo Params Toggle"), this.target.dismissStereoParamsTexture, typeof(Texture), allowSceneObjects, new GUILayoutOption[] {});
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(this.target);
        }
    }

}