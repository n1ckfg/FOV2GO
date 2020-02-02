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

    private s3dDeviceManager target;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(170, 30);
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
        target.phoneLayout = (phoneType) EditorGUILayout.EnumPopup(new GUIContent("Phone Layout", "Select Phone Layout"), target.phoneLayout, new GUILayoutOption[] {});
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        target.use3dCursor = EditorGUILayout.Toggle(new GUIContent("Use 3D Cursor", "Use 3D Cursor"), target.use3dCursor, new GUILayoutOption[] {});
        if (target.use3dCursor != null)
        {
            EditorGUI.indentLevel = 1;
            target.cursor3D = (s3dGuiCursor) EditorGUILayout.ObjectField(new GUIContent("3D Cursor", "Select s3dGuiTexture for Cursor"), target.cursor3D, typeof(s3dGuiCursor), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        target.movePadPosition = (controlPos) EditorGUILayout.EnumPopup(new GUIContent("Move Pad Position", "Select Move Pad Position"), target.movePadPosition, new GUILayoutOption[] {});
        if (!(target.movePadPosition == 0))
        {
            EditorGUI.indentLevel = 1;
            target.moveTouchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Move Touchpad", "Select s3dTouchpad for Move"), target.moveTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        target.turnPadPosition = (controlPos) EditorGUILayout.EnumPopup(new GUIContent("Turn Pad Position", "Select Turn Pad Position"), target.turnPadPosition, new GUILayoutOption[] {});
        if (!(target.turnPadPosition == 0))
        {
            EditorGUI.indentLevel = 1;
            target.turnTouchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Turn Touchpad", "Select s3dTouchpad for Turn"), target.turnTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        target.pointPadPosition = (controlPos) EditorGUILayout.EnumPopup(new GUIContent("Cursor Pad Position", "Select Cursor Pad Position"), target.pointPadPosition, new GUILayoutOption[] {});
        if (!(target.pointPadPosition == 0))
        {
            EditorGUI.indentLevel = 1;
            target.pointTouchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Cursor Touchpad", "Select s3dTouchpad for Cursor"), target.pointTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        target.useStereoParamsTouchpad = EditorGUILayout.Toggle(new GUIContent("Show 3D Params Touchpad", "Use Stereo Params Touchpad"), target.useStereoParamsTouchpad, new GUILayoutOption[] {});
        if (target.useStereoParamsTouchpad != null)
        {
            EditorGUI.indentLevel = 1;
            target.stereoParamsTouchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Stereo Params Touchpad", "Select s3dTouchpad for Stereo Params"), target.stereoParamsTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 2;
            target.interaxialTouchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Interaxial Touchpad", "Select s3dTouchpad for Interaxial"), target.interaxialTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            target.zeroPrlxTouchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Zero Prlx Touchpad", "Select s3dTouchpad for Zero Prlx"), target.zeroPrlxTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            target.hitTouchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("H I T Touchpad", "Select s3dTouchpad for H I T"), target.hitTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        target.showLoadNewScenePad = EditorGUILayout.Toggle(new GUIContent("Show Load Scene Touchpad", "Show Load New Scene Touchpad"), target.showLoadNewScenePad, new GUILayoutOption[] {});
        if (target.showLoadNewScenePad != null)
        {
            EditorGUI.indentLevel = 1;
            target.loadNewSceneTouchpad = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("Load New Scene Touchpad", "Select s3dTouchpad for Load New Scene"), target.loadNewSceneTouchpad, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        target.showFpsTool01 = EditorGUILayout.Toggle(new GUIContent("Show FPS Tool 01 Touchpad", "Show FPS Tool 01 Touchpad"), target.showFpsTool01, new GUILayoutOption[] {});
        if (target.showFpsTool01 != null)
        {
            EditorGUI.indentLevel = 1;
            target.fpsTool01 = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("FPS Tool 01 Touchpad", "Select s3dTouchpad for FPS Tool 01"), target.fpsTool01, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        target.showFpsTool02 = EditorGUILayout.Toggle(new GUIContent("Show FPS Tool 02 Touchpad", "Show FPS Tool 02 Touchpad"), target.showFpsTool02, new GUILayoutOption[] {});
        if (target.showFpsTool02 != null)
        {
            EditorGUI.indentLevel = 1;
            target.fpsTool02 = (s3dTouchpad) EditorGUILayout.ObjectField(new GUIContent("FPS Tool 02 Touchpad", "Select s3dTouchpad for FPS Tool 02"), target.fpsTool02, typeof(s3dTouchpad), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUI.indentLevel = 0;
        }
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}