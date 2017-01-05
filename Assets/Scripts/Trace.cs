﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trace : MonoBehaviour
{
    public Camera PlayerCamera;
    public GameObject Football;
    private RectTransform tran;

    private Vector3 pos; 
    // Use this for initialization
    void Start () {
        tran = GetComponent<RectTransform>();
        pos = tran.localPosition;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    var v = Football.transform.position - PlayerCamera.transform.position;
	    v.y = 0;
	    var v2 = PlayerCamera.transform.forward;
	    v2.y = 0;
	    var r = Vector3.ProjectOnPlane(v.normalized, v2);
	    if (Vector3.Dot(v, v2) < 0)
	    {
	        
            tran.localPosition = pos + r.normalized * 200;
        }
	    else
	    {
            tran.localPosition = pos + r * 200;
        }
	    
	}
}
