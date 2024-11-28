using UnityEngine;

public class RandomCharacterInScene : MonoBehaviour
{
    public GameObject[] characters;

    public static int randomIndex;

    void Start()
    {
        randomIndex = Random.Range(0, characters.Length);
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == randomIndex);
        }
    }

    void Update()
    {
        if (DialogTrigger.isShowCanvasDialogBox)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].SetActive(false);
            }
        }
    }
}
