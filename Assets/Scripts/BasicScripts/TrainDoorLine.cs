using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainDoorLine : DetectLine
{
    protected override void Start()
    {
        base.Start();
        id = 10;
        isDetected = false;
    }

    protected override void Update()
    {
        base.Update();
        if (isDetected)
        {
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
