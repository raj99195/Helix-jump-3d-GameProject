using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageCreator : MonoBehaviour
{
    public Stage Stage => GetComponentInChildren<Stage>();

    private void Awake()
    {
         GenerateOrRefresh();
    }

    public void GenerateOrRefresh()
    {
        var stages = GetComponentsInChildren<Stage>(true).ToList();

        var selected = Random.Range(0, stages.Count);

        for (var i = 0; i < stages.Count; i++)
        {
            stages[i].gameObject.SetActive(selected == i);
        }

        Stage.transform.localPosition = Vector3.zero;
    }

 
}



public static class Extensions
{
    public static T GetRandom<T>(this IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();
        return list.Count > 0 ? list[Random.Range(0, list.Count)] : default(T);
    }
}