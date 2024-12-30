using System;
using UnityEngine;
using UnityEngine.UI;
public class GamePanel : MonoBehaviour
{
    [SerializeField] private Sprite m_pause;
    [SerializeField] private Sprite m_play;
    [SerializeField] private Button btn_pause;
    private bool m_isPause;
    private bool m_isShowEndDialog;
    private void Awake()
    {
        m_isPause = false;
        m_isShowEndDialog = false;
        btn_pause.onClick.AddListener(TogglePause);
    }
    private void Update()
    {
        // if (!m_isShowEndDialog&&Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     FindObjectOfType<DialogPanel>(true)
        //         .SetDialogConversion(1)
        //         .ShowDialog()
        //         .SetActive(true);
        //     m_isShowEndDialog = true;
        // }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    public void TogglePause()
    {
        if (!m_isPause)
        {
            Time.timeScale = 0f;
            btn_pause.image.sprite = m_play;
            FindObjectOfType<PausePanel>(true).gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            btn_pause.image.sprite = m_pause;
            FindObjectOfType<PausePanel>(true).gameObject.SetActive(false);
        }
        m_isPause = !m_isPause;
    }
}