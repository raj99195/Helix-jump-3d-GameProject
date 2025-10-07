using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class BounceTest : MonoBehaviour
{
    public event Action<Collider> Bounced;
    
    [SerializeField] private float _size;
    private float _minPoint;

    public bool Rising { get; private set; }

    public float Velocity { get; private set; }

    public bool Active { get; set; }


    private void Awake()
    {
        Active = true;
    }

    // Use this for initialization
    private void Start () {
		
	}
	
	// Update is called once per frame
    private void Update () {
		
	}

    private void FixedUpdate()
    {
        if(!Active)
            return;

        RaycastHit hit;
        if (Velocity > 0 && Physics.Raycast(transform.position + Vector3.up * (_size ), Vector3.up, out hit, Velocity * Time.fixedDeltaTime + 0.01f,1<<(int)Layer.Platform))
        {
            Velocity *= -1;
            Velocity -= Physics.gravity.y * Time.fixedDeltaTime;
            Bounced?.Invoke(hit.collider);
        }

        transform.Translate(0, Velocity*Time.fixedDeltaTime, 0);
        Velocity -= Physics.gravity.y * Time.fixedDeltaTime;
    }

   


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,_size);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider:"+other.gameObject.name);
    }
}


public enum Layer
{
    Platform=8,Enemy=9,EndPoint=10
}
