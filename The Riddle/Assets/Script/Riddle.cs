using TMPro;
using UnityEngine;

public class Riddle : MonoBehaviour
{
    public enum OptionType
    {
        Correct,
        Wrong
    }

    [Header("Option Type")]
    public OptionType option;

    [Header("UI")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        resultPanel.SetActive(true);

        if (option == OptionType.Correct)
        {
            resultText.text = "Start Next Chapter";
        }
        else
        {
            resultText.text = "Greed makes people blind";
        }
    }
}