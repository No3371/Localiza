using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LocalizaV2{

	[System.Serializable]
	public class LocalizationItemGroup : ScriptableObject{		
		
		public LocalizationDatabase database;

		public List<LocalizationObjectItem> storedItems;

		public string groupID;

		[HideInInspector]
		public bool doRecycle = true, AllowAddItemByUser = true;

		public int ItemCount {
			get {
				return storedItems.Count;
			}
		}

#if UNITY_EDITOR
		public static LocalizationItemGroup Create(LocalizationDatabase database = null, string tag = "Unnamed Group"){
			LocalizationItemGroup created = ScriptableObject.CreateInstance<LocalizationItemGroup>();
			created.groupID = tag;
			created.storedItems = new List<LocalizationObjectItem>();
			created.database = database;
			if (database != null) {
				if (database.entryGroupList.Count > 0) {
					while (database.entryGroupList.Any(e => e.groupID == created.groupID)) {
						created.groupID += "*";
					}
				}
				database.entryGroupList.Add(created);
				AssetDatabase.AddObjectToAsset(created, database);
				created.hideFlags = HideFlags.HideInHierarchy;
			}

			
			return created;
		}

		public void OverwriteWith (LocalizationItemGroup source, bool FullForceOverwrite = false) {
			this.groupID = source.groupID;
			if (FullForceOverwrite) {
				for (int i = storedItems.Count - 1; i >= 0; i--) storedItems[i].Destroy();
				for (int i = 0; i < source.storedItems.Count; i++) source.storedItems[i].CloneTo(this);
			}
			else {
				//For each type of items
				//1. Check if any item with same ID exist in this group
				//2. If no result from step 1, check if any item with same ID exist in the database
				//3. If any result from step 1/2, overwrite the data from source
				//4. If no result from step 1/2, clone the item from source
				foreach (LocalizationObjectItem item in source.storedItems) {
					LocalizationObjectItem temp = this.storedItems.Find(e => e.ID.Equals(item.ID));
					if (temp == null) if (this.database.allItemCache.Contains(item.ID)) temp = this.database.allItemCache[item.ID];
					if (temp != null) {
						temp._ID = item.ID;
						temp.StoredString = item.StoredString;
						temp.Storage = item.Storage;
					}
					else {
						item.CloneTo(this);
					}
				}
			}
		}

		public void Destroy () {
			for (int i = storedItems.Count - 1; i >= 0; i--){
				storedItems[i].Destroy();
			}
			if (database != null) database.entryGroupList.Remove(this);
			Debug.Log("Group Removed.");
			DestroyImmediate(this, true);
		}

		public LocalizationItemGroup CloneTo (LocalizationDatabase database) {
			LocalizationItemGroup newGroup = LocalizationItemGroup.Create(database, this.groupID);
			for (int i = 0; i < storedItems.Count; i++){
				storedItems[i].CloneTo(newGroup);
			}
			return newGroup;
			
		}

#endif

	}
}