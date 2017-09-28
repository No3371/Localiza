using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LocalizaV2 {
	public abstract class ItemDrawerBase {
		
		static Dictionary<int, EditableLabel> labelDict = new Dictionary<int, EditableLabel>();

		public abstract void Draw (LocalizationObjectItem drawing, LocalizationObjectItem reference = null, params GUILayoutOption[] areaOptions);

		public abstract void ClearDrawerCache ();
		
		static GUIStyle _IDFieldStyle, _IDLabelStyle;

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
