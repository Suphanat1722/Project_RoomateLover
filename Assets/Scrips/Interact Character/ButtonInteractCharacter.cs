using UnityEngine;

public class ButtonInteractCharacter : MonoBehaviour
{
    [SerializeField] DialogTrigger dialogTrigger;
    [SerializeField] GameObject menuInteractCharacter;
    [SerializeField] GameObject panelMenuButtonInteract;
    [SerializeField] GameObject panelButtonTalk;
    [SerializeField] TriggerCharacter triggerCharacter;

    public void Chatting()
    {
        panelMenuButtonInteract.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("นีน่า", "เลือกพูดคุย", ActiveButtonInteract);
    }

    public void TalkNomal()
    {
        panelButtonTalk.SetActive(false);
        menuInteractCharacter.SetActive(false);

        dialogTrigger.TriggerDialog("ผู้เล่น", "พูดคุยเรื่องทั่วไป", DoNotThing);
    }

    void ActiveButtonInteract()
    {
        panelButtonTalk.SetActive(true);
        menuInteractCharacter.SetActive(true);
    }

    void DoNotThing()
    {
        triggerCharacter.RandomCharacterInRoomActive();
    }
}
