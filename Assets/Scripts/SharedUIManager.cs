using UnityEngine;

// ReSharper disable once HollowTypeName
public class SharedUIManager : Singleton<SharedUIManager>
{

    [SerializeField] private FadeEffectPanel _fadeEffectPanel;
    [SerializeField] private PopUpPanel _popUpPanel;
    [SerializeField] private ConsentPanel _consentPanel;


    public static ConsentPanel ConsentPanel => Instance._consentPanel;
    public static PopUpPanel PopUpPanel => Instance._popUpPanel;
    public static FadeEffectPanel FadeEffectPanel => Instance._fadeEffectPanel;
}