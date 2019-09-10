﻿
// This is basically a big wrapper for an enum called gameStates.
//      StateController.getState();
//      StateController.setState(gameStates.yourstatehere);

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    private static gameStates state = gameStates.normal;
    public static Region activeRegion;

    // https://gamedev.stackexchange.com/a/116010 singleton pattern.
    private static StateController _instance;
    private static StateController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {Destroy(this.gameObject);}
        else
        {_instance = this;}

        Object.DontDestroyOnLoad(gameObject);
        setState(gameStates.normal);
    }



    public static void setState(gameStates state)
    {
        Debug.Log("Game state changed to "+state);
        StateController.state = state;
    }



    public static gameStates getState()
    {
        return StateController.state;
    }
}

// Menu: In the main menu of the game.
// Paused: Game is paused.
// Normal: Explorer can move around and do stuff.
// Fullmap: We are in fullscreen map mode; no movement allowed.
// Guiding: TODO not sure about this one.
public enum gameStates { menu, paused, normal, fullmap, guiding };