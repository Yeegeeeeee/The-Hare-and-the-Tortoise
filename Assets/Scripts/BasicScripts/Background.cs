using Unity.VisualScripting;
using UnityEngine;

public class Background : MonoBehaviour
{
    private GameObject cam;
    private float length;
    private Vector3 startPosition;
    [SerializeField] private float yOffset;
    [SerializeField] private float smooth = 0.1f;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
        startPosition = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        Vector3 newPosition = new Vector3(cam.transform.position.x, cam.transform.position.y + yOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, smooth);

        if (cam.transform.position.x - transform.position.x >= length)
            startPosition.x += length * 3;
        else if (transform.position.x - cam.transform.position.x >= length)
            startPosition.x -= length * 3;
    }
}
