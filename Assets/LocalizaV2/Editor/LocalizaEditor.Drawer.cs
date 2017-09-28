using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace LocalizaV2 {
	public partial class LocalizaEditor {

		delegate void Drawer();
		
		Drawer DrawHeader, DrawSideBar, DrawEditor;

		Drawer DrawHeaderBuffer, DrawSideBarBuffer, DrawEditorBuffer;

# region Header
		void DrawHeader_None () {
			int pickerID = EditorGUIUtility.GetControlID(FocusType.Passive);
			if (localizationTag == null) localizationTag = new EditableLabel_Auto(OnLocalizationTagEdited, true);

			GUILayout.BeginHorizontal();			
				GUILayout.Space(HeaderLeftMargin);
				GUILayout.BeginVertical(GUILayout.Width(position.width), GUILayout.Height(HeaderHeight));
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();			
						localizationTag.Draw_Advanced("None", false, labelStyle_DatabaseTag, fieldStyle_DatabaseTag, LocalizationTagLabelLayout, LocalizationTagAreaLayout, true);
					GUILayout.EndHorizontal();
						GUILayout.Space(4);
					GUILayout.BeginHorizontal();
						GUILayout.Label("Load a localization file to start editing.", GUILayout.ExpandWidth(false));
						GUI.SetNextControlName("temp");
						if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) EditorGUIUtility.ShowObjectPicker<LocalizationDatabase>(databaseCache, false, "t:LocalizationDatabase", pickerID);
						GUILayout.Label("or create a new empty localization file.", GUILayout.ExpandWidth(false));
						if (GUILayout.Button("+", GUILayout.ExpandWidth(false))) {
							if (CreateNewDatabase()) LoadDatabase(AssetDatabase.LoadMainAssetAtPath(editorConfig.SavePath + editorConfig.defaultDatabaseName) as LocalizationDatabase);
						}
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		

			if (Event.current.commandName == "ObjectSelectorUpdated")
			{
				if (EditorGUIUtility.GetObjectPickerControlID() == pickerID){ 
					databaseCacheBuffer = (EditorGUIUtility.GetObjectPickerObject() as LocalizationDatabase);	
					Repaint();
				}
			}
		}

		EditableLabel_Auto localizationTag;


		void DrawHeader_Normal () {
			if (databaseCache == null) {
				ResetWindow();
			}
			if (localizationTag == null) localizationTag = new EditableLabel_Auto(OnLocalizationTagEdited, true);
			int pickerID = EditorGUIUtility.GetControlID(FocusType.Passive);
			GUILayout.BeginHorizontal(GUILayout.Width(position.width), GUILayout.Height(HeaderHeight));
				GUILayout.Space(HeaderLeftMargin);

				//Main Header (Left)
				GUILayout.BeginVertical(GUILayout.Width(HeaderMainWidth));
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
						localizationTag.Draw_Advanced(databaseCache.name, false, labelStyle_DatabaseTag, fieldStyle_DatabaseTag, LocalizationTagLabelLayout, LocalizationTagAreaLayout);
					GUILayout.EndHorizontal();
					GUILayout.Space(4);
					GUILayout.BeginHorizontal();
						GUILayout.Label("Edit another localization file.", GUILayout.ExpandWidth(false));
						if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) EditorGUIUtility.ShowObjectPicker<LocalizationDatabase>(databaseCache, false, "t:LocalizationRootGroup", pickerID);
						GUILayout.Label("or create a new empty localization file.", GUILayout.ExpandWidth(false));
						if (GUILayout.Button("+", GUILayout.ExpandWidth(false))) CreateNewDatabase();
						GUILayout.Label("or copy this localization file.", GUILayout.ExpandWidth(false));
						if (GUILayout.Button("➜", GUILayout.ExpandWidth(false))) CopyDatabase();
					GUILayout.EndHorizontal();
					GUILayout.Space(8);
				GUILayout.EndVertical();

				//Util Header (Right)	
				CustomEditorUtility.DrawSideBorder(CustomEditorUtility.Directions.Right, GUILayoutUtility.GetLastRect());
				DrawUtilityBlock();	
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			//Detect database selected
			if (Event.current.commandName == "ObjectSelectorUpdated")
			{
				if (EditorGUIUtility.GetObjectPickerControlID() == pickerID){ 
					databaseCacheBuffer = EditorGUIUtility.GetObjectPickerObject() as LocalizationDatabase;
					Repaint();
				}
			}
		}

		int ItemCountCache = -1, RefItemCountCache = -1;

		void DrawUtilityBlock () {
			GUILayout.BeginVertical(GUILayout.Width(HeaderUtilWidth));
				GUILayout.Space(4);
				GUILayout.BeginHorizontal();			
					if (referencingDatabase != null) GUILayout.Label("<color=green><b>✦ Reference Mode</b></color>", labelStyle_rich);
					else GUILayout.Label("<color=grey>- Reference unset</color>", labelStyle_rich);
					EditorGUI.BeginChangeCheck();
					referencingDatabase = EditorGUILayout.ObjectField(referencingDatabase, typeof(LocalizationDatabase), false) as LocalizationDatabase;
					if (referencingDatabase == databaseCache) 
						referencingDatabase = null;
					if (EditorGUI.EndChangeCheck()) {
						RefItemCountCache = (referencingDatabase == null)? -1: referencingDatabase.ItemCount;
					}
					GUILayout.Space(8);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				if (referencingDatabase != null) {
					if (databaseCache != null) if (ItemCountCache < 0) ItemCountCache = databaseCache.ItemCount;	
					if (RefItemCountCache < 0) RefItemCountCache = referencingDatabase.ItemCount;					
					GUILayout.Label("Item Count: " + ItemCountCache + "/" + RefItemCountCache);

					GUILayout.FlexibleSpace();

					if (GUILayout.Button("Missed...", GUILayout.Width(UtilButtonWidth))) ShowMissedItemListFromRef();
					if (groupCache == null) GUI.enabled = false;
					if (GUILayout.Button("Sort", GUILayout.Width(UtilButtonWidth))) {
						groupCache.storedItems.Sort();
						FilterAndCategorize(groupCache.storedItems, true);
						OnDatabaseChanged();
						Repaint();
					}
					if (GUILayout.Button("Add...", GUILayout.Width(UtilButtonWidth))) ShowAddItemMenu();
					GUI.enabled = true;
				}
				else{
					if (ItemCountCache < 0) ItemCountCache = databaseCache.ItemCount;			
					GUILayout.Label("<color=grey>Item Count: " + ItemCountCache + "</color>", labelStyle_rich);

					GUILayout.FlexibleSpace();

					GUI.enabled = false;
					if (GUILayout.Button("Missed...", GUILayout.Width(UtilButtonWidth))) ShowMissedItemListFromRef();
					GUI.enabled = true;
					if (groupCache == null) GUI.enabled = false;
					if (GUILayout.Button("Sort", GUILayout.Width(UtilButtonWidth))) {
						groupCache.storedItems.Sort();
						FilterAndCategorize(groupCache.storedItems, true);
						OnDatabaseChanged();
						Repaint();
					}
					if (GUILayout.Button("Add...", GUILayout.Width(UtilButtonWidth))) ShowAddItemMenu();
					GUI.enabled = true;
				}
				GUILayout.Space(8);
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

#endregion

	#region Sidebar
		Vector2 scrollPos_SideBar;

		void DrawSideBar_None(){
			scrollPos_SideBar = Vector2.zero;
			GUILayout.BeginVertical(GUILayout.Width(SidebarWidth));
				TextAnchor temp = GUI.skin.label.alignment;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUILayout.Label("Load a file to start editing.", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				GUI.skin.label.alignment = temp;
			GUILayout.EndVertical();

			//Draw devider between the sidebar and the editor.
			Rect tempRect = GUILayoutUtility.GetLastRect();
			CustomEditorUtility.DrawSideBorder(CustomEditorUtility.Directions.Right, tempRect);
		}


		void DrawSideBar_Normal(){
			scrollPos_SideBar = GUILayout.BeginScrollView(scrollPos_SideBar, GUILayout.Width(SidebarWidth));
				GUILayout.BeginVertical();
					GUILayout.Space(4);
					GUILayout.Space(8);
					GUILayout.BeginHorizontal();
						GUILayout.Label("<size=14><b>Groups</b></size>", labelStyle_rich, GUILayout.ExpandWidth(false));					
						if (GUILayout.Button ("+", GUILayout.Width(20))) CreateNewItemGroup();
					GUILayout.EndHorizontal();
					GUILayout.Space(8);
					GUILayout.BeginHorizontal();
						GUILayout.Space(4);
						GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
							for (int i = 0; i < databaseCache.entryGroupList.Count; i++) {
								DrawItemGroup (GUILayoutUtility.GetRect(SidebarWidth - 24, 20), databaseCache.entryGroupList[i]);
								GUILayout.Space(4);
							}
							DrawItemGroup(GUILayoutUtility.GetRect(SidebarWidth - 24, 20), databaseCache.RecycleBin, false, false);
						GUILayout.EndVertical();				
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			GUILayout.EndScrollView();

			//Draw devider between the sidebar and the editor.
			Rect tempRect = GUILayoutUtility.GetLastRect();
			CustomEditorUtility.DrawSideBorder(CustomEditorUtility.Directions.Right, tempRect);
		}

		EditableLabelV1R editableSelectedGroupID;

		void DrawItemGroup(Rect position, LocalizationItemGroup group, bool AllowDelete = true, bool AllowRename = true){
			Rect selectionRect = new Rect (position.xMin, position.yMin, position.width - 8, position.height);
			Rect labelRect = new Rect(position.xMin, position.yMin, position.width - 58, position.height);
			Rect renameRect = new Rect(position.xMax - 54, position.yMin, 24, position.height);
			Rect deleteRect = new Rect(position.xMax - 26, position.yMin, 24, position.height);

			if (editableSelectedGroupID == null) editableSelectedGroupID = new EditableLabelV1R(RenameItemGroup);

			if (groupCache == group) {
				EditorGUI.DrawRect(selectionRect, Color.gray);
				editableSelectedGroupID.Draw_RectLayout(group.groupID, labelRect, renameRect, selectionRect, LabelStyle_Sidebar, null, null, false, !AllowRename);
			}
			else {
				GUI.Label(labelRect, group.groupID, LabelStyle_Sidebar);
			}

			if (AllowDelete)
				if (GUI.Button(deleteRect, "X"))
					if (EditorUtility.DisplayDialog("Deleting the group", "Are you sure to delete this lineGroup? All lines in the group will be lost.", "Delete", "Cancel"))
						DeleteItemGroup(group);
			

			if ((Event.current.type == EventType.MouseDown) && (Event.current.button == 0)  && (groupCache != group) && selectionRect.Contains(Event.current.mousePosition)) {
				groupCacheBuffer = group;
				#if UNITY_EDITOR
				UnityEditor.Selection.activeObject = group;
				#endif
				Repaint();
			}
		}

		void RenameItemGroup (string newID) {
			if (!databaseCache.entryGroupList.TrueForAll(e => e.groupID != newID)) {
				ShowNotification(new GUIContent("A group with this ID has existed!"));
				return;
			}
			groupCache.groupID = newID;
		}

	#endregion
	
	
	#region Editor

		Vector2 scrollPos_Editor;
		
		Dictionary<string, float> scrollPos_JumpFlags;

		void DrawEditor_Normal(){
			if (groupCache == null) {
				DrawEditor_NoSelection();
				return;
			}
			GUILayout.BeginHorizontal();
				GUILayout.Space(8);
				scrollPos_Editor = GUILayout.BeginScrollView(scrollPos_Editor);							//======================================
					EditorGUI.BeginChangeCheck();						
					GUILayout.BeginVertical();					//==========================================	
						GUILayout.Space(16);	
						if (groupCache.storedItems.Count > 0){
							foreach (var objectList in SupportedObjectTypeListDict) {
								if (objectList.Value.Count > 0) {
									if (SupportedTypeDrawerMode[objectList.Value[0].ItemType] == "Horizontal") DrawItemListVertical(objectList.Value.ToList(), true);
									else if (SupportedTypeDrawerMode[objectList.Value[0].ItemType] == "Grid") DrawItemGrid(objectList.Value.ToList(), EditorWidth*0.96f, 3, 0.06f, true, 14, true);
									GUILayout.Space(16);
								}
							}
						}
						GUILayout.FlexibleSpace();
					GUILayout.EndVertical();												//===========================================
					if (EditorGUI.EndChangeCheck()) {
						OnDatabaseChanged();
					}
				GUILayout.EndScrollView(); 	
				GUILayout.Space(4);		
			GUILayout.EndHorizontal();
																//======================================
		}

		void DrawEditor_NoSelection(){
			TextAnchor temp = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUILayout.Label("Select a group to start editing.", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			GUI.skin.label.alignment = temp;
		}

		void DrawEditor_Empty(){
			GUILayout.BeginVertical();	
				GUILayout.FlexibleSpace();		
				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (groupCache) if (groupCache.AllowAddItemByUser) DrawAddItemBar();
					else GUILayout.Label("This group does not allow users adding new items.");
					GUILayout.FlexibleSpace();		
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			if (groupCache) if (groupCache.ItemCount > 0) DrawEditorBuffer = DrawEditor_Normal;
		}
		
		Dictionary<string, bool> foldOutGroup;


		void FilterAndCategorize (List<LocalizationObjectItem> list, bool ClearCache = false) {
			if (ClearCache) {
				foreach(var objectList in SupportedObjectTypeListDict){
					objectList.Value.Clear();
				}
			}
			foreach (var item in list) {
				if (SupportedObjectTypeListDict.ContainsKey(item.ItemType)) SupportedObjectTypeListDict[item.ItemType].Add(item);
			}
		}
		void FilterAndCategorizeNewItem (LocalizationObjectItem item) {			
			if (SupportedObjectTypeListDict.ContainsKey(item.ItemType)) SupportedObjectTypeListDict[item.ItemType].Add(item);
		}

		void DrawItemListVertical (List<LocalizationObjectItem> list, float Spacing = 8, bool DrawItemTypeName = true, int NameSize = 14, bool asFoldout = false) {	
			string Cato = list[0].ItemType;
			if (foldOutGroup == null) foldOutGroup = new Dictionary<string, bool>();
			if (!foldOutGroup.ContainsKey(Cato)) foldOutGroup.Add(Cato, true);
			
			GUILayout.BeginVertical();
				if (DrawItemTypeName){
					GUILayout.BeginHorizontal(GUILayout.Width(400));
						if (DrawItemTypeName && asFoldout) foldOutGroup[Cato] = EditorGUILayout.Foldout(foldOutGroup[Cato], string.Format("<b><size={0}>{1} ({2})</size></b>", NameSize, Cato, list.Count), true, foldOutStyle);
						else if (DrawItemTypeName && !asFoldout) GUILayout.Label(string.Format("<b><size={0}>{1} ({2})</size></b>", NameSize, Cato, list.Count), labelStyle_rich);
					GUILayout.EndHorizontal();
				}		
				if (foldOutGroup[Cato]) {
					GUILayout.Space(8);	
					GUILayout.BeginHorizontal();
						GUILayout.Space(30);
						GUILayout.BeginVertical();
							foreach (LocalizationObjectItem item in list.ToList()) {
								LocalizationObjectItem refItem = (referencingDatabase != null)? referencingDatabase.TryGetItem(item.ID) : null;
								ItemDrawer.Draw(item, refItem, null, null, null, OnItemRemoved);
								GUILayout.Space(4);
							}					
						GUILayout.EndVertical();
					GUILayout.EndHorizontal();					
				}		
			GUILayout.EndVertical();	

		}
		void DrawItemListVertical (List<LocalizationObjectItem> list, bool asFoldout = false) {	
			DrawItemListVertical(list, 4, true, 14, asFoldout);
		}

		void DrawItemGrid (List<LocalizationObjectItem> list, float areaWidth, int columnCount = 4, float spacingRatio = 0.1f, bool DrawItemTypeName = true, int NameSize = 14, bool asFoldout = false, float indent = 12) {
			string Cato = list[0].ItemType;
			if (foldOutGroup == null) foldOutGroup = new Dictionary<string, bool>();
			if (!foldOutGroup.ContainsKey(Cato)) foldOutGroup.Add(Cato, true);
			
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
				if (DrawItemTypeName){
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
						if (DrawItemTypeName && asFoldout) foldOutGroup[Cato] = EditorGUILayout.Foldout(foldOutGroup[Cato], string.Format("<b><size={0}>{1} ({2})</size></b>", NameSize, Cato, list.Count), true, foldOutStyle);
						else if (DrawItemTypeName && !asFoldout) GUILayout.Label(string.Format("<b><size={0}>{1} ({2})</size></b>", NameSize, Cato, list.Count), labelStyle_rich);
					GUILayout.EndHorizontal();
				}		
				if (foldOutGroup[Cato]) {
					GUILayout.Space(8);	
					int total = list.Count;
					int row = Mathf.CeilToInt(((float)total/(float)columnCount));	
					int index = 0, indexInRow = 0;
					float tileMaxWidth = areaWidth*(1-spacingRatio)/columnCount, spacing = areaWidth*spacingRatio/(columnCount-1);
					for (int r = 0; r < row && index < total; r++){
						GUILayout.BeginHorizontal(GUILayout.Width(areaWidth), GUILayout.ExpandWidth(false));
							GUILayout.Space(indent);
							while (indexInRow < columnCount && index < total) {
								LocalizationObjectItem refItem = (referencingDatabase != null)? referencingDatabase.TryGetItem(list[index].ID) : null;
								ItemDrawer.Draw(list[index], refItem, null, null, null, OnItemRemoved, GUILayout.MaxWidth(tileMaxWidth));
								if (indexInRow < columnCount - 1) GUILayout.Space(spacing);
								indexInRow += 1;
								index += 1;
							}
						GUILayout.EndHorizontal();
						indexInRow = 0;	
						if (r+1 < row) GUILayout.Space(spacing);
					}
				}	
				GUILayout.EndVertical();	
			GUILayout.EndHorizontal();
			
		}

		///Clone all items with ID that does not exist in this database.
		public void CloneMissingItemFrom (object source) {
			LocalizationItemGroup sGroup = source as LocalizationItemGroup;
			for (int i = 0; i < sGroup.storedItems.Count; i++){
				if (!groupCache.database.ItemExist(sGroup.storedItems[i].ID)) sGroup.storedItems[i].CloneTo(groupCache);
			}
			FilterAndCategorize(groupCache.storedItems, true);
			OnDatabaseChanged();
			return;			
		}

		///Show a generic menu listing all items contained by referencing database that with ID that does not exist in this database.
		void ShowMissedItemListFromRef () {
			GenericMenu menu = new GenericMenu();
			foreach (LocalizationItemGroup group in referencingDatabase.entryGroupList) {
				bool gotSomething = false;
				for (int i = 0; i < group.storedItems.Count; i++) {
					if (!databaseCache.ItemExist(group.storedItems[i].ID)) {
						if (!gotSomething) {
							menu.AddItem(new GUIContent(group.groupID + " (Try Copy All)"), false, CloneMissingItemFrom, group);
							gotSomething = true;
						}
						menu.AddItem(new GUIContent("- " + group.storedItems[i].ItemType + "/" + group.storedItems[i].ID), false, CloneWrapper, new CloneData(groupCache, group.storedItems[i]));
					}
				}
				if (group != referencingDatabase.entryGroupList.Last() && gotSomething) menu.AddSeparator(string.Empty);
			}
			menu.ShowAsContext();
		}

		class CloneData {
			public LocalizationItemGroup targetGroup;
			public LocalizationObjectItem cloning;

			public CloneData (LocalizationItemGroup TargetGroup, LocalizationObjectItem Cloning) {
				targetGroup = TargetGroup;
				cloning = Cloning;
			}
		}

		void CloneWrapper (object data) {
			LocalizationObjectItem temp = (data as CloneData).cloning.CloneTo((data as CloneData).targetGroup);
			FilterAndCategorizeNewItem(temp);
			OnNewItemCreated();
		}

		void ShowAddItemMenu () {
			if (!groupCache.AllowAddItemByUser) {
				ShowNotification(new GUIContent("This group does not allow users adding new items!"));
				return;
			}
			GenericMenu menu = new GenericMenu();
			foreach (var supportType in DynamicSupportTypeList) {
				menu.AddItem(new GUIContent("+" + supportType.Name), false, CreateNewItem, supportType);
			}
			menu.ShowAsContext();
		}

		public virtual void DrawAddItemBar () {
				if (DynamicSupportTypeList.Count <= 4) {
					GUILayout.BeginHorizontal();
					foreach (var supportType in DynamicSupportTypeList) {
						if (GUILayout.Button("+" + supportType.Name, GUILayout.Width(UtilButtonWidth))) CreateNewItem(supportType);
					}
					GUILayout.EndHorizontal();
				}
				else {
					GUILayout.BeginVertical();
						//A row.
						for (int i = 0; i < Mathf.Ceil((float) DynamicSupportTypeList.Count/4f); i++) {
							GUILayout.BeginHorizontal();
								for (int j = 0; j < 4; j++){
									if (DynamicSupportTypeList.Count == 4*i+j) break;					
									if (GUILayout.Button("+" + DynamicSupportTypeList[4*i+j].Name, GUILayout.Width(UtilButtonWidth))) CreateNewItem(DynamicSupportTypeList[4*i+j]);
								}
							GUILayout.EndHorizontal();
						}
					GUILayout.EndVertical();
				}
		}

	#endregion
	}
}