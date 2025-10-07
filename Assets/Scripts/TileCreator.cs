using System;
using System.Collections.Generic;
using UnityEngine;

public class TileCreator : MonoBehaviour
{

    public event Action<ICycleTile> CreatedTile;
    public event Action<ICycleTile> RemovedTile;

    private const float WORLD_WIDTH = Constants.WORLD_WIDTH;
    private readonly List<ICycleTile> _tileList = new List<ICycleTile>();

    [SerializeField] private float _coverage;
    [SerializeField] private Transform _startPointTrans;
    [SerializeField] private TileCreatorPrefabSelector _prefabSelector;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private Transform _targetTransform;

    public bool IsCreating { get; private set; }


    public Transform TargetTransform
    {
        get { return _targetTransform; }
        set { _targetTransform = value; }
    }

    public float Coverage
    {
        get { return _coverage; }
        set { _coverage = value; }
    }

    public IEnumerable<ICycleTile> Tiles => _tileList;

    public void StartCreating()
    {

        IsCreating = true;
        Update();
    }

    public void StopCreating()
    {
        IsCreating = false;
    }

    private void Start()
    {
        //TODO:Remove After Test
        StartCreating();
    }



    private void OnDrawGizmos()
    {
        if(_startPointTrans==null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(_startPointTrans.position - Vector3.right * WORLD_WIDTH / 2, _startPointTrans.position + Vector3.right * WORLD_WIDTH / 2);
    }

    // ReSharper disable once MethodTooLong
    private void Update()
    {
        if (!IsCreating)
            return;
        while (_tileList.Count == 0 ||
            ( (_tileList[_tileList.Count - 1].Position.y + _tileList[_tileList.Count - 1].Height)- TargetTransform.position.y) <
            Coverage)
        {
            Vector3 targetPos = _tileList.Count > 0 ? _tileList[_tileList.Count - 1].Position + Vector2.up * (_tileList[_tileList.Count - 1].Height) :
                (Vector2)_startPointTrans.position;
            targetPos.z = transform.position.z;

            var cycleTile = (ICycleTile)Instantiate((MonoBehaviour)_prefabSelector.GetSelectedPrefab(targetPos.y
                ),_contentTransform);
            var behaviour = (MonoBehaviour) cycleTile;
            behaviour.gameObject.SetActive(true);
            cycleTile.SetPosition(targetPos);
            _tileList.Add(cycleTile);
            CreatedTile?.Invoke(cycleTile);
        }


        if (_tileList.Count > 0 &&
            ((_tileList[0].Position.y - _tileList[0].Height) - TargetTransform.position.y) > Coverage)
        {
            var cycleTile = _tileList[0];
            _tileList.RemoveAt(0);
            RemovedTile?.Invoke(cycleTile);
            var monoBehavior = cycleTile as MonoBehaviour;
            if (monoBehavior != null) Destroy(monoBehavior.gameObject);
        }
    }


    public void ClearLevel()
    {
        _tileList.ForEach(tile => Destroy(((MonoBehaviour)tile).gameObject));
        _tileList.Clear();
    }
}
