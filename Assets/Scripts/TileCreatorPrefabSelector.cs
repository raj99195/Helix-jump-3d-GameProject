using System;
using System.Linq;
using UnityEngine;


public abstract class TileCreatorPrefabSelector : MonoBehaviour
{

    // ReSharper disable once MethodTooLong
    public abstract ICycleTile GetSelectedPrefab(float y);

}