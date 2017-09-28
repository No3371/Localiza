using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioRefPeeker 
#if UNITY_EDITOR 
: PopupWindowContent
#endif
{

    string refName;
    AudioClip peeked;

    AudioClipShowcase showcase;

    float height;

	Rect attached;

    GUIStyle labelStyle;

	public AudioRefPeeker (AudioClip peeked, Rect attached) {
		this.peeked = peeked;
		this.attached = attached;
        Init ();
    }
    
    public AudioRefPeeker (AudioClip peeked, string refName, Rect attached) {
		this.peeked = peeked;
		this.attached = attached;
        this.refName = refName;
        Init ();
	}

    void Init () {
        this.height = 64;
        labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.wordWrap = true;
        labelStyle.fontSize = 12;
        showcase = new AudioClipShowcase(true, false);
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
                else {
                    if (refName != null) GUILayout.Label(refName, EditorStyles.boldLabel);                    
                    GUILayout.Space(6);
                    showcase.DrawLayout(peeked, true);   
                    Rect labelRect = GUILayoutUtility.GetLastRect(); 
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