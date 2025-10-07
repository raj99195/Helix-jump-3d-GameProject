using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private LevelPassPanel _levelPassPanel;
    [SerializeField] private LevelFailedPanel _levelFailedPanel;
    [SerializeField] private MenuPanel _menuPanel;
    [SerializeField] private PlayerSkinSelectionPanel _skinSelectionPanel;

    private LevelManager LevelManager=>LevelManager.Instance;

    public PlayerSkinSelectionPanel SkinSelectionPanel => _skinSelectionPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        LevelManager.LevelOver +=LevelManagerOnLevelOver;
        LevelManager.LevelStarted +=LevelManagerOnLevelStarted;
        LevelManager.LevelContinued +=LevelManagerOnLevelContinued;
        if (LevelManager != null)
        {
            LevelManagerOnLevelStarted();
        }
    }

   

    private void OnDisable()
    {
        LevelManager.LevelOver -=LevelManagerOnLevelOver;
        LevelManager.LevelStarted -= LevelManagerOnLevelStarted;
        LevelManager.LevelContinued -= LevelManagerOnLevelContinued;
    }


    private void LevelManagerOnLevelContinued()
    {
        _levelFailedPanel.Hide();
    }


    private void LevelManagerOnLevelStarted()
    {
       _menuPanel.gameObject.SetActive(LevelManager.GameType == GameType.New);
    }

    // ReSharper disable once FlagArgument
    private void LevelManagerOnLevelOver(bool won)
    {
        if (won)
        {
            _levelPassPanel.Show();
        }
        else
        {
            _levelFailedPanel.Show();
        }
    }
}