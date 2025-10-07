using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    private void Start()
    {
        GameManager.LoadScene(Scene.Main.ToString());
    }
}