using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour
{
     private enum E_DialogUIState
    {
        None,
        Showing,
        Pause,
        End,
    }

    [SerializeField] private DialogConversion[] m_levelDialogConversion;
    [SerializeField] private TextMeshProUGUI txt_name;
    [SerializeField] private TextMeshProUGUI txt_content;
    [SerializeField] private Image img_character_0;
    [SerializeField] private Image img_character_1;
    [SerializeField] private Button btn_next;
    private const string c_player_0_Name = "Turtle";
    private const string c_player_1_Name = "Bird";
    private E_DialogUIState DialogUIState { get; set; }
    private DialogConversion m_curDialogConversion;
    private Action m_callback;

    private void Awake()
    {
        DialogUIState = E_DialogUIState.None;
        btn_next.onClick.AddListener(SkipCurStory);
    }
    private void OnDisable()
    {
        if (m_callback != null)
        {
            m_callback.Invoke();
            m_callback = null;
        }
        ClearDialog();
    }


    private void OnDestroy()
    {
        txt_content.DOKill();
        img_character_0.DOKill();
        img_character_1.DOKill();
    }

    private void SkipCurStory()
    {
        switch (DialogUIState)
        {
            case E_DialogUIState.Showing:
                txt_content.DOKill();
                img_character_0.DOKill();
                img_character_1.DOKill();
                ShowDialog();
                break;
            case E_DialogUIState.Pause:
                ShowDialog();
                break;
            case E_DialogUIState.End:
                if (m_callback != null)
                {
                    m_callback.Invoke();
                    m_callback = null;
                }
                gameObject.SetActive(false);
                break;
        }
    }

    public DialogPanel SetDialogConversion(int _levelIndex, Action _callback = null)
    {
        m_curDialogConversion = m_levelDialogConversion[_levelIndex];
        m_curDialogConversion.Init();
        m_curDialogConversion.ResetStory();
        m_callback = _callback;
        return this;
    }
    public DialogPanel ShowDialog()
    {
        if (m_curDialogConversion != null)
        {
            DialogData dialogData = m_curDialogConversion.ContinueStory();
            if (dialogData != null)
            {
                DialogUIState = E_DialogUIState.Showing;
                txt_name.text = dialogData.name;
                img_character_0.DOKill();
                img_character_1.DOKill();
                switch (dialogData.name)
                {
                    case c_player_0_Name:
                        img_character_0.DOColor(Color.white, 0.5f)
                            .SetEase(Ease.Linear)
                            .SetUpdate(true);
                        img_character_1.DOColor(Color.black, 0.5f)
                            .SetEase(Ease.Linear)
                            .SetUpdate(true);
                        break;
                    case c_player_1_Name:
                        img_character_0.DOColor(Color.black, 0.5f)
                            .SetEase(Ease.Linear)
                            .SetUpdate(true);
                        img_character_1.DOColor(Color.white, 0.5f)
                            .SetEase(Ease.Linear)
                            .SetUpdate(true);
                        break;
                    default:
                        img_character_0.DOColor(Color.black, 0.5f)
                            .SetEase(Ease.Linear)
                            .SetUpdate(true);
                        img_character_1.DOColor(Color.black, 0.5f)
                            .SetEase(Ease.Linear)
                            .SetUpdate(true);
                        break;
                }
                txt_content.text = "";
                txt_content.DOKill();
                txt_content.DOText(dialogData.content, 2.5f).OnComplete(() =>
                    {
                        txt_content.text = dialogData.content;
                        DialogUIState = E_DialogUIState.Pause;
                    })
                    .OnKill(() => txt_content.text = dialogData.content)
                    .SetUpdate(true)
                    .SetEase(Ease.Linear);
            }
            else
            {
                DialogUIState = E_DialogUIState.End;
                if (m_callback != null)
                {
                    m_callback.Invoke();
                    m_callback = null;
                }
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("请先设置对话内容");
        }

        return this;
    }

    public DialogPanel SetActive(bool _active)
    {
        gameObject.SetActive(_active);
        return this;
    }
    private void ClearDialog()
    {
        m_curDialogConversion = null;
        txt_name.text = "";
        txt_content.DOKill();
        img_character_0.DOKill();
        img_character_1.DOKill();
        txt_content.text = "";
        m_callback = null;
    }
}