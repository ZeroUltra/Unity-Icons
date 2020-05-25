using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;


/*
 *参考链接:
 * https://blog.csdn.net/acuriousguy/article/details/105955982
 * https://blog.csdn.net/qq_33337811/article/details/74012108
 * https://www.jianshu.com/p/68e3445a421b
 * http://www.xuanyusong.com/archives/3777
 */


public class IconEditor : EditorWindow, IHasCustomMenu
{
    private static MethodInfo loadIconMethodInfo, findTextureMethodInfo;
    [MenuItem("Window/Icon Window")]
    static void ShowWindow()
    {
        IconEditor window = GetWindow<IconEditor>("IconEditor");
        window.minSize = new Vector2(1280, 720);
        //2018版本这个方法不中用了 找不到了
        //Texture2D tex = EditorGUIUtility.FindTexture("GameObject Icon");
        loadIconMethodInfo = typeof(EditorGUIUtility).GetMethod("LoadIcon", BindingFlags.Static | BindingFlags.NonPublic);
        window.titleContent = new GUIContent("InternalIcon", loadIconMethodInfo.Invoke(null, new object[] { "Image Icon" }) as Texture);

    }

    //Dictionary<Texture, string> listIcons = new Dictionary<Texture, string>();
    List<string> listIcons = null;
    void OnEnable()
    {
        listIcons = new List<string>();
        Texture2D[] texs = Resources.FindObjectsOfTypeAll<Texture2D>();
        Debug.unityLogger.logEnabled = false;
        foreach (Texture2D itemTex in texs)
        {
            GUIContent gc = EditorGUIUtility.IconContent(itemTex.name);
            if (gc != null && gc.image != null)
            {
                listIcons.Add(itemTex.name);
            }
        }
        Debug.unityLogger.logEnabled = true;
        Debug.Log(listIcons.Count);
    }


    Vector2 m_Scroll;
    void OnGUI()
    {
        m_Scroll = GUILayout.BeginScrollView(m_Scroll);
        float width = 50;
        int count = 6;
        for (int i = 0; i < listIcons.Count; i += count)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                for (int j = 0; j < count; j++)
                {
                    int index = i + j;
                    if (index < listIcons.Count)
                    {
                        //创建一个可选标签字段。 （对于显示可能是复制粘贴。）
                        EditorGUILayout.TextField(listIcons[index],GUILayout.Width(120));
                        GUILayout.Button(EditorGUIUtility.IconContent(listIcons[index]), GUILayout.Width(width), GUILayout.Height(30));
                        //GUILayout.Box
                    }
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.Space();
        }
        GUILayout.EndScrollView();

    }

    #region 展示窗体右上角 Lock 锁
    /// <summary>
    /// Keep local copy of lock button style for efficiency.
    /// </summary>
    [System.NonSerialized]
    GUIStyle lockButtonStyle;
    /// <summary>
    /// Indicates whether lock is toggled on/off.
    /// </summary>
    [System.NonSerialized]
    bool locked = false;

    /// <summary>
    /// Magic method which Unity detects automatically.
    /// </summary>
    /// <param name="position">Position of button.</param>
    void ShowButton(Rect position)
    {
        if (lockButtonStyle == null)
            lockButtonStyle = "IN LockButton";
        locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
    }

    public void AddItemsToMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Lock"), locked, () =>
        {
            locked = !locked;
        });
    }
    #endregion
}
