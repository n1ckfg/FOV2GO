using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * s3d Auto Depth Script revised 12.30.12
 * Usage:
 * This script uses information from the s3DdepthInfo script to automatically
 * set the interaxial and/or the convergence (zero parallax distance) to keep screen parallax within a
 * specified percentage of the screen width. The interaxial can be clamped between a minimum and a maximum,
 * and a minimum for zero parallax distance can be specified. 
 * 
 * Objects can be tagged "Ignore Raycast" to hide them from raycasts (this would typically be done with ground planes).
*/
//enum converge {none,percent,center,click,mouse,object};
// none: leave zpd alone
// auto: maintain ratio of negative to positive parallax as set by "percentageNegativeParallax". 
// center: keep convergence at center of scene
// click: click mouse button to set zero parallax distance to point on object
// mouse: zero parallax distance set continuously to mouse position
// object: continuously keep zero parallax distance at clicked object point
// autoInteraxial: Control interaxial. 
// True: keep within "parallaxPercentageOfWidth". 
// False: leaves interaxial alone
// parallaxPercentageOfWidth: Automatically calculates interaxial
// based on a total parallax value expressed as a percentage of image width.
// percentageNegativeParallax: Automatically calculates zero parallax distance (convergence)
// based on a negative parallax value expressed as a percentage of total parallax.
// zeroPrlxDistanceMin: Set zero parallax distance minimum
 // meters
// interaxialMin: Limit minimum allowed interaxial; overrides parallaxPercentageOfWidth
 // millimeters
// interaxialMax: Limit maximum allowed interaxial; overrides parallaxPercentageOfWidth
 // millimeters
// how gradually to change interaxial and zero parallax (bigger numbers are slower - more than 25 is very slow);
//private var farDistance: float;
[UnityEngine.RequireComponent(typeof(s3dCamera))]
[UnityEngine.RequireComponent(typeof(s3dDepthInfo))]
[UnityEngine.AddComponentMenu("Stereoskopix/s3d Auto Depth")]
public partial class s3dAutoDepth : MonoBehaviour
{
    public converge convergenceMethod;
    public bool autoInteraxial;
    public float parallaxPercentageOfWidth;
    public float percentageNegativeParallax;
    public float zeroPrlxDistanceMin;
    public float interaxialMin;
    public float interaxialMax;
    public float lagTime;
    private float cameraWidth;
    private float cameraParallaxNegative;
    private float cameraParallaxPositive;
    private float zeroPrlxNewDistance;
    private Vector3 nearPoint;
    private Vector3 farPoint;
    private float interaxial;
    private Camera mainCam;
    private s3dCamera camScript;
    private s3dDepthInfo infoScript;
    private object[][] rays;
    public virtual void Start()
    {
        this.mainCam = (Camera) this.gameObject.GetComponent(typeof(Camera)); // Main Stereo Camera Component
        this.camScript = (s3dCamera) this.gameObject.GetComponent(typeof(s3dCamera)); // Main Stereo Script
        this.infoScript = (s3dDepthInfo) this.gameObject.GetComponent(typeof(s3dDepthInfo)); // Main Stereo Script
    }

    public virtual void Update()
    {
        if ((this.infoScript.nearDistance < Mathf.Infinity) && (this.infoScript.farDistance > 0))
        {
            // calculate image width at far distance
            float cameraWidthFar = (Mathf.Tan(((this.mainCam.fieldOfView * this.mainCam.aspect) / 2) * Mathf.Deg2Rad) * this.infoScript.farDistance) * 2;
            // calculate total parallax
            float cameraParallaxTotal = (this.parallaxPercentageOfWidth / 100) * cameraWidthFar;
            if (this.autoInteraxial)
            {
                // calculate interaxial
                this.interaxial = ((cameraParallaxTotal * this.infoScript.nearDistance) / (this.infoScript.farDistance - this.infoScript.nearDistance)) * 1000; // convert from meters to millimeters
                // clamp interaxial between minimum & maximum values
                this.interaxial = Mathf.Clamp(this.interaxial, this.interaxialMin, this.interaxialMax);
            }
            else
            {
                this.interaxial = this.camScript.interaxial;
            }
            switch (this.convergenceMethod)
            {
                case converge.percent:
                    // calculate negative parallax from percentage
                    this.cameraParallaxNegative = (cameraParallaxTotal * this.percentageNegativeParallax) / 100;
                    // calculate zero parallax distance - convert from millimeters to meters
                    this.zeroPrlxNewDistance = ((this.infoScript.nearDistance * this.cameraParallaxNegative) / (this.interaxial / 1000)) + this.infoScript.nearDistance;
                    // clamp zero parallax distance to a minimum
                    this.zeroPrlxNewDistance = Mathf.Max(this.zeroPrlxNewDistance, this.zeroPrlxDistanceMin);
                    // calculate positive parallax from percentage
                    this.cameraParallaxPositive = (cameraParallaxTotal * (100 - this.percentageNegativeParallax)) / 100;
                    break;
                case converge.center:
                    this.zeroPrlxNewDistance = this.infoScript.distanceAtCenter;
                    break;
                case converge.click:
                    if (Input.GetMouseButtonDown(0))
                    {
                        this.zeroPrlxNewDistance = this.infoScript.distanceUnderMouse;
                    }
                    break;
                case converge.mouse:
                    this.zeroPrlxNewDistance = this.infoScript.distanceUnderMouse;
                    break;
                case converge.@object:
                    if (this.infoScript.selectedObject && (this.infoScript.objectDistance > 0))
                    {
                        this.zeroPrlxNewDistance = this.infoScript.objectDistance;
                    }
                    else
                    {
                        this.zeroPrlxNewDistance = this.camScript.zeroPrlxDist;
                    }
                    break;
                default:
                    this.zeroPrlxNewDistance = this.camScript.zeroPrlxDist;
                    break;
            }
        }
    }

    // update interaxial and convergence
    public virtual void FixedUpdate()
    {
        if (this.convergenceMethod != converge.none)
        {
            if (this.camScript.zeroPrlxDist > this.zeroPrlxNewDistance)
            {
                this.camScript.zeroPrlxDist = this.camScript.zeroPrlxDist - ((this.camScript.zeroPrlxDist - this.zeroPrlxNewDistance) / (this.lagTime + 1));
            }
            else
            {
                if (this.camScript.zeroPrlxDist < this.zeroPrlxNewDistance)
                {
                    this.camScript.zeroPrlxDist = this.camScript.zeroPrlxDist + ((this.zeroPrlxNewDistance - this.camScript.zeroPrlxDist) / (this.lagTime + 1));
                }
            }
        }
        if (this.autoInteraxial)
        {
            if (this.camScript.interaxial > this.interaxial)
            {
                this.camScript.interaxial = this.camScript.interaxial - ((this.camScript.interaxial - this.interaxial) / (this.lagTime + 1));
            }
            else
            {
                if (this.camScript.interaxial < this.interaxial)
                {
                    this.camScript.interaxial = this.camScript.interaxial + ((this.interaxial - this.camScript.interaxial) / (this.lagTime + 1));
                }
            }
        }
    }

    /*

Given: an on-screen parallax percentage AND image width AND near object distance AND screen plane distance - calculate interaxial
Assume that screen plane = far object distance, so that total parallax = negative parallax

                parallaxNeg                 interaxial
--------------------------------------- = -------------
screen plane distance - object distance    object distance

prlxNeg = (screenDist-objectDist) * interaxial / objectDist
prlxNeg * objectDist = (screenDist-objectDist) * interaxial
prlxNeg * objectDist / (screenDist-objectDist) = interaxial
interaxial = prlxNeg * objectDist / (screenDist-objectDist)

prlxPercent = prlxNeg / screenWidth
prlxNeg = prlxPercent * screenWidth

interaxial = (prlxPercent * screenWidth) * objectDist / (screenDist - objectDist)

to calculate positive parallax

                parallaxPos                 interaxial
--------------------------------------- = -------------
object distance - screen plane distance    object distance


*/
    public s3dAutoDepth()
    {
        this.convergenceMethod = converge.center;
        this.autoInteraxial = true;
        this.parallaxPercentageOfWidth = 3;
        this.percentageNegativeParallax = 66;
        this.zeroPrlxDistanceMin = 1f;
        this.interaxialMin = 30;
        this.interaxialMax = 120;
        this.lagTime = 10;
        this.rays = new object[][] {new object[0], new object[0]};
    }

}