using UnityEngine;

public interface ICycleTile
{
    float Height { get; }
    Vector2 Position { get; }
    void SetPosition(Vector2 position);
}