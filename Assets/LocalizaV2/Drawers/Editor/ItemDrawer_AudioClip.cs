using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace LocalizaV2 {
	public class ItemDrawer_AudioClip : ItemDrawerBase {
		
		public override void ClearDrawerCache () {
			labelDict.Clear();
			showcaseDict.Clear();
		}	

		static Dictionary<int, EditableLabel_Auto> labelDict = new Dictionary<int, EditableLabel_Auto>();

		static Dictionary<int, AudioClipShowcase> showcaseDict = new Dictionary<int, AudioClipShowcase>();

		public override void Draw (LocalizationObjectItem drawing, LocalizationObjectItem reference = null, params GUILayoutOption[] areaOptions) {
			EditableLabel_Auto currentLabel;
			AudioClipShowcase currentShowcase;
			
			if (labelDict == null) labelDict = new Dictionary<int, EditableLabel_Auto>();
			if (!labelDict.ContainsKey(drawing.GetInstanceID())) {
				currentLabel = new EditableLabel_Auto(drawing.SetID);
				labelDict.Add(drawing.GetInstanceID(), currentLabel);
			}
			else currentLabel = labelDict[drawing.GetInstanceID()];
			if (!showcaseDict.ContainsKey(drawing.GetInstanceID())) {
				currentShowcase = new AudioClipShowcase(true, 240);
				showcaseDict.Add(drawing.GetInstanceID(), currentShowcase);
			}
			else currentShowcase = showcaseDict[drawing.GetInstanceID()];

			if (reference != null && reference.ItemType != drawing.ItemType) reference = null;

			GUILayout.BeginHorizontal(areaOptions);
				GUILayout.Label("》", GUILayout.Width(10));
				currentLabel.Draw_Advanced(drawing.ID, true, null, null, IDLabelFieldLayout, IDLabelFieldLayout);
				drawing.Storage = currentShowcase.DrawLayout(drawing.Get<AudioClip>() as AudioClip);
				
				Rect inputRect = GUILayoutUtility.GetLastRect();
				if (reference == null) GUI.enabled = false;
				if (GUILayout.Button("✦", GUILayout.Width(20))) PopupWindow.Show(inputRect, new AudioRefPeeker(reference.Get<AudioClip>() as AudioClip, reference.containerGroup.database.name, inputRect));
				GUI.enabled = true;
				if (GUILayout.Button("X", GUILayout.Width(20))) drawing.Destroy();
			GUILayout.EndHorizontal();
		}
		
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
