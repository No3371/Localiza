using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditableLabelDemo : EditorWindow, IHasCustomMenu {

	private static EditableLabelDemo _editor;
	public static EditableLabelDemo editor { get { AssureEditor(); return _editor; }}

	public static void AssureEditor()
	{
		if (_editor == null) OpenEditor(); 
	}

	[MenuItem("Demo/EditableLabelDemo")]
	public static EditableLabelDemo OpenEditor(){
		_editor = GetWindow<EditableLabelDemo>();
		_editor.minSize = new Vector2(360, 300);
		_editor.maxSize = new Vector2(360, 300);

		_editor.titleContent = new GUIContent ("Demo");
		_editor.wantsMouseMove = true;	//Required For EditableLabelSimple.
		return _editor;
	}

	EditableLabelV1R classic;

	EditableLabel_Auto simple, simpleCustomized;

	GUIStyle style;

	GUIStyle LabelStyle {
		get {
			if (style == null) {
				style = new GUIStyle(GUI.skin.label);
				style.fontSize = 16;
				style.richText = true;
				style.normal.textColor = Color.blue;
			}
			return style;
		}
	}

	GUILayoutOption[] layout;

	GUILayoutOption[] FieldLayout {
		get {
			if (layout == null) {
				layout = new GUILayoutOption[2];
				layout[0] = GUILayout.Width(144);
				layout[1] = GUILayout.Height(40);
			}
			return layout;
		}
	}

	void OnGUI () {
		GUILayout.BeginHorizontal();
			GUILayout.Space(8);		
			GUILayout.BeginVertical();
				GUILayout.Space(8);		
				GUILayout.Label(ClassicCache);
				if (classic == null) classic = new EditableLabelV1R(ClassicEdited);
				classic.Draw(ClassicCache, true, false);

				GUILayout.Space(8);
				GUILayout.Label(SimpleCache);
				if (simple == null) simple = new EditableLabel_Auto(SimpleEdited);
				simple.Draw(SimpleCache, true, false);					

				GUILayout.Space(8);
				GUILayout.Label("Customization");
				if (simpleCustomized == null) simpleCustomized = new EditableLabel_Auto(SimpleEdited, true);
				simpleCustomized.Draw_Advanced(SimpleCache, true, LabelStyle, null, FieldLayout);
			GUILayout.EndVertical();
			GUILayout.Space(8);		
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();		
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Send me a coffee if you like it :)")) Application.OpenURL("https://www.facebook.com/");
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	string ClassicCache = "Classic: Test", SimpleCache = "Simple: Test";

	void ClassicEdited (string Edited) {
		ClassicCache = Edited;
		this.ShowNotification(new GUIContent("Classic! New label string: " + Edited));
	}

	void SimpleEdited (string Edited) {
		SimpleCache = Edited;
		this.ShowNotification(new GUIContent("Simple! New label string: " + Edited));
	}

	public void AddItemsToMenu (GenericMenu menu) {
		menu.AddItem(new GUIContent("Re-cache styles and layout"), false, ReCacheStyleAndLayout);
	}

	void ReCacheStyleAndLayout () {
		style = null;
		layout = null;
	}
}
