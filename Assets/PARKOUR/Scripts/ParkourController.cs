using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] private EnvironmentChecker environmentChecker;

    private void Start()
    {
        environmentChecker = GetComponent<EnvironmentChecker>();
    }
    private void FixedUpdate()
    {
        var hitData = environmentChecker.CheckObstacle();

        if (hitData.hitFound)
        {

        }
    }
}
