// Assuming you have a Text or TextMeshProUGUI component for displaying the gold
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    public void UpdateGoldText(int currentGold)
    {
        goldText.text = currentGold.ToString();
    }
}
