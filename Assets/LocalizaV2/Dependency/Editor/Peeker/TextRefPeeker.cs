using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextRefPeeker
#if UNITY_EDITOR
 : PopupWindowContent
#endif
{

    static Font font;

    string peeked, refName;

    float height;

	Rect attached;

    GUIStyle labelStyle;

    GUIStyle LabelStyle {
        get {
            if (labelStyle == null) {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.wordWrap = true;
                labelStyle.fontSize = 12;
            }
            return labelStyle;
        }
    }

	public TextRefPeeker (string peeked, Rect attached, GUIStyle labelStyle = null) {
		this.peeked = peeked;
		this.attached = attached;
        this.height = 40;
        this.labelStyle = labelStyle;
	}
    
    public TextRefPeeker (string peeked, string refName, Rect attached, GUIStyle labelStyle = null) {
		this.peeked = peeked;
		this.attached = attached;
        this.refName = refName;
        this.height = 40;
        this.labelStyle = labelStyle;
	}

#if UNITY_EDITOR
    public override Vector2 GetWindowSize()
    {
        return new Vector2(attached.width, height);
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
                if (peeked == null) GUILayout.Label("NOT FOUND.");
                else if (peeked == string.Empty || peeked == "") GUILayout.Label("EMPTY STRING.");
                else {
                    if (refName != null) GUILayout.Label(refName, EditorStyles.boldLabel);
                    GUILayout.BeginHorizontal();     
                        GUILayout.Space(6);
                        GUILayout.Label(string.Empty, GUILayout.Width(8), GUILayout.ExpandHeight(true));
                        Rect ColorRect = GUILayoutUtility.GetLastRect();     
                        GUILayout.Space(4);
                        GUILayout.Label(peeked, LabelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));    
                        Rect labelRect = GUILayoutUtility.GetLastRect();                    
                    GUILayout.EndHorizontal();

                    bool check = false;
                    while (labelRect.height + 32 > height) {
                        check = true;
                        height += 16;
                    }
                    if (check) this.editorWindow.Repaint();

                    EditorGUI.DrawRect(ColorRect, new Color(0.2f, 0.2f, 0.5f, 0.2f));
                }
            GUILayout.FlexibleSpace();            
        GUILayout.EndVertical();
    }

    public override void OnOpen()
    {
    }

    public override void OnClose()
    {		
    }
#endif
}