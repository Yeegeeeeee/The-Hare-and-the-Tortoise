using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject optionButton;
    [SerializeField] private Transform buttonGroup;
    [SerializeField] private bool isStart;

    private TMP_Text text;
    private SpriteRenderer man;
    private SpriteRenderer bird;
    private string[] rows;
    private int dialogIndex;
    private Stack<int> dialogHistory = new Stack<int>();
    private Stack<string> effectHistory = new Stack<string>();
    private Stack<bool> optionDialogHistory = new Stack<bool>();
    private bool optionTime;

    // Start is called before the first frame update
    void Start()
    {
        initialize();
        man = leftMan.GetComponent<SpriteRenderer>();
        bird = rightBird.GetComponent<SpriteRenderer>();
        text = dialog.GetComponentInChildren<TextMeshProUGUI>();
        ReadText(dialogDataFile);
        ShowDialogRow();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    private void DisableLeft()
    {
        man.color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
        left.SetActive(false);
    }

    private void EnableLeft()
    {
        man.color = Color.white;
        left.SetActive(true);
    }

    private void DisableRight()
    {
        bird.color = new Color(60 / 255f, 60 / 255f, 60 / 255f);
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
        nextButton.gameObject.SetActive(false);
        DisableLeft();
        DisableRight();
    }

    private void EnablePanel()
    {
        dialog.SetActive(true);
        nextButton.gameObject.SetActive(true);
    }

    private void initialize()
    {
        dialogIndex = 0;
        PlayerPrefs.SetFloat("focus", 0);
        PlayerPrefs.SetFloat("courage", 0);
        PlayerPrefs.SetFloat("determination", 0);
        PlayerPrefs.SetFloat("inspection", 0);
        PlayerPrefs.SetFloat("confidence", 0);
        PlayerPrefs.SetFloat("angry", 0);
        PlayerPrefs.SetFloat("coward", 0);
        PlayerPrefs.Save();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !optionTime)
        {
            ShowDialogRow();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            GoBackToPreviousState();
        }
    }

    private void UpdateText(string position, string dialogText)
    {
        position = position.ToLower();
        if (position == "left")
        {
            DisableRight();
            EnableLeft();
        }
        else if (position == "right")
        {
            DisableLeft();
            EnableRight();
        }
        text.text = dialogText;
    }

    private void GenerateButton(int _index)
    {
        string[] cell = rows[_index].Split(';');
        if (cell[0] == "&")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);
            button.GetComponentInChildren<TextMeshProUGUI>().text = cell[4];
            button.GetComponent<Button>().onClick.AddListener(delegate { OnButtonSelected(cell); });
            GenerateButton(_index + 1);
            optionTime = true;
        }

    }

    private void UpdateEffect(string item)
    {
        string[] items = item.Split('@');
        string effect = items[0].ToLower();
        float value = float.Parse(items[1]);
        PlayerPrefs.SetFloat(effect, value);
        effectHistory.Push(effect);
        PlayerPrefs.Save();
        Debug.Log("Effect: " + effect + ", value: " + value);
        CheckEffect();
    }

    private void ClearText()
    {
        text.text = "";
    }

    private void ReadText(TextAsset _textAsset)
    {
        rows = _textAsset.text.Split('\n');
    }

    private void ShowDialogRow()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            string[] cell = rows[i].Split(';');
            if (cell[0] == "#" && dialogIndex == int.Parse(cell[1]))
            {
                Debug.Log("Dialog: " + dialogIndex);
                if (dialogHistory.Count == 0 || dialogHistory.Peek() != dialogIndex)
                {
                    dialogHistory.Push(dialogIndex);
                }
                EnablePanel();
                UpdateText(cell[3], cell[4]);
                dialogIndex = int.Parse(cell[5]);
                Debug.Log("Dialog to: " + dialogIndex);
                optionTime = false;
                optionDialogHistory.Push(optionTime);
                break;
            }
            else if (cell[0] == "&" && dialogIndex == int.Parse(cell[1]))
            {
                Debug.Log("Option: " + dialogIndex);
                if (dialogHistory.Count == 0 || dialogHistory.Peek() != dialogIndex)
                {
                    dialogHistory.Push(dialogIndex);
                }
                DisablePanel();
                GenerateButton(i);
                optionTime = true;
                optionDialogHistory.Push(optionTime);
            }
            else if (cell[0] == "END" && dialogIndex == int.Parse(cell[1]))
            {
                SceneChangeManager();
            }
        }
        // Debug.Log("Current Stack: " + string.Join(", ", dialogHistory.ToArray()));
    }

    private void SceneChangeManager()
    {
        float _coward = PlayerPrefs.GetFloat("coward", 0);
        if (_coward > 0)
        {
            SceneManager.LoadScene("Giveup");
            return;
        }
        if (isStart)
        {
            SceneManager.LoadScene("Race");
        }

    }

    public void OnClickNext()
    {
        ShowDialogRow();
    }

    private void OnButtonSelected(string[] cell)
    {
        ClearButtonGroup();
        if (!string.IsNullOrWhiteSpace(cell[7]))
        {
            UpdateEffect(cell[6]);
        }
        dialogIndex = int.Parse(cell[5]);
        Debug.Log("Option to: " + dialogIndex);
        ShowDialogRow();
    }

    private void GoBackToPreviousState()
    {
        if (dialogHistory.Count > 0)
        {
            dialogHistory.Pop();
            dialogIndex = dialogHistory.Peek();
            ClearButtonGroup();
            optionDialogHistory.Pop();
            bool option = optionDialogHistory.Peek();
            Debug.Log("current state after rolling: " + option);
            if (option)
                ClearEffect();
            ShowDialogRow();
        }
        else
        {
            Debug.Log("No previous dialog state to return to.");
        }
    }

    private void ClearButtonGroup()
    {
        for (int i = 0; i < buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }

    private void CheckEffect()
    {
        float _focus = PlayerPrefs.GetFloat("focus", 0);
        float _courage = PlayerPrefs.GetFloat("courage", 0);
        float _determination = PlayerPrefs.GetFloat("determination", 0);
        float _inspection = PlayerPrefs.GetFloat("inspection", 0);
        float _confidence = PlayerPrefs.GetFloat("confidence", 0);
        float _angry = PlayerPrefs.GetFloat("angry", 0);
        float _coward = PlayerPrefs.GetFloat("coward", 0);
        Debug.Log("focus: " + _focus + " courage: " + _courage + " determination: " + _determination + " inspection: " + _inspection + " confidence: " + _confidence + " angry: " + _angry + " coward: " + _coward);
    }

    private void ClearEffect()
    {
        if (effectHistory.Count > 0)
        {
            string effect = effectHistory.Pop();
            PlayerPrefs.SetFloat(effect, 0);
            PlayerPrefs.Save();
            Debug.Log("Effect cleared: " + effect);
        }
        else
        {
            Debug.LogWarning("No effect to clear. The effectHistory stack is empty.");
        }
    }
}
