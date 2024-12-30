using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private Button btn_start;
    [SerializeField] private Button btn_exit;
    [SerializeField] private Button btn_backStory;
    [SerializeField] private Button btn_next;
    [SerializeField] private Button btn_previous;
    [SerializeField] private Button btn_close;
    [SerializeField] private GameObject go_backStory;
    [SerializeField] private TextMeshProUGUI txt_story;
    [SerializeField] string[] m_storyTexts;
    private int m_storyIndex;
    private TMP_Text title;
    private void Awake()
    {
        title = GetComponentInChildren<TextMeshProUGUI>();
        btn_start.onClick.AddListener(StartGame);
        btn_exit.onClick.AddListener(ExitGame);
        btn_backStory.onClick.AddListener(ShowBackStory);
        btn_next.onClick.AddListener(() =>
        {
            m_storyIndex++;
            if (m_storyIndex >= m_storyTexts.Length)
            {
                Close();
            }
            else
            {
                ShowBackStory();
            }
        });
        btn_previous.onClick.AddListener(() =>
        {
            m_storyIndex--;
            if (m_storyIndex < 0)
            {
                m_storyIndex = 0;
            }
            ShowBackStory();
        });
        btn_close.onClick.AddListener(Close);
    }
    private void Close()
    {
        m_storyIndex = 0;
        txt_story.text = "";
        txt_story.DOKill();
        go_backStory.gameObject.SetActive(false);
        SwitchStatus(true);
    }
    private void ShowBackStory()
    {
        SwitchStatus(false);
        go_backStory.SetActive(true);
        txt_story.text = "";
        if (m_storyIndex >= 0 && m_storyIndex < m_storyTexts.Length)
        {
            string str = m_storyTexts[m_storyIndex];
            txt_story.text = str;
            txt_story.DOKill();
            Color tempColor = txt_story.color;
            tempColor.a = 0f;
            txt_story.color = tempColor;
            txt_story.DOFade(1f, 2.5f)
                .SetEase(Ease.Linear);
        }
        else
        {
            Debug.LogError($"error: {m_storyIndex} out of range");
        }
    }

    private void StartGame()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene("Conversation");
        // FindObjectOfType<DialogPanel>(true)
        //     .SetDialogConversion(0, () => SceneManager.LoadScene("Race"))
        //     .ShowDialog()
        //     .SetActive(true);
    }

    private void SwitchStatus(bool state)
    {
        title.gameObject.SetActive(state);
        btn_start.gameObject.SetActive(state);
        btn_backStory.gameObject.SetActive(state);
        btn_exit.gameObject.SetActive(state);
    }



    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}