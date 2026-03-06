using UnityEngine;

[CreateAssetMenu(fileName = "NewAchieveSO", menuName = "Achievement/Standard")]
public class AchievementSO : ScriptableObject
{
    public string id;                       //고유ID
    public string title;                    //제목
    [TextArea] public string description;   //설명
    public int targetValue;                 //목표 수치
    public bool isUnlocked;                 //달성 여부
}
