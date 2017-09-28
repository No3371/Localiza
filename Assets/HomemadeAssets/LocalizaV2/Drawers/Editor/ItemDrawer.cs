using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalizaV2 {
	public class ItemDrawer {
		static Dictionary<string, ItemDrawerBase> drawerDict;
		public static void Draw (LocalizationObjectItem drawing,
						  LocalizationObjectItem reference = null,
						  System.Action<LocalizationObjectItem> OnContentChanged = null,
						  System.Action<LocalizationObjectItem> OnItemIDEdited = null, 
						  System.Action<LocalizationObjectItem> OnItemRemovingFromGroup = null, 
						  System.Action<LocalizationObjectItem> OnItemRemovedAndDestroying = null, 
						  params GUILayoutOption[] areaOptions) {

			drawing.OnContentChanged = OnContentChanged;
			drawing.OnIDSet = OnItemIDEdited;
			drawing.OnRemovedAndDestroying = OnItemRemovedAndDestroying;
			drawing.OnRemovingFromGroup = OnItemRemovingFromGroup;

			if (drawerDict == null){
				 drawerDict = new Dictionary<string, ItemDrawerBase>();
				 foreach (ItemTypeSupport its in LocalizaEditor.editorConfig.Supported) {
					 if (its.drawerType != null) drawerDict.Add(its.target.ToString(), System.Activator.CreateInstance(its.drawerType) as ItemDrawerBase);
				 }
			}
			if (!drawerDict.ContainsKey(drawing.ItemType)) return;
			else if (drawerDict[drawing.ItemType] == null) return;
			else drawerDict[drawing.ItemType].Draw(drawing, reference, areaOptions);
		}

		public static void Draw (LocalizationObjectItem drawing, LocalizationObjectItem reference = null, params GUILayoutOption[] areaOptions) {
			Draw(drawing, reference, null, null, null, null, areaOptions);
		}

		public static void ClearDrawerCache () {
			if (drawerDict != null) foreach (var drawer in drawerDict) if (drawer.Value != null) drawer.Value.ClearDrawerCache();
		}

		public static void ResetCache () {
			drawerDict = null;
		}
	}
}