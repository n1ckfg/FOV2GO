using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dAutoDepth.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dAutoDepth))]
public class s3dAutoDepthEditor : Editor
{

    private s3dAutoDepth target;

    public override void OnInspectorGUI()
    {
        target.convergenceMethod = (converge) EditorGUILayout.EnumPopup(new GUIContent("Convergence Method", "Pick dynamic convergence method"), target.convergenceMethod, new GUILayoutOption[] {});
        target.autoInteraxial = EditorGUILayout.Toggle(new GUIContent("Auto Interaxial", "Use dynamic interaxial"), target.autoInteraxial, new GUILayoutOption[] {});
        target.parallaxPercentageOfWidth = EditorGUILayout.Slider(new GUIContent("Parallax Percentage", "Total parallax percentage of image width"), (float) target.parallaxPercentageOfWidth, 1, 100, new GUILayoutOption[] {});
        target.percentageNegativeParallax = EditorGUILayout.Slider(new GUIContent("Negative/Positive Ratio", "Ratio of negative to positive parallax"), (float) target.percentageNegativeParallax, 0, 100, new GUILayoutOption[] {});
        target.zeroPrlxDistanceMin = EditorGUILayout.Slider(new GUIContent("Min Zero Prlx Distance", "Minimum allowable parallax (M)"), (float) target.zeroPrlxDistanceMin, 1, 100, new GUILayoutOption[] {});
        target.interaxialMin = EditorGUILayout.Slider(new GUIContent("Minimum Interaxial", "Minimum allowable interaxial (mm)"), (float) target.interaxialMin, 1, 100, new GUILayoutOption[] {});
        target.interaxialMax = EditorGUILayout.Slider(new GUIContent("Maximum Interaxial", "Maximum allowable interaxial (mm)"), (float) target.interaxialMax, 1, 1000, new GUILayoutOption[] {});
        target.lagTime = EditorGUILayout.Slider(new GUIContent("Lag Time", "Smooth abrupt changes"), (float) target.lagTime, 0, 100, new GUILayoutOption[] {});
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}