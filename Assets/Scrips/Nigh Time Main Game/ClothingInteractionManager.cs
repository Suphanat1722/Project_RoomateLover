using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class ClothingInteractionManager : MonoBehaviour
{
    // ============ ACTION STATE (ข้อ 4) ============
    public enum PartActionState { None, Grabbed, Touched, Spread, Custom1, Custom2 }

    // ============ DATA MODEL ============
    [Serializable]
    public class ClothingPart
    {
        [Tooltip("ID ของชิ้น (ต้องตรงกับ Tag หรือ LayerName ที่เลือกใช้) เช่น Shirt, Pants, Breast, Thigh")]
        public string partId;

        [Tooltip("SpriteRenderer ของชิ้นนี้")]
        public SpriteRenderer renderer;

        [Tooltip("สไปรต์ตามเลเวล (0..N-1). เว้นว่างได้ถ้าใช้แค่ซ่อน/แสดง")]
        public List<Sprite> levelSprites = new();

        [Tooltip("เลเวลเริ่มต้น")]
        public int startLevel = 0;

        [Tooltip("Max level (รวม). 0 = ใช้จำนวน levelSprites-1")]
        public int explicitMaxLevel = 0;

        [Header("Interactivity / Colliders (ข้อ 3)")]
        [Tooltip("Collider ที่จะเปิด/ปิดตาม state")]
        public List<Collider2D> collidersToToggle = new();
        [Tooltip("คลิกได้เมื่อมองเห็นหรือไม่")]
        public bool interactableWhenVisible = true;
        [Tooltip("คลิกได้เมื่อถูกถอดจน 'หาย' หรือไม่")]
        public bool interactableWhenHidden = false;

        [Header("Runtime (read-only)")]
        [HideInInspector] public int level;
        [HideInInspector] public PartActionState actionState = PartActionState.None;

        public int MaxLevel => (explicitMaxLevel > 0)
            ? explicitMaxLevel
            : Mathf.Max((levelSprites.Count > 0 ? levelSprites.Count - 1 : 0), 0);

        public bool IsVisible => renderer && renderer.gameObject.activeSelf && level <= MaxLevel;

        public event Action<string, bool> OnVisibilityChangedInternal; // partId, visible

        public void Init()
        {
            level = Mathf.Max(0, startLevel);
            actionState = PartActionState.None;
            Apply();
        }

        public void WearBack()   // ใส่กลับเต็ม
        {
            level = 0;
            Apply();
        }

        public void RemoveFully() // ถอดจน "หาย"
        {
            level = MaxLevel + 1;
            Apply();
        }

        public void SetActionState(PartActionState s)
        {
            actionState = s;
            // ถ้าการเปลี่ยน state มีผลกับภาพ/คอลลิเดอร์ เพิ่ม logic ได้ที่นี่
            UpdateColliderInteractivity();
        }

        public void Apply()
        {
            if (!renderer) { UpdateColliderInteractivity(); return; }

            bool before = renderer.gameObject.activeSelf;
            bool visible = level <= MaxLevel;

            renderer.gameObject.SetActive(visible);

            if (visible && levelSprites.Count > 0)
            {
                int idx = Mathf.Clamp(level, 0, levelSprites.Count - 1);
                renderer.sprite = levelSprites[idx];
            }

            if (before != visible)
                OnVisibilityChangedInternal?.Invoke(partId, visible);

            UpdateColliderInteractivity();
        }

        void UpdateColliderInteractivity()
        {
            bool visible = level <= MaxLevel;
            bool interactable = visible ? interactableWhenVisible : interactableWhenHidden;

            foreach (var col in collidersToToggle)
            {
                if (!col) continue;
                col.enabled = interactable;
            }
        }
    }

    // ============ ปุ่มเงื่อนไข (Data-driven) ============
    public enum ConditionType
    {
        None,
        RequiresSelfVisible,
        RequiresSelfHidden,
        RequiresOtherVisible,
        RequiresOtherHidden,
        RequiresSelfState,     // ข้อ 4: ต้องอยู่ใน state ที่กำหนด
        RequiresNotSelfState
    }

    [Serializable]
    public class ConditionalAction
    {
        [Tooltip("ปุ่มนี้ไปโผล่ที่ชิ้นไหน (partId) เช่น Thigh, Breast")]
        public string targetPartId;

        [Tooltip("ชื่อปุ่ม")]
        public string label;

        [Tooltip("เงื่อนไขการแสดงปุ่ม")]
        public ConditionType condition = ConditionType.None;

        [Tooltip("ถ้าเงื่อนไขอิงชิ้นอื่น ให้กรอก partId ของชิ้นนั้น เช่น Pants")]
        public string otherPartId;

        [Tooltip("ถ้าเป็นเงื่อนไขอิง state ของตัวเอง ให้เลือก state ที่ต้องการ")]
        public PartActionState requiredSelfState = PartActionState.None;

        [Header("ผลเมื่อกดปุ่ม")]
        [Tooltip("ตั้ง state ให้ชิ้นเป้าหมายเมื่อกดปุ่ม (เว้น None เพื่อไม่แตะ)")]
        public PartActionState setSelfStateTo = PartActionState.None;

        [Tooltip("เหตุการณ์ (UnityEvent) ที่จะถูกเรียกเมื่อกดปุ่ม")]
        public UnityEvent onInvoke;
    }

    // ============ Inspector ============
    [Header("Raycast / Input")]
    public LayerMask bodyPartLayer;
    public Camera mainCamera;
    [Tooltip("เปิด = ใช้ LayerName เป็นตัวบ่งชี้ชิ้น; ปิด = ใช้ Tag (ข้อ 2)")]
    public bool useLayerNameInsteadOfTag = false;

    [Header("UI")]
    public Canvas actionCanvas;             // Screen Space - Overlay แนะนำ
    public ActionMenuUI actionMenuPrefab;   // Prefab เมนู (VerticalLayout + Button prefab)

    [Header("Parts")]
    public List<ClothingPart> parts = new();

    [Header("Extra Actions (Data-driven)")]
    public List<ConditionalAction> extraActions = new();

    // ============ Events (ข้อ 5) ============
    public event Action<string, string> OnActionPerformed;    // (partId, actionLabel)
    public event Action<string, bool> OnPartVisibilityChanged; // (partId, visible)

    // ============ Runtime ============
    private ActionMenuUI _currentMenu;
    private RectTransform _canvasRect;

    void Awake()
    {
        if (!mainCamera) mainCamera = Camera.main;
        if (!actionCanvas) Debug.LogWarning("actionCanvas is not assigned.");
        if (!actionMenuPrefab) Debug.LogWarning("actionMenuPrefab is not assigned.");
        if (actionCanvas) _canvasRect = actionCanvas.transform as RectTransform;

        foreach (var p in parts)
        {
            if (p == null) continue;
            p.Init();
            p.OnVisibilityChangedInternal += HandlePartVisibilityChanged;
        }
        if (actionCanvas) actionCanvas.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        foreach (var p in parts)
            if (p != null)
                p.OnVisibilityChangedInternal -= HandlePartVisibilityChanged;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { HideMenu(); return; }
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!mainCamera) return;

            Vector2 world = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.OverlapPoint(world, bodyPartLayer);
            if (hit != null)
            {
                string id = useLayerNameInsteadOfTag
                    ? LayerMask.LayerToName(hit.gameObject.layer)
                    : hit.tag;

                ShowActionsFor(id, Input.mousePosition);
            }
            else HideMenu();
        }
    }

    // ============ Menu building ============
    void ShowActionsFor(string partId, Vector3 screenPos)
    {
        HideMenu();
        if (!actionCanvas || !actionMenuPrefab) return;

        var items = BuildActions(partId);
        if (items.Count == 0) return;

        _currentMenu = Instantiate(actionMenuPrefab, actionCanvas.transform);
        _currentMenu.Show(items, screenPos, _canvasRect);
    }

    void HideMenu()
    {
        if (_currentMenu)
        {
            Destroy(_currentMenu.gameObject);
            _currentMenu = null;
        }
    }

    List<ActionMenuUI.Item> BuildActions(string partId)
    {
        var list = new List<ActionMenuUI.Item>();
        var self = FindPart(partId);

        // 1) กติกาพื้นฐาน: ถ้าเป็นชิ้นที่มี renderer → ถอด/ใส่กลับ
        if (self != null && self.renderer != null)
        {
            if (self.IsVisible)
                list.Add(new ActionMenuUI.Item($"ถอด{ThaiName(partId)}", () => PerformBuiltIn(self, "ถอด", () => self.RemoveFully())));
            else
                list.Add(new ActionMenuUI.Item($"สวมกลับ{ThaiName(partId)}", () => PerformBuiltIn(self, "สวมกลับ", () => self.WearBack())));
        }

        // 2) ปุ่มเพิ่มเติมตามเงื่อนไข
        foreach (var rule in extraActions)
        {
            if (!string.Equals(rule.targetPartId, partId, StringComparison.Ordinal)) continue;
            if (!CheckCondition(rule, self)) continue;

            list.Add(new ActionMenuUI.Item(rule.label, () =>
            {
                // ตั้ง state ถ้ากำหนดไว้
                if (self != null && rule.setSelfStateTo != PartActionState.None)
                    self.SetActionState(rule.setSelfStateTo);

                // เรียก UnityEvent
                rule.onInvoke?.Invoke();

                // แจ้งออกไประบบอื่น
                OnActionPerformed?.Invoke(partId, rule.label);
            }));
        }

        // 3) ปุ่มปิด
        list.Add(new ActionMenuUI.Item("ปิด", () => { }));
        return list;
    }

    bool CheckCondition(ConditionalAction rule, ClothingPart self)
    {
        switch (rule.condition)
        {
            case ConditionType.None: return true;
            case ConditionType.RequiresSelfVisible: return self != null && self.IsVisible;
            case ConditionType.RequiresSelfHidden: return self != null && !self.IsVisible;
            case ConditionType.RequiresOtherVisible: { var o = FindPart(rule.otherPartId); return o != null && o.IsVisible; }
            case ConditionType.RequiresOtherHidden: { var o = FindPart(rule.otherPartId); return o != null && !o.IsVisible; }
            case ConditionType.RequiresSelfState: return self != null && self.actionState == rule.requiredSelfState;
            case ConditionType.RequiresNotSelfState: return self != null && self.actionState != rule.requiredSelfState;
        }
        return false;
    }

    void PerformBuiltIn(ClothingPart target, string label, Action op)
    {
        op?.Invoke();
        OnActionPerformed?.Invoke(target.partId, label);
    }

    // ============ Events / Helpers ============
    void HandlePartVisibilityChanged(string partId, bool visible)
    {
        OnPartVisibilityChanged?.Invoke(partId, visible);
    }

    ClothingPart FindPart(string id)
        => parts.Find(p => p != null && !string.IsNullOrEmpty(p.partId) && p.partId == id);

    string ThaiName(string id)
    {
        switch (id)
        {
            case "Shirt": return "เสื้อ";
            case "Pants": return "กางเกง";
            case "Breast": return "หน้าอก";
            case "Thigh": return "ต้นขา";
            default: return "ชิ้นส่วน";
        }
    }
}
