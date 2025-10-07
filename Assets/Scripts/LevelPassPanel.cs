using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelPassPanel : ShowHidable{

    private LevelManager LevelManager=>LevelManager.Instance;

    [SerializeField] private Text _text;

    public override void Show(bool animate = true, Action completed = null)
    {
        _text.text = $"Level {LevelManager.Level} Passed";
        base.Show(animate, completed);
    }
}