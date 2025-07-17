using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHost : MonoBehaviour
{
    public static MainMenuUIManager mainMenuUIManager;
    public static MainMenuNET mainMenuNET;
    public static MainMenuDataManager mainMenuDataManager;

    public static MainMenuHost instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Physics.gravity = Vector3.zero;
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }
}
