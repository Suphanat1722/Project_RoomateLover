using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionMenuUI : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform panel;      // ตัวเองก็ได้
    public Transform contentRoot;    // ที่วางปุ่ม (มี VerticalLayoutGroup)
    public Button buttonPrefab;      // ปุ่มต้นแบบ (มี TMP_Text ลูก)

    private readonly List<Button> _pool = new();

    public struct Item
    {
        public string label;
        public Action onClick;
        public Item(string l, Action a) { label = l; onClick = a; }
    }

    public void Show(List<Item> items, Vector3 screenPos, RectTransform canvasRect)
    {
        Clear();

        foreach (var it in items)
        {
            var b = Instantiate(buttonPrefab, contentRoot);
            var txt = b.GetComponentInChildren<TMP_Text>(true);
            if (txt) txt.text = it.label;
            b.onClick.AddListener(() => { it.onClick?.Invoke(); Hide(); });
            _pool.Add(b);
        }

        panel.gameObject.SetActive(true);

        // วางตำแหน่ง + clamp ขอบ (รองรับ Canvas: Screen Space Overlay)
        if (canvasRect != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out var local);
            panel.anchoredPosition = local;

            var size = panel.sizeDelta;
            var half = canvasRect.sizeDelta * 0.5f;
            var pos = panel.anchoredPosition;
            pos.x = Mathf.Clamp(pos.x, -half.x + size.x * 0.5f, half.x - size.x * 0.5f);
            pos.y = Mathf.Clamp(pos.y, -half.y + size.y * 0.5f, half.y - size.y * 0.5f);
            panel.anchoredPosition = pos;
        }
        else
        {
            panel.position = screenPos;
        }
    }

    public void Hide() => panel.gameObject.SetActive(false);

    void Clear()
    {
        foreach (var b in _pool) if (b) Destroy(b.gameObject);
        _pool.Clear();
    }
}
