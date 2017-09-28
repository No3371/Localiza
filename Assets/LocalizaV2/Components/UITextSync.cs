using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalizaV2;

namespace LocalizaV2.Runtime {

	[RequireComponent(typeof(UnityEngine.UI.Text))]
	public class UITextSync : MonoBehaviour {

		public Text Target;

		public string locaID;

		void OnEnable () {
			Target = this.GetComponent<Text>();
			LocalizationCache.Instance.ForceRefreshAll += Sync;
			Sync();
		}

		void OnDisable () {
			LocalizationCache.Instance.ForceRefreshAll -= Sync;
		}

		void OnDestroy () {
			LocalizationCache.Instance.ForceRefreshAll -= Sync;
		}

		void Sync () {
			if (string.IsNullOrEmpty(locaID)) return;
			LocalizationObjectItem item = LocalizationCache.Loaded.TryGetItemVerified(locaID, typeof(System.String));
			if (item != null) Target.text = item.StoredString;
		}

	}
}