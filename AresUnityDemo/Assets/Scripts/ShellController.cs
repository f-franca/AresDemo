using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellController : MonoBehaviour
{
    public int lifeSpan = 2;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, this.lifeSpan);
    }
}
