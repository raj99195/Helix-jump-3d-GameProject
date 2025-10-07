// /*
// Created by Darsan
// */

using UnityEngine;
using UnityEngine.UI;

public class SkinThemeColorUI : MonoBehaviour
{
    [SerializeField] private string _key = "default";
    [SerializeField] private bool _keepAlpha = false;

    private Text _text;
    private Image _image;

    private void Start()
    {
        _text = GetComponent<Text>();
        _image = GetComponent<Image>();
        UpdateColor();
    }


    private void UpdateColor()
    {
        var skin = LevelManager.Instance.Theme;
        if (skin.colors.HasKey(_key))
        {
            var color = skin.colors.GetColorForKey(_key);

            if (_text)
                _text.color = _keepAlpha ? new Color(color.r, color.g, color.b, _text.color.a) : color;

            if (_image)
                _image.color = _keepAlpha ? new Color(color.r, color.g, color.b, _image.color.a) : color;

        }
    }
}