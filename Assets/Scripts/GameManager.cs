using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>,IInitializable
{
    public static event Action<int> TopScoreChanged;

    public static LevelStartData LevelStartData { get; private set; }

    public static int BestScore
    {
        get { return PrefManager.GetInt(nameof(BestScore)); }
        set
        {
            if(value<BestScore)
                return;
            PrefManager.SetInt(nameof(BestScore), value);
            TopScoreChanged?.Invoke(value);
        }
    }

    public static int CurrentLevel
    {
        get
        {
            return PrefManager.GetInt(nameof(CurrentLevel),1);
        }
        set
        {
            PrefManager.SetInt(nameof(CurrentLevel),value);
        }
    }

    public static void LoadGame(LevelStartData data)
    {
        LevelStartData = data;
        LoadScene(Scene.Main.ToString());
    }

    // ReSharper disable once FlagArgument
    public static void LoadScene(string name, bool showFade = true, float waitTime = 0.2f)
    {
        if (showFade)
        {
            var fadeEffectPanel = SharedUIManager.FadeEffectPanel;

            fadeEffectPanel.Show(completed: () =>
            {
                SceneManager.LoadScene(name);
                var targetTime = Time.time + waitTime;
                LateCall.Create(Instance.gameObject)
                    .Call(() => targetTime < Time.time, () => { fadeEffectPanel.Hide(); });
            });
        }
        else
        {
            SceneManager.LoadScene(name);
        }
    }

    public bool Initialized { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    public void Init()
    {
        if (Initialized)
        {
            return;
        }
        Application.targetFrameRate = 60;
        Initialized = true;
    }
}


public enum Scene
{
    Main
}

public struct LevelStartData
{
    public int StartScore { get; set; }
    public GameType GameType { get; set; }
    
}

public enum GameType
{
    New,ContinuesLevel
}