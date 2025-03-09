using System;
using UnityEngine;

public class DeathPose : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
