using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioClipShowcase {

	float fieldWidth = -1, buttonWidth = -1, areaWidth = -1;

	string playText, stopText;

	bool fieldExpandWidth, areaExpandWidth, readOnly;

	public AudioClip stored;

	static bool playing;

	public AudioClipShowcase (float fieldWidth){
		this.areaExpandWidth = false;
		this.fieldWidth = fieldWidth;
		Init();
	}

	public AudioClipShowcase (bool fieldExpandWidth, float areaWidth) {
		this.areaExpandWidth = false;
		this.fieldExpandWidth = fieldExpandWidth;
		this.areaWidth = areaWidth;
		Init();

	}

	public AudioClipShowcase (bool fieldExpandWidth, bool areaExpandWidth) {
		this.fieldExpandWidth = fieldExpandWidth;
		this.areaExpandWidth = areaExpandWidth;
		Init();
	}

	public void SetButtonText(string playText, string stopText, float buttonWidth) {
		this.playText = playText;
		this.stopText = stopText;
		this.buttonWidth = buttonWidth;
	}

	public void Init () {
		SetButtonText("Play", "Stop", 32);
	}	

#if UNITY_EDITOR
	public AudioClip DrawLayout (AudioClip audio, bool readOnly = false) {
		if (audio != stored) stored = audio;
		GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
			if (readOnly) GUI.enabled = false;
			stored = EditorGUILayout.ObjectField(stored, typeof(AudioClip), false, GUILayout.ExpandHeight(true)) as AudioClip;
			if (readOnly) GUI.enabled = true;
			if (stored == null) GUI.enabled = false;
			if (GUILayout.Button(playText, GUILayout.ExpandWidth(false))) {
				PublicAudioUtil.PlayClip(stored);
				playing = true;
			}
			if (stored == null) GUI.enabled = true;
			if (!playing) GUI.enabled = false;
			if (GUILayout.Button(stopText, GUILayout.ExpandWidth(false))) PublicAudioUtil.StopAllClips();
			GUI.enabled = true;
		GUILayout.EndHorizontal();
		return stored;
	}

	public AudioClip Draw (Rect rect, AudioClip audio, bool readOnly = false) {
		if (audio != stored) stored = audio;
		Rect fieldRect = new Rect(rect.position, rect.size - new Vector2(buttonWidth*2 + buttonWidth*0.25f, 0));
		Rect buttonRect = new Rect(rect.position + new Vector2(rect.size.x - buttonWidth*2 - buttonWidth*0.25f, 0), new Vector2(buttonWidth, rect.height));
		Rect buttonRect2 = new Rect(rect.position + new Vector2(rect.size.x - buttonWidth, 0), new Vector2(buttonWidth, rect.height));
		if (readOnly) GUI.enabled = false;
		stored = EditorGUI.ObjectField(fieldRect, stored, typeof(AudioClip), false) as AudioClip;
		if (readOnly) GUI.enabled = true;
		if (stored == null) GUI.enabled = false;
		if (GUI.Button(buttonRect, playText)) {
			PublicAudioUtil.PlayClip(stored);
			playing = true;
		}
		if (stored == null) GUI.enabled = true;
		if (!playing) GUI.enabled = false;
		if (GUI.Button(buttonRect2, stopText)) PublicAudioUtil.StopAllClips();
		GUI.enabled = true;
		return stored;
	}
#endif
	
}
