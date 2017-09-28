using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace LocalizaV2 {
	public class ItemDrawer_Sprite : ItemDrawerBase {
		
		public override void ClearDrawerCache () {
			labelDict.Clear();
			selectorDict.Clear();
		}	

		static Dictionary<int, EditableLabel_Auto> labelDict = new Dictionary<int, EditableLabel_Auto>();

		static Dictionary<int, SpriteSelector> selectorDict = new Dictionary<int, SpriteSelector>();

		public override void Draw (LocalizationObjectItem drawing, LocalizationObjectItem reference = null, params GUILayoutOption[] areaOptions) {
			EditableLabel_Auto currentLabel;
			SpriteSelector currentSelector;
			
			if (labelDict == null) labelDict = new Dictionary<int, EditableLabel_Auto>();
			if (!labelDict.ContainsKey(drawing.GetInstanceID())) {
				currentLabel = new EditableLabel_Auto(drawing.SetID);
				labelDict.Add(drawing.GetInstanceID(), currentLabel);
			}
			else currentLabel = labelDict[drawing.GetInstanceID()];
			if (!selectorDict.ContainsKey(drawing.GetInstanceID())) {
				currentSelector = SpriteSelector.Create_ScaleByWidth();
				selectorDict.Add(drawing.GetInstanceID(), currentSelector);
			}
			else currentSelector = selectorDict[drawing.GetInstanceID()];

			if (reference != null && reference.ItemType != drawing.ItemType) reference = null;

			GUILayout.BeginVertical(areaOptions);
				drawing.Storage = currentSelector.DrawLayout(drawing.Get<Sprite>() as Sprite, false, areaOptions);
				Rect inputRect = GUILayoutUtility.GetLastRect();
				GUILayout.BeginHorizontal();
					GUILayout.Label("》", GUILayout.Width(10));
					currentLabel.Draw(drawing.ID, true);
					
					if (reference == null) GUI.enabled = false;
					if (GUILayout.Button("✦", GUILayout.Width(20))) PopupWindow.Show(inputRect, new SpriteRefPeeker(reference.Get<Sprite>() as Sprite, reference.containerGroup.database.name, inputRect));
					GUI.enabled = true;
					if (GUILayout.Button("X", GUILayout.Width(20))) drawing.Destroy();
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		
		static GUIStyle _IDFieldStyle, _IDLabelStyle, refLabelStyle;

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
	}
}
