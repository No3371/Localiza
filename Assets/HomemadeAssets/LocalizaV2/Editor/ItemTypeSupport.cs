using System;
using UnityEngine;


namespace LocalizaV2 {

	[System.Serializable]
	public class ItemTypeSupport {

		string _Mode = "Horizontal";

		public string Mode {
			get { return _Mode; }
		}

		public Type target, drawerType;

		// public delegate void DrawItem (LocalizationObjectItem reference, Action OnContentChanged, Action OnItemIDEdited, Action OnItemRemovingFromGroup, Action OnItemRemovedAndDestroying, params GUILayoutOption[] areaOptions);


		///Decide items of this type show in "Horizontal" or "Grid" mode in Localiza, and assign a drawer class.
		public ItemTypeSupport (string TargetType, string Mode, string DrawerType) {
			this.target = Type.GetType(TargetType);
			this._Mode = Mode;
			this.drawerType = Type.GetType(DrawerType);
		}
	}
}