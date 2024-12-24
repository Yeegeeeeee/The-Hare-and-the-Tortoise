using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public void DestroyGhost()
    {
        Destroy(gameObject);
    }
}
