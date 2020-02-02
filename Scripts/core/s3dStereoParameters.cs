using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Stereo Parameters revised 12-30-12
 * Usage: Provides an interface to adjust stereo parameters (interaxial, zero parallax distance, and horizontal image transform (HIT)
 * Option to use in conjunction with s3dDeviceManager.js or on its own 
 * Has companion Editor script to create custom inspector 
 */
[UnityEngine.ExecuteInEditMode]
public partial class s3dStereoParameters : MonoBehaviour
{
    private s3dCamera camera3D;
    private s3dDeviceManager s3dDeviceMan;
    public s3dTouchpad stereoParamsTouchpad;
    public bool saveStereoParamsToDisk;
    public Texture2D showStereoParamsTexture;
    public Texture2D dismissStereoParamsTexture;
    private bool showParamGui;
    public s3dGuiText stereoReadoutText;
    public s3dTouchpad interaxialTouchpad;
    private float interaxialStart;
    private float interaxialInc;
    public s3dTouchpad zeroPrlxTouchpad;
    private float zeroPrlxStart;
    private float zeroPrlxInc;
    public s3dTouchpad hitTouchpad;
    private float hitStart;
    private float hitInc;
    public virtual void Awake()
    {
        this.s3dDeviceMan = (s3dDeviceManager) this.gameObject.GetComponent(typeof(s3dDeviceManager));
        // if object has s3dDeviceManager.js, use that script's camera & touchpads
        if (this.s3dDeviceMan)
        {
            this.stereoParamsTouchpad = this.s3dDeviceMan.stereoParamsTouchpad;
            this.interaxialTouchpad = this.s3dDeviceMan.interaxialTouchpad;
            this.zeroPrlxTouchpad = this.s3dDeviceMan.zeroPrlxTouchpad;
            this.hitTouchpad = this.s3dDeviceMan.hitTouchpad;
        }
    }

    public virtual void Start()
    {
        this.findS3dCamera();
        if (this.saveStereoParamsToDisk)
        {
            if (PlayerPrefs.GetFloat(Application.loadedLevelName + "_interaxial") != 0f)
            {
                this.camera3D.interaxial = PlayerPrefs.GetFloat(Application.loadedLevelName + "_interaxial");
            }
            if (PlayerPrefs.GetFloat(Application.loadedLevelName + "_zeroPrlxDistance") != 0f)
            {
                this.camera3D.zeroPrlxDist = PlayerPrefs.GetFloat(Application.loadedLevelName + "_zeroPrlxDist");
            }
            if (PlayerPrefs.GetFloat(Application.loadedLevelName + "_H_I_T") != 0f)
            {
                this.camera3D.H_I_T = PlayerPrefs.GetFloat(Application.loadedLevelName + "_H_I_T");
            }
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
        if (this.stereoParamsTouchpad && (this.stereoParamsTouchpad.tap > 0))
        {
            this.showParamGui = !this.showParamGui;
            this.toggleStereoParamGui(this.showParamGui);
            this.stereoParamsTouchpad.reset();
            if (this.showParamGui)
            {
                this.interaxialStart = this.camera3D.interaxial;
                this.zeroPrlxStart = this.camera3D.zeroPrlxDist;
                this.hitStart = this.camera3D.H_I_T;
            }
            else
            {
                 // showParamGui has just been dismissed, write new values to disk
                if (this.saveStereoParamsToDisk)
                {
                    PlayerPrefs.SetFloat(Application.loadedLevelName + "_interaxial", this.camera3D.interaxial);
                    PlayerPrefs.SetFloat(Application.loadedLevelName + "_zeroPrlxDist", this.camera3D.zeroPrlxDist);
                    PlayerPrefs.SetFloat(Application.loadedLevelName + "_H_I_T", this.camera3D.H_I_T);
                }
                this.interaxialStart = this.camera3D.interaxial;
                this.zeroPrlxStart = this.camera3D.zeroPrlxDist;
                this.hitStart = this.camera3D.H_I_T;
            }
        }
        if (this.showParamGui)
        {
            // touchpad should be set to moveLikeJoystick = false, actLikeJoystick = true
            // so that a tapdown generates a position change
            // grab position while touchpad is being dragged - because when we actually get the tap (at TouchPhase.Ended) 
            // or the click (on Input.GetMouseButtonUp), position has already been reset to 0
            if (this.interaxialTouchpad.position.x != 0f)
            {
                // position values are between -1.0 and 1.0 - values < 1.0 are converted to -1.0, values > 1.0 are converted to 1.0
                this.interaxialInc = Mathf.Round(this.interaxialTouchpad.position.x + (0.49f * Mathf.Sign(this.interaxialTouchpad.position.x)));
            }
            if (this.interaxialTouchpad.tap > 0)
            {
                this.camera3D.interaxial = this.camera3D.interaxial + this.interaxialInc;
                this.camera3D.interaxial = Mathf.Max(this.camera3D.interaxial, 0);
                this.interaxialTouchpad.reset();
            }
            if (this.zeroPrlxTouchpad.position.x != 0f)
            {
                this.zeroPrlxInc = Mathf.Round(this.zeroPrlxTouchpad.position.x + (0.49f * Mathf.Sign(this.zeroPrlxTouchpad.position.x))) * 0.25f;
            }
            if (this.zeroPrlxTouchpad.tap > 0)
            {
                this.camera3D.zeroPrlxDist = this.camera3D.zeroPrlxDist + this.zeroPrlxInc;
                this.camera3D.zeroPrlxDist = Mathf.Max(this.camera3D.zeroPrlxDist, 1f);
                this.zeroPrlxTouchpad.reset();
            }
            if (this.hitTouchpad.position.x != 0f)
            {
                this.hitInc = Mathf.Round(this.hitTouchpad.position.x + (0.49f * Mathf.Sign(this.hitTouchpad.position.x))) * 0.1f;
            }
            if (this.hitTouchpad.tap > 0)
            {
                this.camera3D.H_I_T = this.camera3D.H_I_T + this.hitInc;
                this.hitTouchpad.reset();
            }
            this.stereoReadoutText.setText((((("Interaxial: " + (Mathf.Round(this.camera3D.interaxial * 10) / 10)) + "mm \nZero Prlx: ") + (Mathf.Round(this.camera3D.zeroPrlxDist * 10) / 10)) + "M \nH.I.T.: ") + (Mathf.Round(this.camera3D.H_I_T * 10) / 10));
        }
    }

    public virtual void toggleStereoParamGui(bool on)
    {
        if (on)
        {
            ((GUITexture) this.stereoParamsTouchpad.gameObject.GetComponent(typeof(GUITexture))).texture = this.dismissStereoParamsTexture;
            this.stereoReadoutText.toggleVisible(true);
        }
        else
        {
            ((GUITexture) this.stereoParamsTouchpad.gameObject.GetComponent(typeof(GUITexture))).texture = this.showStereoParamsTexture;
            this.stereoReadoutText.toggleVisible(false);
        }
    }

}