using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Stereo Window Script revised 12.30.12
 * Usage:
 * Attach to main camera. This script requires s3Dcamera script.
 * It automatically provides side masks to prevent stereo window violations.
 * Dependencies: s3dCamera.js
 */
//enum maskDistance {MaxDistance, ScreenPlane, FarFrustum};
[UnityEngine.RequireComponent(typeof(s3dCamera))]
[UnityEngine.AddComponentMenu("Stereoskopix/s3d Window")]
public partial class s3dWindow : MonoBehaviour
{
    public bool on;
    public int sideSamples;
    public maskDistance maskLimit;
    [UnityEngine.HideInInspector]
    public string[] maskStrings;
    public float maximumDistance;
    public bool drawDebugRays;
    private Camera mainCam;
    private s3dCamera camScript;
    private GameObject leftMask;
    private GameObject rightMask;
    private Camera lCam;
    private Camera rCam;
    private float cutInL;
    private float cutInR;
    public virtual void Start()
    {
        this.mainCam = (Camera) this.gameObject.GetComponent(typeof(Camera));
        this.camScript = (s3dCamera) this.gameObject.GetComponent(typeof(s3dCamera));
        this.lCam = this.camScript.leftCam.GetComponent<Camera>();
        this.rCam = this.camScript.rightCam.GetComponent<Camera>();
        GameObject masks = new GameObject("masks");
        this.leftMask = new GameObject("leftMask");
        this.leftMask.transform.parent = masks.transform;
        this.leftMask.layer = LayerMask.NameToLayer("Ignore Raycast");
        MeshFilter filterL = (MeshFilter) this.leftMask.AddComponent(typeof(MeshFilter));
        this.leftMask.AddComponent(typeof(MeshRenderer));
        Mesh leftMesh = filterL.mesh;
        leftMesh.Clear();
        leftMesh.vertices = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
        leftMesh.normals = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
        leftMesh.triangles = new int[] {0, 2, 1, 0, 3, 2};
        leftMesh.uv = new Vector2[] {new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1)};
        this.leftMask.GetComponent<Renderer>().material = new Material(Shader.Find("Self-Illumin/Diffuse"));
        this.leftMask.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
        this.leftMask.GetComponent<Renderer>().castShadows = false;
        this.rightMask = new GameObject("rightMask");
        this.rightMask.transform.parent = masks.transform;
        this.rightMask.layer = LayerMask.NameToLayer("Ignore Raycast");
        MeshFilter filterR = (MeshFilter) this.rightMask.AddComponent(typeof(MeshFilter));
        this.rightMask.AddComponent(typeof(MeshRenderer));
        Mesh rightMesh = filterR.mesh;
        rightMesh.Clear();
        rightMesh.vertices = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
        rightMesh.normals = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
        rightMesh.triangles = new int[] {0, 2, 1, 0, 3, 2};
        rightMesh.uv = new Vector2[] {new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1)};
        this.rightMask.GetComponent<Renderer>().material = new Material(Shader.Find("Self-Illumin/Diffuse"));
        this.rightMask.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
        this.rightMask.GetComponent<Renderer>().castShadows = false;
    }

    public virtual void toggleVis(object a)
    {
        if (a != null)
        {
            this.leftMask.GetComponent<Renderer>().enabled = true;
            this.rightMask.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            this.leftMask.GetComponent<Renderer>().enabled = false;
            this.rightMask.GetComponent<Renderer>().enabled = false;
        }
    }

    public virtual void Update()
    {
        RaycastHit hit = default(RaycastHit);
        if (this.on)
        {
            bool leftBool = false;
            float leftDepth = Mathf.Infinity;
            Vector3 leftCoord = Vector3.zero;
            int yy = 0;
            while (yy < this.sideSamples)
            {
                Ray ray = this.lCam.ViewportPointToRay(new Vector3(1, yy / (this.sideSamples - 1f), 0)); // test lCam along right edge
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (this.drawDebugRays)
                    {
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, new Color(0, 1, 1, 1));
                    }
                    if (hit.distance < this.camScript.zeroPrlxDist)
                    {
                        leftBool = true;
                        if (hit.distance < leftDepth)
                        {
                            leftDepth = hit.distance;
                            leftCoord = hit.point; // absolute point in world space where right edge of left view was violated
                        }
                    }
                }
                yy++;
            }
            if (leftBool)
            {
                this.cutInR = this.rCam.WorldToViewportPoint(leftCoord).x; // x coord to cut in
            }
            else
            {
                this.cutInR = 1;
            }
            bool rightBool = false;
            float rightDepth = Mathf.Infinity;
            Vector3 rightCoord = Vector3.zero;
            yy = 0;
            while (yy < this.sideSamples)
            {
                ray = this.rCam.ViewportPointToRay(new Vector3(0, yy / (this.sideSamples - 1f), 0)); // test rCam along left edge
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (this.drawDebugRays)
                    {
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, new Color(1, 0, 0, 1));
                    }
                    if (hit.distance < this.camScript.zeroPrlxDist)
                    {
                        rightBool = true;
                        if (hit.distance < rightDepth)
                        {
                            rightDepth = hit.distance;
                            rightCoord = hit.point; // absolute point in world space where left edge of right view was violated
                        }
                    }
                }
                yy++;
            }
            if (rightBool)
            {
                this.cutInL = this.lCam.WorldToViewportPoint(rightCoord).x;
            }
            else
            {
                this.cutInL = 0;
            }
        }
    }

    public virtual void LateUpdate()
    {
        if (this.on)
        {
            Mesh leftMesh = ((MeshFilter) this.leftMask.GetComponent(typeof(MeshFilter))).mesh;
            Vector3[] vertsL = leftMesh.vertices;
            vertsL[0] = this.lCam.ViewportToWorldPoint(new Vector3(0, 1, this.lCam.nearClipPlane)); // near upper left
            vertsL[1] = this.lCam.ViewportToWorldPoint(new Vector3(0, 0, this.lCam.nearClipPlane)); // near lower left
            if (this.maskLimit == maskDistance.FarFrustum)
            {
                vertsL[2] = this.lCam.ViewportToWorldPoint(new Vector3(this.cutInL, 0, this.lCam.farClipPlane)); // far lower left
                vertsL[3] = this.lCam.ViewportToWorldPoint(new Vector3(this.cutInL, 1, this.lCam.farClipPlane)); // far upper left
            }
            else
            {
                if (this.maskLimit == maskDistance.ScreenPlane)
                {
                    vertsL[2] = this.lCam.ViewportToWorldPoint(new Vector3(this.cutInL, 0, this.camScript.zeroPrlxDist)); // far lower left
                    vertsL[3] = this.lCam.ViewportToWorldPoint(new Vector3(this.cutInL, 1, this.camScript.zeroPrlxDist)); // far upper left
                }
                else
                {
                    if (this.maskLimit == maskDistance.MaxDistance)
                    {
                        vertsL[2] = this.lCam.ViewportToWorldPoint(new Vector3(this.cutInL, 0, this.maximumDistance)); // far lower left
                        vertsL[3] = this.lCam.ViewportToWorldPoint(new Vector3(this.cutInL, 1, this.maximumDistance)); // far upper left
                    }
                }
            }
            Vector3 vecL1 = vertsL[3] - vertsL[0];
            Vector3 vecL2 = vertsL[1] - vertsL[0];
            Vector3 normL = Vector3.Cross(vecL1, vecL2);
            leftMesh.vertices = vertsL;
            leftMesh.normals = new Vector3[] {normL, normL, normL, normL};
            Bounds leftBounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (Vector3 vert in vertsL)
            {
                leftBounds.Encapsulate(vert);
            }
            leftMesh.bounds = leftBounds;
            Mesh rightMesh = ((MeshFilter) this.rightMask.GetComponent(typeof(MeshFilter))).mesh;
            Vector3[] vertsR = rightMesh.vertices;
            if (this.maskLimit == maskDistance.FarFrustum)
            {
                vertsR[0] = this.rCam.ViewportToWorldPoint(new Vector3(this.cutInR, 1, this.rCam.farClipPlane)); // far upper right
                vertsR[1] = this.rCam.ViewportToWorldPoint(new Vector3(this.cutInR, 0, this.rCam.farClipPlane)); // far lower right
            }
            else
            {
                if (this.maskLimit == maskDistance.ScreenPlane)
                {
                    vertsR[0] = this.rCam.ViewportToWorldPoint(new Vector3(this.cutInR, 1, this.camScript.zeroPrlxDist)); // far upper right
                    vertsR[1] = this.rCam.ViewportToWorldPoint(new Vector3(this.cutInR, 0, this.camScript.zeroPrlxDist)); // far lower right
                }
                else
                {
                    if (this.maskLimit == maskDistance.MaxDistance)
                    {
                        vertsR[0] = this.rCam.ViewportToWorldPoint(new Vector3(this.cutInR, 1, this.maximumDistance)); // far upper right
                        vertsR[1] = this.rCam.ViewportToWorldPoint(new Vector3(this.cutInR, 0, this.maximumDistance)); // far lower right
                    }
                }
            }
            vertsR[2] = this.rCam.ViewportToWorldPoint(new Vector3(1, 0, this.rCam.nearClipPlane)); // near lower right
            vertsR[3] = this.rCam.ViewportToWorldPoint(new Vector3(1, 1, this.rCam.nearClipPlane)); // near upper right
            Vector3 vecR1 = vertsR[3] - vertsR[0];
            Vector3 vecR2 = vertsR[1] - vertsR[0];
            Vector3 normR = Vector3.Cross(vecR1, vecR2);
            rightMesh.vertices = vertsR;
            rightMesh.normals = new Vector3[] {normR, normR, normR, normR};
            Bounds rightBounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (Vector3 vert in vertsR)
            {
                rightBounds.Encapsulate(vert);
            }
            rightMesh.bounds = rightBounds;
        }
    }

    public s3dWindow()
    {
        this.on = true;
        this.sideSamples = 15;
        this.maskLimit = maskDistance.ScreenPlane;
        this.maskStrings = new string[] {"Max Distance", "Screen Plane", "Far Frustum"};
        this.maximumDistance = 2;
        this.drawDebugRays = true;
    }

}