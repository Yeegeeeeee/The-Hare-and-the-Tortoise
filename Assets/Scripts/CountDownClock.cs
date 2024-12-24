using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownClock : MonoBehaviour
{
    [SerializeField] private MainPlayer player;
    [SerializeField] private Bird component;
    private TMP_Text text;
    [SerializeField] private int start = 3;
    [SerializeField] private MainCamera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        GameObject canvas = transform.GetChild(0).gameObject;
        text = canvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int currentTime = start;
        if (currentTime == 3)
        {
            enable(currentTime);
            text.text = "Ready!";
            yield return new WaitForSeconds(1);
        }
        mainCamera.StartMovingCamera();
        while (currentTime > 0)
        {
            SetText(currentTime);
            yield return new WaitForSeconds(1);
            currentTime--;
        }

        SetText(0);
    }

    private void SetText(int num)
    {
        enable(num);
        text.text = num.ToString();
    }

    private void enable(int num)
    {
        if (num == 0)
        {
            SetAllowMoving(true);
            SelfDestruction();
            return;
        }
    }

    private void SetAllowMoving(bool allow)
    {
        player.SetAllowMoving(allow);
        component.SetAllowMoving(allow);
    }

    private void SelfDestruction()
    {
        Destroy(gameObject);
    }
}
