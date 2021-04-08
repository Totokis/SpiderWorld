using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    [SerializeField] Transform targetToMove;//
                                            //TODO wystarczyłoby puszczać raycast z pozycji o np 10 wyższej niż nasz targetToMove
    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            targetToMove.position = hit.point;
        }
        Debug.DrawRay(targetToMove.position,Vector3.left*15f,Color.red);
    }
}
