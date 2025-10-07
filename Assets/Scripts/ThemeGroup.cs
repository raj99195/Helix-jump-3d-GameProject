using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ThemeGroup : ScriptableObject,IEnumerable<Theme>
{
    public const string DEFAULT_NAME = nameof(ThemeGroup);

    [SerializeField]private List<Theme> _themes = new List<Theme>();

    public IEnumerable<Theme> Themes => _themes;

    public IEnumerator<Theme> GetEnumerator() => _themes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


#if UNITY_EDITOR
    [UnityEditor.MenuItem("MyGames/Themes")]
    public static void Open()
    {
        GamePlayEditorManager.OpenScriptableAtDefault<ThemeGroup>();
    }
#endif
}