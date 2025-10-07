using UnityEngine;


public class CameraFollower : MonoBehaviour
{
    [SerializeField] private float _yBound;
    [SerializeField] private float _offset;
    [SerializeField] private float _lerbStrength;


    public bool Following { get; set; } = true;

    public Transform Target { get; set; }

    private float _targetY;

    public float TargetY
    {
        get { return _targetY; }
        set { _targetY = value; }
    }

    public float Offset
    {
        get { return _offset; }
        set { _offset = value; }
    }

    public float FollowY
    {
        get { return transform.position.y + _offset; }
        set
        {
            var position = transform.position;
            position.y = value - _offset;
            transform.position = position;
        }
    }

   

    private void FixedUpdate()
    {
        if (!Following)
        {
            return;
        }

        if(Target==null)
            return;
        
        if (Target.position.y > _targetY-0.1f)
        {

        }
        else
        {
            _targetY = Target.position.y;
        }

        
        FollowY = Mathf.Lerp(FollowY, _targetY,
            _lerbStrength*Mathf.InverseLerp(0, _yBound, Mathf.Abs(_targetY - FollowY))*Time.fixedDeltaTime);
    }

}
