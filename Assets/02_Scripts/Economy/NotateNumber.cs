using UnityEngine;

public class NotateNumber
{
    public static string ChangeNumber(long originNumber)
    {
        string[] symbol = new string[7] { "K", "M", "B", "T", "Qa", "Qi", "Sx"};

        string result = originNumber.ToString();

        // 숫자 + 심볼은 최대 4자리 까지 표기하는데 숫자가 4자리 이하라면 심볼 없이 출력
        if (result.Length < 4)
        {
            return result;
        }

        for (int i = 0; i < symbol.Length; ++i)
        {
            if( 4 + 3 * i <= result.Length && result.Length < 4 + 3*(i + 1))
            {
                //3으로 나눠 나온 나머지 값(n)은 0, 1, 2
                int n = result.Length % 3;
                // 나머지 값(n)이 0이면 3으로 설정
                n = n == 0 ? 3 : n;

                //나머지 값 개수(n) = 앞자리 개수
                //앞자리 표현에 사용한 값 바로 뒤의 값을 소수점 이하 숫자로 표현
                result = $"{result.Substring(0, n)}.{result.Substring(n, 1)}";
                //축약 기호 추가
                result += symbol[i];
                break;
            }
        }

        return result;
    }
}
