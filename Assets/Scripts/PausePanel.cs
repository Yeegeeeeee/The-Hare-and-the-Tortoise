using System;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;


public class PausePanel : MonoBehaviour
    {
        [SerializeField]
        private Button btn_continue;
        [SerializeField]
        private Button btn_quit;

        private void Awake()
        {
            btn_continue.onClick.AddListener(ContinueGame);
            btn_quit.onClick.AddListener(QuitGame);
        }
        private void ContinueGame()
        {
            FindObjectOfType<GamePanel>(true).TogglePause();
        }
        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
