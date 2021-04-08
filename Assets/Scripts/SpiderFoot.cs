using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class SpiderFoot : MonoBehaviour
{
    [SerializeField] Transform footMountPoint;
    [SerializeField] Transform nextStepTarget;
    [SerializeField] Transform footIKTarget;
    [SerializeField] float maxDistance = 10;
    [SerializeField] float speed;
    [SerializeField] float stepHeight;
    
    bool isNotGrounded;
    float _distance;
    Vector3 newPosition;
    Vector3 oldPosition;
    float percentage;
    
    public bool IsNotGrounded => isNotGrounded;
   


    void Awake()
    {
        _distance = (transform.position - footMountPoint.position).magnitude;
        newPosition = footIKTarget.position;
        oldPosition = newPosition;
        percentage = 0;
    }

    void Update()
    {
        CheckDistance();
        MoveIntoNewPosition();
    }

    void CalculateNewPosition()
    {
        newPosition = (nextStepTarget.position) + nextStepTarget.up * (_distance);
        oldPosition = footIKTarget.position;
        percentage = 0;
    }

    void MoveIntoNewPosition()
    {
        Vector3 firstHalf = new Vector3(newPosition.x, newPosition.y + 2 * stepHeight, newPosition.z);
        Vector3 secondHalf = new Vector3(oldPosition.x, oldPosition.y + 2 * stepHeight, oldPosition.z);
        Vector3 toPosition = new Vector3();
        Vector3 fromPosition = new Vector3();
        if (percentage < 0.5f)
        {
            toPosition = firstHalf;
            fromPosition = oldPosition;
        }

        if (percentage >= 0.5f)
        {
            toPosition = newPosition;
            fromPosition = secondHalf;
        }

        footIKTarget.position = Vector3.Lerp(fromPosition, toPosition, percentage);;
        percentage += speed * Time.deltaTime;
        if (percentage >= 1)
            isNotGrounded = false;
    }

    void CheckDistance()
    {
        var distanceToTarget = (transform.position - nextStepTarget.position).magnitude;
        if ( distanceToTarget >= maxDistance&&isNotGrounded==false)
        {
            CalculateNewPosition();
            isNotGrounded = true;
        }
    }
}
