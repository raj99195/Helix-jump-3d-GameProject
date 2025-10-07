using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelFailedPanel : ShowHidable
{

    [SerializeField] private Text _startLvlTxt,_endLvlTxt;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private Text _progressTxt;
    [SerializeField] private Text _scoreTxt;
    [SerializeField] private GameObject _newRecordGroup,_continueGroup;
    [SerializeField] private Text _newRecordTxt;

    private LevelManager LevelManager=> LevelManager.Instance;

    public override void Show(bool animate = true, Action completed = null)
    {
        _startLvlTxt.text = LevelManager.Level.ToString();
        _endLvlTxt.text = (LevelManager.Level+1).ToString();
        _progressSlider.normalizedValue = LevelManager.Progress;
        _progressTxt.text = $"{(int)(LevelManager.Progress*100)}% COMPLETED";
        _scoreTxt.text = LevelManager.Score.ToString();

        AdsManager.ShowOrPassAdsIfCan();

        _continueGroup.SetActive(LevelManager.PlayCount<2 && AdsManager.IsVideoAvailable());
        _newRecordGroup.SetActive(GameManager.BestScore<LevelManager.Score);

        if (GameManager.BestScore < LevelManager.Score)
        {
            GameManager.BestScore = LevelManager.Score;
            _newRecordTxt.text = GameManager.BestScore.ToString();
        }

        base.Show(animate, completed);
    }

    public void OnClickRestart()
    {
        GameManager.LoadGame(new LevelStartData());
    }

    public void OnClickContinue()
    {
        AdsManager.ShowVideoAds(true,success =>
        {
            if(success)
            LevelManager.ContinueTheGame();
        });

    }
}