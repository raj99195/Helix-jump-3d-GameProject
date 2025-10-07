using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public static event Action<int, ScoringDetails> GotScore;
    public static event Action<int> ScoreChanged;
    public static event Action LevelStarted;
    public static event Action<bool> LevelOver;
    public static event Action LevelContinued;

    [SerializeField] private LevelCreator _levelCreator;
    [SerializeField] private Player _player;
    [SerializeField] private AudioClip _gotPointClip;
    [SerializeField] private Stage _startStage;
    [SerializeField] private CameraFollower _cameraFollower;
    [SerializeField] private Material _enemyMaterial,_platformMaterial,_rodMaterial;
    [SerializeField] private SpriteRenderer _bgRenderer;
    [SerializeField] private ThemeGroup _themeGroup;

    public Color BallNormalColor => _player.NormalColor;

    public Player Player => _player;

    public int Score
    {
        get => _score;
        private set
        {
            if (_score == value)
            {
                return;
            }

            _score = value;
            ScoreChanged?.Invoke(value);
        }
    }

    public GameType GameType { get; private set; }

    private readonly List<Stage> _stages = new List<Stage>();

    public Stage CurrentStage => _stages.FirstOrDefault();
    public Stage NextStage => _stages.Count > 1 ? _stages[1] : null;



    public int Level { get; private set; }
    public int TotalStageCount { get; private set; }
    public float Progress => (TotalStageCount - _stages.Count + 0f) / TotalStageCount;
    public State CurrentState { get; private set; } = State.WaitingForStart;
    public int PlayCount { get; private set; } = 1;

    public Theme Theme
    {
        set
        {
            _enemyMaterial.color = value.enemyColor;
            _platformMaterial.color = value.platformColor;
            _rodMaterial.color = value.rodColor;
            _bgRenderer.sprite = value.background;
            Player.NormalColor = value.ballColor;
            _theme = value;
        }
        get => _theme;
    }

    // ReSharper disable once NotAccessedField.Local
    private int _boostStageLeftCount;
    private int _comboCount;
    private int _score;
    private Theme _theme;

    private void Awake()
    {
        Instance = this;
        _player.Init();
        Theme = _themeGroup.GetRandom();

        var levelStartData = GameManager.LevelStartData;
        GameType = levelStartData.GameType;
        Level = GameManager.CurrentLevel <= 0 ? 1 : GameManager.CurrentLevel;

        Score = levelStartData.StartScore;
    }

    private void Start()
    {
        _levelCreator = FindObjectOfType<LevelCreator>();
        _levelCreator.Level = Level;
        _levelCreator.Init();
        _player.Rod = _levelCreator.transform;
        
        StartTheGame();
    }


    public void StartTheGame()
    {
        _player.Died += PlayerOnDied;
        _player.BoostedStageChanged += PlayerOnBoostedStageChanged;
        _player.Bounced += PlayerOnBounced;
        _player.CollisionEntered += PlayerOnCollisionEntered;
        _stages.AddRange(_levelCreator.Stages);
        _stages.ForEach(stage => stage.Broke += StageOnBroke);
        TotalStageCount = _stages.Count;
        _player.Active = true;
        CurrentState = State.Playing;
        _cameraFollower.Target = Player.transform;
        LevelStarted?.Invoke();
    }

    // ReSharper disable once FlagArgument
    private void PlayerOnBoostedStageChanged(bool boosted)
    {
        if (boosted)
        {
            _boostStageLeftCount = 3;
        }
    }

    private void StageOnBroke(Stage stage)
    {
        if (stage != CurrentStage)
            throw new InvalidOperationException();
        if (_comboCount == 0)
            _comboCount = 1;
        _boostStageLeftCount--;

        _stages.Remove(CurrentStage);
        GetScore();
    }


    // ReSharper disable once FlagArgument
    private void PlayerOnCollisionEntered(Collider col, bool bottom)
    {
        if (bottom && col.gameObject.layer == (int) Layer.EndPoint)
        {
            TakeCurrentStage();
            OverTheGame(true);
        }
    }

    private void PlayerOnBounced(Collider col)
    {
        if (Player.Boosted)
        {
            CurrentStage.Break();
            Player.Boosted = false;
        }

        _comboCount = 0;


    }


    private void PlayerOnDied()
    {
        OverTheGame(false);
    }


    // ReSharper disable once MethodTooLong
    private void Update()
    {
        if (CurrentState != State.Playing)
            return;

//        _cameraFollower.Offset = Player.Boosted ? _playerOffsetFollow : _stageOffsetFollow;
        //Player.Boosted && Player.IsZeroGravity ? Player.transform : CurrentStage?.transform;


        if (CurrentStage != null && Player.transform.position.y < CurrentStage.transform.position.y)
        {
            TakeCurrentStage();
        }
    }

    private void TakeCurrentStage()
    {
        if (CurrentStage == null)
            return;
        _comboCount++;

        CurrentStage.Take();
        _stages.Remove(CurrentStage);

        GetScore();

        if (_comboCount >= 3)
            Player.Boosted = true;
    }

    private void GetScore(bool isEnemyHit = false)
    {
        Score += _comboCount * Level;
        if (AudioManager.IsSoundEnable && _gotPointClip != null)
        {
            // ReSharper disable once PossibleNullReferenceException
            AudioSource.PlayClipAtPoint(_gotPointClip, Camera.main.transform.position,0.35f);
        }

        GotScore?.Invoke(_comboCount * Level, new ScoringDetails
        {
            ScoreType = ScoringDetails.Type.Break,
            Combo = _comboCount,
            IsEnemyHit = isEnemyHit
        });
    }


    // ReSharper disable once FlagArgument
    private void OverTheGame(bool won)
    {
        if (Player.Active)
            Player.Active = false;
        CurrentState = State.Over;
        LevelOver?.Invoke(won);

        if (won)
        {
            GameManager.CurrentLevel = Level + 1;
            var targetTime = Time.time + 1.5f;
            LateCall.Create(gameObject).Call(() => targetTime < Time.time, () =>
            {
                GameManager.LoadGame(new LevelStartData
                {
                    StartScore = Score,
                    GameType = GameType.ContinuesLevel
                });
            });
        }
    }


    public enum State
    {
        WaitingForStart,
        Playing,
        Over
    }
}

public partial class LevelManager
{
    // ReSharper disable once MethodTooLong
    private void SetPlayerAndRodForContinue()
    {
        var currentStage = CurrentStage;
        var stage = Instantiate(_startStage, currentStage.transform.position + Vector3.up*_levelCreator.Space, Quaternion.identity);
        stage.transform.parent = currentStage.transform.parent;
        _stages.Insert(0, stage);

        Player.transform.parent = null;
        // ReSharper disable once TooManyChainedReferences
        var position = Player.transform.position;
        position.y = CurrentStage.transform.position.y + 2f;
        Player.transform.position = position;
        Player.IsDied = false;

        _cameraFollower.FollowY = Player.transform.position.y;
        _cameraFollower.TargetY = _cameraFollower.FollowY;
        _cameraFollower.Target = Player.transform;
    }

    public void ContinueTheGame()
    {
        if (CurrentState != State.Over)
        {
            return;
        }

        CurrentState = State.Playing;
        SetPlayerAndRodForContinue();
        PlayCount++;
        LevelContinued?.Invoke();
    }
}

public struct ScoringDetails
{
    public Type ScoreType { get; set; }
    public int Combo { get; set; }
    public bool? IsEnemyHit { get; set; }


    public enum Type
    {
        Taken,
        Break
    }
}


[Serializable]
public struct Theme
{
    public Sprite background;
    public Color ballColor;
    public Color platformColor;
    public Color enemyColor;
    public Color rodColor;
    public ThemeColors colors;
}

namespace Models
{
    [Serializable]
    public struct ThemeColors
    {
        [SerializeField] private List<KeyAndColor> _colors;

        public Color GetColorForKey(string key) => _colors.FirstOrDefault(color => color.key == key).color;

        public bool HasKey(string key) => _colors.Any(color => color.key == key);

        [Serializable]
        public struct KeyAndColor
        {
            public string key;
            public Color color;
        }
    }
}