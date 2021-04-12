using UnityEngine;
using UnityEngine.Serialization;

public class SpiderBody : MonoBehaviour
{
    [SerializeField] Transform frontLeft;
    [SerializeField] Transform frontRight;
    [SerializeField] Transform backLeft;
    [SerializeField] Transform backRight;
    [SerializeField] float offset = 0.2f;
    [SerializeField] bool start = false;
    SpiderFoot _frontLeftFoot;
    SpiderFoot _frontRightFoot;
    SpiderFoot _backLeftFoot;
    SpiderFoot _backtRightFoot;

    SpiderFoot[] _firstPair;
    SpiderFoot[] _secondPair;
    bool _waitForFirstPairMove;
    bool _waitForFirstPairToGround;
    bool _waitForSecondPairMove;
    bool _waitForSecondPairToGround;

    void Awake()
    {
        _firstPair = new SpiderFoot[2];
        _secondPair = new SpiderFoot[2];
        CacheSpiderFeets();
        UnlockFeets(_firstPair);
        LockFeets(_secondPair);
        _waitForFirstPairMove = true;
    }
    void CacheSpiderFeets()
    {

        foreach (SpiderFoot child in frontLeft.GetComponentsInChildren<SpiderFoot>())
        {
            _frontLeftFoot = child;
            _firstPair[0] = _frontLeftFoot;
            break;
        }
        foreach (SpiderFoot child in frontRight.GetComponentsInChildren<SpiderFoot>())
        {
            _frontRightFoot = child;
            _secondPair[0] = _frontRightFoot;
            break;
        }
        foreach (SpiderFoot child in backLeft.GetComponentsInChildren<SpiderFoot>())
        {
            _backLeftFoot = child;
            _secondPair[1] = _backLeftFoot;
            break;
        }
        foreach (SpiderFoot child in backRight.GetComponentsInChildren<SpiderFoot>())
        {
            _backtRightFoot = child;
            _firstPair[1] = _backtRightFoot;
            break;
        }
    }
    
    void UnlockFeets(SpiderFoot[] table)
    {
        foreach (var foot in table)
        {
            foot.CanMove = true;
        }
    }
    void LockFeets(SpiderFoot[] table)
    {
        foreach (var foot in table)
        {
            foot.CanMove = false;
        }
    }
    
    void Update()
    {
        MoveLegsZigZag();
       if (start)
       {
           SetHeight();
       }
    }
    void SetHeight()
    {
        var nominator = _backLeftFoot.transform.position + _backtRightFoot.transform.position + _frontLeftFoot.transform.position + _frontRightFoot.transform.position;
        var averagePosition = nominator / 4f;
        averagePosition.y = averagePosition.y + offset;
        var newPosition = new Vector3(transform.position.x, averagePosition.y, transform.position.z);
        if (Vector3.Distance(transform.position, newPosition) > 0.3f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, 0.5f);
        }
        
    }

    void MoveLegsZigZag()
    {
        if (_waitForFirstPairMove && _firstPair[0].IsNotGrounded && _firstPair[1].IsNotGrounded)
        {
            _waitForFirstPairToGround = true;
            _waitForFirstPairMove = false;
            Debug.Log("First Check");
            
        }
        if (_waitForFirstPairToGround && !_firstPair[0].IsNotGrounded && !_firstPair[1].IsNotGrounded)
        {
            _waitForFirstPairToGround = false;
            LockFeets(_firstPair);
            UnlockFeets(_secondPair);
            _waitForSecondPairMove = true;
           
            Debug.Log("Second Check");

        }
        if (_waitForSecondPairMove && _secondPair[0].IsNotGrounded && _secondPair[1].IsNotGrounded)
        {
            _waitForSecondPairToGround = true;
            _waitForSecondPairMove = false;
            Debug.Log("Third Check");
        }
        if (_waitForSecondPairToGround && !_secondPair[0].IsNotGrounded && !_secondPair[1].IsNotGrounded)
        {
            _waitForSecondPairToGround = false;
            LockFeets(_secondPair);
            UnlockFeets(_firstPair);
            _waitForFirstPairMove = true;
            Debug.Log("Fourth Check");
        }
    }
}
