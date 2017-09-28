using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditableLabelV1R {

	static GUILayoutOption[] StaticLabelFieldLayout, StaticAreaLayout, StaticButtonLayout;

	static GUIStyle StaticLabelStyle, StaticFieldStyle, StaticButtonStyle;

	bool isEditing;

	string EditingCache, ButtonLabel_Edit = "♢", ButtonLabel_Save = "✓";

	protected System.Action<string> Edited;

	public EditableLabelV1R (System.Action<string> EditedAction) {
		Init(EditedAction);
	}

	protected void Init (System.Action<string> EditedAction) {
		
		if (StaticLabelFieldLayout == null) {
			StaticLabelFieldLayout = new GUILayoutOption[1];
			StaticLabelFieldLayout[0] = GUILayout.ExpandHeight(true);
		}
		if (StaticAreaLayout == null) {
			StaticAreaLayout = new GUILayoutOption[1];
			StaticAreaLayout[0] = GUILayout.MinHeight(20);
		}
		if (StaticButtonLayout == null) {
			StaticButtonLayout = new GUILayoutOption[2];
			StaticButtonLayout[0] = GUILayout.MinHeight(20);
			StaticButtonLayout[1] = GUILayout.Width(24);
		}
		if (StaticLabelStyle == null) {
			StaticLabelStyle = new GUIStyle(GUI.skin.label);
			StaticLabelStyle.fontSize = 14;
		}
		if (StaticFieldStyle == null) {
			StaticFieldStyle = new GUIStyle(GUI.skin.textField);
			StaticFieldStyle.fontSize = 14;
		}
		if (StaticButtonStyle == null) {
			StaticButtonStyle = new GUIStyle(GUI.skin.button);
			StaticButtonStyle.fontSize = 12;
		}

		if (EditedAction != null) Edited = EditedAction;
	}

	public void Draw (string LabelText, bool InputUnderline = false, bool ReadOnly = false) {
		Draw(LabelText, InputUnderline, null, null, null, null, null, null, ReadOnly);
	}

	public void Draw (string LabelText, GUILayoutOption[] LabelAndFieldLayout = null, GUILayoutOption[] ButtonLayout = null, GUILayoutOption[] AreaLayout = null, bool InputUnderline = false, bool ReadOnly = false) {
		Draw(LabelText, InputUnderline, null, null, null, LabelAndFieldLayout, ButtonLayout, AreaLayout, ReadOnly);
	}

	public void Draw_RectLayout (string LabelText, Rect LabelAndFieldRect, Rect ButtonRect, Rect AreaRect, bool InputUnderline = false, bool ReadOnly = false) {
		Draw_RectLayout(LabelText, LabelAndFieldRect, ButtonRect, AreaRect, null, null, null, InputUnderline, ReadOnly);
	}

	public void Draw_RectLayout (string LabelText, Rect LabelAndFieldRect, Rect ButtonRect, Rect AreaRect, GUIStyle LabelStyle = null, GUIStyle InputFieldStyle = null, GUIStyle ButtonStyle = null, bool InputUnderline = false, bool ReadOnly = false) {

		if (LabelStyle == null) LabelStyle = StaticLabelStyle;	
		if (InputFieldStyle == null) InputFieldStyle = StaticFieldStyle;
		if (ButtonStyle == null) ButtonStyle = StaticButtonStyle;

		if (!isEditing) {
			GUI.Label(LabelAndFieldRect, LabelText, LabelStyle);
			GUILayout.Space(4);
			if (!ReadOnly) {
				if (GUI.Button(ButtonRect, ButtonLabel_Edit, ButtonStyle)) {
					isEditing = true;
					if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();		
				}
			}

			if (InputUnderline) CustomEditorUtility.DrawSideBorder(CustomEditorUtility.Directions.Bottom, LabelAndFieldRect);

		}
		else {
			bool temp = false;
			if (EditingCache == null) {
				EditingCache = LabelText;
				temp = true;
				GUI.SetNextControlName("input");			
			}	

			EditingCache = EditorGUI.TextField(LabelAndFieldRect, EditingCache, InputFieldStyle);
			GUILayout.Space(4);

			if (temp) {
				GUI.FocusControl("input");
				temp = false;
			}

			if (GUI.Button(ButtonRect, ButtonLabel_Save, ButtonStyle) || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) {
				if (EditingCache != LabelText) Edited.Invoke(this.EditingCache); 
				EditingCache = null;
				isEditing = false;
				GUI.FocusControl("0");
				if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
			}

			if (Event.current.type == EventType.MouseUp) {
				if (!LabelAndFieldRect.Contains(Event.current.mousePosition)){
					isEditing = false;
					EditingCache = null;
					GUI.FocusControl("0");
					if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
				}
			}
					
		}
	}

	public void Draw (string labelText, bool inputUnderline = false, GUIStyle LabelStyle = null, GUIStyle InputFieldStyle = null, GUIStyle ButtonStyle = null, GUILayoutOption[] LabelAndFieldLayout = null, GUILayoutOption[] ButtonLayout = null, GUILayoutOption[] AreaLayout = null, bool ReadOnly = false) {

		if (LabelAndFieldLayout == null) LabelAndFieldLayout = StaticLabelFieldLayout;
		if (AreaLayout == null) AreaLayout = StaticAreaLayout;
		if (LabelStyle == null) LabelStyle = StaticLabelStyle;	
		if (InputFieldStyle == null) InputFieldStyle = StaticFieldStyle;
		if (ButtonStyle == null) ButtonStyle = StaticButtonStyle;
		if (ButtonLayout == null) ButtonLayout = StaticButtonLayout;

		GUILayout.BeginHorizontal(AreaLayout);

		Rect labelAndFieldRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, LabelAndFieldLayout);
		Rect spacingRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none ,GUILayout.Width(4));
		Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, ButtonLayout);

		if (!isEditing) {
			GUI.Label(labelAndFieldRect, labelText, LabelStyle);
			if (!ReadOnly) {
				if (GUI.Button(buttonRect, ButtonLabel_Edit, ButtonStyle)) {
					isEditing = true;
					if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();		
				}
			}

			if (inputUnderline) CustomEditorUtility.DrawSideBorder(CustomEditorUtility.Directions.Bottom, labelAndFieldRect);

		}
		else {
			bool temp = false;
			if (EditingCache == null) {
				EditingCache = labelText;
				temp = true;
				GUI.SetNextControlName("input");			
			}	

			EditingCache = EditorGUI.TextField(labelAndFieldRect, EditingCache, InputFieldStyle);

			if (temp) {
				GUI.FocusControl("input");
				temp = false;
			}

			if (GUI.Button(buttonRect, ButtonLabel_Save, ButtonStyle) || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) {
				if (EditingCache != labelText) Edited.Invoke(this.EditingCache); 
				EditingCache = null;
				isEditing = false;
				GUI.FocusControl("0");
				if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
			}

			if (Event.current.type == EventType.MouseUp) {
				if (!labelAndFieldRect.Contains(Event.current.mousePosition)){
					isEditing = false;
					EditingCache = null;
					GUI.FocusControl("0");
					if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
				}
			}
					
		}
		GUILayout.EndHorizontal();		
	}
}
