using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * first tap picks up object, object can be dragged around
 * next tap drops object
 * requires rigidbody
 */
// DragRigidBody
 //50
 //5
 //0.2  
 // 1 = short tap 2 = long tap 3 = any tap
// if we move forward, object stays where it is 
// position of the object when clicked on
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
[UnityEngine.RequireComponent(typeof(Rigidbody))]
public partial class pickUpPutDown : MonoBehaviour
{
    public float spring;
    public float damper;
    public float drag;
    public float angularDrag;
    public float distance;
    public int tapType;
    public bool attachToCenterOfMass;
    private SpringJoint springJoint;
    public Vector3 customCenterOfMass;
    public bool moveTowardsObject;
    public float minDist;
    public float maxDist;
    public Vector3 grabOffset;
    private GameObject mainCamObj;
    private GameObject cursorObj;
    private s3dGuiCursor cursorScript;
    private bool activated;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool readyForStateChange;
    private Vector3 newPosition;
    private Vector3 clickPosition;
    private float hitDistance;
    public virtual void Start()
    {
        this.mainCamObj = GameObject.FindWithTag("MainCamera"); // Main Camera
        this.cursorObj = GameObject.FindWithTag("cursor");
        this.cursorScript = (s3dGuiCursor) this.cursorObj.GetComponent(typeof(s3dGuiCursor)); // Main Stereo Camera Script
        this.startPos = this.transform.position;
        this.startRot = this.transform.rotation;
        this.GetComponent<Rigidbody>().centerOfMass = this.customCenterOfMass;
    }

    public virtual void NewTap(TapParams @params)
    {
        if (this.readyForStateChange)
        {
            if (!this.activated)
            {
                if ((@params.tap == this.tapType) || (this.tapType == 3))
                {
                    this.activated = true;
                }
            }
            else
            {
                this.activated = false;
            }
            //activated = !activated;
            this.hitDistance = @params.hit.distance;
            this.readyForStateChange = false;
            this.StartCoroutine(this.pauseAfterStateChange());
        }
        if (this.activated)
        {
            this.clickPosition = @params.hit.point;
            this.cursorScript.activeObj = this.gameObject; // tell cursorScript that we have an active object
            if (!this.springJoint)
            {
                GameObject go = new GameObject("Rigidbody dragger");
                Rigidbody body = ((Rigidbody) go.AddComponent(typeof(Rigidbody))) as Rigidbody;
                this.springJoint = (SpringJoint) go.AddComponent(typeof(SpringJoint));
                body.isKinematic = true;
            }
            this.springJoint.transform.position = @params.hit.point;
            if (this.attachToCenterOfMass)
            {
                Vector3 anchor = this.transform.TransformDirection(this.GetComponent<Rigidbody>().centerOfMass) + this.GetComponent<Rigidbody>().transform.position;
                anchor = this.springJoint.transform.InverseTransformPoint(anchor);
                this.springJoint.anchor = anchor;
            }
            else
            {
                this.springJoint.anchor = Vector3.zero;
            }
            this.springJoint.spring = this.spring;
            this.springJoint.damper = this.damper;
            this.springJoint.maxDistance = this.distance;
            this.springJoint.connectedBody = this.GetComponent<Rigidbody>();
            this.StartCoroutine(this.increaseSpringAfterPickup());
            this.StartCoroutine("DragObject");
        }
        else
        {
            this.cursorScript.activeObj = null;
        }
    }

    public virtual IEnumerator DragObject()
    {
        float oldDrag = this.springJoint.connectedBody.drag;
        float oldAngularDrag = this.springJoint.connectedBody.angularDrag;
        this.springJoint.connectedBody.drag = this.drag;
        this.springJoint.connectedBody.angularDrag = this.angularDrag;
        while (this.activated) // end when receive another double-click touch
        {
            this.springJoint.transform.position = this.newPosition + this.grabOffset;
            yield return null;
        }
        if (this.springJoint.connectedBody)
        {
            this.springJoint.connectedBody.drag = oldDrag;
            this.springJoint.connectedBody.angularDrag = oldAngularDrag;
            this.springJoint.connectedBody = null;
        }
    }

    public virtual void Deactivate()
    {
        this.activated = false;
        this.cursorScript.activeObj = null;
        this.readyForStateChange = false;
        this.springJoint.spring = this.springJoint.spring / 10;
        this.StartCoroutine(this.pauseAfterStateChange());
    }

    public virtual IEnumerator pauseAfterStateChange()
    {
        yield return new WaitForSeconds(0.25f);
        this.readyForStateChange = true;
    }

    public virtual IEnumerator increaseSpringAfterPickup()
    {
        yield return new WaitForSeconds(1);
        this.springJoint.spring = this.springJoint.spring * 10;
    }

    public virtual void NewPosition(Vector3 pos)
    {
        float currentDistance = 0.0f;
        if (this.activated)
        {
            Vector3 viewPos = this.mainCamObj.GetComponent<Camera>().WorldToViewportPoint(pos);
            Ray ray = this.mainCamObj.GetComponent<Camera>().ViewportPointToRay(viewPos);
            if (this.moveTowardsObject)
            {
                currentDistance = Vector3.Distance(this.mainCamObj.transform.position, this.clickPosition);
                currentDistance = Mathf.Clamp(currentDistance, this.minDist, this.maxDist);
            }
            else
            {
                currentDistance = this.hitDistance;
            }
            this.newPosition = ray.GetPoint(currentDistance);
        }
    }

    public pickUpPutDown()
    {
        this.spring = 100f;
        this.damper = 100f;
        this.drag = 10f;
        this.angularDrag = 5f;
        this.distance = 0.1f;
        this.tapType = 1;
        this.customCenterOfMass = Vector3.zero;
        this.moveTowardsObject = true;
        this.minDist = 1;
        this.maxDist = 10;
        this.grabOffset = new Vector3(0, 0.5f, 0);
        this.readyForStateChange = true;
    }

}