using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class SpiderFoot : MonoBehaviour
{
    [SerializeField] Transform footMountPoint;
    [SerializeField] Transform nextStepTarget;
    [SerializeField] Transform footIKTarget;
    [SerializeField] float maxDistance = 1.5f;
    [SerializeField] float speed = 10f;
    [SerializeField] float stepHeight = 1f;
    
    public bool CanMove { get=>_canMove; set => _canMove = value; }

    public bool IsNotGrounded => _isNotGrounded;
    
    Vector3 _newPosition;
    Vector3 _oldPosition;
    bool _canMove;
    bool _isNotGrounded;
    float _distance;
    float _percentage;
    
    void Awake()
    {
        _distance = (transform.position - footMountPoint.position).magnitude;
        _newPosition = footIKTarget.position;
        _oldPosition = _newPosition;
        _percentage = 0;
        _canMove = true;
    }

    void Update()
    {
        CheckDistance();
        MoveIntoNewPosition();
    }

    void CalculateNewPosition()
    {
        _newPosition = (nextStepTarget.position) + nextStepTarget.up * (_distance);
        _oldPosition = footIKTarget.position;
        _percentage = 0;
    }

    void MoveIntoNewPosition()
    {
        Vector3 firstHalf = new Vector3(_newPosition.x, _newPosition.y + 2 * stepHeight, _newPosition.z);
        Vector3 secondHalf = new Vector3(_oldPosition.x, _oldPosition.y + 2 * stepHeight, _oldPosition.z);
        Vector3 toPosition = new Vector3();
        Vector3 fromPosition = new Vector3();
        if (_percentage < 0.5f)
        {
            toPosition = firstHalf;
            fromPosition = _oldPosition;
        }

        if (_percentage >= 0.5f)
        {
            toPosition = _newPosition;
            fromPosition = secondHalf;
        }

        footIKTarget.position = Vector3.Lerp(fromPosition, toPosition, _percentage);;
        _percentage += speed * Time.deltaTime;
        if (_percentage >= 1)
            _isNotGrounded = false;
    }

    void CheckDistance()
    {
        var distanceToTarget = (transform.position - nextStepTarget.position).magnitude;
        if ( distanceToTarget >= maxDistance&&_isNotGrounded==false && _canMove)
        {
            CalculateNewPosition();
            _isNotGrounded = true;
        }
    }
}
