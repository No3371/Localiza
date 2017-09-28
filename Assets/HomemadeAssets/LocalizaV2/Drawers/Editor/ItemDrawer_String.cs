using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace LocalizaV2 {
	public class ItemDrawer_String : ItemDrawerBase {

		public override void ClearDrawerCache () {
			labelDict.Clear();
		}	

		static Dictionary<int, EditableLabel_Auto> labelDict = new Dictionary<int, EditableLabel_Auto>();

		public override void Draw (LocalizationObjectItem drawing, LocalizationObjectItem reference = null, params GUILayoutOption[] areaOptions) {
			EditableLabel_Auto currentLabel;
			
			if (labelDict == null) labelDict = new Dictionary<int, EditableLabel_Auto>();
			if (!labelDict.ContainsKey(drawing.GetInstanceID())) {
				currentLabel = new EditableLabel_Auto(drawing.SetID);
				labelDict.Add(drawing.GetInstanceID(), currentLabel);
			}
			else currentLabel = labelDict[drawing.GetInstanceID()];
			
			if (reference != null && reference.ItemType != drawing.ItemType) reference = null;

			GUILayout.BeginHorizontal(areaOptions);
				GUILayout.Label("》", IDLabelStyle, GUILayout.Width(10));
				currentLabel.Draw_Advanced(drawing.ID, true, null, null, IDLabelFieldLayout, IDLabelFieldLayout);
				drawing.StoredString = EditorGUILayout.TextArea(drawing.Get<string>() as string, TextAreaStyle, GUILayout.ExpandWidth(true));				
				Rect textAreaRect = GUILayoutUtility.GetLastRect();

				if (reference == null) GUI.enabled = false;
				if (GUILayout.Button("✦", GUILayout.Width(20))) PopupWindow.Show(textAreaRect, new TextRefPeeker(reference.Get<string>() as string, 
																											reference.containerGroup.database.name, textAreaRect, RefLabelStyle));
				GUI.enabled = true;
				if (GUILayout.Button("X", GUILayout.Width(20))) drawing.Destroy();
			GUILayout.EndHorizontal();
		}
		
		static GUIStyle textAreaStyle, _IDFieldStyle, _IDLabelStyle, refLabelStyle;

		static GUILayoutOption[] _IDLabelFieldLayout;

		GUILayoutOption[] IDLabelFieldLayout{
			get{
				if (_IDLabelFieldLayout == null) {
					_IDLabelFieldLayout = new GUILayoutOption[2];
					_IDLabelFieldLayout[0] = GUILayout.Height(24);
					_IDLabelFieldLayout[1] = GUILayout.Width(440);
				}
				return _IDLabelFieldLayout;
			}
		}

		GUIStyle IDFieldStyle { 
			get {
				if (_IDFieldStyle == null) {
					_IDFieldStyle = new GUIStyle(GUI.skin.textField);
					_IDFieldStyle.font = Resources.Load<Font>("SourceHanSansTC-Light");
					_IDFieldStyle.fontSize = 14;
					_IDFieldStyle.richText = true;
				}
				return _IDFieldStyle;
			}
		}		
		
		GUIStyle IDLabelStyle { 
			get {
				if (_IDLabelStyle == null) {
					_IDLabelStyle = new GUIStyle(GUI.skin.label);
					_IDLabelStyle.font = Resources.Load<Font>("SourceHanSansTC-Light");
					_IDLabelStyle.fontSize = 14;
					_IDLabelStyle.richText = true;
					_IDLabelStyle.alignment = TextAnchor.MiddleLeft;
				}
				return _IDLabelStyle;
			}
		}

		GUIStyle TextAreaStyle { 
			get {
				if (textAreaStyle == null) {
					textAreaStyle = new GUIStyle(GUI.skin.textArea);
					textAreaStyle.font = Resources.Load<Font>("SourceHanSansTC-Light");
					textAreaStyle.fontSize = 14;
					textAreaStyle.richText = true;
					textAreaStyle.wordWrap = true;
				}
				return textAreaStyle;
			}
		}		

		GUIStyle RefLabelStyle { 
			get {
				if (refLabelStyle == null) {
					refLabelStyle = new GUIStyle(GUI.skin.label);
					refLabelStyle.font = Resources.Load<Font>("SourceHanSansTC-Light");
					refLabelStyle.fontSize = 11;
					refLabelStyle.richText = true;
					refLabelStyle.wordWrap = true;
				}
				return refLabelStyle;
			}
		}
	}
}
