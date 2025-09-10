using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ClothingInteractionManager : MonoBehaviour
{
    // ====== STATE / CONDITION ======
    public enum PartActionState { None, Grabbed, Touched, Spread, Custom1, Custom2 }

    public enum ConditionType
    {
        None,
        RequiresSelfVisible,
        RequiresSelfHidden,
        RequiresOtherVisible,
        RequiresOtherHidden,
        RequiresSelfState,
        RequiresNotSelfState
    }

    // ====== DATA: BUTTON DEF ======
    [Serializable]
    public class ButtonDef
    {
        [Tooltip("ข้อความบนปุ่ม")]
        public string label = "ปุ่ม";

        [Header("เงื่อนไขแสดงผล")]
        public ConditionType condition = ConditionType.None;
        [Tooltip("ใช้เมื่อเงื่อนไขอิงอีกชิ้น เช่น ต้องถอด Pants ก่อน")]
        public string otherPartId;
        [Tooltip("ใช้เมื่อเงื่อนไขอิงสถานะของตัวเอง")]
        public PartActionState requiredSelfState = PartActionState.None;

        [Header("เมื่อกดปุ่ม (Inspector จะลากเมทอดมาใส่ได้)")]
        public UnityEvent onClick;
    }

    // ====== DATA: PART ======
    [Serializable]
    public class ClothingPart
    {
        [Tooltip("ID ของชิ้นส่วน (ต้องตรงกับ Tag หรือ LayerName ที่เลือกใช้)")]
        public string partId;

        [Header("Visual / Level")]
        public SpriteRenderer renderer;
        public List<Sprite> levelSprites = new();
        public int startLevel = 0;
        [Tooltip("Max level (รวม). 0 = ใช้จำนวน sprites-1")]
        public int explicitMaxLevel = 0;

        [Tooltip("เมื่อกดถอดที่เลเวลสุดท้าย จะให้หายไปไหม")]
        public bool hideWhenBeyondMax = true;

        [Header("Interact / Collider")]
        public List<Collider2D> collidersToToggle = new();
        public bool interactableWhenVisible = true;
        public bool interactableWhenHidden = false;

        [Header("Buttons (ปรับใน Inspector)")]
        public List<ButtonDef> buttons = new();

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

        public void StepUndress()
        {
            if (level < MaxLevel)
            {
                level++;
                Apply();
            }
            else
            {
                if (hideWhenBeyondMax)
                {
                    level = MaxLevel + 1; // หายไป
                    Apply();
                }
                // ถ้าไม่ให้หาย: ค้างที่ Max ไม่ทำอะไร
            }
        }

        public void WearBack()
        {
            level = 0;
            Apply();
        }

        public void RemoveFully()
        {
            level = MaxLevel + 1;
            Apply();
        }

        public void SetLevel(int toLevel)
        {
            level = Mathf.Max(0, toLevel);
            Apply();
        }

        public void SetActionState(PartActionState s)
        {
            actionState = s;
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

    // ====== INSPECTOR: INPUT / UI / PARTS ======
    [Header("Raycast / Input")]
    public LayerMask bodyPartLayer;
    public Camera mainCamera;
    [Tooltip("ON = ใช้ LayerName, OFF = ใช้ Tag")]
    public bool useLayerNameInsteadOfTag = false;

    [Header("UI")]
    public Canvas actionCanvas;
    public ActionMenuUI actionMenuPrefab;

    [Header("Parts")]
    public List<ClothingPart> parts = new();

    // ====== EVENTS (ออกไประบบอื่น) ======
    public event Action<string, string> OnActionPerformed;      // (partId, buttonLabel)
    public event Action<string, bool> OnPartVisibilityChanged; // (partId, visible)

    // ====== RUNTIME ======
    private ActionMenuUI _currentMenu;
    private RectTransform _canvasRect;

    [SerializeField] bool debugLogs = false;

    void Awake()
    {
        if (!mainCamera) mainCamera = Camera.main;
        if (actionCanvas) _canvasRect = actionCanvas.transform as RectTransform;
        if (!actionCanvas) Debug.LogWarning("actionCanvas is not assigned.");
        if (!actionMenuPrefab) Debug.LogWarning("actionMenuPrefab is not assigned.");

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
            if (p != null) p.OnVisibilityChangedInternal -= HandlePartVisibilityChanged;
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

            if (debugLogs)
            {
                if (!hit) Debug.Log("Click: no collider hit");
                else
                {
                    string idDbg = useLayerNameInsteadOfTag ? LayerMask.LayerToName(hit.gameObject.layer) : hit.tag;
                    Debug.Log($"Click hit: {hit.name} | tag={hit.tag} | layer={LayerMask.LayerToName(hit.gameObject.layer)} | resolvedId={idDbg}");
                }
            }

            if (hit != null)
            {
                string id = useLayerNameInsteadOfTag ? LayerMask.LayerToName(hit.gameObject.layer) : hit.tag;
                ShowActionsFor(id, Input.mousePosition);
            }
            else HideMenu();
        }
    }

    // ====== MENU BUILDING ======
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
        if (self == null) return list;

        // วนตามลำดับปุ่มที่กำหนดใน Inspector
        foreach (var def in self.buttons)
        {
            if (!CheckCondition(def, self)) continue;

            // สร้างปุ่มพร้อม action
            list.Add(new ActionMenuUI.Item(def.label, () =>
            {
                // เรียก UnityEvent ที่ผูกไว้
                def.onClick?.Invoke();

                // แจ้งออกไป (ไว้ต่อระบบอื่น เช่น คะแนน/เสียง)
                OnActionPerformed?.Invoke(partId, def.label);
            }));
        }

        // ปุ่มปิด
        list.Add(new ActionMenuUI.Item("ปิด", () => { }));
        return list;
    }

    bool CheckCondition(ButtonDef def, ClothingPart self)
    {
        switch (def.condition)
        {
            case ConditionType.None: return true;
            case ConditionType.RequiresSelfVisible: return self != null && self.IsVisible;
            case ConditionType.RequiresSelfHidden: return self != null && !self.IsVisible;
            case ConditionType.RequiresOtherVisible: { var o = FindPart(def.otherPartId); return o != null && o.IsVisible; }
            case ConditionType.RequiresOtherHidden: { var o = FindPart(def.otherPartId); return o != null && !o.IsVisible; }
            case ConditionType.RequiresSelfState: return self != null && self.actionState == def.requiredSelfState;
            case ConditionType.RequiresNotSelfState: return self != null && self.actionState != def.requiredSelfState;
        }
        return false;
    }

    // ====== PUBLIC UTILS (ไว้ผูก UnityEvent ได้ง่าย) ======
    // คุณลากเมทอดเหล่านี้ไปใส่ใน onClick ของปุ่มได้เลย
    public void StepUndress(string id) { var p = FindPart(id); if (p != null) { p.StepUndress(); OnActionPerformed?.Invoke(id, "StepUndress"); } }
    public void WearBack(string id) { var p = FindPart(id); if (p != null) { p.WearBack(); OnActionPerformed?.Invoke(id, "WearBack"); } }
    public void RemoveFully(string id) { var p = FindPart(id); if (p != null) { p.RemoveFully(); OnActionPerformed?.Invoke(id, "RemoveFully"); } }
    public void SetLevel(string id, int lvl) { var p = FindPart(id); if (p != null) { p.SetLevel(lvl); OnActionPerformed?.Invoke(id, $"SetLevel:{lvl}"); } }
    public void SetState(string id, PartActionState st) { var p = FindPart(id); if (p != null) { p.SetActionState(st); OnActionPerformed?.Invoke(id, $"SetState:{st}"); } }

    // ====== EVENTS / HELPERS ======
    void HandlePartVisibilityChanged(string partId, bool visible)
    {
        OnPartVisibilityChanged?.Invoke(partId, visible);
    }

    ClothingPart FindPart(string id)
        => parts.Find(p => p != null && !string.IsNullOrEmpty(p.partId) && p.partId == id);

    // ช่วยแปลชื่อ ถ้าอยากแสดงไทยใน label ของคุณเองก็ทำได้
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
