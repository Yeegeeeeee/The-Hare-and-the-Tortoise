using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.Scripting;

public class DialogManager : MonoBehaviour
{
    [Header(".csv file")]
    [SerializeField] private TextAsset dialogDataFile;

    [Header("Left Sprite")]
    [SerializeField] private GameObject leftMan;
    [SerializeField] private GameObject left;

    [Header("Right Sprite")]
    [SerializeField] private GameObject rightBird;
    [SerializeField] private GameObject right;

    [Header("Conversation")]
    [SerializeField] private GameObject dialog;

    private TMP_Text text;
    private SpriteRenderer man;
    private SpriteRenderer bird;
    private string[] rows;
    private int dialogIndex;

    // Start is called before the first frame update
    void Start()
    {
        man = leftMan.GetComponent<SpriteRenderer>();
        bird = rightBird.GetComponent<SpriteRenderer>();
        text = dialog.GetComponentInChildren<TextMeshProUGUI>();
        ReadText(dialogDataFile);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText("bird", "Good Morning!");
    }

    private void DisableLeft()
    {
        man.color = Color.gray;
        left.SetActive(false);
    }

    private void EnableLeft()
    {
        man.color = Color.white;
        left.SetActive(true);
    }

    private void DisableRight()
    {
        bird.color = Color.gray;
        right.SetActive(false);
    }

    private void EnableRight()
    {
        bird.color = Color.white;
        right.SetActive(true);
    }

    private void DisablePanel()
    {
        dialog.SetActive(false);
    }

    private void EnablePanel()
    {
        dialog.SetActive(true);
    }

    public void UpdateText(string character, string dialogText)
    {
        if (character == "man")
        {
            DisableRight();
            EnableLeft();
        }
        else if (character == "bird")
        {
            DisableLeft();
            EnableRight();
        }
        text.text = dialogText;
    }

    public void ClearText()
    {
        text.text = "";
    }

    private void ReadText(TextAsset _textAsset)
    {
        rows = _textAsset.text.Split('\n');
    }

    public void ShowDialogRow()
    {
        foreach (var row in rows)
        {
            string[] cell = row.Split(',');
            if (cell[0] == "#" && int.Parse(cell[1]) == dialogIndex)
            {

            }
        }
    }
}
