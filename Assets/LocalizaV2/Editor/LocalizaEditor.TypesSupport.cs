using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace LocalizaV2 {
	public partial class LocalizaEditor {

		Dictionary<string, List<LocalizationObjectItem>> SupportedObjectTypeListDict = new Dictionary<string, List<LocalizationObjectItem>>();

		Dictionary<string, string> SupportedTypeDrawerMode = new Dictionary<string, string>();

		List<Type> DynamicSupportTypeList = new List<Type>();

		void CacheSupportingTypes () {
			SupportedObjectTypeListDict.Clear();
			SupportedTypeDrawerMode.Clear();
			DynamicSupportTypeList.Clear();
			ItemDrawer.ClearDrawerCache();
			
			
			foreach (ItemTypeSupport t in editorConfig.Supported) {
				DynamicSupportTypeList.Add(t.target);
				SupportedObjectTypeListDict.Add(t.target.ToString(), new List<LocalizationObjectItem>());
				SupportedTypeDrawerMode.Add(t.target.ToString(), t.Mode);
			}
		}

	}
}