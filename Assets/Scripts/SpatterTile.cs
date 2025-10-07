using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpatterTile : MonoBehaviour {

    public Sprite Image
    {
        set { GetComponent<SpriteRenderer>().sprite = value; }
    }

    public Color Color
    {
        get { return GetComponent<SpriteRenderer>().color; }
        set { GetComponent<SpriteRenderer>().color = value; }
    }
}
