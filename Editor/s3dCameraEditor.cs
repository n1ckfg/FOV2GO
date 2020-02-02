using UnityEngine;
using UnityEditor;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dCamera.
 */
[System.Serializable]
[UnityEditor.CustomEditor(typeof(s3dCamera))]
public class s3dCameraEditor : Editor
{
    public static bool foldout1;
    public static bool foldout2;
    public static bool foldout3;

    private s3dCamera target;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(110, 30);
        bool allowSceneObjects = !EditorUtility.IsPersistent(this.target);
        s3dCameraEditor.foldout1 = EditorGUILayout.Foldout(s3dCameraEditor.foldout1, new GUIContent("Stereo Parameters", "Configure stereo camera"));
        if (s3dCameraEditor.foldout1)
        {
            EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
            this.target.interaxial = EditorGUILayout.IntSlider(new GUIContent("Interaxial (mm)", "Distance (in millimeters) between cameras."), (int) this.target.interaxial, 0, 1000, new GUILayoutOption[] {});
            this.target.zeroPrlxDist = EditorGUILayout.Slider(new GUIContent("Zero Prlx Dist (M)", "Distance (in meters) at which left and right images overlap exactly."), (float) this.target.zeroPrlxDist, 0.1f, 100, new GUILayoutOption[] {});
            this.target.toedIn = EditorGUILayout.Toggle(new GUIContent("Toed-In ", "Angle cameras inward to converge. Bad idea!"), this.target.toedIn, new GUILayoutOption[] {});
            this.target.cameraSelect = (cams3D) EditorGUILayout.EnumPopup(new GUIContent("Camera Order", "Swap cameras for cross-eyed free-viewing."), this.target.cameraSelect, new GUILayoutOption[] {});
            this.target.H_I_T = EditorGUILayout.Slider(new GUIContent("H I T", "horizontal image transform (default 0)"), (float) this.target.H_I_T, -25, 25, new GUILayoutOption[] {});
            EditorGUILayout.EndVertical();
        }
        s3dCameraEditor.foldout2 = EditorGUILayout.Foldout(s3dCameraEditor.foldout2, new GUIContent("Stereo Render", "Configure display format"));
        if (s3dCameraEditor.foldout2)
        {
            EditorGUILayout.BeginVertical("box", new GUILayoutOption[] {});
            this.target.useStereoShader = EditorGUILayout.Toggle(new GUIContent("Stereo Shader (Pro)", "Enable for anaglyph and other modes. Unity Pro required. Not necessary for side-by-side."), this.target.useStereoShader, new GUILayoutOption[] {});
            this.target.format3D = (mode3D) EditorGUILayout.EnumPopup(new GUIContent("Stereo Format", "Select 3D render format."), this.target.format3D, new GUILayoutOption[] {});
            if ((this.target.format3D == (mode3D) 0) || (this.target.format3D == (mode3D) 2)) // side by side
            {
                if (this.target.format3D == 0)
                {
                    this.target.sideBySideSqueezed = EditorGUILayout.Toggle(new GUIContent("Squeezed", "For 3D TV frame-compatible format"), this.target.sideBySideSqueezed, new GUILayoutOption[] {});
                }
                else
                {
                    if (this.target.format3D == (mode3D) 2)
                    {
                        this.target.overUnderStretched = EditorGUILayout.Toggle(new GUIContent("Stretched", "For 3D TV frame-compatible format"), this.target.overUnderStretched, new GUILayoutOption[] {});
                    }
                }
                if (this.target.useStereoShader == null)
                {
                    this.target.usePhoneMask = EditorGUILayout.Toggle(new GUIContent("Use Phone Mask", "Mask for side-by-side mobile phone formats"), this.target.usePhoneMask, new GUILayoutOption[] {});
                    if (this.target.usePhoneMask != null)
                    {
                        EditorGUI.indentLevel = 1;
                        this.target.leftViewRect = EditorGUILayout.Vector4Field("Left View Rect (x y width height)", this.target.leftViewRect, new GUILayoutOption[] {});
                        this.target.rightViewRect = EditorGUILayout.Vector4Field("Right View Rect (x y width height)", this.target.rightViewRect, new GUILayoutOption[] {});
                        EditorGUI.indentLevel = 0;
                    }
                }
                else
                {
                }
            }
            else
            {
                //target.usePhoneMask = false;
                if (this.target.format3D == (mode3D) 1) // anaglyph
                {
                    this.target.anaglyphOptions = (anaType) EditorGUILayout.EnumPopup(new GUIContent("Anaglyph Mode", "Anaglyph color formats"), this.target.anaglyphOptions, new GUILayoutOption[] {});
                }
                else
                {
                    if (this.target.format3D == (mode3D) 3) // interlace
                    {
                        this.target.interlaceRows = EditorGUILayout.IntSlider(new GUIContent("Rows", "Vertical resolution for interlace format"), (int) this.target.interlaceRows, 1, 1080, new GUILayoutOption[] {});
                    }
                    else
                    {
                        if (this.target.format3D == (mode3D) 4) // checkerboard
                        {
                            this.target.checkerboardColumns = EditorGUILayout.IntSlider(new GUIContent("Columns", "Horizontal resolution for checkerboard format"), (int) this.target.checkerboardColumns, 1, 1920, new GUILayoutOption[] {});
                            this.target.checkerboardRows = EditorGUILayout.IntSlider(new GUIContent("Rows", "Vertical resolution for checkerboard format"), (int) this.target.checkerboardRows, 1, 1080, new GUILayoutOption[] {});
                        }
                        else
                        {
                            if (this.target.format3D == (mode3D) 5) // scene screens
                            {
                                this.target.useStereoShader = false;
                                //this.target.sceneScreenL = EditorGUILayout.ObjectField(new GUIContent("Scene Screen L", " "), this.target.sceneScreenL, typeof(RenderTexture), allowSceneObjects, new GUILayoutOption[] {});
                                //this.target.sceneScreenR = EditorGUILayout.ObjectField(new GUIContent("Scene Screen R", " "), this.target.sceneScreenR, typeof(RenderTexture), allowSceneObjects, new GUILayoutOption[] {});
                            }
                        }
                    }
                }
            }
            if (this.target.useStereoShader != null)
            {
                this.target.stereoMaterial = (Material) EditorGUILayout.ObjectField(new GUIContent("Stereo Material", "Assign stereoMat material (included)."), this.target.stereoMaterial, typeof(Material), allowSceneObjects, new GUILayoutOption[] {});
            }
            this.target.depthPlane = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Depth Plane", "Assign Depth Plane Prefab"), this.target.depthPlane, typeof(GameObject), allowSceneObjects, new GUILayoutOption[] {});
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {});
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        /*
			*** This section can be uncommented to expose various esoteric settings that are only needed for specialized applications ***
			foldout3 = EditorGUILayout.Foldout(foldout3, GUIContent("Advanced Options","Customize render for special fx")); 
			if (foldout3) {
	      		EditorGUILayout.BeginVertical("box");
 				target.offAxisFrustum = EditorGUILayout.Slider (GUIContent("Off Axis Frustum", "assymetrical frustum (default 0)"),target.offAxisFrustum, -10, 10);

				target.useCustomStereoLayer = EditorGUILayout.Toggle(GUIContent("Use Custom Stereo Layer","Set a custom layer to use multiple stereo cameras."), target.useCustomStereoLayer);  
	 			if (target.useCustomStereoLayer) {
	 				EditorGUI.indentLevel = 1;
	 				target.stereoLayer = EditorGUILayout.LayerField(GUIContent("Stereo Layer:","Camera will render this layer only."), target.stereoLayer);
	 				target.renderOrderDepth = EditorGUILayout.IntField(GUIContent("Render Order Depth:","For multiple stereo cameras. Higher layers are rendered on top of lower layers."), target.renderOrderDepth);  
	 				EditorGUI.indentLevel = 0;
	 			} else {
					target.stereoLayer = 0;
	 			}
	 			target.useLeftRightOnlyLayers = EditorGUILayout.Toggle(GUIContent("Use Left Right Only Layers","Enable layers seen by only one camera.") , target.useLeftRightOnlyLayers);  
		 		if (target.useLeftRightOnlyLayers) {
		 			EditorGUI.indentLevel = 1;
		 			target.leftOnlyLayer = EditorGUILayout.LayerField(GUIContent("Left Only Layer:","Layer seen by left camera only."), target.leftOnlyLayer);
		 			target.rightOnlyLayer = EditorGUILayout.LayerField(GUIContent("Right Only Layer:","Layer seen by right camera only."), target.rightOnlyLayer);  
		 			target.guiOnlyLayer = EditorGUILayout.LayerField(GUIContent("Gui Only Layer:","Layer seen by center camera (guiCam) only."), target.guiOnlyLayer);  
		 			EditorGUI.indentLevel = 0;
		 		} else {
		 			target.leftOnlyLayer = 20;
		 			target.rightOnlyLayer = 21;
		 			target.maskOnlyLayer = 22;
		 		}
		 		EditorGUILayout.EndVertical();
			}*/
        if (GUI.changed)
        {
            EditorUtility.SetDirty(this.target);
        }
    }

    static s3dCameraEditor()
    {
        s3dCameraEditor.foldout1 = true;
        s3dCameraEditor.foldout2 = true;
        s3dCameraEditor.foldout3 = true;
    }

}