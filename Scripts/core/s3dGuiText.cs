using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d GUI Text Script revised 12.30.12
 * Usage: Attach to GUIText
 * Creates left and right copies of GUIText for stereoscopic view
 * Can adjust GUIText parallax automatically to keep the GUIText closer than anything it occludes.
 * Assumes that the GUIText Pixel Offset is set to the default (0,0) position, and that the
 * x and y position (each set between 0.0 and 1.0) are used to place the GUIText.
 * Dependencies: requires one s3dCamera.js with useLeftRightOnlyLayers boolean set to TRUE (default) 
 */
// should GUI Text track mouse position?
// should GUI Text track mouse only when down?
// set initial GUIElement distance from camera
// keep this GUIElement closer than anything behind it
// make GUIElement a bit closer than nearest object or object under mouse
// minimum distance for this GUIELement
// maximum distance for this GUIElement, no matter what's behind it
// how gradually to change depth (bigger numbers are slower - more than 25 is very slow);
// start with guiText visible;
// timer to turn off text if visible (0 to leave on)
// string to begin with
// text color
// shadows?
// shadowColor
// shadowOffset
[UnityEngine.AddComponentMenu("Stereoskopix/s3d GUI Text")]
public partial class s3dGuiText : MonoBehaviour
{
    public bool trackMouseXYPosition;
    public bool onlyWhenMouseDown;
    public float objectDistance;
    public bool keepCloser;
    public float nearPadding;
    public float minimumDistance;
    public float maximumDistance;
    public float lagTime;
    public bool beginVisible;
    public float timeToDisplay;
    public string initString;
    public Color TextColor;
    public bool shadowsOn;
    public Color ShadowColor;
    public float shadowOffset;
    private s3dCamera camera3D;
    private GameObject objectCopyR;
    private GameObject shadowL;
    private GameObject shadowR;
    private float screenWidth;
    public Vector3 obPosition;
    private float scrnPrlx;
    private float curScrnPrlx;
    private object[] rays;
    private Vector2[] corners;
    private Vector2 corner;
    private bool textOn;
    private float unitWidth;
    public virtual IEnumerator Start()//toggleVisible(false);
    {
        this.findS3dCamera();
        this.corners = new Vector2[4];
        this.objectCopyR = UnityEngine.Object.Instantiate(this.gameObject, this.transform.position, this.transform.rotation);
        UnityEngine.Object.Destroy((s3dGuiText) this.objectCopyR.GetComponent(typeof(s3dGuiText)));
        this.objectCopyR.name = this.gameObject.name + "_R";
        this.objectCopyR.transform.parent = this.gameObject.transform.parent;
        this.gameObject.name = this.gameObject.name + "_L";
        this.gameObject.layer = this.camera3D.leftOnlyLayer;
        this.gameObject.GetComponent<GUIText>().material.color = this.TextColor;
        this.objectCopyR.layer = this.camera3D.rightOnlyLayer;
        this.objectCopyR.gameObject.GetComponent<GUIText>().material.color = this.TextColor;
        if (this.shadowsOn)
        {
            this.shadowL = UnityEngine.Object.Instantiate(this.objectCopyR.gameObject, this.transform.position, this.transform.rotation);
            this.shadowL.name = this.gameObject.name + "_shadL";
            this.shadowL.gameObject.layer = this.camera3D.leftOnlyLayer;
            this.shadowL.GetComponent<GUIText>().material.color = this.ShadowColor;
            this.shadowL.transform.parent = this.transform;
            this.shadowR = UnityEngine.Object.Instantiate(this.objectCopyR.gameObject, this.transform.position, this.transform.rotation);
            this.shadowR.name = this.gameObject.name + "_shadR";
            this.shadowR.gameObject.layer = this.camera3D.rightOnlyLayer;
            this.shadowR.GetComponent<GUIText>().material.color = this.ShadowColor;
            this.shadowR.transform.parent = this.objectCopyR.transform;
        }
        this.obPosition = this.gameObject.transform.position;
        this.setText(this.initString);
        this.toggleVisible(this.beginVisible);
        float horizontalFOV = (2 * Mathf.Atan(Mathf.Tan((this.camera3D.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad) / 2) * this.camera3D.GetComponent<Camera>().aspect)) * Mathf.Rad2Deg;
        this.unitWidth = Mathf.Tan((horizontalFOV / 2) * Mathf.Deg2Rad); // need unit width to calculate cursor depth when there's a HIT
        this.screenWidth = (this.unitWidth * this.camera3D.zeroPrlxDist) * 2;
        if (this.timeToDisplay != 0f)
        {
            yield return new WaitForSeconds(this.timeToDisplay);
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
        float obPrlx = 0.0f;
        if (this.textOn)
        {
            if (this.trackMouseXYPosition)
            {
                if (!this.onlyWhenMouseDown || (this.onlyWhenMouseDown && Input.GetMouseButton(0)))
                {
                    this.obPosition = this.matchMousePos();
                }
            }
            if (this.keepCloser)
            {
                this.findDistanceUnderObject();
                this.objectDistance = Mathf.Max(this.objectDistance, this.camera3D.GetComponent<Camera>().nearClipPlane);
            }
            this.setScreenParallax();
            if (this.curScrnPrlx != this.scrnPrlx)
            {
                this.curScrnPrlx = this.curScrnPrlx + ((this.scrnPrlx - this.curScrnPrlx) / (this.lagTime + 1));
            }
            this.gameObject.transform.position = new Vector3(this.obPosition.x + (this.curScrnPrlx / 2), this.obPosition.y, this.gameObject.transform.position.z + 1);
            if (this.shadowsOn)
            {
                this.shadowL.gameObject.transform.localPosition = new Vector3(this.shadowOffset / 1100, -this.shadowOffset / 1000, 0);
            }
            this.objectCopyR.transform.position = new Vector3(this.obPosition.x - (this.curScrnPrlx / 2), this.obPosition.y, this.objectCopyR.gameObject.transform.position.z + 1);
            if (this.shadowsOn)
            {
                this.shadowR.gameObject.transform.localPosition = new Vector3(this.shadowOffset / 900, -this.shadowOffset / 1000, 0);
            }
        }
    }

    public virtual void findDistanceUnderObject()
    {
        Vector2 dPosition = default(Vector2);
        RaycastHit hit = default(RaycastHit);
        float nearDistance = Mathf.Infinity;
        if ((this.camera3D.format3D == (mode3D) 0) && !this.camera3D.sideBySideSqueezed)
        {
            dPosition = new Vector2((this.obPosition.x / 2) + 0.25f, this.obPosition.y); // 0 = left, 0.5 = center, 1 = right
        }
        else
        {
            dPosition = this.obPosition;
        }
        Ray ray = this.camera3D.GetComponent<Camera>().ViewportPointToRay(dPosition);
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, new Color(0, 1, 0, 1));
            Plane camPlane = new Plane(this.camera3D.GetComponent<Camera>().transform.forward, this.camera3D.GetComponent<Camera>().transform.position);
            Vector3 thePoint = ray.GetPoint(hit.distance);
            nearDistance = camPlane.GetDistanceToPoint(thePoint);
        }
        if (nearDistance < Mathf.Infinity)
        {
            this.objectDistance = Mathf.Clamp(nearDistance, this.minimumDistance, this.maximumDistance);
        }
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
        this.textOn = t;
    }

    public virtual void setText(string theText)
    {
        this.gameObject.GetComponent<GUIText>().text = theText;
        if (this.objectCopyR)
        {
            this.objectCopyR.GetComponent<GUIText>().text = theText;
        }
        if (this.shadowsOn)
        {
            if (this.shadowL)
            {
                this.shadowL.GetComponent<GUIText>().text = theText;
            }
            if (this.shadowR)
            {
                this.shadowR.GetComponent<GUIText>().text = theText;
            }
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

    // set position from another script (rollover text)
    public virtual void setObPosition(Vector2 obPos)
    {
        this.obPosition.x = obPos.x;
        this.obPosition.y = obPos.y;
    }

    public s3dGuiText()
    {
        this.onlyWhenMouseDown = true;
        this.objectDistance = 1f;
        this.nearPadding = 1f;
        this.minimumDistance = 1f;
        this.maximumDistance = 3f;
        this.beginVisible = true;
        this.timeToDisplay = 2f;
        this.TextColor = Color.white;
        this.shadowsOn = true;
        this.ShadowColor = Color.black;
        this.shadowOffset = 5f;
        this.rays = new object[0];
    }

}