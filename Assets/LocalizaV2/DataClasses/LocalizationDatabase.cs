using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LocalizaV2{

	[System.Serializable]
	public class Dict_LocalizationItem : SerializableDictionary<string, LocalizationObjectItem>{
		
	}

	// [ContainedObjectTypesAttribute(typeof(System.String), typeof(AudioClip), typeof(Sprite), typeof(GameObject))]
	// [ItemDrawerTypeAttribute(typeof(Sprite), "Grid")]
	// [SupportItemTypeAttribute(typeof(System.String), "Horizontal", typeof(ItemDrawer_String))]
	// [SupportItemTypeAttribute(typeof(AudioClip), "Horizontal", typeof(ItemDrawer_AudioClip))]
	// [SupportItemTypeAttribute(typeof(Sprite), "Grid", typeof(ItemDrawer_Sprite))]
	public class LocalizationDatabase : ScriptableObject {

		public string ID;

		internal LocalizationDatabase lastRef;

		public List<LocalizationItemGroup> entryGroupList;

		[SerializeField] internal LocalizationItemGroup _RecycleBin;

		[HideInInspector] public int RecycleBinMax = 30;

#if UNITY_EDITOR
		public LocalizationItemGroup RecycleBin {

			get {

				if (_RecycleBin == null) CreateRecycleBin();				
				return _RecycleBin;

			}
			set {
				if (_RecycleBin == null) CreateRecycleBin();	
				_RecycleBin = value;

			}
		}
#endif

		[SerializeField] internal Dict_LocalizationItem allItemCache;

		public static LocalizationDatabase Create(string Name = "Unnamed"){
			LocalizationDatabase created  = ScriptableObject.CreateInstance<LocalizationDatabase>();
			created.entryGroupList = new List<LocalizationItemGroup>();
			created.allItemCache = new Dict_LocalizationItem();
			created.name = Name;
			created.ID = Name;
			return created;
		}

		public int ItemCount {			
			get {
				return allItemCache.Count; 
			}
		}

		///Return null if doesn't found it.
		public LocalizationObjectItem TryGetItem (string ID) {
			if (allItemCache.ContainsKey(ID)) return allItemCache[ID];
			else return null;
		} 

		public LocalizationObjectItem TryGetItemVerified (string ID, System.Type DemandType) {
			if (allItemCache.ContainsKey(ID)) {
				if (allItemCache[ID].ItemType == DemandType.ToString()) return allItemCache[ID];
			}
			return null;
		}

		public bool ItemExist (string ID) {
			return allItemCache.ContainsKey(ID);
		}

		public System.Type GetItemType (string ID) {
			if (!allItemCache.Contains(ID)) return null;
			return System.Type.GetType(allItemCache[ID].ItemType);
		}

		public string GetItemTypeName(string ID) {
			if (!allItemCache.Contains(ID)) return null;
			return allItemCache[ID].ItemType;
		}

		///Check the item exist or not before use this. It's possible the data object be returned as wrong type, You can check for the item's ItemType beforehand.
		public T GetData<T> (string ID) {
			return (T) allItemCache[ID].Get<T>();
		}

#if UNITY_EDITOR
		public LocalizationDatabase CopyAs (string databaseName) {
			string path = AssetDatabase.GetAssetPath(this);
			path = path.Substring(0, path.LastIndexOf("/")+1);
			path += databaseName + ".asset";
			if (System.IO.File.Exists(path)) {
				EditorUtility.DisplayDialog("File existed", "A database called " + databaseName + " exist, try again after you rename/delete it", "OK");
				return null;
			}
			
			LocalizationDatabase newDatabase = LocalizationDatabase.Create();
			AssetDatabase.CreateAsset(newDatabase, path);

			for (int i = 0; i < entryGroupList.Count; i++){
				entryGroupList[i].CloneTo(newDatabase);
			}

			EditorUtility.SetDirty(newDatabase);
			AssetDatabase.SaveAssets();

			return newDatabase;
		}
		
		internal void ReCacheAll () {
			allItemCache.Clear();
			foreach (LocalizationItemGroup group in entryGroupList) {
				foreach (LocalizationObjectItem Item in group.storedItems) allItemCache.Add(Item.ID, Item);
			}
		}

		internal void BackupDeletingItem (LocalizationObjectItem DeletingItem) {
			DeletingItem.CloneTo(RecycleBin);
			if (RecycleBin.storedItems.Count > RecycleBinMax) RecycleBin.storedItems[0].Destroy();
		}
		 
		void CreateRecycleBin () {
			_RecycleBin = LocalizationItemGroup.Create(null, "Recycle Bin");
			_RecycleBin.doRecycle = false;
			_RecycleBin.AllowAddItemByUser = false;
			AssetDatabase.AddObjectToAsset(_RecycleBin, this);
			_RecycleBin.hideFlags = HideFlags.HideInHierarchy;			
		}
		
#endif



// #region Json Import/export

// 		// public void CreateFromJsonArray(string[] jsonArr){
// 		// 	while (entryGroupList.Count < jsonArr.Length){
// 		// 		InfoLogEntry newEntry = InfoLogEntry.Create();
// 		// 		AddSubAsset(newEntry);
// 		// 		entryGroupList.Add(newEntry);
// 		// 	}
// 		// 	for (int i = 0; i < jsonArr.Length; i++){
// 		// 		JsonUtility.FromJsonOverwrite(jsonArr[i], entryGroupList[i]);
// 		// 	}

// 		// }
// 		// public void AddSubAsset(ScriptableObject subAsset)
// 		// {
// 		// 	if (subAsset != null)
// 		// 	{
// 		// 		AssetDatabase.AddObjectToAsset(subAsset, this);
// 		// 		subAsset.hideFlags = HideFlags.HideInHierarchy;
// 		// 	}
// 		// }

// 		// public string[] ParseJsonArray(string json){
// 		// 	return Regex.Matches(json, "{.*}").Cast<Match>().Select(m => m.Value).ToArray();
// 		// }

// 		public void ExportToJsonArray(){
			
// 			Debug.Log(Serialize(typeof(LocalizationDatabase), this));
// 			// foreach (LocalizationItemGroup group in entryGroupList) {
// 			// 	group.ToJson();
// 			// 	foreach (LocalizationStringItem strItem in group.textEntries) strItem.ToJson();
// 			// 	foreach (LocalizationAudioItem audItem in group.audioEntries) audItem.ToJson();
// 			// 	foreach (LocalizationSpriteItem sprItem in group.spriteEntries) sprItem.ToJson();
// 			// }
// 		}

// 		private static readonly LocalizationDatabaseConverter _converter = new LocalizationDatabaseConverter();

// 		public static string Serialize(System.Type type, object value) {
// 			// serialize the data
// 			fsData data;
// 			_converter.TrySerialize(value, out data, type).AssertSuccessWithoutWarnings();

// 			// emit the data via JSON
// 			return fsJsonPrinter.PrettyJson(data);
// 		}

// 		public static object Deserialize(System.Type type, string serializedState, object overriding = null) {
// 			// step 1: parse the JSON data
// 			fsData data = fsJsonParser.Parse(serializedState);

// 			// step 2: deserialize the data
// 			_converter.TryDeserialize(data, ref overriding, type).AssertSuccessWithoutWarnings();

// 			return overriding;
// 		}
// 		#endregion
	}

}