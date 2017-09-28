using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using FullSerializer;


namespace LocalizaV2{	
	
	public class EditorConfig {
		public enum UIScale {
			///1x
			Normal,
			///1.25x
			Bigger,
			///1.5x
			Big,			
			///2x
			Huge
			
		}

		public UIScale Scale;

		public int TextSize {
			get {
				switch (Scale){
					case UIScale.Normal:
						return DefaultFontSize;
					case UIScale.Bigger:
						return  (int) (DefaultFontSize*1.25f);
					case UIScale.Big:
						return  (int) (DefaultFontSize*1.5f);
					case UIScale.Huge:
						return  (int) (DefaultFontSize*2f);
					default:
						return DefaultFontSize;
				}
			}
		}

		public int TitleSize {
			get {
				switch (Scale){
					case UIScale.Normal:
						return DefaultTitleSize;
					case UIScale.Bigger:
						return  DefaultTitleSize;
					case UIScale.Big:
						return  (int) (DefaultTitleSize*1.2f);
					case UIScale.Huge:
						return  (int) (DefaultTitleSize*1.5f);
					default:
						return DefaultTitleSize;
				}
			}
		}
		
		const int DefaultFontSize = 14, DefaultTitleSize = 22;

		public int RecycleBinMax;

		public List<ItemTypeSupport> Supported;

		public string SavePath;

		public EditorConfig () {
			RecycleBinMax = 30;
			Scale = UIScale.Normal;
			Supported = new List<ItemTypeSupport>();
			SavePath = "Assets/Localiza/Saves/";
			this.TryLoadConfigFromFile();
		}


		public void ImportFromJson (string data) {
			EditorJsonUtility.FromJsonOverwrite(data, this);
		}

		public string ExportAsJsonString () {
			return EditorJsonUtility.ToJson(this);
		}

		const string configFileName = "Config.json";

		public string defaultDatabaseName = "Unnamed.asset";

		internal void TryLoadConfigFromFile () {
			string configFilePath = LocalizaEditor.editor.SelfFolderPathCache;
			// Debug.Log("TryLoadConfig at " + configFilePath + configFileName);
			if (File.Exists(configFilePath + configFileName) && !System.String.IsNullOrEmpty(File.ReadAllText(configFilePath + configFileName))) 
			{
				string serialized = File.ReadAllText(configFilePath + configFileName);
				//Debug.Log(serialized);
				Deserialize(typeof(EditorConfig), serialized, this);
			}
			else {
				Debug.Log("<Localiza>:: Config file not found. Creating new...");
				this.SaveConfigToFile();
			}
		}

		internal void SaveConfigToFile () {
			string configFilePath = LocalizaEditor.editor.SelfFolderPathCache;
			string saving = Serialize(typeof(EditorConfig), this);
			// Debug.Log("TrySaveConfig at " + configFilePath + configFileName);		
			using (FileStream fs = new FileStream(configFilePath + configFileName, FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(fs))
				{
					writer.Write(saving);
					writer.Close();
					writer.Dispose();
				}
				fs.Close();
				fs.Dispose();
			}
		}
		static readonly EditorConfig_Converter _converter = new EditorConfig_Converter();

		public static string Serialize(System.Type type, object value) {
			// serialize the data
			fsData data;
			_converter.TrySerialize(value, out data, type).AssertSuccessWithoutWarnings();

			// emit the data via JSON
			return fsJsonPrinter.PrettyJson(data);
		}

		public static object Deserialize(System.Type type, string serializedState, object overriding = null) {
			// step 1: parse the JSON data
			fsData data = fsJsonParser.Parse(serializedState);

			// step 2: deserialize the data
			_converter.TryDeserialize(data, ref overriding, type).AssertSuccessWithoutWarnings();

			return overriding;
		}
	}
}