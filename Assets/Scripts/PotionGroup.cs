using UnityEngine;

public class PotionGroup : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private GameObject fake;
    [SerializeField] private GameObject real;
    [SerializeField] private Transform initiateSpot;

    void Start()
    {
        CheckInspection();
    }

    private void ShowRegular()
    {
        GameObject obj = Instantiate(fake, transform.position, Quaternion.identity);
        GameObject realObj = Instantiate(real, initiateSpot.position, Quaternion.identity);
        Potions fakePotion = obj.GetComponent<Potions>();
        Potions realPotion = realObj.GetComponent<Potions>();
        if (fakePotion != null && realPotion != null)
        {
            int _id = realPotion.GetId();
            int fakeId = fakePotion.GetId();
            Debug.Log($"change from fake id {fakeId} to real id: {_id}");
            fakePotion.SetId(_id);
        }
        Destroy(realObj);
    }

    private void ShowReal()
    {
        Instantiate(real, transform.position, Quaternion.identity);
    }

    private void CheckInspection()
    {
        float inspection = PlayerPrefs.GetFloat("inspection", 0);
        Debug.Log("Inspection: " + inspection);
        if (inspection != 0)
        {
            ShowReal();
        }
        else
        {
            ShowRegular();
        }
    }
}