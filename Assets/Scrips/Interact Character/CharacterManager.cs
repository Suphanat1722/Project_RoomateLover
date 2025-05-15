using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterManager : MonoBehaviour
{
    [Header("Dialog")]
    public DialogTrigger dialogTrigger;
    [Header("PlayerStats")]
    public PlayerStats playerStats;
    [Header("GameTime")]
    public GameTime gameTime;
    [Header("SceneController")]
    public SceneController sceneController;

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

        // สมัครสมาชิกกับ Event
        dialogTrigger.OnDialogEndedEvent += HandleDialogEnded;
        CharacterSitdown();


    }
    private void OnDestroy()
    {
        // ยกเลิกการสมัครสมาชิกเมื่อ Destroy
        dialogTrigger.OnDialogEndedEvent -= HandleDialogEnded;
    }

    private void Update()
    {
        HandleCharacterInteraction();

        // เปลี่ยนฉากเมื่อเกินเที่ยงคืนและไดอะล็อกจบลงแล้ว
        if (gameTime.GetHourCurrentTime() == 0 && isDialogEnded)
        {           
            SleepingCharacter(true);
            isDialogEnded = false; // รีเซ็ตสถานะ
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Sleeping Character"))
        {
            if (Input.GetMouseButton(0))
            {
                Debug.Log("จะลักหลับหรือไม่");
                sceneController.SwitchScene(SceneController.SceneType.BedRoom);
                SleepingCharacter(false);
            }

        }

    }
    private void HandleDialogEnded()
    {
        //Debug.Log("ไดอะล็อกสิ้นสุด");
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


    public void CharacterSitdown()
    {
        characters[0].SetActive(true);
    }


    public void SleepingCharacter(bool isSleeping)
    {
        if (isSleeping)
        {
            characters[0].SetActive(false);
            characters[1].SetActive(false);
            characters[2].SetActive(true);
        }
        else
        {
            characters[0].SetActive(true);
            characters[1].SetActive(false);
            characters[2].SetActive(false);
        }

    }

    /*
    private void DisableAllCharacters()
    {
        if (menuInteractCharacter.activeSelf || DialogTrigger.IsDialogActive)
        {
            foreach (var character in characters)
            {
                character.SetActive(false);
            }
        }else if (!menuInteractCharacter.activeSelf || !DialogTrigger.IsDialogActive)
        {
            foreach (var character in characters)
            {
                character.SetActive(true);
            }
        }
    }*/

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

    }

    /// <summary>
    /// เปลี่ยนฉากเข้าช่วงกลางคืน
    /// </summary>
    private void IntoNighScene(int sceneIndex)
    {
       // sceneController.LoadSceneByIndex(sceneIndex);
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
            // ใช้อันนี้แทน AddTime เพื่อไม่ให้เกินเที่ยงคืน
            gameTime.AddTimeUntilMidnight(5, 30);
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
        gameTime.SetTimeCurrentTime(00,00);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกเข้านอน", () => SleepingCharacter(true));
    }
}
