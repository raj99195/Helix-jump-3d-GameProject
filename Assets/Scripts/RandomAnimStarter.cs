using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomAnimStarter : MonoBehaviour {
    private void Awake()
    {
        var animator = GetComponent<Animator>();
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(stateInfo.shortNameHash,0,Random.Range(0,1f));
    }
}
