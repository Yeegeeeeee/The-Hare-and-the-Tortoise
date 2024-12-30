using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

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
    [Header("Conversation Info")]
    [SerializeField] private bool isStart;
    [SerializeField] private bool isDead;
    [SerializeField] private bool isWin;
    [SerializeField] private bool isLost;
    [SerializeField] private float textSpeed = 0.05f;

    private TMP_Text text;
    private SpriteRenderer man;
    private SpriteRenderer bird;
    private string[] rows;
    private int dialogIndex;
    private Stack<int> dialogHistory = new Stack<int>();
    private Stack<string> effectHistory = new Stack<string>();
    private Stack<bool> optionDialogHistory = new Stack<bool>();
    private bool optionTime;
    private bool isMouseClicked;
    private Coroutine currentCoroutine;
    private bool retry;
    private bool redo;
    private bool quit;

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

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
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

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && dialogHistory.Count > 1 && optionDialogHistory.Count > 1)
        {
            GoBackToPreviousState();
        }
        if (Input.GetMouseButtonDown(0))
        {
            isMouseClicked = true;
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
        currentCoroutine = StartCoroutine(ShowTextCharacterByCharacter(dialogText));
    }


    private IEnumerator ShowTextCharacterByCharacter(string dialogText)
    {
        isMouseClicked = false;
        nextButton.gameObject.SetActive(false);
        ClearText();

        foreach (char dialogChar in dialogText)
        {
            if (isMouseClicked)
            {
                text.text = dialogText;
                nextButton.gameObject.SetActive(true);
                yield break;
            }


            text.text += dialogChar;
            yield return new WaitForSeconds(textSpeed);
        }

        nextButton.gameObject.SetActive(true);
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
        if (isStart)
            CheckEffect();
        else
        {
            CheckPrefs();
        }
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
        StopCurrentCoroutine();
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
        DisplayStack(dialogHistory, "dialogHistory");
        // DisplayStack(optionDialogHistory, "optionDialogHistory");
    }

    private void SceneChangeManager()
    {
        float _coward = PlayerPrefs.GetFloat("coward", 0);
        if (_coward > 0)
        {
            ClearCoward();
            SceneManager.LoadScene("Giveup");
            return;
        }
        if (isStart)
        {
            SceneManager.LoadScene("Race");
            return;
        }
        if (isDead)
        {
            SceneManager.LoadScene("AfterLife");
            return;
        }
        if (retry)
        {
            SceneManager.LoadScene("RetryConversation");
            return;
        }
        if (quit)
        {
            SceneManager.LoadScene("MainMenu");
            return;
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
        StopCurrentCoroutine();
        ClearText();
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

    private void DisplayStack<T>(Stack<T> stack, string stackName)
    {
        if (stack.Count == 0)
        {
            Debug.Log($"{stackName} is empty.");
            return;
        }

        string stackContent = $"{stackName} (top to bottom): ";
        foreach (T item in stack)
        {
            stackContent += item + " -> ";
        }

        stackContent = stackContent.TrimEnd(' ', '-', '>');
        Debug.Log(stackContent);
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

    private void CheckPrefs()
    {
        float _retry = PlayerPrefs.GetFloat("retry", 0);
        float _rest = PlayerPrefs.GetFloat("rest", 0);
        float _redo = PlayerPrefs.GetFloat("redo", 0);
        if (_retry != 0)
        {
            retry = true;
        }
        if (_rest != 0)
        {
            quit = true;
        }
        if (_redo != 0)
        {
            redo = true;
        }
        PlayerPrefs.SetFloat("retry", 0);
        PlayerPrefs.SetFloat("rest", 0);
        PlayerPrefs.SetFloat("redo", 0);
        PlayerPrefs.Save();
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

    private void ClearCoward()
    {
        PlayerPrefs.SetFloat("coward", 0);
        PlayerPrefs.Save();
    }
}
