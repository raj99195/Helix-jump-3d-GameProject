using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour,IInitializable
{
//    public static readonly Color BOOSTED_COLOR = Color.red;

    public event Action<Collider> Bounced;
    public event Action<Collider,bool> CollisionEntered; 
    public event Action Died;
    public event Action<bool> BoostedStageChanged; 

    [SerializeField] private float _size;
    [SerializeField] private float _maxVel;
    [SerializeField] private Vector2 _minAndMaxAngularVel;
    [SerializeField]private List<GameObject> _effects = new List<GameObject>();
    [SerializeField] private List<GameObject> _boostedEffects = new List<GameObject>();
    [SerializeField] private Transform _modelContentTransform;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField]private List<Sprite> _spatterImages = new List<Sprite>();
    [SerializeField] private SpatterTile _spatterRenderer;
    [SerializeField] private ParticleSystem _jumpParticleSystem;
    [SerializeField]private Color _boostedColor = Color.red;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private AudioClip _jumpClip, _dieClip;
    [Range(0,1f)]
    [SerializeField] private float _smoothingFactor;

    [SerializeField] private float _maxSweep = 1f;

    private  bool _isDied;

    public float Velocity { get; private set; }

    public Transform Rod { get; set; }

//    public bool Active { get; set; }

    private float _targetSweep;
    private float _currentSweep;

    private float _lastPosition;
    private bool _active;
    private Color _normalColor;
    private bool _boosted;

    private Vector3 _modelAngularVelocity;

    public bool IsDied
    {
        get { return _isDied; }
         set
        {
            if(value == _isDied)
                return;
            
            _isDied = value;
            if(value)
            Died?.Invoke();
        }
    }

    public bool Active
    {
        get { return _active && !IsDied; }
        set
        {
            if (!value)
                Boosted = false;
            _active = value;
        }
    }

    public bool Initialized { get; private set; }

    public bool IsZeroGravity { get; set; }

    public bool Boosted
    {
        get { return _boosted; }
        set
        {
            if(value == _boosted)
                return;

            Color = value ?BoostedColor: NormalColor;
            _boostedEffects.ForEach(go => go.SetActive(value));
            _boosted = value;
//            if (_boosted)
//            {
//                Velocity = _jumpSpeed;
//                IsZeroGravity = true;
//            }

            BoostedStageChanged?.Invoke(value);
        }
    }

    private Color BoostedColor => Color.Lerp(NormalColor, _boostedColor, 0.7f);

    public Color Color
    {
        get { return _meshRenderer?.material.color??Color.white; }
        set
        {
            MeshRenderer.material.color = value;
            var color = Color.Lerp(value,Color.white, 0.3f);
            color.a = 0.8f;
            _trailRenderer.startColor = color;
            var main = _jumpParticleSystem.main;
            main.startColor = value;
            color.a = 0.2f;
            _trailRenderer.endColor = color;
        }
    }

    public MeshRenderer MeshRenderer
    {
        get { return _meshRenderer; }
        private set
        {
            value.material.color = Color;
            if (_meshRenderer!=null)
            {
                Destroy(_meshRenderer);
            }
            _meshRenderer = value;
        }
    }

    public Color NormalColor
    {
        get { return _normalColor; }
        set
        {
            if(Boosted)
                throw new InvalidOperationException();
            Color = value;
            _normalColor = value;
        }
    }

    private void Awake()
    {
        
       
    }

    private void OnEnable()
    {
        ResourceManager.PlayerSkinSelectionChanged +=ResourceManagerOnPlayerSkinSelectionChanged;
    }

  
    private void OnDisable()
    {
        ResourceManager.PlayerSkinSelectionChanged -= ResourceManagerOnPlayerSkinSelectionChanged;
    }

    public void Init()
    {
        var skin = ResourceManager.GetSkinById(ResourceManager.GetSelectedSkin());
        MeshRenderer = Instantiate(skin.meshPrefab, _modelContentTransform);
    }

    private void ResourceManagerOnPlayerSkinSelectionChanged(PlayerSkin playerSkin)
    {
        MeshRenderer = Instantiate(playerSkin.meshPrefab, _modelContentTransform);
    }


    private void Update()
    {
        if(!Active)
            return;
        _currentSweep = Mathf.Lerp(_currentSweep, _targetSweep, Time.deltaTime * 50f);
        Rod.Rotate(Vector3.up,-_currentSweep*_smoothingFactor);

//        if (Boosted && Velocity <= 0)
//        {
//            Boosted = false;
//        }

        if (Input.GetMouseButtonDown(0))
        {
            _lastPosition = Input.mousePosition.x;
        }

        if (Input.GetMouseButton(0))
        {
            _targetSweep = Mathf.Clamp((Input.mousePosition.x - _lastPosition),-_maxSweep,_maxSweep);
            _lastPosition = Input.mousePosition.x;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _targetSweep = 0;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Boosted = !Boosted;
        }
    }

    // ReSharper disable once MethodTooLong
    // ReSharper disable once ExcessiveIndentation
    private void FixedUpdate()
    {
        if (!Active)
            return;

        if (Velocity < 0 && Physics.Raycast(transform.position, -Vector3.up, out var hit, Mathf.Abs(Velocity) * Time.fixedDeltaTime + 0.05f+_size,~(1<<11)))
        {
            if (!Boosted && hit.transform.gameObject.layer == (int)Layer.Enemy)
            {
                transform.position = hit.point + Vector3.up * _size;
                AddBounceEffect(hit);
                Die(hit.transform);
                return;
            }
           
            if (
                (hit.transform.gameObject.layer == (int)Layer.Platform || (Boosted && hit.transform.gameObject.layer == (int)Layer.Enemy)))
            {
                Bounce(hit,Boosted?0.8f:1f);
            }
            else
            {
                transform.position = hit.point + Vector3.up * _size;
            }

            _modelAngularVelocity = (new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized
                                    *Random.Range(_minAndMaxAngularVel.x,_minAndMaxAngularVel.y);
            AddBounceEffect(hit);

            CollisionEntered?.Invoke(hit.collider, true);
        }

        if (!Active)
            return;

        transform.Translate(0, Velocity * Time.fixedDeltaTime, 0);
        Velocity += IsZeroGravity ? 0 : Physics.gravity.y * Time.fixedDeltaTime;
        Velocity = Mathf.Clamp(Velocity, -_maxVel, _maxVel);
        _modelContentTransform.Rotate(_modelAngularVelocity*Time.fixedDeltaTime,Space.World);
    }

    private void AddBounceEffect(RaycastHit hit)
    {
        var ren = Instantiate(_spatterRenderer);
        ren.transform.parent = hit.transform;
        ren.Image = _spatterImages[Random.Range(0, _spatterImages.Count)];
        ren.Color = NormalColor;
        ren.transform.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * Quaternion.AngleAxis(90, Vector3.right);
        ren.transform.position = hit.point + hit.normal * 0.005f;
    }

    private void Bounce(RaycastHit hit,float speedFactor=1)
    {
        Velocity = _jumpSpeed*speedFactor;
        Velocity += Physics.gravity.y * Time.fixedDeltaTime;
        _jumpParticleSystem.Play();
        if (AudioManager.IsSoundEnable && _jumpClip != null)
        {
            AudioSource.PlayClipAtPoint(_jumpClip, transform.position, 0.5f);
        }
        Bounced?.Invoke(hit.collider);
    }


    public void Boost()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _size);
    }

    private void OnTriggerEnter(Collider other)
    {
//        Debug.Log(nameof(OnTriggerEnter) + $" {other.gameObject.name}");
//        if (other.gameObject.layer == (int) Layer.Enemy)
//        {
//            Die();
//        }
//        CollisionEntered?.Invoke(other);
    }

    // ReSharper disable once MethodNameNotMeaningful
    private void Die(Transform parent)
    {
        Velocity = 0f;
        _targetSweep = 0;
        _currentSweep = 0;
        Boosted = false;
        _effects.ForEach(g => g.SetActive(false));
        transform.parent = parent;
        if (AudioManager.IsSoundEnable && _dieClip!=null)
        {
            AudioSource.PlayClipAtPoint(_dieClip,transform.position, 0.5f);
        }
        IsDied = true;
    }
}