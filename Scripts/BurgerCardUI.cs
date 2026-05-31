using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BurgerCardUI : MonoBehaviour
{
    public TextMeshProUGUI burgerNameText;
    public TextMeshProUGUI ingredientsText;
    public TextMeshProUGUI scoreText;
    public Image BurgerImage;

    public void SetData(BurgerData data)
    {
        burgerNameText.text = data.name;
        int half = Mathf.CeilToInt(data.ingredients.Count / 2f);
        List<string> leftList = new List<string>();
        List<string> rightList = new List<string>();

        for (int i = 0; i < data.ingredients.Count; i++)
        {
            // 後ろから入れる：最初の half 個が右、残りが左
            if (i < data.ingredients.Count - half)
                rightList.Add("・" + data.ingredients[i]);
            else
                leftList.Add("・" + data.ingredients[i]);
        }

        // 行ごとに左右合成
        string combined = "";
        int maxRows = Mathf.Max(leftList.Count, rightList.Count);
        for (int i = 0; i < maxRows; i++)
        {
            string left = i < leftList.Count ? leftList[i] : "";
            string right = i < rightList.Count ? rightList[i] : "";
            combined += left.PadRight(10) + right + "\n"; // PadRightで左列を揃える
        }
        ingredientsText.text = combined.TrimEnd('\n');
        scoreText.text = $"Score : {data.score}";
        BurgerImage.sprite = data.image;
    }
}
