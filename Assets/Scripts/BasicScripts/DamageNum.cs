using TMPro;
using UnityEngine;

public class DamageNum : MonoBehaviour
{

    public float offsetX;

    public float offsetY;
    private TMP_Text text;

    private Entity target;

    private bool isPlaying = false;

    private float posY;

    public float fontSize;

    public void Initialize(Entity entity)
    {
        GameObject canvas = transform.GetChild(0).gameObject;
        text = canvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        transform.localScale = Vector3.one;
        posY = entity.transform.position.y + offsetY;
        target = entity;
        text.fontSize = fontSize;
    }

    public void Play()
    {
        isPlaying = true;
    }

    void Update()
    {
        if (target == null)
            PlayAnimationOver();
        if (isPlaying)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.2f, 0.2f, 0.2f), Time.deltaTime * 3f);
            transform.position = new Vector3(target.transform.position.x + offsetX, posY += Time.deltaTime * 0.6f, target.transform.position.z);
            if (transform.localScale.x <= 0.25f)
                PlayAnimationOver();
        }
    }

    public string GetText()
    {
        return text.text;
    }

    public void SetDamage(float damage)
    {
        text.text = damage.ToString();
    }

    public void PlayAnimationOver()
    {
        Destroy(gameObject);
    }
}
