using System;

public class AudioManager : Singleton<AudioManager>
{
    public static event Action<bool> SoundStateChanged;

    public static bool IsSoundEnable
    {
        get { return PrefManager.GetInt(nameof(IsSoundEnable), 1) == 1;}
        set
        {
            if (IsSoundEnable == value)
            {
                return;
            }
            PrefManager.SetInt(nameof(IsSoundEnable),value?1:0);
            SoundStateChanged?.Invoke(value);
        }
    }

    private void Start()
    {
        
    }

}