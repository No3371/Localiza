using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

///This gui element require wantsMouseMove of the window set to true to react properly., otherwise it will be as slow as an old turtle.
public class EditableLabel_Auto {

	static GUILayoutOption[] staticLabelFieldLayout, staticAreaLayout;

	static GUIStyle staticLabelStyle, staticFieldStyle, TipStyle;

	bool isEditing, showEnterTip, isHovering;

	string EditingCache;

	protected System.Action<string> Edited;

	public EditableLabel_Auto (System.Action<string> EditedAction, bool ShowEnterTip = false) {
		Init(EditedAction);
		this.showEnterTip = ShowEnterTip;
	}

	protected void Init (System.Action<string> EditedAction) {
		
		if (staticLabelFieldLayout == null) {
			staticLabelFieldLayout = new GUILayoutOption[2];
			staticLabelFieldLayout[0] = GUILayout.ExpandHeight(false);
			staticLabelFieldLayout[1] = GUILayout.MinHeight(20);
		}
		if (staticAreaLayout == null) {
			staticAreaLayout = new GUILayoutOption[2];
			staticAreaLayout[0] = GUILayout.MinHeight(16);
			staticAreaLayout[1] = GUILayout.Width(400);
		}
		if (staticLabelStyle == null) {
			staticLabelStyle = new GUIStyle(GUI.skin.label);
			staticLabelStyle.fontSize = 14;
			staticLabelStyle.richText = true;
			staticLabelStyle.hover.textColor = new Color (0.3f, 0.2f, 1f);
		}
		if (staticFieldStyle == null) {
			staticFieldStyle = new GUIStyle(GUI.skin.textField);
			staticFieldStyle.fontSize = 14;
		}
		if (TipStyle == null) {
			TipStyle = new GUIStyle(GUI.skin.label);
			TipStyle.fontSize = 10;
		}

		if (EditedAction != null) Edited = EditedAction;
	}

	public string Draw (string LabelText, bool InputUnderline = false, bool ReadOnly = false) {
		return Draw_Advanced(LabelText, InputUnderline, null, null, null, null, ReadOnly);
	}

	public string Draw_CustomLayout (string LabelText, GUILayoutOption[] LabelAndFieldLayout = null, GUILayoutOption[] AreaLayout = null, bool InputUnderline = false, bool ReadOnly = false) {
		return Draw_Advanced(LabelText, InputUnderline, null, null, LabelAndFieldLayout, AreaLayout, ReadOnly);
	}

	public string Draw_Advanced (string labelText, bool inputUnderline = false, GUIStyle labelStyle = null, GUIStyle inputFieldStyle = null, GUILayoutOption[] labelAndFieldLayout = null, GUILayoutOption[] areaLayout = null, bool ReadOnly = false) {

		if (labelAndFieldLayout == null) labelAndFieldLayout = staticLabelFieldLayout;
		if (areaLayout == null) areaLayout = staticAreaLayout;
		if (labelStyle == null) labelStyle = staticLabelStyle;
		if (inputFieldStyle == null) inputFieldStyle = staticFieldStyle;

		Rect labelAndFieldRect = GUILayoutUtility.GetRect(GUIContent.none, new GUIStyle(), labelAndFieldLayout);

		GUILayout.BeginHorizontal(areaLayout);

		if (!isEditing) {

			if (Event.current.type == EventType.MouseMove && !ReadOnly) {
				isHovering = (labelAndFieldRect.Contains(Event.current.mousePosition))? true : false;
				if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();	
			}

			// if (isHovering) {
			// 	GUI.Label(labelAndFieldRect, "<color=#4228FF>" + labelText + "</color>", labelStyle);
			// }
			GUI.Label(labelAndFieldRect, labelText, labelStyle);

			if (inputUnderline) CustomEditorUtility.DrawSideBorder(CustomEditorUtility.Directions.Bottom, labelAndFieldRect);

			if (!ReadOnly) EditorGUIUtility.AddCursorRect(labelAndFieldRect, MouseCursor.Text);

			if (!ReadOnly) if (Event.current.type == EventType.MouseUp) {
				if (labelAndFieldRect.Contains(Event.current.mousePosition)) {
					isEditing = true;
					if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();					
				}

			}
		}
		else {
			bool temp = false;
			if (EditingCache == null) {
				EditingCache = labelText;
				temp = true;
				GUI.SetNextControlName("input");			
			}	

			EditingCache = EditorGUI.TextField(labelAndFieldRect, EditingCache, inputFieldStyle);

			if (showEnterTip) GUI.Label(new Rect (labelAndFieldRect.xMax + 4, labelAndFieldRect.yMax - 14, 128, 14), "Press Enter to save.");

			if (temp) {
				GUI.FocusControl("input");
				temp = false;
			}
		
			if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) {
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
		return EditingCache;
	}

}
