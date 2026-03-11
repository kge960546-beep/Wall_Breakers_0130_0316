using TMPro;
using UnityEngine;
public enum Speed
{
    Original,
    TwoXSpeed,
    FourXSpeed
}
public class DoubleSpeed : MonoBehaviour
{
    [SerializeField] Speed speed;
    [SerializeField] TextMeshProUGUI speedText;

    private void Start()
    {
        speedText.text = "1X";
    }

    public void OnClickDoubleSpeed()
    {
        switch (speed)
        {
            case (Speed.Original):
                speedText.text = "2X";
                speed = Speed.TwoXSpeed;
                Time.timeScale = 2.0f;
                break;
            case (Speed.TwoXSpeed):
                speedText.text = "4X";
                speed = Speed.FourXSpeed;
                Time.timeScale = 4.0f;
                break;
            case (Speed.FourXSpeed):
                speedText.text = "1X";
                speed = Speed.Original;
                Time.timeScale = 1.0f;
                break;
        }

    }
}
