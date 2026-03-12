using TMPro;
using UnityEngine;

public class TestGoldUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI goldText;

    private long number = 0;

    private void Update()
    {
        if (long.TryParse(inputField.text, out number))
        {
            goldText.text = NotateNumber.ChangeNumber(number);
        }
    }
}
