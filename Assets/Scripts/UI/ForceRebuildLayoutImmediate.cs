﻿using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class ForceRebuildLayoutImmediate : MonoBehaviour
    {
        [SerializeField] private bool OnAwakeObj;
        [SerializeField] private bool OnEnableObj;

        private void Awake() { if (OnAwakeObj) LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>()); }
        private void OnEnable() { if (OnEnableObj) LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>()); }
    }
}