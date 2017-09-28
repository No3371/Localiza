using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace LocalizaV2{
	public partial class LocalizaEditor : EditorWindow, IHasCustomMenu{

#region Editor Initialization

		private static LocalizaEditor _editor;
		public static LocalizaEditor editor { get { AssureEditor(); return _editor; }}

		internal static EditorConfig editorConfig;

		public string SelfFolderPathCache {
			get {
				string temp = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject (this))) + "/";
				return temp;
			}
		}

		GUIStyle labelStyle_rich, buttonStyle, foldOutStyle, labelStyle_DatabaseTag, fieldStyle_DatabaseTag, LabelStyle_Sidebar;

		GUILayoutOption[] LocalizationTagAreaLayout, LocalizationTagLabelLayout;

		public static void AssureEditor()
		{
			if (_editor == null) OpenEditor(); 
		}

		[MenuItem("Window/Localiza")]
		public static LocalizaEditor OpenEditor(){
			_editor = GetWindow<LocalizaEditor>();
			_editor.minSize = new Vector2(1280, 600);
			_editor.maxSize = new Vector2(1440, 720);

			_editor.titleContent = new GUIContent ("Localiza");
			_editor.wantsMouseMove = true;	
			_editor.LoadConfig();
			return _editor;
		}

		void Init(){
			if (editorConfig == null) editorConfig = new EditorConfig();

			if (windowWidthCache < 0 || windowWidthCache != position.width || windowHeightCache < 0 || windowHeightCache != position.height) OnWindowSizeChanged();

			if (DrawHeader == null) {
				DrawHeader = DrawHeader_None;
			}
			if (DrawSideBar == null) {
				DrawSideBar = DrawSideBar_None;
			}
			if (DrawEditor == null) DrawEditor = DrawEditor_NoSelection;

			if (labelStyle_rich == null ) {
				labelStyle_rich = new GUIStyle(GUI.skin.GetStyle("Label"));
				labelStyle_rich.richText = true;
			}
			if (LabelStyle_Sidebar == null ) {
				LabelStyle_Sidebar = new GUIStyle(GUI.skin.GetStyle("Label"));
				LabelStyle_Sidebar.richText = true;
				LabelStyle_Sidebar.fontSize = 12;
			}
			if (foldOutStyle == null ) {
				foldOutStyle = new GUIStyle(EditorStyles.foldout);
				foldOutStyle.richText = true;
				// foldOutStyle.fontSize = 14;
			}
			if (labelStyle_DatabaseTag == null ) {
				labelStyle_DatabaseTag = new GUIStyle(GUI.skin.GetStyle("Label"));
				labelStyle_DatabaseTag.richText = true;
				labelStyle_DatabaseTag.fontSize = 22;
			}
			if (LocalizationTagAreaLayout == null) {
				LocalizationTagAreaLayout = new GUILayoutOption[2];
				LocalizationTagAreaLayout[0] = GUILayout.Height(24);
				LocalizationTagAreaLayout[1] = GUILayout.ExpandWidth(false);
			}
			if (LocalizationTagLabelLayout == null) {
				LocalizationTagLabelLayout = new GUILayoutOption[3];
				LocalizationTagLabelLayout[0] = GUILayout.Height(24);
				LocalizationTagLabelLayout[1] = GUILayout.MinWidth(120);
				LocalizationTagLabelLayout[2] = GUILayout.MaxWidth(HeaderMainWidth/2);
			}
			if (fieldStyle_DatabaseTag == null) {
				fieldStyle_DatabaseTag = new GUIStyle(GUI.skin.textField);
				fieldStyle_DatabaseTag.fontSize = 22;
			}

		} 
		
		public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("ReloadConfig"), false, new GenericMenu.MenuFunction(LoadConfig));
        }

		void LoadConfig () {
			editorConfig = new EditorConfig();
			editor.ResetWindow();
		}



#endregion

#region Declarations

		float windowWidthCache = -1, windowHeightCache = -1;

		float HeaderLeftMargin = 8, HeaderRightMargin = 8;
		
		float EditorWidth, EditorMargin = 16;

		float HeaderHeight, HeaderMainWidth, HeaderUtilWidth;

		float UtilButtonWidth, UtilElementSpacing;

		float SidebarWidth = 240;

#endregion

#region GUI
		
		bool ForceSaveFlag;

		void OnGUI(){
			DrawerTransition();
			DataTransition();
			Init();

			GUILayout.BeginVertical();
				DrawHeader();
				Rect temp = GUILayoutUtility.GetLastRect();
				GUILayout.BeginHorizontal();
					DrawSideBar();
					DrawEditor();
				GUILayout.EndHorizontal();	
			GUILayout.EndVertical();

			CustomEditorUtility.DrawShadow(CustomEditorUtility.Directions.Bottom, temp);

			DoChangeCheck();
			if (ForceSaveFlag) {
				OnDatabaseChanged();
				ForceSaveFlag = false;
			}
			if (EditorApplication.timeSinceStartup - lastSavedTime > AutoSaveInterval) SaveDatabase();
		}

		void DoChangeCheck() {
			// if (selectedGroupIndex != selectedGroupIndexCache) OnSidebarSelectionChanged();
		}
		
		void OnWindowSizeChanged () {
			windowWidthCache = position.width;
			windowHeightCache = position.height;

			HeaderMainWidth = windowWidthCache * 0.64f - HeaderLeftMargin;
			HeaderUtilWidth = windowWidthCache * 0.36f;
			HeaderHeight = 72;

			UtilButtonWidth = HeaderUtilWidth * 0.2f;
			UtilElementSpacing = HeaderUtilWidth * 0.024f;

			SidebarWidth = windowWidthCache * 0.2f;

			EditorWidth = windowWidthCache - SidebarWidth;

		}
		
		void DrawerTransition () {	
			if (Event.current.type != EventType.Layout) return;

			// if (databaseCache == null) {
			// 	DrawHeader = DrawHeader_None;
			// 	DrawSideBar = DrawSideBar_None;
			// 	DrawEditor = DrawEditor_NoSelection;
			// }

			if (DrawHeaderBuffer != null) {
				DrawHeader = DrawHeaderBuffer;
				DrawHeaderBuffer = null;
			}
			if (DrawSideBarBuffer != null) {
				DrawSideBar = DrawSideBarBuffer;
				DrawSideBarBuffer = null;
			}
			if (DrawEditorBuffer != null) {
				DrawEditor = DrawEditorBuffer;
				DrawEditorBuffer = null;
			}

		}

		void DataTransition () {
			if (Event.current.type != EventType.Layout) return;

			if (databaseCacheBuffer != null) {
				LoadDatabase(databaseCacheBuffer);
			}

			if (groupCacheBuffer != null) {
				LoadItemGroup(groupCacheBuffer);
			}

		}



#endregion

#region New/Remove/Delete Item

		void CreateNewItem (object ItemType) {
			LocalizationObjectItem newItem = LocalizationObjectItem.Create(groupCache, null, (ItemType as Type).ToString());
			FilterAndCategorizeNewItem(newItem);
			OnNewItemCreated();
		}

		void CreateNewItemGroup () {
			LocalizationItemGroup newGroup = LocalizationItemGroup.Create(databaseCache);
			OnDatabaseChanged();
		}

		void DeleteItemGroup (LocalizationItemGroup group) {
			if (group == groupCache) {
				groupCache = null;
				DrawEditorBuffer = DrawEditor_NoSelection;
				Repaint();
			}
			group.Destroy();
			OnItemGroupRemoved();
		}

		void CopyDatabase () {
			LocalizationDatabase newDatabase = databaseCache.CopyAs(databaseCache.name + "(Copy)");
		}

#endregion

#region Item Edited Handler

		string newItemIDCache;

		///Cache the item whose ID is edited by user for ChangeItemID();
		LocalizationObjectItem ID_EditedItemCache;

#endregion

#region Event Handlers

		void OnNewItemCreated (LocalizationObjectItem newItem = null) {
			OnDatabaseChanged();
			if (DrawEditor != DrawEditor_Normal) DrawEditorBuffer = DrawEditor_Normal;
			ItemCountCache = databaseCache.ItemCount;
			Repaint();
		}

		public void OnLocalizationTagEdited (string newTag) {
			databaseCache.name = newTag;
			databaseCache.ID = newTag;
			AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(databaseCache), newTag);
			return;			
		}

		
		///This is called after a full OnGUI, since it moved the edited item into/out in the process.
		void SetItemID () {
			//Try to rename.
			ID_EditedItemCache.SetID(newItemIDCache);
			newItemIDCache = null;
			ID_EditedItemCache = null;
			SaveDatabase();
		}

		void OnDatabaseCreated () {
		}



		void OnItemGroupRemoved () {
			SaveDatabase();
		}

		void OnItemRemoved (LocalizationObjectItem RemovedItem) {
			SupportedObjectTypeListDict[RemovedItem.ItemType].Remove(RemovedItem);	
			OnDatabaseChanged();
			ItemCountCache -= 1;
			Repaint();
		}

		///This method will start a save timer, if users stop editing for about seconds, a SaveDataBase() will be executed.
		void OnDatabaseChanged(){
			lastDatabaseChangedTime = EditorApplication.timeSinceStartup;
			EditorApplication.update -= SaveTimer;
			EditorApplication.update += SaveTimer;
		}

		void OnDestroy(){
			EditorApplication.update -= SaveTimer;
			SaveDatabase();
			ResetWindow();
		}
		


#endregion

#region Asset Handling

		void ResetWindow () {
			databaseCache = null;
			groupCache = null;
			referencingDatabase = null;
			// DrawEditor = DrawEditorBuffer = null;
			// DrawSideBar = DrawSideBarBuffer = null;
			// DrawHeader = DrawHeaderBuffer = null;
			DrawEditor = DrawEditor_NoSelection;
			DrawSideBar = DrawSideBar_None;
			DrawHeader = DrawHeader_None;
			editableSelectedGroupID = null;
			CacheSupportingTypes();
			ItemDrawer.ResetCache();

			ItemCountCache = -1;
			RefItemCountCache = -1;
		}

		static LocalizationDatabase databaseCache, databaseCacheBuffer, referencingDatabase;
		static LocalizationItemGroup groupCache, groupCacheBuffer;

		static void SaveDatabase(){
			lastSavedTime = EditorApplication.timeSinceStartup;
			if (databaseCache == null) return;
			EditorUtility.SetDirty(databaseCache);
			if (groupCache != null) EditorUtility.SetDirty(groupCache);
			AssetDatabase.SaveAssets();
		}

		LocalizationDatabase CreateNewDatabase () {
			if (!Directory.Exists(editorConfig.SavePath)) Directory.CreateDirectory(editorConfig.SavePath);
			if (File.Exists(editorConfig.SavePath + editorConfig.defaultDatabaseName)) {
				EditorUtility.DisplayDialog("File existed", "A database called Unnamed exist, try again after you rename/delete it", "OK");
				return null;
			}
			LocalizationDatabase newDatabase = LocalizationDatabase.Create();
			AssetDatabase.CreateAsset(newDatabase, editorConfig.SavePath + editorConfig.defaultDatabaseName);
			SaveDatabase();
			OnDatabaseCreated();
			return newDatabase;
		}

		// void OverrideDatabaseFromJson (object json){
		// 	string Json = json as string;		
		// 	LocalizationDatabase created = (LocalizationDatabase) LocalizationDatabase.Deserialize(typeof(LocalizationDatabase), Json, databaseCache);
		// 	if (created.name != created.ID) {
		// 		OnLocalizationTagEdited(created.ID);
		// 	}
		// 	SaveDatabase();
		// 	return;	
		// }		
		void SyncDatabaseConfig () {
			databaseCache.RecycleBinMax = editorConfig.RecycleBinMax;
		}

		void LoadDatabase(LocalizationDatabase database){
			if (database == null){
				Debug.LogError("LoadDatabase():: The passed in database object is null!");
				return;
			}
			if (database == databaseCache) return;
			SaveDatabase();	
			UnloadItemGroup();
			databaseCache = database;
			SyncDatabaseConfig();
			DrawHeaderBuffer = DrawHeader_Normal;
			ItemCountCache = databaseCache.ItemCount;
			
			CacheSupportingTypes();

			GUI.FocusControl("0");
			databaseCacheBuffer = null;
			Repaint();
		}

		///Load entry content into Editor block, does not thing to do with (int) selected.
		void LoadItemGroup(LocalizationItemGroup group){
			groupCache = group;
			DrawSideBarBuffer = DrawSideBar_Normal;
			if (group.ItemCount == 0) DrawEditorBuffer = DrawEditor_Empty;
			else DrawEditorBuffer = DrawEditor_Normal;

			FilterAndCategorize(group.storedItems, true);

			GUI.FocusControl("0");
			
			groupCacheBuffer = null;
			Repaint();
		}

		void UnloadItemGroup () {
			groupCache = null;
			DrawSideBarBuffer = DrawSideBar_Normal;
			DrawEditorBuffer = DrawEditor_NoSelection;
			Repaint();
		}
		

		public static void AddSubAsset(ScriptableObject subAsset, ScriptableObject mainAsset)
		{
			if (subAsset != null && mainAsset != null)
			{
				AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
				subAsset.hideFlags = HideFlags.HideInHierarchy;
			}
		}

		static double lastDatabaseChangedTime, lastSavedTime = 0;

		static float AutoSaveInterval = 300;

		static void SaveTimer(){
			double temp = EditorApplication.timeSinceStartup - lastDatabaseChangedTime;
			if (temp > 2){
				SaveDatabase();
				EditorApplication.update -= SaveTimer;
			}
		}
#endregion

 	}
}