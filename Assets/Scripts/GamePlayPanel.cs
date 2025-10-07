using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayPanel : MonoBehaviour
{
    [SerializeField] private GotScoreTileUI _gotScoreTilePrefab;
    [SerializeField] private RectTransform _gotScorePoint;
    [SerializeField] private Text _scoreTxt,_bestTxt;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Text _startLvlTxt, _endLvlTxt;

    private RectTransform GotScoreContent => (RectTransform) _gotScorePoint.parent;

    private LevelManager LevelManager => LevelManager.Instance;

    private void OnEnable()
    {
        LevelManager.GotScore +=LevelManagerOnGotScore;
        _scoreTxt.text = (LevelManager.Instance?.Score ?? 0).ToString();
        _bestTxt.gameObject.SetActive(LevelManager.Instance?.GameType == GameType.New);        
        LevelManager.LevelStarted += LevelManagerOnLevelStarted;
    }


    private void OnDisable()
    {
        LevelManager.GotScore -= LevelManagerOnGotScore;
        LevelManager.LevelStarted -= LevelManagerOnLevelStarted;
    }


    private void LevelManagerOnGotScore(int score, ScoringDetails scoringDetails)
    {
        var tileUI = Instantiate(_gotScoreTilePrefab,GotScoreContent);
        tileUI.transform.position = _gotScorePoint.position;
        tileUI.Score = score;
        tileUI.Show();

        if(_bestTxt.gameObject.activeSelf)
            _bestTxt.gameObject.SetActive(false);
    }


    private void LevelManagerOnLevelStarted()
    {
        _startLvlTxt.text = LevelManager.Level.ToString();
        _endLvlTxt.text = (LevelManager.Level + 1).ToString();
        _bestTxt.gameObject.SetActive(LevelManager.Instance?.GameType == GameType.New);
        _bestTxt.text = $"BEST - {GameManager.BestScore}";
    }

    private void Update()
    {
        _progressSlider.normalizedValue = LevelManager.Progress;
        _scoreTxt.text = LevelManager.Instance.Score.ToString();
    }
}