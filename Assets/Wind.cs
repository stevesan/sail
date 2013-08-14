using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour
{
    public static Wind main;

    public Vector3 force;

    void Awake()
    {
        main = this;
    }
}
