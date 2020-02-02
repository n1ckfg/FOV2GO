using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* s3dCamera.js revised 12/30/12. Usage:
 * Attach to camera. Creates, manages and renders stereoscopic view.
 * NOTE: interaxial is measured in millimeters; zeroPrlxDist is measured in meters 
 * Has companion Editor script to create custom inspector 
 */
// 1. Camera
 // left view camera
 // right view camera
 // black mask for mobile formats
 // mask for gui overlay for mobile formats
// Stereo Parameters
 // Distance (in millimeters) between cameras
 // Distance (in meters) at which left and right images overlap exactly
// 3D Camera Configuration // 
 // Angle cameras inward to converge. Bad idea!
// Camera Selection // 
//enum cams3D {Left_Right, Left_Only, Right_Only, Right_Left} // declared in s3dEnums.js
 // View order - swap cameras for cross-eyed free-viewing
//private var screenSize : Vector2;
// Options
 // Set a custom layer to use multiple stereo cameras
 // Camera will render this layer only
 // Enable layers seen by only one camera
 // Layer seen by left camera only
 // Layer seen by right camera only
 // Layer seen by gui camera only
 // For multiple stereo cameras - higher layers are rendered on top of lower layers
// 2. Render
// enable useStereoShader to use RenderTextures & stereo shader (Unity Pro only) for desktop applications - allows anaglyph format
// turn off for Unity Free, Android and iOS (allows side by side mode only)
 // track changes to useStereoShader
// Stereo Material + Stereo Shader (uses FOV2GO/Shaders/stereo3DViewMethods)
 // Assign FOV2GO/Materials/stereoMat material in inspector
// Render Textures
// 3D Display Mode // 
//enum mode3D {SideBySide, Anaglyph, OverUnder, Interlace, Checkerboard}; // declared in s3dEnums.js
// Anaglyph Mode
//enum anaType {Monochrome, HalfColor, FullColor, Optimized, Purple};  // declared in s3dEnums.js
// Side by Side Mode
// Over Under Mode
// Interlace Variables
[UnityEngine.AddComponentMenu("Stereoskopix/s3d Camera")]
[UnityEngine.ExecuteInEditMode]
public partial class s3dCamera : MonoBehaviour
{
    public GameObject leftCam;
    public GameObject rightCam;
    public GameObject maskCam;
    public GameObject guiCam;
    public float interaxial;
    public float zeroPrlxDist;
    public bool toedIn;
    public cams3D cameraSelect;
    public float H_I_T;
    public float offAxisFrustum;
    public GameObject depthPlane;
    public GameObject planeNear;
    public GameObject planeZero;
    public GameObject planeFar;
    public float horizontalFOV;
    public float verticalFOV;
    public bool useCustomStereoLayer;
    public int stereoLayer;
    public bool useLeftRightOnlyLayers;
    public int leftOnlyLayer;
    public int rightOnlyLayer;
    public int guiOnlyLayer;
    public int renderOrderDepth;
    public bool useStereoShader;
    private bool useStereoShaderPrev;
    public Material stereoMaterial;
    private RenderTexture leftCamRT;
    private RenderTexture rightCamRT;
    public mode3D format3D;
    public anaType anaglyphOptions;
    public bool sideBySideSqueezed;
    public bool overUnderStretched;
    public bool usePhoneMask;
    public Vector4 leftViewRect;
    public Vector4 rightViewRect;
    public int interlaceRows;
    public int checkerboardColumns;
    public int checkerboardRows;
    public Plane[] planes;
    private bool initialized;
    public virtual void Awake()
    {
        this.initStereoCamera();
    }

    public virtual void initStereoCamera()
    {
        this.SetupCameras();
        this.SetupShader();
        this.SetStereoFormat();
        s3dDepthInfo infoScript = null;
        infoScript = (s3dDepthInfo) this.GetComponent(typeof(s3dDepthInfo));
        if (infoScript)
        {
            this.SetupScreenPlanes(); // only create screen planes if necessary
        }
    }

    public virtual void SetupCameras()
    {
        Transform lcam = this.transform.Find("leftCam"); // check if we've already created a leftCam
        if (lcam)
        {
            this.leftCam = lcam.gameObject;
            this.leftCam.GetComponent<Camera>().CopyFrom(this.GetComponent<Camera>());
        }
        else
        {
            this.leftCam = new GameObject("leftCam", new System.Type[] {typeof(Camera)});
            this.leftCam.AddComponent(typeof(GUILayer));
            this.leftCam.GetComponent<Camera>().CopyFrom(this.GetComponent<Camera>());
            this.leftCam.transform.parent = this.transform;
        }
        Transform rcam = this.transform.Find("rightCam"); // check if we've already created a rightCam
        if (rcam)
        {
            this.rightCam = rcam.gameObject;
            this.rightCam.GetComponent<Camera>().CopyFrom(this.GetComponent<Camera>());
        }
        else
        {
            this.rightCam = new GameObject("rightCam", new System.Type[] {typeof(Camera)});
            this.rightCam.AddComponent(typeof(GUILayer));
            this.rightCam.GetComponent<Camera>().CopyFrom(this.GetComponent<Camera>());
            this.rightCam.transform.parent = this.transform;
        }
        Transform mcam = this.transform.Find("maskCam"); // check if we've already created a maskCam
        if (mcam)
        {
            this.maskCam = mcam.gameObject;
        }
        else
        {
            this.maskCam = new GameObject("maskCam", new System.Type[] {typeof(Camera)});
            this.maskCam.AddComponent(typeof(GUILayer));
            this.maskCam.GetComponent<Camera>().CopyFrom(this.GetComponent<Camera>());
            this.maskCam.transform.parent = this.transform;
        }
        Transform gcam = this.transform.Find("guiCam"); // check if we've already created a maskCam
        if (gcam)
        {
            this.guiCam = gcam.gameObject;
        }
        else
        {
            this.guiCam = new GameObject("guiCam", new System.Type[] {typeof(Camera)});
            this.guiCam.AddComponent(typeof(GUILayer));
            this.guiCam.GetComponent<Camera>().CopyFrom(this.GetComponent<Camera>());
            this.guiCam.transform.parent = this.transform;
        }
        GUILayer guiC = (GUILayer) this.GetComponent(typeof(GUILayer));
        guiC.enabled = false;
        this.GetComponent<Camera>().depth = -2; // rendering order (back to front): centerCam/maskCam/leftCam1/rightCam1/leftCam2/rightCam2/ etc
        this.horizontalFOV = (2 * Mathf.Atan(Mathf.Tan((this.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad) / 2) * this.GetComponent<Camera>().aspect)) * Mathf.Rad2Deg;
        this.verticalFOV = this.GetComponent<Camera>().fieldOfView;
        this.leftCam.GetComponent<Camera>().depth = (this.GetComponent<Camera>().depth + (this.renderOrderDepth * 2)) + 2;
        this.rightCam.GetComponent<Camera>().depth = (this.GetComponent<Camera>().depth + ((this.renderOrderDepth * 2) + 1)) + 3;
        if (this.useLeftRightOnlyLayers)
        {
            if (this.useCustomStereoLayer)
            {
                this.leftCam.GetComponent<Camera>().cullingMask = (1 << this.stereoLayer) | (1 << this.leftOnlyLayer); // show stereo + left only
                this.rightCam.GetComponent<Camera>().cullingMask = (1 << this.stereoLayer) | (1 << this.rightOnlyLayer); // show stereo + right only
            }
            else
            {
                this.leftCam.GetComponent<Camera>().cullingMask = ~((1 << this.rightOnlyLayer) | (1 << this.guiOnlyLayer)); // show everything but right only layer & mask only layer
                this.rightCam.GetComponent<Camera>().cullingMask = ~((1 << this.leftOnlyLayer) | (1 << this.guiOnlyLayer)); // show everything but left only layer & mask only layer
            }
        }
        else
        {
            if (this.useCustomStereoLayer)
            {
                this.leftCam.GetComponent<Camera>().cullingMask = 1 << this.stereoLayer; // show stereo layer only
                this.rightCam.GetComponent<Camera>().cullingMask = 1 << this.stereoLayer; // show stereo layer only
            }
        }
        this.maskCam.GetComponent<Camera>().depth = this.leftCam.GetComponent<Camera>().depth - 1;
        this.guiCam.GetComponent<Camera>().depth = this.rightCam.GetComponent<Camera>().depth + 1;
        this.maskCam.GetComponent<Camera>().cullingMask = 0;
        this.guiCam.GetComponent<Camera>().cullingMask = 1 << this.guiOnlyLayer; // only show what's in the guiOnly layer
        this.maskCam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        this.guiCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
        this.maskCam.GetComponent<Camera>().backgroundColor = Color.black;
    }

    public virtual void SetupShader()
    {
        if (this.useStereoShader)
        {
            if (!this.leftCamRT || !this.rightCamRT)
            {
                this.leftCamRT = new RenderTexture(Screen.width, Screen.height, 24);
                this.rightCamRT = new RenderTexture(Screen.width, Screen.height, 24);
            }
            this.stereoMaterial.SetTexture("_LeftTex", this.leftCamRT);
            this.stereoMaterial.SetTexture("_RightTex", this.rightCamRT);
            this.leftCam.GetComponent<Camera>().targetTexture = this.leftCamRT;
            this.rightCam.GetComponent<Camera>().targetTexture = this.rightCamRT;
        }
        else
        {
            if (this.format3D == mode3D.SideBySide)
            {
                if (!this.usePhoneMask)
                {
                    this.leftCam.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1);
                    this.rightCam.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1);
                }
                else
                {
                    this.leftCam.GetComponent<Camera>().rect = this.Vector4toRect(this.leftViewRect);
                    this.rightCam.GetComponent<Camera>().rect = this.Vector4toRect(this.rightViewRect);
                }
                this.leftViewRect = this.RectToVector4(this.leftCam.GetComponent<Camera>().rect);
                this.rightViewRect = this.RectToVector4(this.rightCam.GetComponent<Camera>().rect);
            }
            else
            {
                if (this.format3D == mode3D.OverUnder)
                {
                    if (!this.usePhoneMask)
                    {
                        this.leftCam.GetComponent<Camera>().rect = new Rect(0, 0.5f, 1, 0.5f);
                        this.rightCam.GetComponent<Camera>().rect = new Rect(0, 0, 1, 0.5f);
                    }
                    else
                    {
                        this.leftCam.GetComponent<Camera>().rect = this.Vector4toRect(this.leftViewRect);
                        this.rightCam.GetComponent<Camera>().rect = this.Vector4toRect(this.rightViewRect);
                    }
                    this.leftViewRect = this.RectToVector4(this.leftCam.GetComponent<Camera>().rect);
                    this.rightViewRect = this.RectToVector4(this.rightCam.GetComponent<Camera>().rect);
                }
                else
                {
                    MonoBehaviour.print("Unity Free only supports Side-by-Side and Over-Under modes!");
                }
            }
            this.fixCameraAspect();
        }
    }

    public virtual void SetStereoFormat()
    {
        switch (this.format3D)
        {
            case mode3D.SideBySide:
                if (this.useStereoShader)
                {
                    this.maskCam.GetComponent<Camera>().enabled = false;
                }
                else
                {
                    this.maskCam.GetComponent<Camera>().enabled = this.usePhoneMask;
                }
                break;
            case mode3D.Anaglyph:
                this.maskCam.GetComponent<Camera>().enabled = false;
                this.SetAnaType();
                break;
            case mode3D.OverUnder:
                if (this.useStereoShader)
                {
                    this.maskCam.GetComponent<Camera>().enabled = false;
                }
                else
                {
                    this.maskCam.GetComponent<Camera>().enabled = this.usePhoneMask;
                }
                break;
            case mode3D.Interlace:
                this.maskCam.GetComponent<Camera>().enabled = false;
                this.SetWeave(false);
                break;
            case mode3D.Checkerboard:
                this.maskCam.GetComponent<Camera>().enabled = false;
                this.SetWeave(true);
                break;
        }
    }

    public virtual void SetupScreenPlanes()
    {
        Transform screenTest = this.transform.Find("depthPlanes");
        if (this.depthPlane) // first make sure that user has assigned a depthPlane prefab
        {
            if (!screenTest)
            {
                this.planeZero = UnityEngine.Object.Instantiate(this.depthPlane, this.transform.position, this.transform.rotation);
                GameObject depthPlanes = new GameObject("depthPlanes");
                depthPlanes.transform.parent = this.transform;
                depthPlanes.transform.localPosition = Vector3.zero;
                this.planeZero.transform.parent = depthPlanes.transform;
                this.planeZero.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                this.planeZero.name = "screenDistPlane";
                this.planeNear = UnityEngine.Object.Instantiate(this.depthPlane, this.transform.position, this.transform.rotation);
                this.planeNear.transform.parent = depthPlanes.transform;
                this.planeNear.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                this.planeNear.name = "nearDistPlane";
                Shader shader1 = Shader.Find("Particles/Additive");
                this.planeNear.GetComponent<Renderer>().sharedMaterial = new Material(shader1);
                this.planeNear.GetComponent<Renderer>().sharedMaterial.mainTexture = this.depthPlane.GetComponent<Renderer>().sharedMaterial.mainTexture;
                this.planeNear.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", Color.yellow);
                this.planeFar = UnityEngine.Object.Instantiate(this.depthPlane, this.transform.position, this.transform.rotation);
                this.planeFar.transform.parent = depthPlanes.transform;
                this.planeFar.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                this.planeFar.name = "farDistPlane";
                Shader shader2 = Shader.Find("Particles/Additive");
                this.planeFar.GetComponent<Renderer>().sharedMaterial = new Material(shader2);
                this.planeFar.GetComponent<Renderer>().sharedMaterial.mainTexture = this.depthPlane.GetComponent<Renderer>().sharedMaterial.mainTexture;
                this.planeFar.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", Color.green);
            }
            else
            {
                this.planeZero = GameObject.Find("screenDistPlane");
                this.planeNear = GameObject.Find("nearDistPlane");
                this.planeFar = GameObject.Find("farDistPlane");
            }
            this.planeZero.GetComponent<Renderer>().enabled = false;
            this.planeNear.GetComponent<Renderer>().enabled = false;
            this.planeFar.GetComponent<Renderer>().enabled = false;
        }
    }

    // called from initStereoCamera (above), and from s3dGyroCam.js (when phone orientation is changed due to AutoRotation)
    public virtual void fixCameraAspect()
    {
        this.GetComponent<Camera>().ResetAspect();
        //yield WaitForSeconds(0.25);
        this.GetComponent<Camera>().aspect = this.GetComponent<Camera>().aspect * ((this.leftCam.GetComponent<Camera>().rect.width * 2) / this.leftCam.GetComponent<Camera>().rect.height);
        this.leftCam.GetComponent<Camera>().aspect = this.GetComponent<Camera>().aspect;
        this.rightCam.GetComponent<Camera>().aspect = this.GetComponent<Camera>().aspect;
    }

    public virtual Rect Vector4toRect(Vector4 v)
    {
        Rect r = new Rect(v.x, v.y, v.z, v.w);
        return r;
    }

    public virtual Vector4 RectToVector4(Rect r)
    {
        Vector4 v = new Vector4(r.x, r.y, r.width, r.height);
        return v;
    }

    public virtual void Update()
    {
        if (!this.useStereoShader)
        {
            if (UnityEditor.EditorApplication.isPlaying) // speeds up rendering while in play mode, but doesn't work if useStereoShader is true
            {
                this.GetComponent<Camera>().enabled = false;
            }
            else
            {
                this.GetComponent<Camera>().enabled = true; // need camera enabled when in edit mode
            }
        }
        if (this.useStereoShader)
        {
            if (this.useStereoShaderPrev == false)
            {
                this.initStereoCamera();
            }
        }
        else
        {
            if (this.useStereoShaderPrev == true)
            {
                this.releaseRenderTextures();
                this.SetupShader();
                this.SetStereoFormat();
            }
        }
        this.useStereoShaderPrev = this.useStereoShader;
        this.planes = GeometryUtility.CalculateFrustumPlanes(this.GetComponent<Camera>());
        if (Application.isPlaying)
        {
            if (!this.initialized)
            {
                this.initialized = true;
            }
        }
        else
        {
            this.initialized = false;
            this.SetupShader();
            this.SetStereoFormat();
        }
        this.UpdateView();
    }

    public virtual void UpdateView()
    {
        switch (this.cameraSelect)
        {
            case cams3D.Left_Right:
                this.leftCam.transform.position = this.transform.position + this.transform.TransformDirection(-this.interaxial / 2000f, 0, 0);
                this.rightCam.transform.position = this.transform.position + this.transform.TransformDirection(this.interaxial / 2000f, 0, 0);
                break;
            case cams3D.Left_Only:
                this.leftCam.transform.position = this.transform.position + this.transform.TransformDirection(-this.interaxial / 2000f, 0, 0);
                this.rightCam.transform.position = this.transform.position + this.transform.TransformDirection(-this.interaxial / 2000f, 0, 0);
                break;
            case cams3D.Right_Only:
                this.leftCam.transform.position = this.transform.position + this.transform.TransformDirection(this.interaxial / 2000f, 0, 0);
                this.rightCam.transform.position = this.transform.position + this.transform.TransformDirection(this.interaxial / 2000f, 0, 0);
                break;
            case cams3D.Right_Left:
                this.leftCam.transform.position = this.transform.position + this.transform.TransformDirection(this.interaxial / 2000f, 0, 0);
                this.rightCam.transform.position = this.transform.position + this.transform.TransformDirection(-this.interaxial / 2000f, 0, 0);
                break;
        }
        if (this.toedIn)
        {
            this.leftCam.GetComponent<Camera>().projectionMatrix = this.GetComponent<Camera>().projectionMatrix;
            this.rightCam.GetComponent<Camera>().projectionMatrix = this.GetComponent<Camera>().projectionMatrix;
            this.leftCam.transform.LookAt(this.transform.position + (this.transform.TransformDirection(Vector3.forward) * this.zeroPrlxDist));
            this.rightCam.transform.LookAt(this.transform.position + (this.transform.TransformDirection(Vector3.forward) * this.zeroPrlxDist));
        }
        else
        {
            this.leftCam.transform.rotation = this.transform.rotation;
            this.rightCam.transform.rotation = this.transform.rotation;
            switch (this.cameraSelect)
            {
                case cams3D.Left_Right:
                    this.leftCam.GetComponent<Camera>().projectionMatrix = this.setProjectionMatrix(true);
                    this.rightCam.GetComponent<Camera>().projectionMatrix = this.setProjectionMatrix(false);
                    break;
                case cams3D.Left_Only:
                    this.leftCam.GetComponent<Camera>().projectionMatrix = this.setProjectionMatrix(true);
                    this.rightCam.GetComponent<Camera>().projectionMatrix = this.setProjectionMatrix(true);
                    break;
                case cams3D.Right_Only:
                    this.leftCam.GetComponent<Camera>().projectionMatrix = this.setProjectionMatrix(false);
                    this.rightCam.GetComponent<Camera>().projectionMatrix = this.setProjectionMatrix(false);
                    break;
                case cams3D.Right_Left:
                    this.leftCam.GetComponent<Camera>().projectionMatrix = this.setProjectionMatrix(false);
                    this.rightCam.GetComponent<Camera>().projectionMatrix = this.setProjectionMatrix(true);
                    break;
            }
        }
    }

    // Calculate Stereo Projection Matrix
    public virtual Matrix4x4 setProjectionMatrix(bool isLeftCam)
    {
        float left = 0.0f;
        float right = 0.0f;
        float a = 0.0f;
        float b = 0.0f;
        float FOVrad = 0.0f;
        float tempAspect = this.GetComponent<Camera>().aspect;
        FOVrad = (this.GetComponent<Camera>().fieldOfView / 180f) * Mathf.PI;
        if (this.format3D == mode3D.SideBySide)
        {
            if (!this.sideBySideSqueezed)
            {
                tempAspect = tempAspect / 2; // if side by side unsqueezed, double width
            }
        }
        else
        {
            if (this.format3D == mode3D.OverUnder)
            {
                if (this.overUnderStretched)
                {
                    tempAspect = tempAspect / 4;
                }
                else
                {
                    tempAspect = tempAspect / 2;
                }
            }
        }
        a = this.GetComponent<Camera>().nearClipPlane * Mathf.Tan(FOVrad * 0.5f);
        b = this.GetComponent<Camera>().nearClipPlane / this.zeroPrlxDist;
        if (isLeftCam)
        {
            left = (((-tempAspect * a) + ((this.interaxial / 2000f) * b)) + (this.H_I_T / 100)) + (this.offAxisFrustum / 100);
            right = (((tempAspect * a) + ((this.interaxial / 2000f) * b)) + (this.H_I_T / 100)) + (this.offAxisFrustum / 100);
        }
        else
        {
            left = (((-tempAspect * a) - ((this.interaxial / 2000f) * b)) - (this.H_I_T / 100)) + (this.offAxisFrustum / 100);
            right = (((tempAspect * a) - ((this.interaxial / 2000f) * b)) - (this.H_I_T / 100)) + (this.offAxisFrustum / 100);
        }
        return this.PerspectiveOffCenter(left, right, -a, a, this.GetComponent<Camera>().nearClipPlane, this.GetComponent<Camera>().farClipPlane);
    }

    public virtual Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        Matrix4x4 m = default(Matrix4x4);
        float x = (2f * near) / (right - left);
        float y = (2f * near) / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -((2f * far) * near) / (far - near);
        float e = -1f;
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }

    public virtual void releaseRenderTextures()
    {
        this.leftCam.GetComponent<Camera>().targetTexture = null;
        this.rightCam.GetComponent<Camera>().targetTexture = null;
        this.leftCamRT.Release();
        this.rightCamRT.Release();
    }

    // Draw Scene Gizmos
    public virtual void OnDrawGizmos()//Gizmos.DrawWireCube (gizmoTarget, Vector3(screenSize.x,screenSize.y,0.01));
    {
        Vector3 gizmoLeft = this.transform.position + this.transform.TransformDirection(-this.interaxial / 2000f, 0, 0); // interaxial/2/1000mm
        Vector3 gizmoRight = this.transform.position + this.transform.TransformDirection(this.interaxial / 2000f, 0, 0);
        Vector3 gizmoTarget = this.transform.position + (this.transform.TransformDirection(Vector3.forward) * this.zeroPrlxDist);
        Gizmos.color = new Color(1, 1, 1, 1);
        Gizmos.DrawLine(gizmoLeft, gizmoTarget);
        Gizmos.DrawLine(gizmoRight, gizmoTarget);
        Gizmos.DrawLine(gizmoLeft, gizmoRight);
        Gizmos.DrawSphere(gizmoLeft, 0.02f);
        Gizmos.DrawSphere(gizmoRight, 0.02f);
        Gizmos.DrawSphere(gizmoTarget, 0.02f);
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (this.useStereoShader)
        {
            RenderTexture.active = destination;
            GL.PushMatrix();
            GL.LoadOrtho();
            switch (this.format3D)
            {
                case mode3D.Anaglyph:
                    this.stereoMaterial.SetPass(0);
                    this.DrawQuad(0);
                    break;
                case mode3D.SideBySide:
                case mode3D.OverUnder:
                    int i = 1;
                    while (i <= 2)
                    {
                        this.stereoMaterial.SetPass(i);
                        this.DrawQuad(i);
                        i++;
                    }
                    break;
                case mode3D.Interlace:
                case mode3D.Checkerboard:
                    this.stereoMaterial.SetPass(3);
                    this.DrawQuad(3);
                    break;
                default:
                    break;
            }
            GL.PopMatrix();
        }
    }

    // Interlace & Checkerboard Modes
    public virtual void SetWeave(object xy)
    {
        if (xy != null)
        {
            this.stereoMaterial.SetFloat("_Weave_X", this.checkerboardColumns);
            this.stereoMaterial.SetFloat("_Weave_Y", this.checkerboardRows);
        }
        else
        {
            this.stereoMaterial.SetFloat("_Weave_X", 1);
            this.stereoMaterial.SetFloat("_Weave_Y", this.interlaceRows);
        }
    }

    // Anaglyph Mode
    public virtual void SetAnaType()
    {
        switch (this.anaglyphOptions)
        {
            case anaType.Monochrome:
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0.299f, 0.587f, 0.114f, 0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0.299f, 0.587f, 0.114f, 0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0.299f, 0.587f, 0.114f, 0));
                break;
            case anaType.HalfColor:
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0.299f, 0.587f, 0.114f, 0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0, 1, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0, 0, 1, 0));
                break;
            case anaType.FullColor:
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4(1, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0, 1, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0, 0, 1, 0));
                break;
            case anaType.Optimized:
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0, 0.7f, 0.3f, 0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0, 1, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0, 0, 1, 0));
                break;
            case anaType.Purple:
                this.stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0.299f, 0.587f, 0.114f, 0));
                this.stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0, 0, 0, 0));
                this.stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0.299f, 0.587f, 0.114f, 0));
                break;
        }
    }

    // Draw Render Textures Quads
    public virtual void DrawQuad(int cam)
    {
        if (this.format3D == mode3D.Anaglyph)
        {
            GL.Begin(GL.QUADS);
            GL.TexCoord3(0f, 0f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.TexCoord3(1f, 0f, 0f);
            GL.Vertex3(1f, 0f, 0f);
            GL.TexCoord3(1f, 1f, 0f);
            GL.Vertex3(1f, 1f, 0f);
            GL.TexCoord3(0f, 1f, 0f);
            GL.Vertex3(0f, 1f, 0f);
            GL.End();
        }
        else
        {
            if (this.format3D == mode3D.SideBySide)
            {
                if (cam == 1)
                {
                    GL.Begin(GL.QUADS);
                    GL.TexCoord2(0f, 0f);
                    GL.Vertex3(0f, 0f, 0.1f);
                    GL.TexCoord2(1f, 0f);
                    GL.Vertex3(0.5f, 0f, 0.1f);
                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(0.5f, 1f, 0.1f);
                    GL.TexCoord2(0f, 1f);
                    GL.Vertex3(0f, 1f, 0.1f);
                    GL.End();
                }
                else
                {
                    GL.Begin(GL.QUADS);
                    GL.TexCoord2(0f, 0f);
                    GL.Vertex3(0.5f, 0f, 0.1f);
                    GL.TexCoord2(1f, 0f);
                    GL.Vertex3(1f, 0f, 0.1f);
                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(1f, 1f, 0.1f);
                    GL.TexCoord2(0f, 1f);
                    GL.Vertex3(0.5f, 1f, 0.1f);
                    GL.End();
                }
            }
            else
            {
                if (this.format3D == mode3D.OverUnder)
                {
                    if (cam == 1)
                    {
                        GL.Begin(GL.QUADS);
                        GL.TexCoord2(0f, 0f);
                        GL.Vertex3(0f, 0.5f, 0.1f);
                        GL.TexCoord2(1f, 0f);
                        GL.Vertex3(1f, 0.5f, 0.1f);
                        GL.TexCoord2(1f, 1f);
                        GL.Vertex3(1f, 1f, 0.1f);
                        GL.TexCoord2(0f, 1f);
                        GL.Vertex3(0f, 1f, 0.1f);
                        GL.End();
                    }
                    else
                    {
                        GL.Begin(GL.QUADS);
                        GL.TexCoord2(0f, 0f);
                        GL.Vertex3(0f, 0f, 0.1f);
                        GL.TexCoord2(1f, 0f);
                        GL.Vertex3(1f, 0f, 0.1f);
                        GL.TexCoord2(1f, 1f);
                        GL.Vertex3(1f, 0.5f, 0.1f);
                        GL.TexCoord2(0f, 1f);
                        GL.Vertex3(0f, 0.5f, 0.1f);
                        GL.End();
                    }
                }
                else
                {
                    if ((this.format3D == mode3D.Interlace) || (this.format3D == mode3D.Checkerboard))
                    {
                        GL.Begin(GL.QUADS);
                        GL.TexCoord2(0f, 0f);
                        GL.Vertex3(0f, 0f, 0.1f);
                        GL.TexCoord2(1f, 0f);
                        GL.Vertex3(1, 0f, 0.1f);
                        GL.TexCoord2(1f, 1f);
                        GL.Vertex3(1, 1f, 0.1f);
                        GL.TexCoord2(0f, 1f);
                        GL.Vertex3(0f, 1f, 0.1f);
                        GL.End();
                    }
                }
            }
        }
    }

    public s3dCamera()
    {
        this.interaxial = 65;
        this.zeroPrlxDist = 3f;
        this.cameraSelect = cams3D.Left_Right;
        this.useLeftRightOnlyLayers = true;
        this.leftOnlyLayer = 20;
        this.rightOnlyLayer = 21;
        this.guiOnlyLayer = 22;
        this.format3D = mode3D.SideBySide;
        this.anaglyphOptions = anaType.HalfColor;
        this.usePhoneMask = true;
        this.leftViewRect = new Vector4(0, 0, 0.5f, 1);
        this.rightViewRect = new Vector4(0.5f, 0, 1, 1);
        this.interlaceRows = 1080;
        this.checkerboardColumns = 1920;
        this.checkerboardRows = 1080;
    }

}