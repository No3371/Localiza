using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpriteSelector {

	float width = -1, height = -1, widthAuto = -1, heightAuto = -1, maxWidth = -1, maxHeight = -1;

	float widthRatio = 16, heightRatio = 9;

	bool expandWidth, expandHeight, useGUILayoutSizeOptions, layoutJustChanged;
	
	Sprite stored;

	static Texture2D TransparentBoxBG;

	Rect boxRect, previewTexRectCache, previewRect;

	LayoutMode layoutMode;

	static GUIStyle hoverLabelStyle, boxStyle; 

	List<GUILayoutOption> fixedOptions;

	public SpriteSelector () {
		if (hoverLabelStyle == null){
			hoverLabelStyle = new GUIStyle(GUI.skin.label);
			hoverLabelStyle.alignment = TextAnchor.MiddleCenter;
			hoverLabelStyle.normal.textColor = Color.white;
		}
		if (boxStyle == null){
			boxStyle = new GUIStyle(GUI.skin.box);
			boxStyle.alignment = TextAnchor.MiddleCenter;
			boxStyle.normal.textColor = Color.grey;
		}
	}

	public static SpriteSelector Create_FixedSize (float width, float height) {
		SpriteSelector created = new SpriteSelector();
		created.layoutMode = LayoutMode.FixedSize;
		created.width = width;
		created.height = height;
		return created;
	}

	public static SpriteSelector Create_ScaleByWidth (float maxWidth, float widthRatio = 16, float heightRatio = 9) {
		SpriteSelector created = new SpriteSelector();
		created.layoutMode = LayoutMode.ScaleByWidth;
		created.maxWidth = maxWidth;
		created.widthRatio = widthRatio;
		created.heightRatio = heightRatio;
		return created;
	}
	
	public static SpriteSelector Create_ScaleByWidth () {
		SpriteSelector created = new SpriteSelector();
		created.layoutMode = LayoutMode.ScaleByWidth;
		created.widthRatio = 16;
		created.heightRatio = 9;
		return created;
	}


	public static SpriteSelector Create_ScaleByHeight (float maxHeight, float widthRatio = 16, float heightRatio = 9) {
		SpriteSelector created = new SpriteSelector();
		created.layoutMode = LayoutMode.ScaleByHeight;
		created.maxHeight = maxHeight;
		created.widthRatio = widthRatio;
		created.heightRatio = heightRatio;
		return created;
	}

	public static SpriteSelector Create_Stretch (float maxWidth, float maxHeight) {
		SpriteSelector created = new SpriteSelector();
		created.layoutMode = LayoutMode.Stretch;
		created.maxWidth = maxWidth;
		created.maxHeight = maxHeight;
		return created;
	}	

	// public static SpriteSelector Create_Layout (float widthRatio = 16, float heightRatio = 9, params GUILayoutOption[] options) {
	// 	SpriteSelector created = new SpriteSelector();
	// 	created.options = new List<GUILayoutOption>(options);
	// 	created.useGUILayoutOptions = true;
	// 	return created;
	// }	

	public enum LayoutMode {
		ScaleByHeight,
		ScaleByWidth,
		Stretch,
		FixedSize
	}

#if UNITY_EDITOR
	public Sprite DrawLayout (Sprite sprite, bool readOnly = false, params GUILayoutOption[] SizeLayoutOptions) {

		if (TransparentBoxBG == null) TransparentBoxBG = (Texture2D) EditorGUIUtility.Load("TransparentBGPattern.png");

		stored = sprite;

		//Auto Layout
		List<GUILayoutOption> options = new List<GUILayoutOption>();
		switch (layoutMode){
			case LayoutMode.FixedSize:
				if (!useGUILayoutSizeOptions) {
					options.Add(GUILayout.Width(width));
					options.Add(GUILayout.Height(height));
				}
				break;
			case LayoutMode.ScaleByWidth:
				if (!useGUILayoutSizeOptions) {
					options.Add(GUILayout.MaxWidth(maxWidth));
				}
				options.Add(GUILayout.Height(heightAuto));
				options.Add(GUILayout.ExpandHeight(false));
				break;
			case LayoutMode.ScaleByHeight:
				if (!useGUILayoutSizeOptions) {
					options.Add(GUILayout.MaxHeight(maxHeight));
				}
				options.Add(GUILayout.Width(widthAuto));
				options.Add(GUILayout.ExpandWidth(false));
				break;
			case LayoutMode.Stretch:
				if (!useGUILayoutSizeOptions) {
					options.Add(GUILayout.MaxWidth(maxWidth));
					options.Add(GUILayout.MaxHeight(maxHeight));
				}
				
				options.Add(GUILayout.ExpandWidth(true));
				options.Add(GUILayout.ExpandHeight(true));
				break;
		}
		if (useGUILayoutSizeOptions) options.AddRange(fixedOptions);
		else options.AddRange(SizeLayoutOptions);


		boxRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.box, options.ToArray());

		if (previewTexRectCache == null || previewRect == null) {
			UpdateCache(sprite);
		}

		if (stored == null) {
			GUI.Box(boxRect, string.Empty, boxStyle);
		}
		else {
			GUI.DrawTextureWithTexCoords(boxRect, TransparentBoxBG, new Rect (0, 0, widthRatio, heightRatio));
			CustomEditorUtility.DrawSideBorders(boxRect, new Color(0.24f, 0.24f, 0.24f, 1f));
			GUI.DrawTextureWithTexCoords(previewRect, sprite.texture, previewTexRectCache);
		}

		if (layoutMode == LayoutMode.ScaleByWidth) {
			float cache = heightAuto;
			if (Event.current.type == EventType.Repaint) {
				// boxRect = GUILayoutUtility.GetLastRect();
				heightAuto = boxRect.width * heightRatio/widthRatio;
				layoutJustChanged = true;
				if (heightAuto != cache) if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
			}
		}
		else if (layoutMode == LayoutMode.ScaleByHeight) {
			float cache = widthAuto;
			if (Event.current.type == EventType.Repaint) {
				// boxRect = GUILayoutUtility.GetLastRect();
				Debug.Log(boxRect.width + ", " + boxRect.height);
				widthAuto = boxRect.height * widthRatio/heightRatio;
				layoutJustChanged = true;
				if (cache != widthAuto) if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
			}
		}

		if (boxRect.Contains(Event.current.mousePosition)) {
			EditorGUI.DrawRect(boxRect, new Color(0, 0, 0, 0.2f));
			GUI.Label(boxRect, "Select", hoverLabelStyle);
			if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
		}	

		int ID = EditorGUIUtility.GetControlID(FocusType.Passive);
		if (Event.current.type == EventType.MouseDown)
		{
			if (boxRect.Contains(Event.current.mousePosition)) EditorGUIUtility.ShowObjectPicker<Sprite>(sprite, false, "t:Sprite", ID);
		}

		if (layoutJustChanged) {
			UpdateCache(sprite);
			layoutJustChanged = false;
			if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
		}

		if (Event.current.commandName == "ObjectSelectorUpdated")
		{
			if (EditorGUIUtility.GetObjectPickerControlID() == ID){ 
				stored = (Sprite) EditorGUIUtility.GetObjectPickerObject();
				UpdateCache(stored);
			}
		}

		return stored;

	}


	public Rect GetRect () {
		return boxRect;
	}

	void UpdateCache (Sprite sprite) {
		if (sprite == null) return;
		previewTexRectCache = new Rect(sprite.rect.position.x/sprite.texture.width,
									sprite.rect.position.y/sprite.texture.height,
									sprite.rect.width/sprite.texture.width, 
									sprite.rect.height/sprite.texture.height);
		previewRect = CustomEditorUtility.ScaleRect(boxRect, 0.96f);
		previewRect = CustomEditorUtility.TransfromRectRatio(previewRect, false, previewRect.size, new Vector2 (16, 9), new Vector2(sprite.rect.width, sprite.rect.height));

	}
#endif
}
