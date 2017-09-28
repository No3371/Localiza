using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FullSerializer;
using System.Linq;

namespace LocalizaV2 {
	public class EditorConfig_Converter : fsDirectConverter<EditorConfig> {
		public override object CreateInstance(fsData data, Type storageType) {
			return new EditorConfig();
		}

		protected override fsResult DoSerialize(EditorConfig model, Dictionary<string, fsData> serialized) {
			// Serialize name manually
			serialized["RecycleBinMax"] = new fsData(model.RecycleBinMax);
			serialized["UI_Scale"] = new fsData(model.Scale.ToString());
			serialized["SupportedType"] = fsData.CreateList();
			serialized["SavePath"] = new fsData(model.SavePath);
			serialized["DefaultDatabaseName"] = new fsData(model.defaultDatabaseName);

			foreach (ItemTypeSupport its in model.Supported){
				fsData temp = fsData.CreateDictionary();
				temp.AsDictionary.Add("Type", new fsData(its.target.ToString()));
				temp.AsDictionary.Add("DrawerLayout", new fsData(its.Mode));
				temp.AsDictionary.Add("DrawerType", new fsData(its.drawerType.ToString()));
				serialized["SupportedType"].AsList.Add(temp);
			}

			return fsResult.Success;
		}

		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref EditorConfig model) {
			var result = fsResult.Success;

			if (data.ContainsKey("RecycleBinMax")) model.RecycleBinMax = (int) data["RecycleBinMax"].AsInt64;
			if (data.ContainsKey("UI_Scale")) model.Scale = (EditorConfig.UIScale) Enum.Parse(typeof(LocalizaV2.EditorConfig.UIScale), data["UI_Scale"].AsString);
			if (data.ContainsKey("SupportedType")) foreach (fsData f in data["SupportedType"].AsList) {
				model.Supported.Add(new ItemTypeSupport(f.AsDictionary["Type"].AsString, f.AsDictionary["DrawerLayout"].AsString, f.AsDictionary["DrawerType"].AsString));
			}
			if (data.ContainsKey("SavePath")) model.SavePath = data["SavePath"].AsString;
			if (data.ContainsKey("DefaultDatabaseName")) model.defaultDatabaseName = data["DefaultDatabaseName"].AsString;

			return result;
		}
	}

	// public class LocalizationDatabaseConverter : fsDirectConverter<LocalizationDatabase> {

	// 	static readonly LocalizationItemGroupConverter groupConverter = new LocalizationItemGroupConverter();

	// 	public override object CreateInstance(fsData data, Type storageType) {
	// 		return LocalizationDatabase.Create();
	// 	}

	// 	protected override fsResult DoSerialize(LocalizationDatabase model, Dictionary<string, fsData> serialized) {
	// 		// Serialize name manually
	// 		serialized["ID"] = new fsData(model.ID);
	// 		serialized["Groups"] = fsData.CreateList();

	// 		List<fsData> entriesTemp = new List<fsData>();
	// 		// Serialize age using helper methods
	// 		foreach (LocalizationItemGroup group in model.entryGroupList) {
	// 			fsData groupData = new fsData();
	// 			groupConverter.TrySerialize(group, out groupData, typeof(LocalizationItemGroup));
	// 			serialized["Groups"].AsList.Add(groupData);
	// 		}
	// 		return fsResult.Success;
	// 	}

	// 	protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref LocalizationDatabase model) {
	// 		var result = fsResult.Success;

	// 		// Deserialize name mainly manually (helper methods CheckKey and CheckType)
	// 		fsData nameData;
	// 		if ((result += CheckKey(data, "ID", out nameData)).Failed) return result;
	// 		model.ID = nameData.AsString;
		
	// 		if (data.ContainsKey("Groups")) {
	// 			foreach(fsData groupData in data["Groups"].AsList) {
	// 				object extracting = LocalizationItemGroup.Create();
	// 				groupConverter.TryDeserialize(groupData, ref extracting, typeof(LocalizationItemGroup));
					
	// 				LocalizationItemGroup temp = model.entryGroupList.Find(e => e.groupID == (extracting as LocalizationItemGroup).groupID);
	// 				if (temp != null) temp.OverwriteWith(extracting as LocalizationItemGroup);
	// 				else temp.Join(model);
	// 			}
	// 		}
	// 		return result;
	// 	}
	// }


	// public class LocalizationItemGroupConverter : fsDirectConverter<LocalizationItemGroup> {

	// 	public override object CreateInstance(fsData data, Type storageType) {
	// 		return LocalizationItemGroup.Create();
	// 	}

	// 	protected override fsResult DoSerialize(LocalizationItemGroup model, Dictionary<string, fsData> serialized) {
	// 		// Serialize name manually
	// 		serialized["ID"] = new fsData(model.groupID);
	// 		serialized["Items"] = fsData.CreateList();

	// 		foreach (LocalizationObjectItem Item in model.storedItems) {
	// 			if 
	// 			serialized["Items"].AsList.Add(new fsData(strItem.ID + "::::" + strItem.Stored));
	// 		}

	// 		return fsResult.Success;
	// 	}

	// 	protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref LocalizationItemGroup model) {
	// 		var result = fsResult.Success;

	// 		// Deserialize name mainly manually (helper methods CheckKey and CheckType)
	// 		fsData nameData;
	// 		if ((result += CheckKey(data, "ID", out nameData)).Failed) return result;
	// 		model.groupID = nameData.AsString;

	// 		if (data.ContainsKey("StringItems")) {
	// 			if (data["StringItems"].AsList.Count > 0) foreach (fsData itemData in data["StringItems"].AsList) {
	// 				string[] temp = itemData.AsString.Split(new string[]{"::::"}, 2, StringSplitOptions.None);
	// 				LocalizationStringItem tempItem = LocalizationStringItem.Create(model, temp[0]);
	// 				tempItem.Stored = temp[1];
	// 			}
	// 		}
	// 		if (data.ContainsKey("AudioItems")) {
	// 			if (data["AudioItems"].AsList.Count > 0) foreach (fsData itemData in data["AudioItems"].AsList) {
	// 				string[] temp = itemData.AsString.Split(new string[]{"::::"}, 2, StringSplitOptions.None);
	// 				LocalizationAudioItem tempItem = LocalizationAudioItem.Create(model, temp[0]);
	// 				tempItem.Stored = (AudioClip) AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( temp[1]));
	// 			}
	// 		}
	// 		if (data.ContainsKey("SpriteItems")) {
	// 			if (data["SpriteItems"].AsList.Count > 0) foreach (fsData itemData in data["SpriteItems"].AsList) {
	// 				string[] temp = itemData.AsString.Split(new string[]{"::::"}, 2, StringSplitOptions.None);
	// 				LocalizationSpriteItem tempItem = LocalizationSpriteItem.Create(model, temp[0]);
	// 				tempItem.Stored = (Sprite) AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( temp[1]));
	// 			}
	// 		}

	// 		return result;
	// 	}
	// }
}