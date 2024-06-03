using System;
using System.Collections;
using System.Collections.Generic;
using _IUTHAV.Scripts.ComicPanel;
using UnityEngine;

public class VectorTest : MonoBehaviour
{
    [SerializeField] private Transform u;

    [SerializeField]
    [Range(-1f, 1f)] private float dX = 0; 
    [SerializeField]
    [Range(-1f, 1f)] private float dY = 0; 
    [SerializeField]
    [Range(-1f, 1f)] private float dZ = 0;


    private Vector3 initPos;
    
    private static Vector3 resultZ;
    private static Vector3 resultY;
    private static Vector3 resultX; 

    private void Start()
    {
        initPos = transform.position;
        
        resultZ = u.transform.rotation * Vector3.forward;
        resultY = u.transform.rotation * Vector3.up;
        resultX = u.transform.rotation * Vector3.right;
    }

    private void Update()
    {
        CameraMovement.InitProjection(u, transform.position);
        Debug.DrawRay(transform.position, u.rotation * Vector3.forward, Color.blue);
        Debug.DrawRay(transform.position, u.rotation * Vector3.right, Color.red);
        Debug.DrawRay(transform.position, u.rotation * Vector3.up, Color.green);
        
        Vector3 x = new Vector3(dX, 0,0);
        Vector3 y = new Vector3(0, dY, 0);
        Vector3 z = new Vector3(0, 0, dZ);
            
        Vector3 output;
        output = z.z < 0 ? z.magnitude * -resultZ : z.magnitude * resultZ;
        output += x.x < 0 ? x.magnitude * -resultX : x.magnitude * resultX;
        output += y.y < 0 ? y.magnitude * -resultY : y.magnitude * resultY;

        transform.position = output + initPos; 
    }
}
