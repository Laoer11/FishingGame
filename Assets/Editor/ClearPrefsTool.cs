using UnityEditor;
using UnityEngine;

/// <summary>编辑器工具:一键清空 PlayerPrefs,重置首局教学标记(测试教学流程用)。</summary>
public static class ClearPrefsTool
{
    /// <summary>菜单项:Tools/清空 PlayerPrefs。编辑器专用,不进包。</summary>
    [MenuItem("Tools/清空 PlayerPrefs")]
    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[Prefs] 已清空,重新运行即可再见到首局教学");
    }
}