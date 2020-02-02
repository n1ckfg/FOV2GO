using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d GUI Texture Script revised 12.30.12
 * Usage: Attach to a GUI Texture.
 * Creates left and right copies of GUITexture for stereoscopic view. 
 * Can adjust GUI Texture parallax automatically to keep the GUITexture closer than anything it occludes.
 * GUITextures are placed in leftOnlyLayer and rightOnlyLayer (layers are set in s3dCamera.js).
 * Assumes that the GUI Texture Pixel Inset is set to the default (center) position, and that the
   x and y position (each set between 0.0 and 1.0) are used to place the GUITexture.
 * Dependencies: 
 * s3dCamera.js (on main camera) with useLeftRightOnlyLayers boolean set to TRUE (default)  
 */
// *** Z Depth ***
// set initial GUIElement distance from camera
// keep this GUIElement closer than anything behind it
// make GUIElement this a bit closer than nearest object or object under mouse
// minimum distance for this GUIELement
// maximum distance for this GUIElement, no matter what's behind it
// *** Display & Movement ***
// start with guiText visible?
// timer to turn off texture if visible (0 = stays on forever)
// how gradually to change depth (bigger numbers are slower - more than 25 is very slow);
[UnityEngine.AddComponentMenu("Stereoskopix/s3d GUI Texture")]
public partial class s3dGuiTexture : MonoBehaviour
{
    public float objectDistance;
    public bool keepCloser;
    public float nearPadding;
    public float minimumDistance;
    public float maximumDistance;
    public bool beginVisible;
    public float timeToDisplay;
    public float lagTime;
    private s3dCamera camera3D;
    public GameObject objectCopyR;
    private float screenWidth;
    public Vector3 obPosition;
    private float scrnPrlx;
    private float curScrnPrlx;
    private Vector2[] checkpoints;
    private Vector2 corner;
    public bool on;
    private float unitWidth;
    private s3dGuiCursor s3dCursor;
    public virtual IEnumerator Start()
    {
        this.findS3dCamera();
        this.checkpoints = new Vector2[5];
        this.objectCopyR = UnityEngine.Object.Instantiate(this.gameObject, this.transform.position, this.transform.rotation);
        UnityEngine.Object.Destroy((s3dGuiTexture) this.objectCopyR.GetComponent(typeof(s3dGuiTexture)));
        this.objectCopyR.name = this.gameObject.name + "_R";
        this.gameObject.name = this.gameObject.name + "_L";
        this.gameObject.layer = this.camera3D.leftOnlyLayer;
        this.objectCopyR.layer = this.camera3D.rightOnlyLayer;
        this.obPosition = this.gameObject.transform.position;
        // if using stereo shader + side-by-side + not squeezed, double width of guiTexture
        if ((this.camera3D.useStereoShader && (this.camera3D.format3D == (mode3D) 0)) && !this.camera3D.sideBySideSqueezed)
        {
            float xWidth = this.GetComponent<GUITexture>().pixelInset.width * 2;

            {
                float _33 = xWidth;
                Rect _34 = this.gameObject.GetComponent<GUITexture>().pixelInset;
                _34.width = _33;
                this.gameObject.GetComponent<GUITexture>().pixelInset = _34;
            }

            {
                float _35 = xWidth;
                Rect _36 = this.objectCopyR.GetComponent<GUITexture>().pixelInset;
                _36.width = _35;
                this.objectCopyR.GetComponent<GUITexture>().pixelInset = _36;
            }
            float xInset = this.gameObject.GetComponent<GUITexture>().pixelInset.width / -2;

            {
                float _37 = xInset;
                Rect _38 = this.gameObject.GetComponent<GUITexture>().pixelInset;
                _38.x = _37;
                this.gameObject.GetComponent<GUITexture>().pixelInset = _38;
            }

            {
                float _39 = xInset;
                Rect _40 = this.objectCopyR.GetComponent<GUITexture>().pixelInset;
                _40.x = _39;
                this.objectCopyR.GetComponent<GUITexture>().pixelInset = _40;
            }
        }
        else
        {
            // if not using stereo shader + squeezed, halve width of guiTexture
            if (!this.camera3D.useStereoShader && this.camera3D.sideBySideSqueezed)
            {
                xWidth = this.gameObject.GetComponent<GUITexture>().pixelInset.width * 0.5f;

                {
                    float _41 = xWidth;
                    Rect _42 = this.gameObject.GetComponent<GUITexture>().pixelInset;
                    _42.width = _41;
                    this.gameObject.GetComponent<GUITexture>().pixelInset = _42;
                }

                {
                    float _43 = xWidth;
                    Rect _44 = this.objectCopyR.GetComponent<GUITexture>().pixelInset;
                    _44.width = _43;
                    this.objectCopyR.GetComponent<GUITexture>().pixelInset = _44;
                }
                xInset = this.gameObject.GetComponent<GUITexture>().pixelInset.width / -2;

                {
                    float _45 = xInset;
                    Rect _46 = this.gameObject.GetComponent<GUITexture>().pixelInset;
                    _46.x = _45;
                    this.gameObject.GetComponent<GUITexture>().pixelInset = _46;
                }

                {
                    float _47 = xInset;
                    Rect _48 = this.objectCopyR.GetComponent<GUITexture>().pixelInset;
                    _48.x = _47;
                    this.objectCopyR.GetComponent<GUITexture>().pixelInset = _48;
                }
            }
        }
        // find corner offset
        this.corner = new Vector2((this.gameObject.GetComponent<GUITexture>().pixelInset.width / 2) / Screen.width, (this.gameObject.GetComponent<GUITexture>().pixelInset.height / 2) / Screen.height);
        this.toggleVisible(this.beginVisible);
        float horizontalFOV = (2 * Mathf.Atan(Mathf.Tan((this.camera3D.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad) / 2) * this.camera3D.GetComponent<Camera>().aspect)) * Mathf.Rad2Deg;
        this.unitWidth = Mathf.Tan((horizontalFOV / 2) * Mathf.Deg2Rad); // need unit width to calculate cursor depth when there's a HIT (horizontal image transform)
        this.screenWidth = (this.unitWidth * this.camera3D.zeroPrlxDist) * 2;
        this.setScreenParallax();
        if (this.timeToDisplay != 0f)
        {
            yield return new WaitForSeconds(this.timeToDisplay);
            this.toggleVisible(false);
        }
        this.s3dCursor = (s3dGuiCursor) this.gameObject.GetComponent(typeof(s3dGuiCursor));
        if (this.s3dCursor)
        {
            this.s3dCursor.initialize();
        }
    }

    public virtual void findS3dCamera()
    {
        s3dCamera[] cameras3D = ((s3dCamera[]) UnityEngine.Object.FindObjectsOfType(typeof(s3dCamera))) as s3dCamera[];
        if (cameras3D.Length == 1)
        {
            this.camera3D = cameras3D[0];
        }
        else
        {
            if (cameras3D.Length > 1)
            {
                MonoBehaviour.print("There is more than one s3dCamera in this scene.");
            }
            else
            {
                MonoBehaviour.print("There is no s3dCamera in this scene.");
            }
        }
    }

    public virtual void Update()
    {
        if (this.on)
        {
            if (this.keepCloser)
            {
                this.findDistanceUnderObject();
            }
            if (this.curScrnPrlx != this.scrnPrlx)
            {
                this.curScrnPrlx = this.curScrnPrlx + ((this.scrnPrlx - this.curScrnPrlx) / (this.lagTime + 1));
            }
            this.gameObject.transform.position = new Vector3(this.obPosition.x + (this.curScrnPrlx / 2), this.obPosition.y, this.gameObject.transform.position.z);
            this.objectCopyR.transform.position = new Vector3(this.obPosition.x - (this.curScrnPrlx / 2), this.obPosition.y, this.objectCopyR.gameObject.transform.position.z);
        }
    }

    public virtual Vector2 matchMousePos()
    {
        Vector2 mousePos = Input.mousePosition;
        if (this.camera3D.format3D == (mode3D) 0) // side by side
        {
            mousePos.x = mousePos.x / (Screen.width / 2);
        }
        else
        {
            mousePos.x = mousePos.x / Screen.width;
        }
        mousePos.y = mousePos.y / Screen.height;
        return mousePos;
    }

    public virtual void findDistanceUnderObject()
    {
        Vector2 dPosition = default(Vector2);
        RaycastHit hit = default(RaycastHit);
        float nearDistance = Mathf.Infinity;
        s3dInteractor actScript = null;
        if ((this.camera3D.format3D == (mode3D) 0) && !this.camera3D.sideBySideSqueezed)
        {
            dPosition = new Vector2((this.obPosition.x / 2) + 0.25f, this.obPosition.y); // 0 = left, 0.5 = center, 1 = right
        }
        else
        {
            dPosition = this.obPosition;
        }
        this.checkpoints[0] = dPosition; // raycast against object center
        this.checkpoints[1] = dPosition + new Vector2(-this.corner.x, -this.corner.y); // raycast against object corners
        this.checkpoints[2] = dPosition + new Vector2(this.corner.x, -this.corner.y);
        this.checkpoints[3] = dPosition + new Vector2(this.corner.x, this.corner.y);
        this.checkpoints[4] = dPosition + new Vector2(-this.corner.x, this.corner.y);
        // raycast against all objects
        int cor = 0;
        while (cor < 5)
        {
            Ray ray = this.camera3D.GetComponent<Camera>().ViewportPointToRay(this.checkpoints[cor]);
            if (Physics.Raycast(ray, out hit, 100f))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, new Color(0, 1, 0, 1));
                Plane camPlane = new Plane(this.camera3D.GetComponent<Camera>().transform.forward, this.camera3D.GetComponent<Camera>().transform.position);
                Vector3 thePoint = ray.GetPoint(hit.distance);
                float currentDistance = camPlane.GetDistanceToPoint(thePoint);
                if (currentDistance < nearDistance)
                {
                    nearDistance = currentDistance;
                }
            }
            cor++;
        }
        if (nearDistance < Mathf.Infinity)
        {
            this.objectDistance = Mathf.Clamp(nearDistance, this.minimumDistance, this.maximumDistance);
        }
        this.objectDistance = Mathf.Max(this.objectDistance, this.camera3D.GetComponent<Camera>().nearClipPlane);
        this.setScreenParallax();
    }

    public virtual void setScreenParallax()
    {
        float obPrlx = ((this.camera3D.interaxial / 1000) * (this.camera3D.zeroPrlxDist - this.objectDistance)) / this.objectDistance;
        if ((this.camera3D.format3D == (mode3D) 0) && !this.camera3D.sideBySideSqueezed)
        {
            this.scrnPrlx = (((obPrlx / this.screenWidth) * 2) + (this.nearPadding / 1000)) - (this.camera3D.H_I_T / (this.unitWidth * 15)); // why 15? no idea.
        }
        else
        {
            this.scrnPrlx = (((obPrlx / this.screenWidth) * 1) + (this.nearPadding / 1000)) - (this.camera3D.H_I_T / (this.unitWidth * 15));
        }
    }

    public virtual void toggleVisible(bool t)
    {
        this.on = t;
    }

    public s3dGuiTexture()
    {
        this.objectDistance = 1f;
        this.keepCloser = true;
        this.minimumDistance = 0.01f;
        this.maximumDistance = 100f;
        this.beginVisible = true;
    }

}