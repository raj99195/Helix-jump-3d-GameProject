using UnityEngine;
using UnityEngine.UI;

public class GotScoreTileUI : MonoBehaviour
{

    [SerializeField] private Text _text;
    public int Score
    {
        set { _text.text = "+"+value; }
    }

    public void Show()
    {
        Destroy(gameObject,5);
    }

}