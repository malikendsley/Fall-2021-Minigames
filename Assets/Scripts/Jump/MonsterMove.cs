using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class MonsterMove : MonoBehaviour
{
    // best practice is to wrap member fields in properties
    // and then manually define read/write access

    // member field = class variable (indicated by underscore)
    // local variables don't have underscore

    // [SerializeField] lets you serialize private variables
    // (i.e. make them editable in inspector)

    // OnDrawGizmos lets you draw widgets in the editor scene view
    // (helpful for visualizing transforms)

    [SerializeField]
    [Tooltip("How fast should the monster move?")]
    float _averageMoveSpeed = 2f; // measured in meters per seconds

    public float AverageMoveSpeed
    {
        get { return _averageMoveSpeed; }
        set {
            _averageMoveSpeed = value;

            float dist = (_minPos - _maxPos).magnitude;
            _movePeriod = dist / value;
        }
    }

    [SerializeField]
    [HideInInspector]
    float _movePeriod = 4f; // measured in seconds

    public float MovePeriod
    {
        get { return _movePeriod; }
        set
        {
            _movePeriod = value;

            float dist = (_minPos - _maxPos).magnitude;
            _averageMoveSpeed = dist / value;
        }
    }

    [SerializeField]
    [HideInInspector]
    Vector3 _origin; 

    public Vector3 Origin
    {
        get { return _origin; }
    }

   // [SerializeField]
   // Vector3 _minPos;

    public Vector3 MinPos
    {
        get { return _minPos; }
        set { _minPos = value; }
    }
    public Transform startPlatform;
    public Transform endPlatform;

    //[SerializeField]
    //Vector3 _maxPos;

    

    public Vector3 MaxPos
    {
        get { return _maxPos; }
        set { _maxPos = value; }
    }

    float _fudgeFactor = 0.03f;

    Vector3 _targetPos;
    // float _startTime;

    float _timeCount; 
    Vector3 _maxPos;
    Vector3 _minPos;

    void Start()
    {
        _maxPos = endPlatform.position;
        _minPos = startPlatform.position;
        _targetPos = _minPos;
        // _startTime = Time.time;
        _timeCount = 0f;

        _origin = transform.position;
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            if (transform.hasChanged)
            {
                _origin = transform.position;
            }
        }
        else
        {

            _timeCount += Time.deltaTime;

            // percentage of half a unit circle rotation
            float t = (_timeCount / _movePeriod + 0.005f) * Mathf.PI;

            // what's the amplitude of the sign curve? (varies 0 to 1)
            float percent = Mathf.Clamp(0.5f * (Mathf.Sin(t - 0.5f * Mathf.PI) + 1f), 0f, 1f);


            // do we need to switch the target?
            if ((_targetPos - transform.position).magnitude <= _fudgeFactor && _timeCount >= _movePeriod)
            {
                // Debug.Log($"Flipped after t= {t} / 2Pi, %= {percent} / 1");

                FlipDirection();
            }
            else
            {

                // there's floating point precision issues with dividing -- as elapsed time increases, we start getting drift
                // float percent = _movePeriod * (Time.time / _startTime - 1f) + 0.005f;

                // Debug.Log($"{t}, {percent}");

                Vector3 last = InvertTargetPosition();

                transform.position = (_targetPos - last) * percent + last + _origin;
            }
        }

    }

    Vector3 InvertTargetPosition()
    {
        return (_targetPos - _minPos).magnitude <= _fudgeFactor ? _maxPos : _minPos;
    }

    public void FlipDirection()
    {
        _targetPos = InvertTargetPosition();
        _timeCount = 0;
    }

    public void SetRandomDirection()
    {
        _targetPos = Random.Range(0, 2) == 1 ? _minPos : _maxPos;
        _timeCount = 0; 
    }

    // visualize the monster's path in editor (optional)
    void OnDrawGizmos()
    {

        Color originColor, targetColor, lineColor;

        originColor = Color.red;
        targetColor = Color.green;
        lineColor = Color.yellow;

        Gizmos.color = lineColor;
        Gizmos.DrawLine(_origin + _minPos, _origin + _maxPos);

        Gizmos.color = originColor;
        Gizmos.DrawSphere(_origin + _minPos, 0.1f);

        Gizmos.color = targetColor;
        Gizmos.DrawSphere(_origin + _maxPos, 0.1f);
    }
}