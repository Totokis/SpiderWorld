using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    [SerializeField] Transform targetToMove;//TODO wystarczyłoby puszczać raycast z pozycji o np 10 wyższej niż nasz targetToMove
    int _layerMask;
    
    void Awake()
    {
        _layerMask = LayerMask.GetMask("Terrain");
    }
    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,Mathf.Infinity,_layerMask))
        {
            targetToMove.position = hit.point;
        }
        Debug.DrawRay(targetToMove.position,Vector3.left*1f,Color.red);
    }
}
