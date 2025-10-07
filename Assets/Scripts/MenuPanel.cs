using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{

    [SerializeField] private Button _soundBtn;
    [SerializeField] private Sprite[] _soundOnAndOffSprites = new Sprite[2];

    private void OnEnable()
    {
        AudioManager.SoundStateChanged += AudioManagerOnSoundStateChanged;
        AudioManagerOnSoundStateChanged(AudioManager.IsSoundEnable);
    }

    private void OnDisable()
    {
        AudioManager.SoundStateChanged -= AudioManagerOnSoundStateChanged;
    }

    private void AudioManagerOnSoundStateChanged(bool enable)
    {
        _soundBtn.image.sprite = _soundOnAndOffSprites[enable ? 0 : 1];
    }


    public void OnClickSound()
    {
        AudioManager.IsSoundEnable = !AudioManager.IsSoundEnable;
    }

    public void OnClickSkins()
    {
        UIManager.Instance.SkinSelectionPanel.Show();
    }
}
