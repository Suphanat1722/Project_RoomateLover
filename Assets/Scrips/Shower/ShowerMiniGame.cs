using UnityEngine;

public class ShowerMiniGame : MonoBehaviour
{
    [SerializeField] private DialogTrigger dialogTrigger;
    [SerializeField] private SceneController sceneController;

    private int stateAlert = 0; // ค่าความสงสัย
    private int maxAlert = 10;  // ค่าความสงสัยสูงสุด
    private int stateJF = 0;
    private bool hasAlerted; // ตัวแปรตรวจสอบสถานะ

    private void Update()
    {
        
    }
    // เพิ่มความสงสัย
    public void IncreaseAlert()
    {
        if (Random.Range(0f, 1f) <= 0.1f) // โอกาส 10%
        {
            stateAlert = maxAlert; // จับได้ทันที
            Debug.Log("คุณถูกจับได้ทันที!");
        }
        else
        {
            stateAlert = stateAlert + 3;
            Debug.Log($"ความสงสัยเพิ่มขึ้น: {stateAlert}");
            CheckAlertState();
        }
    }

    // ลดความสงสัย
    public void DecreaseAlert()
    {
        if (Random.Range(0f, 1f) <= 0.05f) // โอกาส 5%
        {
            stateAlert = 0; // ล้างค่าความสงสัยทั้งหมด
            Debug.Log("ความสงสัยถูกล้างออกทั้งหมด!");
        }
        else if (stateAlert > 0)
        {
            stateAlert = stateAlert - 2;
            Debug.Log($"ความสงสัยลดลง: {stateAlert}");
            CheckAlertState();
        }
    }

    // ตรวจสอบสถานะ
    public void CheckAlertState()
    {


            if (stateAlert >= 3 && stateAlert < 6)
            {
                Debug.Log("รู้สึกแปลกๆ");
 
            }
            else if (stateAlert >= 6 && stateAlert < 10)
            {
                Debug.Log("เริ่มสงสัย");
    
            }
            else if (stateAlert >= 10)
            {
                test();
                Debug.Log("จับได้");

            }
        
    }

    public void IncreaseJF()
    {
        if (Random.Range(0f, 1f) <= 0.1f) // โอกาส 10%
        {
            stateAlert = maxAlert; // จับได้ทันที
        }
        else
        {
            stateJF = stateJF + 4;
            stateAlert = stateAlert + 3;
            Debug.Log($"ความสงสัยเพิ่มขึ้น: {stateAlert}");
            Debug.Log($"ความเสวเพิ่มขึ้น: {stateJF}");
            CheckJfState();
        }
    }

    public void CheckJfState()
    {
        if (stateJF >= 10)
        {
            //dialogTrigger.TriggerDialog("test", "test", () => sceneController.LoadSceneByIndex(0));
            Debug.Log("แตก 1!!");
        }

    }

    public void test()
    {
        Debug.Log("test");
        //dialogTrigger.TriggerDialog("test", "test", () => sceneController.LoadSceneByIndex(0));
    }
}
