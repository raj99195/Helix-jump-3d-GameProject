using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSkinSelectionPanel : ShowHidable
{
    [SerializeField] private SkinTileUI _skinTileUIPrefab;
    [SerializeField] private RectTransform _content;

    private readonly List<SkinTileUI> _tiles= new List<SkinTileUI>();
    private SkinTileUI _selectedTile;

    public SkinTileUI SelectedTile
    {
        get { return _selectedTile; }
        set
        {
            if (_selectedTile!=null)
            {
                _selectedTile.Selected = false;
            }
            _selectedTile = value;
            _selectedTile.Selected = true;
            ResourceManager.SetSelectedSkin(_selectedTile.MViewModel.Skin.id);
        }
    }

    private void Awake()
    {
        foreach (var playerSkin in ResourceManager.PlayerSkins)
        {
            var skinTileUI = Instantiate(_skinTileUIPrefab,_content);
            skinTileUI.MViewModel = new SkinTileUI.ViewModel
            {
                Skin = playerSkin,
                Locked = ResourceManager.IsSkinLocked(playerSkin.id)
            };
            skinTileUI.Clicked +=SkinTileUIOnClicked;
            _tiles.Add(skinTileUI);
        }

        var skin = ResourceManager.GetSelectedSkin();
        SelectedTile = _tiles.First(ui => ui.MViewModel.Skin.id == skin);
    }

    private void SkinTileUIOnClicked(SkinTileUI tileUI)
    {
        if (tileUI.MViewModel.Locked)
        {
            var popUpPanel = SharedUIManager.PopUpPanel;
            popUpPanel.ShowAsInfo("Locked!",$"You have Reach Level {tileUI.MViewModel.Skin.lockDetails.minLevel}");
            return;
        }

        SelectedTile = tileUI;
    }

    public void OnClickBack()
    {
        Hide();
    }
}
