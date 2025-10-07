using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelCreator : MonoBehaviour,IInitializable
{
    [SerializeField] protected float startOffset = 5;
    [SerializeField] protected float endOffset;
    [SerializeField] protected float space = 3;
    [SerializeField] protected Stage startStage;
    [SerializeField] protected GameObject endPoint;
    [SerializeField] protected Transform rod;
    [SerializeField] protected bool autoGenerate;
    [SerializeField] protected Transform contentTransform;
    [SerializeField] protected List<Stage> stagePrefabs;

    public float StartOffset => startOffset;

    public float EndOffset => endOffset;
    public float Space => space;

    public GameObject EndPoint => endPoint;

    public IEnumerable<Stage> Stages => GetComponentsInChildren<Stage>();// GetComponentsInChildren<StageCreator>().Select(creator => creator.Stage);

    public Transform Rod => rod;

    public bool Initialized { get; private set; }

    public int Level { get; set; } = 1;

    private void Start()
    {

    }

    public virtual void Init()
    {
        if(Initialized)
            return;

        if (autoGenerate)
        {
            AutoGenerate();
        }

        foreach (var meshRenderer in endPoint.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material.color = LevelManager.Instance.BallNormalColor;
        }

        var jumpBoosts = GetComponentsInChildren<JumpBoost>().ToList();
        jumpBoosts.ForEach(boost =>
        {
            boost.gameObject.SetActive(Random.Range(0,2)==0);
        });
        Initialized = true;
    }

    [ContextMenu(nameof(AutoGenerate))]
    public void AutoGenerate()
    {
        
        var lastStages =  Stages.Except(new List<Stage>{startStage});
        foreach (var lastStage in lastStages)
        {
#if UNITY_EDITOR
            if(!Application.isPlaying)
            DestroyImmediate(lastStage.gameObject);
            else
            {
                DestroyImmediate(lastStage.gameObject);
            }
#else
            Destroy(lastStage.gameObject);
#endif
        }
        var stages = stagePrefabs
            .Where(stage => (stage.RecommendStageGenerateData.minAndMaxLevel.x <= Level || stage.RecommendStageGenerateData.minAndMaxLevel.x < 0) 
                            && (stage.RecommendStageGenerateData.minAndMaxLevel.y >= Level || stage.RecommendStageGenerateData.minAndMaxLevel.y <0)).ToList();
        var stageCount = Mathf.FloorToInt(20 + Mathf.Min(Level * 3, 20) + Mathf.Min(Level*1f,20)+ Mathf.Min(Level * 0.1f, 10));
        startStage.transform.position = transform.position - Vector3.up * startOffset;

        var list = new List<KeyValuePair<float,Stage>>();

        var total = 0f;
        foreach (var stage in stages)
        {
            list.Add(new KeyValuePair<float, Stage>(total+=stage.RecommendStageGenerateData.probability,stage));
        }


        for (var i = 0; i < stageCount; i++)
        {
            var sel = Random.Range(0,list.Last().Key);
            var stage = Instantiate(list.FirstOrDefault(pair => pair.Key>sel).Value, transform.position - Vector3.up * startOffset - Vector3.up * (i + 1) * space, Quaternion.AngleAxis(Random.Range(0,360),Vector3.up));
            stage.transform.parent = contentTransform;
        }

       

        var scale = Rod.localScale;
        scale.y = StartOffset + space * (stageCount+1) + EndOffset;
        Rod.localScale = scale;

        EndPoint.transform.position = transform.position -
                                      Vector3.up *
                                      startOffset - Vector3.up * (stageCount + 1) * space;
    }
}
