using System.Collections;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Dialog Settings")]
    [SerializeField] private DialogTrigger dialogTrigger;

    [Header("UI Elements")]
    [SerializeField] private GameObject menuInteractCharacter;
    [SerializeField] private GameObject panelMenuButtonInteract;
    [SerializeField] private GameObject panelButtonTalk;
    [SerializeField] private GameObject dialogBoxActive;

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
        if (Input.GetMouseButtonUp(0)) // คลิกซ้าย
        {
            if (DialogManager.isTyping && menuInteractCharacter.activeSelf && dialogBoxActive.activeSelf) // ถ้ากำลังพิมพ์ ให้ข้ามไปแสดงประโยคทั้งหมด
            {
                DialogManager.isLeftClickedToSkip = true;
            }
            else // ถ้าไม่กำลังพิมพ์ ให้เริ่มการสนทนา
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

        if (Input.GetMouseButtonUp(1) && menuInteractCharacter.activeSelf) // คลิกขวาเพื่อปิดเมนู
        {
            menuInteractCharacter.SetActive(false);
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
        RandomCharacterInRoom();
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
    /// เปิดเมนูพูดคุย
    /// </summary>
    private void ActivateTalkMenu()
    {
        panelButtonTalk.SetActive(true);
        menuInteractCharacter.SetActive(true);
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
    public void TalkNormally()
    {
        panelButtonTalk.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องทั่วไป", ResetRoom);
    }
    public void TalkFunny()
    {
        panelButtonTalk.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องตลก", ResetRoom);
    }
    public void TalkDirty()
    {
        panelButtonTalk.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องลามก", ResetRoom);
    }
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

        dialogTrigger.TriggerDialog("นีน่า", "เลือกเครื่องดื่ม", ResetRoom);
    }

    public void PlayGameWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกเล่นเกม", ResetRoom);
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

        dialogTrigger.TriggerDialog("นีน่า", "เลือกอาบน้ำ", ResetRoom);
    }

    public void ShoppingWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกซื้อของออนไลน์", ResetRoom);
    }

    public void GoToBedWithCharacter()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกเข้านอน", ResetRoom);
    }
}
