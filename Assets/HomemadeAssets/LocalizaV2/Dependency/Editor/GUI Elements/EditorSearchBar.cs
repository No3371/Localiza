using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorSearchBar {


	public EditorSearchBar () {

	}
	
	UnityEditor.IMGUI.Controls.SearchField temp = new UnityEditor.IMGUI.Controls.SearchField();

	string EditingCache;

	public string DrawLayout (string cache, System.Action OnSearch, GUILayoutOption[] InputFieldLayout = null, GUILayoutOption[] SearchButtonLayout = null) {
		GUILayout.BeginHorizontal();
			EditingCache = temp.OnGUI(cache, InputFieldLayout);
		GUILayout.EndHorizontal();
		return cache;
	}
}
