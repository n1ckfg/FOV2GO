using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class s3dSmoothMouseLook : MonoBehaviour
{
    /* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Smooth Mouse Look revised 12.30.12
 * javascript version for Stereoskopix package
 * Usage: Replace standard Unity MouseLook character controller script with this script.
 * Provides smoother mouse movement, by default only active when mouse button is down 
 * Uncheck MouseDownRequired to make always active.
 * Only active on desktop, automatically disabled on iOS and Android.
 * Note: conflicts to some extent with s3dTouchpad system, because this script uses entire screen for input, so touchpad movement triggers it
 */
    //enum Axes {MouseXandY, MouseX, MouseY} - declared in s3denums.js
    public Axes Axis;
    public bool MouseDownRequired;
    public float frameCounter;
    private Quaternion originalRotation;
    public float sensitivityX;
    public float sensitivityY;
    public float minimumX;
    public float maximumX;
    public float minimumY;
    public float maximumY;
    private float rotationX;
    private float rotationY;
    private object[] rotArrayX;
    private float rotAverageX;
    private object[] rotArrayY;
    private float rotAverageY;
    private Quaternion xQuaternion;
    private Quaternion yQuaternion;
    public virtual void Update()
    {
        float tempFloat = 0.0f;
        if (this.Axis == Axes.MouseXandY)
        {
            this.rotAverageY = 0;
            this.rotAverageX = 0;
            if (Input.GetMouseButton(0) || !this.MouseDownRequired)
            {
                this.rotationX = this.rotationX + (Input.GetAxis("Mouse X") * this.sensitivityX);
                this.rotationY = this.rotationY + (Input.GetAxis("Mouse Y") * this.sensitivityY);
            }
            this.rotArrayY.Add(this.rotationY);
            this.rotArrayX.Add(this.rotationX);
            if (this.rotArrayY.Length >= this.frameCounter)
            {
                this.rotArrayY.RemoveAt(0);
            }
            if (this.rotArrayX.Length >= this.frameCounter)
            {
                this.rotArrayX.RemoveAt(0);
            }
            int j = 0;
            while (j < this.rotArrayY.Length)
            {
                tempFloat = (float) this.rotArrayY[j];
                this.rotAverageY = this.rotAverageY + tempFloat;
                j++;
            }
            int i = 0;
            while (i < this.rotArrayX.Length)
            {
                tempFloat = (float) this.rotArrayX[i];
                this.rotAverageX = this.rotAverageX + tempFloat;
                i++;
            }
            this.rotAverageY = this.rotAverageY / this.rotArrayY.Length;
            this.rotAverageX = this.rotAverageX / this.rotArrayX.Length;
            this.rotAverageY = Mathf.Clamp(this.rotAverageY, this.minimumY, this.maximumY);
            this.rotAverageX = Mathf.Clamp(this.rotAverageX % 360, this.minimumX, this.maximumX);
            this.yQuaternion = Quaternion.AngleAxis(this.rotAverageY, Vector3.left);
            this.xQuaternion = Quaternion.AngleAxis(this.rotAverageX, Vector3.up);
            this.transform.localRotation = (this.originalRotation * this.xQuaternion) * this.yQuaternion;
        }
        else
        {
            if (this.Axis == Axes.MouseX)
            {
                this.rotAverageX = 0;
                if (Input.GetMouseButton(0) || !this.MouseDownRequired)
                {
                    this.rotationX = this.rotationX + (Input.GetAxis("Mouse X") * this.sensitivityX);
                }
                this.rotArrayX.Add(this.rotationX);
                if (this.rotArrayX.Length >= this.frameCounter)
                {
                    this.rotArrayX.RemoveAt(0);
                }
                i = 0;
                while (i < this.rotArrayX.Length)
                {
                    tempFloat = (float) this.rotArrayX[i];
                    this.rotAverageX = this.rotAverageX + tempFloat;
                    i++;
                }
                this.rotAverageX = this.rotAverageX / this.rotArrayX.Length;
                this.rotAverageX = Mathf.Clamp(this.rotAverageX % 360, this.minimumX, this.maximumX);
                this.xQuaternion = Quaternion.AngleAxis(this.rotAverageX, Vector3.up);
                this.transform.localRotation = this.originalRotation * this.xQuaternion;
            }
            else
            {
                this.rotAverageY = 0;
                if (Input.GetMouseButton(0) || !this.MouseDownRequired)
                {
                    this.rotationY = this.rotationY + (Input.GetAxis("Mouse Y") * this.sensitivityY);
                }
                this.rotArrayY.Add(this.rotationY);
                if (this.rotArrayY.Length >= this.frameCounter)
                {
                    this.rotArrayY.RemoveAt(0);
                }
                j = 0;
                while (j < this.rotArrayY.Length)
                {
                    tempFloat = (float) this.rotArrayY[j];
                    this.rotAverageY = this.rotAverageY + tempFloat;
                    j++;
                }
                this.rotAverageY = this.rotAverageY / this.rotArrayY.Length;
                this.rotAverageY = Mathf.Clamp(this.rotAverageY % 360, this.minimumY, this.maximumY);
                this.yQuaternion = Quaternion.AngleAxis(this.rotAverageY, Vector3.left);
                this.transform.localRotation = this.originalRotation * this.yQuaternion;
            }
        }
    }

    public virtual void Start()
    {
        // Make the rigid body not change rotation
        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().freezeRotation = true;
        }
        this.originalRotation = this.transform.localRotation;
    }

    public s3dSmoothMouseLook()
    {
        this.Axis = Axes.MouseXandY;
        this.MouseDownRequired = true;
        this.frameCounter = 20;
        this.sensitivityX = 1f;
        this.sensitivityY = 1f;
        this.minimumX = -360f;
        this.maximumX = 360f;
        this.minimumY = -60f;
        this.maximumY = 60f;
        this.rotArrayX = new object[0];
        this.rotArrayY = new object[0];
    }

}