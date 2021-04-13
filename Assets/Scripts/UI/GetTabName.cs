﻿using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), ExecuteInEditMode, DisallowMultipleComponent]
    public class GetTabName : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private int offset = 0;

#if (UNITY_EDITOR)
        private void Awake() { if (text == null) text = GetComponent<TextMeshProUGUI>(); }
#endif

        public void UpdateText() { if (text != null) text.text = Singletons.main.uiManager.History.Last(offset).GetName(); }
    }
}