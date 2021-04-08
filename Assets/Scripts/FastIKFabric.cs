using UnityEditor;
using UnityEngine;

public class FastIKFabric : MonoBehaviour
{
    public int chainLength = 2;

    public Transform target;

    public Transform pole;

    [Header("Solver Parameters")] public int iterations = 10;

    public float delta = 0.001f;

    [Range(0, 1)] public float snapBackStrength = 1f;
    private Transform[] _bones;

    private float[] _bonesLength;
    private float _completeLength;
    private Vector3[] _positions;

    private Vector3[] _startDirectionSucc;
    private Quaternion[] _startRotationBone;
    private readonly Quaternion _startRotationRoot;
    private Quaternion _startRotationTarget;

    public FastIKFabric(Quaternion startRotationRoot)
    {
        _startRotationRoot = startRotationRoot;
    }

    private void Awake()
    {
        Init();
    }
    
    private void LateUpdate()
    {
        ResolveIK();
    }

    private void OnDrawGizmos()
    {
        var current = transform;
        for (var i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position,
                Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position),
                new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }

    private void Init()
    {
        _bones = new Transform[chainLength + 1];
        _positions = new Vector3[chainLength + 1];
        _bonesLength = new float[chainLength];
        _startDirectionSucc = new Vector3[chainLength + 1];
        _startRotationBone = new Quaternion[chainLength + 1];

        if (target == null)
        {
            target = new GameObject(gameObject.name + " Target").transform;
            target.position = transform.position;
        }

        _startRotationTarget = target.rotation;
        _completeLength = 0;

        var current = transform;
        for (var i = _bones.Length - 1; i >= 0; i--)
        {
            _bones[i] = current;
            _startRotationBone[i] = current.rotation;

            if (i == _bones.Length - 1)
            {
                _startDirectionSucc[i] = target.position - current.position;
            }
            else
            {
                _startDirectionSucc[i] = _bones[i + 1].position - current.position;
                _bonesLength[i] = (_bones[i + 1].position - current.position).magnitude;
                _completeLength += _bonesLength[i];
            }

            current = current.parent;
        }
    }


    private void ResolveIK()
    {
        if (target == null)
            return;

        if (_bonesLength.Length != chainLength)
            Init();

        //get positions
        for (var i = 0; i < _bones.Length; i++) _positions[i] = _bones[i].position;

        var RootRot = _bones[0].parent != null ? _bones[0].parent.rotation : Quaternion.identity;
        var RootRotDiff = RootRot * Quaternion.Inverse(_startRotationRoot);

        if ((target.position - _bones[0].position).sqrMagnitude >= _completeLength * _completeLength
        ) //faster than "normal" distance checking, when we compare magnitiude and complete length
        {
            var direction = (target.position - _positions[0]).normalized;

            for (var i = 1; i < _positions.Length; i++)
                _positions[i] = _positions[i - 1] + direction * _bonesLength[i - 1];
        }
        else
        {
            for (var iteration = 0; iteration < iterations; iteration++)
            {
                // back
                for (var i = _positions.Length - 1; i > 0; i--)
                    if (i == _positions.Length - 1)
                        _positions[i] = target.position;
                    else
                        _positions[i] = _positions[i + 1] +
                                        (_positions[i] - _positions[i + 1]).normalized * _bonesLength[i];

                // forward
                for (var i = 1; i < _positions.Length; i++)
                    _positions[i] = _positions[i - 1] +
                                    (_positions[i] - _positions[i - 1]).normalized * _bonesLength[i - 1];

                if ((_positions[_positions.Length - 1] - target.position).sqrMagnitude < delta * delta) break;
            }
        }

        if (pole != null)
            for (var i = 1; i < _positions.Length - 1; i++)
            {
                var plane = new Plane(_positions[i + 1] - _positions[i - 1], _positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(pole.position);
                var projectedBone = plane.ClosestPointOnPlane(_positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - _positions[i - 1], projectedPole - _positions[i - 1],
                    plane.normal);
                _positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (_positions[i] - _positions[i - 1]) +
                                _positions[i - 1];
            }


        //set position
        for (var i = 0; i < _positions.Length; i++)
        {
            if (i == _positions.Length - 1)
                _bones[i].rotation = target.rotation * Quaternion.Inverse(_startRotationTarget) * _startRotationBone[i];
            else
                _bones[i].rotation = Quaternion.FromToRotation(
                    _startDirectionSucc[i],
                    _positions[i + 1] - _positions[i]
                ) * _startRotationBone[i];
            _bones[i].position = _positions[i];
        }
    }
}