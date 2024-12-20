using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterManager : MonoBehaviour
{
    [Header("Dialog Settings")]
    [SerializeField] private DialogTrigger dialogTrigger;

    [Header("UI Elements")]
    [SerializeField] private GameObject menuInteractCharacter;
    [SerializeField] private GameObject panelMenuButtonInteract;
    [SerializeField] private GameObject panelButtonTalk;
    [SerializeField] private GameObject dialogBoxActive;
    [SerializeField] private GameObject ItemIventoryActive;
    [SerializeField] private GameObject ItemGameIventoryActive;
    [SerializeField] private GameObject ShowerOptionsActive;
    [SerializeField] private GameObject ShoppingActive;

    [Header("Character Settings")]
    [SerializeField] private GameObject[] characters;

    private void Start()
    {
        RandomCharacterInRoom();
    }

    private void Update()
    {
        HandleCharacterInteraction();

        // ปิดตัวละครเมื่อเมนูโต้ตอบแสดงอยู่
        if (menuInteractCharacter.activeSelf)
        {
            DisableAllCharacters();
        }
    }
    /// <summary>
    /// ตรวจจับการคลิกตัวละครในฉาก และเปิดการโต้ตอบ
    /// </summary>
    private void HandleCharacterInteraction()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // ตรวจสอบว่ามีการคลิก UI ก่อนหรือไม่
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // ถ้าคลิก UI ไม่ต้องทำอะไร
            }

            if (DialogManager.isTyping && menuInteractCharacter.activeSelf && dialogBoxActive.activeSelf)
            {
                DialogManager.isLeftClickedToSkip = true;
            }
            else
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
                {
                    dialogTrigger.TriggerDialog("นีน่า", "นีน่าในห้อง", ActivateInteractMenu);
                    DisableAllCharacters();
                }
            }
        }

        if (Input.GetMouseButtonUp(1) && (
            menuInteractCharacter.activeSelf || 
            ItemIventoryActive.activeSelf || 
            ItemGameIventoryActive.activeSelf ||
            ShowerOptionsActive.activeSelf ||
            ShoppingActive.activeSelf
            )) // คลิกขวาเพื่อปิดเมนู
        {
            menuInteractCharacter.SetActive(false);
            ItemIventoryActive.SetActive(false);
            ItemGameIventoryActive.SetActive(false);
            ShowerOptionsActive.SetActive(false);
            ShoppingActive.SetActive(false);
            RandomCharacterInRoom();
        }
    }

    /// <summary>
    /// เปิดเมนูโต้ตอบกับตัวละคร
    /// </summary>
    private void ActivateInteractMenu()
    {
        menuInteractCharacter.SetActive(true);
        panelMenuButtonInteract.SetActive(true);
    }

    /// <summary>
    /// สุ่มเปิดตัวละครในห้อง
    /// </summary>
    public void RandomCharacterInRoom()
    {
        StartCoroutine(RandomCharacterWithDelay());
    }
    private IEnumerator RandomCharacterWithDelay()
    {
        yield return new WaitForSeconds(0.3f); // หน่วงเวลา 0.5 วินาที

        int randomIndex = Random.Range(0, characters.Length);
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == randomIndex);
        }
    }

    /// <summary>
    /// ปิดตัวละครทั้งหมดในฉาก
    /// </summary>
    private void DisableAllCharacters()
    {
        if (menuInteractCharacter.activeSelf || dialogBoxActive.activeSelf)
        {
            foreach (var character in characters)
            {
                character.SetActive(false);
            }
        }

        // ปิด Collider เมื่อ UI เปิด
        foreach (var character in characters)
        {
            Collider2D collider = character.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = !menuInteractCharacter.activeSelf;
            }
        }
    }

    /// <summary>
    /// เปิดเมนูพูดคุย
    /// </summary>
    private void ActivateTalkMenu()
    {
        panelButtonTalk.SetActive(true);
        menuInteractCharacter.SetActive(true);
    }

    private void ActiveItemInventory()
    {
        ItemIventoryActive.SetActive(true);
    }

    private void ActiveItemGameInventory()
    {
        ItemGameIventoryActive.SetActive(true);
    }

    private void ActiveButtonShowerOptions()
    {
        ShowerOptionsActive.SetActive(true);
    }

    private void ActiveButtonShopping()
    {
        ShoppingActive.SetActive(true);
    }


    /// <summary>
    /// รีเซ็ตการตั้งค่าห้อง
    /// </summary>
    private void ResetRoom()
    {
        RandomCharacterInRoom();
    }

    //------------------------------------------------ส่วนการกระทำต่างๆ ของปุ่ม----------------------------------

    public void ChatWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกพูดคุย", ActivateTalkMenu);
    }
    /// <summary>
    /// ปุ่มแยกย่อยของ ChatWithCharacter()
    /// </summary>
    public void TalkNormally()
    {
        panelButtonTalk.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องทั่วไป", ResetRoom);
    }
    /// <summary>
    /// ปุ่มแยกย่อยของ ChatWithCharacter()
    /// </summary>
    public void TalkFunny()
    {
        panelButtonTalk.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องตลก", ResetRoom);
    }
    /// <summary>
    /// ปุ่มแยกย่อยของ ChatWithCharacter()
    /// </summary>
    public void TalkDirty()
    {
        panelButtonTalk.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องลามก", ResetRoom);
    }
    /// <summary>
    /// ปุ่มแยกย่อยของ ChatWithCharacter()
    /// </summary>
    public void TalkSerious()
    {
        panelButtonTalk.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องจริงจัง", ResetRoom);
    }


    public void DrinkWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกเครื่องดื่ม", ActiveItemInventory);
    }

    public void PlayGameWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกเล่นเกม", ActiveItemGameInventory);
    }

    public void WatchTvWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("ผู้เล่น", "เลือกดูTV", ResetRoom);
    }

    public void ShowerWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกอาบน้ำ", ActiveButtonShowerOptions);
    }

    public void ShoppingWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกซื้อของออนไลน์", ActiveButtonShopping);
    }

    public void GoToBedWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกเข้านอน", ResetRoom);
    }
}
