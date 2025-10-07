using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public static event Action<PlayerSkin> PlayerSkinSelectionChanged; 

    public const string SKIN_LOCK_PREFIX = "skin_locked";
    private const string SELECTED_SKIN_KEY = "SelectedSkin";

    [SerializeField] private PlayerSkins _playerSkins;

    public static PlayerSkins PlayerSkins => Instance._playerSkins;

    public static bool EnableAds
    {
        get => PrefManager.GetInt(nameof(EnableAds), 1) == 1;
        set => PrefManager.SetInt(nameof(EnableAds),value?1:0);
    }
    public static bool AbleToRestore => EnableAds;

#if IN_APP

    public static string NoAdsProductID => GameSettings.Default.InAppSetting.removeAdsId;

    public static event Action<string> ProductPurchased;
    public static event Action<bool> ProductRestored;

    public Purchaser Purchaser { get; private set; }

#endif

    public static GameSettings GameSettings => Resources.Load<GameSettings>(nameof(GameSettings));

    public static PlayerSkin GetSkinById(string id) => PlayerSkins.FirstOrDefault(skin => skin.id == id);



    public static void SetSelectedSkin(string id)
    {
        PrefManager.SetString(SELECTED_SKIN_KEY, id);
        PlayerSkinSelectionChanged?.Invoke(GetSkinById(id));
    }

    public static string GetSelectedSkin()
    {
        return PrefManager.GetString(SELECTED_SKIN_KEY,PlayerSkins.FirstOrDefault().id);
    }

    public static bool IsSkinLocked(string skinId)
    {
        var skin = GetSkinById(skinId);
        return  skin.preLocked && skin.lockDetails.minLevel > GameManager.CurrentLevel;
    }

    protected override void OnInit()
    {
        base.OnInit();
#if IN_APP
        Purchaser = new Purchaser(new List<string>(), new[] { NoAdsProductID });
        Purchaser.RestorePurchased += PurchaserOnRestorePurchased;
#endif
    }

#if IN_APP

    private void PurchaserOnRestorePurchased(bool success)
    {
        if (EnableAds && Purchaser.ItemAlreadyPurchased(NoAdsProductID))
        {
            EnableAds = false;
            ProductPurchased?.Invoke(NoAdsProductID);
        }
        ProductRestored?.Invoke(success);
    }

    public static void RestorePurchase()
    {
        Instance.Purchaser.Restore();
    }

    public static void PurchaseNoAds(Action<bool> completed = null)
    {
        if (!EnableAds)
        {
            return;
        }

        Instance.Purchaser.BuyProduct(NoAdsProductID, success =>
        {
            if (success)
            {
                EnableAds = false;
            }
            completed?.Invoke(success);
            if (success)
                ProductPurchased?.Invoke(NoAdsProductID);
        });
    }
#endif

}