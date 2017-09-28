using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocalizaV2;


namespace LocalizaV2.Runtime {
	public class LocalizationCache : MonoBehaviour {

		public static LocalizationCache Instance;

		public LocalizationDatabase defaultLocale;


		public System.Action ForceRefreshAll;

		void Awake(){
			if (Instance == null) Instance = this;
			if (Instance != this) Destroy(this);
			DontDestroyOnLoad(this);			

			if (defaultLocale != null) _Loaded = defaultLocale;
		}

		static LocalizationDatabase _Loaded;

		public static LocalizationDatabase Loaded {
			get {
				return _Loaded;
			}
		}

		static string LoadedLocTagCache;

		static void LoadTarget (LocalizationDatabase target) {
			_Loaded = target;
			LoadedLocTagCache = target.ID;
		}

		static void LoadTarget (string Tag) {

		}

		static void ForceRefreshAllCached () {
			if (Instance.ForceRefreshAll != null) Instance.ForceRefreshAll.Invoke();
		}
	}
}