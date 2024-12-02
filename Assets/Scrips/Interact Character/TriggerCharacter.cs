using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerCharacter : MonoBehaviour
{
    [SerializeField] DialogTrigger dialogTrigger;
    [SerializeField] GameObject menuInteractCharacter;
    [SerializeField] GameObject panelMenuButtonInteract;
    [SerializeField] GameObject canvasDialogBoxActive;
    [SerializeField] GameObject[] characters;

    void Start()
    {
        RandomCharacterInRoomActive();
    }

    void Update()
    {
        CilckCharacterInRoom();

        if (menuInteractCharacter.activeSelf)
        {
            NotActiveCharacter();
        }
    }

    void CilckCharacterInRoom()//เช็คว่าเมาส์ชนกับตัวละครไหม
    {
        if (Input.GetMouseButtonUp(0)) // เมื่อคลิกเมาส์ซ้าย
        {          
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                dialogTrigger.TriggerDialog("นีน่า", "นีน่าในห้อง", ButtonInteractActive);
                
                NotActiveCharacter();
            }
        }

        if (Input.GetMouseButtonUp(1)&& menuInteractCharacter.activeSelf)
        {
            menuInteractCharacter.SetActive(false);
            RandomCharacterInRoomActive();
        }
    }

    void ButtonInteractActive()//เปิด ปุ่มที่กระทำกับตัวละคร
    {
        menuInteractCharacter.SetActive(true);
        panelMenuButtonInteract.SetActive(true);
        RandomCharacterInRoomActive();
    }

    public void RandomCharacterInRoomActive()
    {
        int randomIndex = Random.Range(0, characters.Length);
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == randomIndex);
        }
    }//สุ่มตัวละครขึ้นในฉาก

    void NotActiveCharacter()//ปิดตัวละครทั้งหมดในฉาก
    {
        if (menuInteractCharacter.activeSelf || canvasDialogBoxActive.activeSelf)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].SetActive(false);
            }
        }   
    }
}
