using UnityEngine;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Touchpad Script - revised 12-30-12 - based on Joystick.js
 * rewritten to combine elements of touchpad behavior with optional joystick visual movement, or joystick behavior without visual movement.
 * and to assume one touch per touchpad, so no multiple finger latching
 * and to deal only with single taps, so no tapCount - thus we can use TouchPhase to deal with taps
 * also, we don't need to worry about keeping track of all the touchpads, because they don't interfere with each other
 * if a TouchPhase has never been TouchPhase.Moved when it becomes TouchPhase.Ended (or if it hasn't moved more than tapDistanceLimit), then it's a tap
 * otherwise its a drag/swipe
 * distinguishes between short and long taps with shortTapTimeMax and longTapTimeMax
 */
// A simple class for bounding how far the GUITexture will move
[System.Serializable]
public class Boundary : object
{
    public Vector2 min;
    public Vector2 max;
    public Boundary()
    {
        this.min = Vector2.zero;
        this.max = Vector2.zero;
    }

}
[System.Serializable]
[UnityEngine.RequireComponent(typeof(GUITexture))]
 // [-1, 1] in x,y
 // 1 for short tap, 2 for long tap
 // Joystick graphic
 // Default position / extents of the joystick graphic
 // Boundary for joystick graphic
 // Offset to apply to touch input
 // Center of joystick
[UnityEngine.AddComponentMenu("Stereoskopix/s3d Touchpad")]
public partial class s3dTouchpad : MonoBehaviour
{
    public bool moveLikeJoystick;
    public bool actLikeJoystick;
    public float shortTapTimeMax;
    public float longTapTimeMax;
    public float tapDistanceLimit;
    private Rect touchZone;
    public Vector2 position;
    public int tap;
    private GUITexture gui;
    private Rect defaultRect;
    private Boundary guiBoundary;
    private Vector2 guiTouchOffset;
    private Vector2 guiCenter;
    private int thisTouchID;
    private float thisTouchDownTime;
    private bool thisTouchMoved;
    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;
    private float fingerDownTime;
    public virtual void Start()
    {
        this.setUp();
    }

    public virtual void setUp()
    {
        // Cache this component at startup instead of looking up every frame	
        this.gui = (GUITexture) this.GetComponent(typeof(GUITexture));
        // Store the default rect for the gui, so we can snap back to it
        this.defaultRect = this.gui.pixelInset;
        this.defaultRect.x = this.defaultRect.x + (this.transform.position.x * Screen.width);// + gui.pixelInset.x; // -  Screen.width * 0.5;
        this.defaultRect.y = this.defaultRect.y + (this.transform.position.y * Screen.height);// - Screen.height * 0.5;

        {
            float _53 = 0f;
            Vector3 _54 = this.transform.position;
            _54.x = _53;
            this.transform.position = _54;
        }

        {
            float _55 = 0f;
            Vector3 _56 = this.transform.position;
            _56.y = _55;
            this.transform.position = _56;
        }
        // If a texture has been assigned, then use the rect from the gui as our touchZone
        if (this.gui.texture)
        {
            this.touchZone = this.defaultRect;
        }
        // This is an offset for touch input to match with the top left corner of the GUI
        this.guiTouchOffset.x = this.defaultRect.width * 0.5f;
        this.guiTouchOffset.y = this.defaultRect.height * 0.5f;
        // Cache the center of the GUI, since it doesn't change
        this.guiCenter.x = this.defaultRect.x + this.guiTouchOffset.x;
        this.guiCenter.y = this.defaultRect.y + this.guiTouchOffset.y;
        // Let's build the GUI boundary, so we can clamp joystick movement
        this.guiBoundary.min.x = this.defaultRect.x - this.guiTouchOffset.x;
        this.guiBoundary.max.x = this.defaultRect.x + this.guiTouchOffset.x;
        this.guiBoundary.min.y = this.defaultRect.y - this.guiTouchOffset.y;
        this.guiBoundary.max.y = this.defaultRect.y + this.guiTouchOffset.y;
        this.gui.pixelInset = this.defaultRect;
    }

    public virtual void Update()
    {
        Vector2 guiTouchPos = (Vector2) Input.mousePosition - this.guiTouchOffset;
        if (this.touchZone.Contains(Input.mousePosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.thisTouchID = 1;
                this.fingerDownPos = Input.mousePosition;
                this.thisTouchDownTime = Time.time;
                this.thisTouchMoved = false;
            }
        }
        if (this.thisTouchID == 1)
        {
            if (!this.actLikeJoystick)
            {
                this.position.x = Mathf.Clamp((Input.mousePosition.x - this.fingerDownPos.x) / (this.touchZone.width / 2), -1, 1);
                this.position.y = Mathf.Clamp((Input.mousePosition.y - this.fingerDownPos.y) / (this.touchZone.height / 2), -1, 1);
            }
            if (this.moveLikeJoystick)
            {

                {
                    float _57 = Mathf.Clamp(guiTouchPos.x, this.guiBoundary.min.x, this.guiBoundary.max.x);
                    Rect _58 = this.gui.pixelInset;
                    _58.x = _57;
                    this.gui.pixelInset = _58;
                }

                {
                    float _59 = Mathf.Clamp(guiTouchPos.y, this.guiBoundary.min.y, this.guiBoundary.max.y);
                    Rect _60 = this.gui.pixelInset;
                    _60.y = _59;
                    this.gui.pixelInset = _60;
                }
            }
            if (this.actLikeJoystick)
            {
                float dummyInsetX = Mathf.Clamp(guiTouchPos.x, this.guiBoundary.min.x, this.guiBoundary.max.x);
                float dummyInsetY = Mathf.Clamp(guiTouchPos.y, this.guiBoundary.min.y, this.guiBoundary.max.y);
                this.position.x = ((dummyInsetX + this.guiTouchOffset.x) - this.guiCenter.x) / this.guiTouchOffset.x;
                this.position.y = ((dummyInsetY + this.guiTouchOffset.y) - this.guiCenter.y) / this.guiTouchOffset.y;
            }
        }
        if (Input.GetMouseButtonUp(0) && (this.thisTouchID == 1))
        {
            this.fingerUpPos = Input.mousePosition;
            float dist = Vector2.Distance(this.fingerDownPos, this.fingerUpPos);
            if (dist < this.tapDistanceLimit)
            {
                if (Time.time < (this.thisTouchDownTime + this.shortTapTimeMax))
                {
                    this.tap = 1;
                }
                else
                {
                    if (Time.time < (this.thisTouchDownTime + this.longTapTimeMax))
                    {
                        this.tap = 2;
                    }
                }
            }
            this.thisTouchID = -1;
            this.position = Vector2.zero;
            if (this.moveLikeJoystick)
            {
                this.gui.pixelInset = this.defaultRect;
            }
        }
    }

    /* The client that directly registers the tap is responsible for resetting touchpad. Currently, the following scripts call this function: s3dDeviceManager.js, s3dFirstPersonController.js, s3dGuiTexture.js, triggerObjectButton.js, triggerSceneChange.js. Note that since s3dInteractor.js (called by s3dGuiTexture.js) & all interaction scripts (called by s3dInteractor.js) are not direct clients, they aren't responsible for resetting touchpad */
    public virtual void reset()
    {
        this.tap = 0;
    }

    public s3dTouchpad()
    {
        this.moveLikeJoystick = true;
        this.shortTapTimeMax = 0.2f;
        this.longTapTimeMax = 0.5f;
        this.tapDistanceLimit = 10f;
        this.guiBoundary = new Boundary();
        this.thisTouchID = -1;
    }

}