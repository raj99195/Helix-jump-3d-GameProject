using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkins : ScriptableObject, IEnumerable<PlayerSkin>
{
    public const string DEFAULT_NAME = nameof(PlayerSkins);

    [SerializeField]private List<PlayerSkin> _playerSkins = new List<PlayerSkin>();

    public IEnumerator<PlayerSkin> GetEnumerator() => _playerSkins.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#if UNITY_EDITOR
    [UnityEditor.MenuItem("MyGames/Skins")]
    public static void Open()
    {
        GamePlayEditorManager.OpenScriptableAtDefault<PlayerSkins>();
    }
#endif

}