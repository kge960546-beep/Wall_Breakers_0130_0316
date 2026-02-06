using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestCreditButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        CreditService creditService = GameManager.Instance?.GetService<CreditService>();
        if (creditService != null)
        {
            creditService.AddCredit(100);
            Debug.Log("Added 100 credits via button");
        }
    }
}