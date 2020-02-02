using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* s3d Stereo Window Editor Script revised 12.30.12
 * This script goes in the Editor folder. It provides a custom inspector for s3dMouse.js.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dWindow))]
public class s3dWindowEditor : Editor
{
    public static bool foldout1;

    private s3dWindow target;

    public override void OnInspectorGUI()
    {
        s3dWindowEditor.foldout1 = EditorGUILayout.Foldout(s3dWindowEditor.foldout1, "Options");
        if (s3dWindowEditor.foldout1)
        {
            EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
            this.target.on = EditorGUILayout.Toggle("Active", this.target.on, new GUILayoutOption[] {});
            if (GUI.changed)
            {
                EditorUtility.SetDirty(this.target);
                this.target.toggleVis(this.target.on);
            }
            GUI.changed = false;
            this.target.drawDebugRays = EditorGUILayout.Toggle("Draw Debug Rays", this.target.drawDebugRays, new GUILayoutOption[] {});
            this.target.sideSamples = (int) EditorGUILayout.Slider("Side Samples", (float) this.target.sideSamples, 3, 50, new GUILayoutOption[] {});
            this.target.maskLimit = (maskDistance) EditorGUILayout.EnumPopup("Mask Limit", this.target.maskLimit, new GUILayoutOption[] {});
            if (this.target.maskLimit == 0)
            {
                this.target.maximumDistance = EditorGUILayout.Slider("Maximum Distance", (float) this.target.maximumDistance, 0, 50, new GUILayoutOption[] {});
            }
            EditorGUILayout.EndVertical();
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(this.target);
        }
    }

    static s3dWindowEditor()
    {
        s3dWindowEditor.foldout1 = true;
    }

}