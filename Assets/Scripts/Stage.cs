using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Stage : MonoBehaviour
{
    [SerializeField] private StageGenerateData _recommendStageGenerateData;

    public StageGenerateData RecommendStageGenerateData => _recommendStageGenerateData;

    private bool _isTaken;
    private bool _isBroke;
    public event Action<Stage> Broke; 
    public event Action<Stage> Taken;
//    [SerializeField]private List<GameObject> _parts = new List<GameObject>();

    public bool IsTaken
    {
        get { return _isTaken; }
        private set
        {
            if(!value || _isTaken)
                return;
            _isTaken = true;
            Taken?.Invoke(this);
        }
    }

    public bool IsBroke
    {
        get { return _isBroke; }
        private set
        {
            if(!value || _isBroke)
                return;
            _isBroke = true;
            Broke?.Invoke(this);
        }
    }


    public void Take()
    {
        if(IsBroke)
            throw new InvalidOperationException();

        Throw();
        IsTaken = true;
    }

    public static Stage GetStage(Collider col)
    {
        var trans = col.transform;

        while (trans.GetComponent<Stage>() == null)
        {
            trans = trans.parent;
        }

        return trans.GetComponent<Stage>();
    }

    [ContextMenu("Break")]
    public void Break()
    {
       
        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material.color = LevelManager.Instance?.BallNormalColor ?? Color.red;
        }

        Throw();
        IsBroke = true;
    }

    public void Damage()
    {
        StartCoroutine(DamageCor());
    }

    // ReSharper disable once MethodTooLong
    private IEnumerator DamageCor()
    {
        var meshRenderers = GetComponentsInChildren<MeshRenderer>().ToArray();
        var colors = meshRenderers.Select(meshRenderer => meshRenderer.material.color).ToArray();

        var color = Color.Lerp(Color.white, LevelManager.Instance?.BallNormalColor ?? Color.red, 0.9f);
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.material.color =  color;
        }
        yield return new WaitForSeconds(0.17f);
        var normalized = 0f;
        while (normalized<1)
        {
            normalized = Mathf.Clamp(Mathf.MoveTowards(normalized, 1, 3* Time.deltaTime), 0, 1f);
            for (var i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material.color = Color.Lerp(color, colors[i], normalized);
            }

            yield return null;
        }

        for (var i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = colors[i];
        }
    }

    [ContextMenu("Throw")]
    private void Throw()
    {
        foreach (var child in GetComponentsInChildren<Collider>())
        {
            child.enabled = false;
        }

        
        var list = new List<Transform>();
        foreach (Transform t in transform)
        {

           list.Add(t);
        }

        foreach (var t in list)
        {
            t.parent = null;
            var rigid = t.gameObject.AddComponent<Rigidbody>();
            //            rigid.useGravity = false;
            rigid.linearVelocity = t.forward * 10 - Vector3.up * 3;
            rigid.angularVelocity = Vector3.Cross(-Vector3.up, t.forward) * 2;
            Destroy(t.gameObject, 1.5f);
        }
    }
}

public partial class Stage
{
    [SerializeField] private List<TileGroup> _tileGroups = new List<TileGroup>();

#if UNITY_EDITOR
    [ContextMenu(nameof(AutoReplace))]
    private void AutoReplace()
    {
        var transforms = Enumerable.Range(0,transform.childCount).Select(i => transform.GetChild(i)).ToList();

        foreach (var t in transforms)
        {
            var tileGroup = _tileGroups.FirstOrDefault(group => t.gameObject.name.Contains(@group.matchString));

            if(tileGroup.prefab==null)
                continue;

            var prefab = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(tileGroup.prefab,transform);

            prefab.transform.position = t.position;
            prefab.transform.rotation = t.rotation;
            prefab.transform.localScale = t.localScale;

            foreach (var child in prefab.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = t.gameObject.layer;
            }
            foreach (var meshRenderer in prefab.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.sharedMaterial = t.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            }
       
            
            DestroyImmediate(t.gameObject);
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    [Serializable]
    public struct TileGroup
    {
        public string matchString;
        public GameObject prefab;
    }

}

[Serializable]
public struct StageGenerateData
{
    public Vector2Int minAndMaxLevel;
    public float probability;
}

