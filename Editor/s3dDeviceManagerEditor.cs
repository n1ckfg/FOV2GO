using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dDeviceManager.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dDeviceManager))]
public class s3dDeviceManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(170, 30);
        bool allowSceneObjects = !EditorUtility.IsPersistent(this.target);
        EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
        this.target.phoneLayout = EditorGUILayout.EnumPopup(new GUIContent("Phone Layout", "Select Phone Layout"), this.target.phoneLayout, new GUILayoutOption[] {});
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        this.target.use3dCursor = EditorGUILayout.Toggle(new GUIContent("Use 3D Cursor", "Use 3D Cursor"), this.target.use3dCursor, new GUILayoutOption[] {});
        if (this.target.use3dCursor != null)
        {
            EditorGUI.indentLevel = 1;
            this.target.cursor3D = EditorGUILayout.ObjectField(new GUIContent("3D Cursor", "Select s3dGuiTexture for Cursor"), this.target.cursor3D, typeof(s3dGuiCursor), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        this.target.movePadPosition = EditorGUILayout.EnumPopup(new GUIContent("Move Pad Position", "Select Move Pad Position"), this.target.movePadPosition, new GUILayoutOption[] {});
        if (!(this.target.movePadPosition == 0))
        {
            EditorGUI.indentLevel = 1;
            this.target.moveTouchpad = EditorGUILayout.ObjectField(new GUIContent("Move Touchpad", "Select s3dTouchpad for Move"), this.target.moveTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        this.target.turnPadPosition = EditorGUILayout.EnumPopup(new GUIContent("Turn Pad Position", "Select Turn Pad Position"), this.target.turnPadPosition, new GUILayoutOption[] {});
        if (!(this.target.turnPadPosition == 0))
        {
            EditorGUI.indentLevel = 1;
            this.target.turnTouchpad = EditorGUILayout.ObjectField(new GUIContent("Turn Touchpad", "Select s3dTouchpad for Turn"), this.target.turnTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        this.target.pointPadPosition = EditorGUILayout.EnumPopup(new GUIContent("Cursor Pad Position", "Select Cursor Pad Position"), this.target.pointPadPosition, new GUILayoutOption[] {});
        if (!(this.target.pointPadPosition == 0))
        {
            EditorGUI.indentLevel = 1;
            this.target.pointTouchpad = EditorGUILayout.ObjectField(new GUIContent("Cursor Touchpad", "Select s3dTouchpad for Cursor"), this.target.pointTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        this.target.useStereoParamsTouchpad = EditorGUILayout.Toggle(new GUIContent("Show 3D Params Touchpad", "Use Stereo Params Touchpad"), this.target.useStereoParamsTouchpad, new GUILayoutOption[] {});
        if (this.target.useStereoParamsTouchpad != null)
        {
            EditorGUI.indentLevel = 1;
            this.target.stereoParamsTouchpad = EditorGUILayout.ObjectField(new GUIContent("Stereo Params Touchpad", "Select s3dTouchpad for Stereo Params"), this.target.stereoParamsTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 2;
            this.target.interaxialTouchpad = EditorGUILayout.ObjectField(new GUIContent("Interaxial Touchpad", "Select s3dTouchpad for Interaxial"), this.target.interaxialTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            this.target.zeroPrlxTouchpad = EditorGUILayout.ObjectField(new GUIContent("Zero Prlx Touchpad", "Select s3dTouchpad for Zero Prlx"), this.target.zeroPrlxTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            this.target.hitTouchpad = EditorGUILayout.ObjectField(new GUIContent("H I T Touchpad", "Select s3dTouchpad for H I T"), this.target.hitTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        this.target.showLoadNewScenePad = EditorGUILayout.Toggle(new GUIContent("Show Load Scene Touchpad", "Show Load New Scene Touchpad"), this.target.showLoadNewScenePad, new GUILayoutOption[] {});
        if (this.target.showLoadNewScenePad != null)
        {
            EditorGUI.indentLevel = 1;
            this.target.loadNewSceneTouchpad = EditorGUILayout.ObjectField(new GUIContent("Load New Scene Touchpad", "Select s3dTouchpad for Load New Scene"), this.target.loadNewSceneTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        this.target.showFpsTool01 = EditorGUILayout.Toggle(new GUIContent("Show FPS Tool 01 Touchpad", "Show FPS Tool 01 Touchpad"), this.target.showFpsTool01, new GUILayoutOption[] {});
        if (this.target.showFpsTool01 != null)
        {
            EditorGUI.indentLevel = 1;
            this.target.fpsTool01 = EditorGUILayout.ObjectField(new GUIContent("FPS Tool 01 Touchpad", "Select s3dTouchpad for FPS Tool 01"), this.target.fpsTool01, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        this.target.showFpsTool02 = EditorGUILayout.Toggle(new GUIContent("Show FPS Tool 02 Touchpad", "Show FPS Tool 02 Touchpad"), this.target.showFpsTool02, new GUILayoutOption[] {});
        if (this.target.showFpsTool02 != null)
        {
            EditorGUI.indentLevel = 1;
            this.target.fpsTool02 = EditorGUILayout.ObjectField(new GUIContent("FPS Tool 02 Touchpad", "Select s3dTouchpad for FPS Tool 02"), this.target.fpsTool02, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(this.target);
        }
    }

}