using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerCharacter : MonoBehaviour
{
    [SerializeField] private DialogTrigger dialogTrigger;
    [SerializeField] private GameObject canvasButtonInteract;
    [SerializeField] private GameObject[] characters;

    void Start()
    {
        RandomCharacterInRoomActive();
    }

    void Update()
    {
        RaycastInteraction();  
    }

    void RaycastInteraction()
    {
        if (Input.GetMouseButtonUp(0)) // เมื่อคลิกเมาส์ซ้าย
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                Debug.Log("TriggerDialog");
                dialogTrigger.TriggerDialog("นีน่า", "นีน่าในห้อง", ButtonInteractActive);
                
                NotActiveCharacter();
            }
        }
    }

    void ButtonInteractActive()
    {
        canvasButtonInteract.SetActive(true);
        RandomCharacterInRoomActive();
    }

    void RandomCharacterInRoomActive()
    {
        int randomIndex = Random.Range(0, characters.Length);
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == randomIndex);
        }
    }

    void NotActiveCharacter()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(false);
        }
    }
}
