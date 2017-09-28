using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LocalizaV2{

	[System.Serializable]
	public class LocalizationObjectItem : ScriptableObject, System.IComparable<LocalizationObjectItem> {

		public LocalizationItemGroup containerGroup;

#region Content
		public string ItemType{
			get { return _ItemType; }
		}

		[SerializeField] string _ItemType = string.Empty;


		[SerializeField] internal string _ID;

		public string ID {
			get { return _ID; }
			set {
				_ID = value;
				if (OnIDSet != null) OnIDSet.Invoke(this);
			}
		}

		[SerializeField] string storedString;

		[SerializeField] UnityEngine.Object storage;

		public UnityEngine.Object Storage {
			get { return storage; }
			set {
				if (value is MonoBehaviour) {
					Debug.LogError("MonoBehaviour can not be serialized.");
					return;
				}
				else {
					if (value != null) {
						storage = value;
						_ItemType = value.GetType().ToString();
					}
				}
				if (OnContentChanged != null) OnContentChanged.Invoke(this);
			}
		}
		
		public string StoredString {
			get { return storedString; }
			set {
				_ItemType = typeof(System.String).ToString();
				storedString = value;
				if (OnContentChanged != null) OnContentChanged.Invoke(this);
			}
		}

#endregion

#region Getter & Setter
		///Get content, return the stored string if T is string, or return the stored object as UnityEngine.Object or any specified derived class; 
		public object Get<T> () {
			if (typeof(T).Equals( typeof(System.String))) return storedString;
			return Storage;
		}

		public void Set<T> (T obj) {
			if (typeof(T).Equals( typeof(System.String))) StoredString = obj as string;
			else Storage = obj as UnityEngine.Object;
		}

#endregion

		public System.Action<LocalizationObjectItem> OnIDSet, OnRemovingFromGroup, OnRemovedAndDestroying, OnContentChanged;
		
		public void SetID (string newID) {

			if (containerGroup) if (containerGroup.database) if (containerGroup.database.allItemCache.ContainsKey(newID)) {
				Debug.LogError("An item with same ID has existed! Aborting!");
				return;
			}

			if (this.containerGroup != null) if(this.containerGroup.database != null) containerGroup.database.allItemCache.Remove(ID);
			this._ID = newID;
			if (this.containerGroup != null) if(this.containerGroup.database != null) containerGroup.database.allItemCache.Add(this.ID, this);
			if (OnIDSet != null) OnIDSet.Invoke(this);
		}

#if UNITY_EDITOR
		///Item factory.
		public static LocalizationObjectItem Create(LocalizationItemGroup group, string ID = null, string ItemType = null){
			LocalizationObjectItem created = ScriptableObject.CreateInstance<LocalizationObjectItem>();

			//Use instance ID if ID is not specified.
			//If containing database has any item with duplicated ID, get a random one.
			created._ID = (ID == null)? created.GetInstanceID().ToString() : ID;
			if (group.database != null) while (group.database.allItemCache.ContainsKey(created.ID)) created._ID = Random.Range(-65535, 65534).ToString();

			//Set ItemType if specified.
			if (ItemType != null) created._ItemType = ItemType;

			//Getting referred in the containing group and database.
			created.containerGroup = group;
			group.storedItems.Add(created);
			if (group.database != null) group.database.allItemCache.Add(created.ID, created);

			//Asset operation
			AssetDatabase.AddObjectToAsset(created, group);
			created.hideFlags = HideFlags.HideInHierarchy;

			return created;
		}

		public void Destroy () {
			if (this.containerGroup != null) {
				this.containerGroup.storedItems.Remove(this);
				if (this.containerGroup.database != null) {
					if (this.containerGroup.doRecycle) this.containerGroup.database.BackupDeletingItem(this);
					this.containerGroup.database.allItemCache.Remove(this.ID);
				}
			}
			if (OnRemovedAndDestroying != null) OnRemovedAndDestroying.Invoke(this);
			DestroyImmediate(this, true);
		}

		///Clone this item to the target group.
		public LocalizationObjectItem CloneTo(LocalizationItemGroup group){
			LocalizationObjectItem created = LocalizationObjectItem.Create(group, this.ID, this._ItemType);

			//The properties will check and change the type of passed in object, so we directly change the private field. 
			created.storage = this.storage;
			created.storedString = this.storedString;
			return created;
		}

		///Clone this item to the target group. This version works for generic menu.
		public void CloneTo(object group){
			LocalizationItemGroup targetGroup = group as LocalizationItemGroup;
			LocalizationObjectItem created = LocalizationObjectItem.Create(targetGroup, this.ID, this._ItemType);
			
			//The properties will check and change the type of passed in object, so we directly change the private field. 
			created.storage = this.storage;
			created.storedString = this.storedString;
			return;
		}

#endif
#region Drawer
		// static Dictionary<string, ItemDrawerBase> drawerDict;


		// ///This method will call the actual drawer according to the ItemType.
		// public void Draw (LocalizationObjectItem reference = null,
		// 				  System.Action<LocalizationObjectItem> OnContentChanged = null,
		// 				  System.Action<LocalizationObjectItem> OnItemIDEdited = null, 
		// 				  System.Action<LocalizationObjectItem> OnItemRemovingFromGroup = null, 
		// 				  System.Action<LocalizationObjectItem> OnItemRemovedAndDestroying = null, 
		// 				  params GUILayoutOption[] areaOptions) {

		// 	this.OnContentChanged = OnContentChanged;
		// 	this.OnIDSet = OnItemIDEdited;
		// 	this.OnRemovedAndDestroying = OnItemRemovedAndDestroying;
		// 	this.OnRemovingFromGroup = OnItemRemovingFromGroup;

		// 	if (drawerDict == null){
		// 		 drawerDict = new Dictionary<string, ItemDrawerBase>();
		// 		 SupportItemTypeAttribute[] temp = (SupportItemTypeAttribute[]) System.Attribute.GetCustomAttributes(typeof(LocalizationDatabase), typeof(SupportItemTypeAttribute));
		// 		 foreach (SupportItemTypeAttribute attr in temp) {
		// 			 if (attr.drawerType != null) drawerDict.Add(attr.target.ToString(), System.Activator.CreateInstance(attr.drawerType) as ItemDrawerBase);
		// 		 }
		// 	}
		// 	if (!drawerDict.ContainsKey(ItemType)) return;
		// 	else if (drawerDict[ItemType] == null) return;
		// 	else drawerDict[ItemType].Draw(this, reference, areaOptions);
		// }

		// public void Draw (LocalizationObjectItem reference = null, params GUILayoutOption[] areaOptions) {

		// 	this.OnContentChanged = null;
		// 	this.OnIDSet = null;
		// 	this.OnRemovedAndDestroying = null;
		// 	this.OnRemovingFromGroup = null;

		// 	Draw(reference, OnContentChanged, OnIDSet, OnRemovingFromGroup, OnRemovedAndDestroying, areaOptions);
		// }

		// public static void ClearDrawerCache () {
		// 	if (drawerDict != null) foreach (var drawer in drawerDict) if (drawer.Value != null) drawer.Value.ClearDrawerCache();
		// }

#endregion

#region IComparable

		///Used by IList.Sort().
		public int CompareTo (LocalizationObjectItem other) {
			return string.Compare(this.ID, other.ID, false);
		}

#endregion

	}

}