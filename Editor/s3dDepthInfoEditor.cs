using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dDepthInfo.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dDepthInfo))]
public class s3dDepthInfoEditor : Editor
{

    private s3dDepthInfo target;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(120, 30);
        this.target.raysH = EditorGUILayout.IntSlider("Ray Columns", (int) this.target.raysH, 3, 100, new GUILayoutOption[] {});
        this.target.raysV = EditorGUILayout.IntSlider("Ray Rows", (int) this.target.raysV, 3, 100, new GUILayoutOption[] {});
        this.target.maxSampleDistance = EditorGUILayout.Slider("Max Sample Distance", (float) this.target.maxSampleDistance, 1, 500, new GUILayoutOption[] {});
        EditorGUIUtility.LookLikeControls(130, 70);
        this.target.drawDebugRays = EditorGUILayout.Toggle("Draw Debug Rays", this.target.drawDebugRays, new GUILayoutOption[] {});
        bool allowSceneObjects = !EditorUtility.IsPersistent(this.target);
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        this.target.showScreenPlane = EditorGUILayout.Toggle(new GUIContent("Show Screen Plane ", "Translucent Plane at Zero Prlx Dist"), this.target.showScreenPlane, new GUILayoutOption[] {});
        EditorGUIUtility.LookLikeControls(150, 70);
        this.target.clickToSelect = EditorGUILayout.Toggle("Click Selects Object", this.target.clickToSelect, new GUILayoutOption[] {});
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        EditorGUIUtility.LookLikeControls(130, 70);
        this.target.showNearFarPlanes = EditorGUILayout.Toggle(new GUIContent("Show Near/Far Planes ", "Translucent Plane at Near/Far Dist"), this.target.showNearFarPlanes, new GUILayoutOption[] {});
        EditorGUIUtility.LookLikeControls(90, 70);
        this.target.selectedObject = (GameObject) EditorGUILayout.ObjectField("Selected Object", this.target.selectedObject, typeof(GameObject), allowSceneObjects, new GUILayoutOption[] {});
        EditorGUILayout.EndHorizontal();
        Rect r = EditorGUILayout.BeginVertical("TextField", new GUILayoutOption[] {});
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        EditorGUILayout.LabelField("Distances: ", new GUILayoutOption[] {});
        EditorGUILayout.LabelField("Near: " + (Mathf.Round((float) (((int) this.target.nearDistance) * 10)) / 10), new GUILayoutOption[] {});
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        EditorGUILayout.LabelField("Mouse: " + (Mathf.Round((float) (((int) this.target.distanceUnderMouse) * 10)) / 10), new GUILayoutOption[] {});
        EditorGUILayout.LabelField("Center: " + (Mathf.Round((float) (((int) this.target.distanceAtCenter) * 10)) / 10), new GUILayoutOption[] {});
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
        EditorGUILayout.LabelField("Object: " + (Mathf.Round((float) (((int) this.target.objectDistance) * 10)) / 10), new GUILayoutOption[] {});
        EditorGUILayout.LabelField("Far: " + (Mathf.Round((float) (((int) this.target.farDistance) * 10)) / 10), new GUILayoutOption[] {});
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(this.target);
        }
    }

}