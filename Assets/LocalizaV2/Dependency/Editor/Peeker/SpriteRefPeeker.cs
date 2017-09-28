using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpriteRefPeeker
#if UNITY_EDITOR
 : PopupWindowContent
#endif
{

    string refName;
    
    Sprite peeked;
    float height;

	Rect attached;

    GUIStyle labelStyle;

    SpriteSelector selector;

	public SpriteRefPeeker (Sprite peeked, Rect attached) {
		this.peeked = peeked;
		this.attached = attached;
        Init ();
    }
    
    public SpriteRefPeeker (Sprite peeked, string refName, Rect attached) {
		this.peeked = peeked;
		this.attached = attached;
        this.refName = refName;
        Init ();
	}

    void Init () {
        if (peeked == null) this.height = 48;
        else this.height = 160;
        labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.wordWrap = true;
        labelStyle.fontSize = 12;
        selector = SpriteSelector.Create_ScaleByWidth(attached.width);
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
                if (peeked == null) {
                    this.height = 48;
                    GUILayout.Label("NOT FOUND.");
                }
                else {
                    if (refName != null) GUILayout.Label(refName, EditorStyles.boldLabel);                    
                    GUILayout.Space(6);
                    GUILayout.Label(peeked.name);
                    selector.DrawLayout(peeked, true);
                    while (selector.GetRect().height >= height*0.7f) height += 10;
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