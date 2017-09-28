using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditableLabel {

	protected float areaHeight = -1, areaWidth = -1, labelWidth = -1, buttonWidth = -1, buttonHeight = -1;

	protected bool editing, labelExpandWidth, areaExpandWidth;

	public string editCache;

	protected string editText, saveText;

	protected Vector2 topLeftPoint, bottomRightPoint;

	protected System.Action<string> Edited;

	protected int fontSize, inputControlID = 0, buttonControlID = 0;

	Rect rect;

	GUIStyle LabelStyle, InputFieldStyle;

	public EditableLabel (GUILayoutOption[] labelOption, GUILayoutOption[] areaOption, GUIStyle labelStyle = null, GUIStyle inputFieldStyle = null) {

	}

	public EditableLabel (int fontSize, int areaWidth, bool labelExpandWidth, System.Action<string> action, GUIStyle labelStyle = null, GUIStyle inputFieldStyle = null, float areaHeight = -1) {
		this.fontSize = fontSize;
		this.labelExpandWidth = labelExpandWidth;
		this.areaExpandWidth = false;
		this.areaWidth = areaWidth;
		if (areaHeight > 0) this.areaHeight = areaHeight;
		Init(action, labelStyle, inputFieldStyle);
	}
	
	public EditableLabel (int fontSize, int areaWidth, int labelWidth, System.Action<string> action, GUIStyle labelStyle = null, GUIStyle inputFieldStyle = null) {
		this.fontSize = fontSize;
		this.labelExpandWidth = false;
		this.areaExpandWidth = false;
		this.areaWidth = areaWidth;
		this.labelWidth = labelWidth;
		Init(action, labelStyle, inputFieldStyle);
	}
	
	public EditableLabel (int fontSize, bool areaExpandWidth, bool labelExpandWidth, System.Action<string> action, GUIStyle labelStyle = null, GUIStyle inputFieldStyle = null) {
		this.fontSize = fontSize;
		this.labelExpandWidth = labelExpandWidth;
		this.areaExpandWidth = areaExpandWidth;
		Init(action, labelStyle, inputFieldStyle);
	}
	
	public EditableLabel (int fontSize, bool labelExpandWidth, System.Action<string> action, GUIStyle labelStyle = null, GUIStyle inputFieldStyle = null) {
		this.fontSize = fontSize;
		this.labelExpandWidth = labelExpandWidth;
		Init(action, labelStyle, inputFieldStyle);
	}
	
	public EditableLabel (int fontSize, System.Action<string> action, GUIStyle labelStyle = null, GUIStyle inputFieldStyle = null) {
		this.fontSize = fontSize;
		this.labelExpandWidth = true;
		this.areaExpandWidth = true;
		Init(action, labelStyle, inputFieldStyle);
	}

	protected void Init (System.Action<string> action, GUIStyle labelStyle = null, GUIStyle inputFieldStyle = null) {
		if (areaHeight < 0) this.areaHeight = fontSize + 6;
		this.editing = false;
		this.editCache = null;
		Edited += action;
		SetButtonText("✐", "✓");
		SetButtonSize();
        if (labelStyle == null) {
			LabelStyle = new GUIStyle(GUI.skin.label);
        	LabelStyle.fontSize = fontSize;
		}
		else this.LabelStyle = labelStyle;
        if (inputFieldStyle == null) {
			InputFieldStyle = new GUIStyle(GUI.skin.textField);
        	InputFieldStyle.fontSize = fontSize;
		}
		else InputFieldStyle = inputFieldStyle;
	}

	public void SetButtonText (string editButtonText, string saveButtonText) {
		this.saveText = saveButtonText;
		this.editText = editButtonText;
	}

	public void SetButtonSize (float width = 24, float height = -1){
		buttonWidth = width;
		buttonHeight = height;
	}

	public void DrawLayout (string labelText, bool inputUnderline = false, GUILayoutOption[] labelLayout = null, GUILayoutOption[] fieldLayout = null) {
		if (areaWidth > 0) GUILayout.BeginHorizontal(GUILayout.Width(areaWidth));
		else GUILayout.BeginHorizontal(GUILayout.ExpandWidth(areaExpandWidth));

		if (!editing) {
			if (labelWidth > 0) GUILayout.Label(labelText, LabelStyle, GUILayout.Width(labelWidth), GUILayout.Height(areaHeight));
			else GUILayout.Label(labelText, LabelStyle, GUILayout.ExpandWidth(labelExpandWidth), GUILayout.Height(areaHeight));	

			if (inputUnderline) CustomEditorUtility.DrawSideBorder(CustomEditorUtility.Directions.Bottom, GUILayoutUtility.GetLastRect());
	
			if (GUILayout.Button(editText, GUILayout.ExpandWidth(false), GUILayout.Height(areaHeight))) editing = true; 
		}
		else {
			bool temp = false;
			if (editCache == null) {
				editCache = labelText;
				temp = true;
				GUI.SetNextControlName("input");			
			}	
			if (labelWidth > 0)	editCache = EditorGUILayout.TextField(editCache, InputFieldStyle, GUILayout.Width(labelWidth + 60), GUILayout.MinWidth(80), GUILayout.Height(areaHeight));
			else editCache = EditorGUILayout.TextField(editCache, InputFieldStyle, GUILayout.ExpandWidth(labelExpandWidth), GUILayout.MinWidth(80), GUILayout.Height(areaHeight));
			if (temp) {
				GUI.FocusControl("input");
				temp = false;
			}
			Rect inputRect = GUILayoutUtility.GetLastRect();
		
			if (GUILayout.Button(saveText, GUILayout.ExpandWidth(false), GUILayout.Height(areaHeight)) || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) {
				if (editCache != labelText) Edited.Invoke(this.editCache); 
				editCache = null;
				editing = false;
				GUI.FocusControl("0");
				if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
			}

			Rect buttonRect = GUILayoutUtility.GetLastRect();

			if (Event.current.type == EventType.MouseUp) {
				if (!inputRect.Contains(Event.current.mousePosition) && !buttonRect.Contains(Event.current.mousePosition)){
					editing = false;
					editCache = null;
					GUI.FocusControl("0");
					if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
				}
			}
					
		}
		GUILayout.EndHorizontal();		

		if (Event.current.type == EventType.Repaint){
			if (rect == null) rect = GUILayoutUtility.GetLastRect();
			topLeftPoint = new Vector2(rect.xMin, rect.yMin);
			bottomRightPoint = new Vector2(rect.xMax, rect.yMax);			
		}
	}

	public void Draw (Rect position, string labelText, bool inputUnderline = false) {
		Rect labelRect;
		if (labelExpandWidth) labelRect = new Rect(position.position, position.size - new Vector2(buttonWidth, 0));
		else labelRect = new Rect(position.position, new Vector2(labelWidth, position.height));
		Rect inputRect = new Rect (labelRect.position, labelRect.size - new Vector2(buttonWidth + 8, 0));
		Rect buttonRect;
		buttonHeight = position.height;
		if (labelExpandWidth) buttonRect = new Rect (position.x + position.width - buttonWidth, position.y, buttonWidth, buttonHeight); 
		else buttonRect = new Rect (position.x + labelWidth + 8, position.y, buttonWidth, buttonHeight); 

		int cache;

		if (!editing) {
			cache = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = fontSize;			
			if (labelWidth > 0) GUI.Label(labelRect, labelText);
			else GUI.Label(labelRect, labelText);			
			GUI.skin.label.fontSize = cache;

			if (inputUnderline) CustomEditorUtility.DrawSideBorder(CustomEditorUtility.Directions.Bottom, labelRect);
	
			if (GUI.Button(buttonRect, editText)) editing = true; 
		}
		else {
			if (editCache == null) editCache = labelText;			
			cache = GUI.skin.textField.fontSize;
			GUI.skin.textField.fontSize = fontSize;		
			if (labelWidth > 0)	editCache = EditorGUI.TextField(inputRect, editCache);
			else editCache = EditorGUI.TextField(inputRect, editCache);				
			GUI.skin.textField.fontSize = cache;
			
			if (GUI.Button(buttonRect, saveText) || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) {
				if (editCache != labelText) Edited.Invoke(this.editCache); 
				editCache = null;
				editing = false;
				GUI.FocusControl("0");
				if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
			}
			
			if (Event.current.type == EventType.MouseUp) {
				if (!inputRect.Contains(Event.current.mousePosition) && !buttonRect.Contains(Event.current.mousePosition)){
					editing = false;
					editCache = null;
					if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
				}
			}
		}	

	}

	public Rect GetRect () {
		return rect;
	}
}
