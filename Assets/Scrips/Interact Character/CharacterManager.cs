using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterManager : MonoBehaviour
{
    [Header("Dialog Settings")]
    [SerializeField] private DialogTrigger dialogTrigger;
    [Header("PlayerStats Settings")]
    [SerializeField] private PlayerStats playerStats;
    [Header("GameTime Settings")]
    [SerializeField] private GameTime gameTime;
    [Header("SceneController Settings")]
    [SerializeField] private SceneController sceneController;

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

    private bool isDialogEnded = false;

    private void Start()
    {
        RandomCharacterInRoom();
        // สมัครสมาชิกกับ Event
        dialogTrigger.OnDialogEndedEvent += HandleDialogEnded;
    }
    private void OnDestroy()
    {
        // ยกเลิกการสมัครสมาชิกเมื่อ Destroy
        dialogTrigger.OnDialogEndedEvent -= HandleDialogEnded;
    }

    private void Update()
    {
        HandleCharacterInteraction();

        // ปิดตัวละครเมื่อเมนูโต้ตอบแสดงอยู่
        if (menuInteractCharacter.activeSelf)
        {
            DisableAllCharacters();
        }
        // เปลี่ยนฉากเมื่อเกินเที่ยงคืนและไดอะล็อกจบลงแล้ว
        if (gameTime.IsAfterMidnight() && isDialogEnded)
        {
            dialogTrigger.TriggerDialog("test", "test", ()=>IntoNighScene(2));
            
            isDialogEnded = false; // รีเซ็ตสถานะ
        }

    }
    private void HandleDialogEnded()
    {
        Debug.Log("ไดอะล็อกสิ้นสุด");
        isDialogEnded = true;
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
    }

    /// <summary>
    /// เช็คว่าเปิดเมนูหรือ UI อะไรอยู่ไหม (ไม่ได้ใช้งาน)
    /// </summary>
    public void ToggleCharacterInteraction(bool isMenuOpen)
    {
        foreach (var character in characters)
        {
            Collider2D collider = character.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = !isMenuOpen && character.activeSelf;
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

    /// <summary>
    /// เปลี่ยนฉากเข้าช่วงกลางคืน
    /// </summary>
    private void IntoNighScene(int sceneIndex)
    {
        sceneController.LoadSceneByIndex(sceneIndex);
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
        if (playerStats.actionPoint >= 20)
        {
            panelButtonTalk.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องทั่วไป", ResetRoom);
            playerStats.UseActionPoint(20);
            gameTime.AddTime(5,30);
        }
        else
        {
            panelButtonTalk.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("test", "test", ResetRoom);
        }
    }
    /// <summary>
    /// ปุ่มแยกย่อยของ ChatWithCharacter()
    /// </summary>
    public void TalkFunny()
    {
        if (playerStats.actionPoint >= 20)
        {
            panelButtonTalk.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องตลก", ResetRoom);
            playerStats.UseActionPoint(20);
        }
        else
        {
            panelButtonTalk.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("test", "test", ResetRoom);
        }
        
    }
    /// <summary>
    /// ปุ่มแยกย่อยของ ChatWithCharacter()
    /// </summary>
    public void TalkDirty()
    {
        if (playerStats.actionPoint >= 20)
        {
            panelButtonTalk.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องลามก", ResetRoom);
            playerStats.UseActionPoint(20);
        }
        else
        {
            panelButtonTalk.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("test", "test", ResetRoom);
        }

    }
    /// <summary>
    /// ปุ่มแยกย่อยของ ChatWithCharacter()
    /// </summary>
    public void TalkSerious()
    {
        if (playerStats.actionPoint >= 20)
        {
            panelButtonTalk.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องจริงจัง", ResetRoom);
            playerStats.UseActionPoint(20);
        }
        else
        {
            panelButtonTalk.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("test", "test", ResetRoom);
        }

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
        if (playerStats.actionPoint >= 20)
        {
            panelMenuButtonInteract.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("ผู้เล่น", "เลือกดูTV", ResetRoom);
            playerStats.UseActionPoint(20);
        }
        else
        {
            panelButtonTalk.SetActive(false);
            menuInteractCharacter.SetActive(false);

            dialogTrigger.TriggerDialog("test", "test", ResetRoom);
        }
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
