using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Traveller : MonoBehaviour
{
    #region [Public]
    /// <summary> The first traveller that was instantiated. </summary>
    public static Traveller Active { get; private set; }

    // This list is here so that we can have multiple travellers
    /// <summary> All travellers that have been instantiated in the game. </summary>
    public static List<Traveller> AllActiveTravellers =  new List<Traveller>();

    /// <summary> If this traveller is 'real' or just for validating redlines. </summary>
    [SerializeField] private bool isReal;

    [SerializeField] private TravellerType type;
    #endregion

    #region [Private]
    private NavMeshAgent _Agent;
    private GameObject _Model;
    private List<LineVertex> _RedLineToFollow;
    private Quest _QuestToComplete;
    private bool _isRunningPath;
    #endregion




    #region [Events]
    private void OnEnable() {
        _Agent = GetComponent<NavMeshAgent>();
        _Model = transform.Find("Model").gameObject;
        AllActiveTravellers.Add(this);
        if(!isReal) _Model.SetActive(false);
    }

    private void OnDestroy() {
        AllActiveTravellers.Remove(this);
    }

    private void Update() {
        
    }
    #endregion

    #region [Methods]
    // Make this traveller invisibly check if it can make it to a given position.
    public bool ValidatePathPoint(Vector3 pos) {
        isReal = false;
        return _Agent.Warp(pos);
    }

    // Find a traveller of the given type and instantiate a random one of its variants at given position.
    public static Traveller InstantiateTraveller(TravellerType type, Vector3 pos) {
        foreach(TravellerPrefab pref in Expedition.Travellers) {
            if(pref.type == type && pref.Variants != null && pref.Variants.Count > 0) {
                return GameObject.Instantiate(pref.Variants[UnityEngine.Random.Range(0, pref.Variants.Count - 1)]).GetComponent<Traveller>();
            }
        }
        throw new Exception("Could not find traveller of that type. Check to make sure one is assigned");
    }

    public static void DestroyAllTravellers() {
        foreach(Traveller t in AllActiveTravellers) {
            Destroy(t.gameObject);
        }
        AllActiveTravellers = new List<Traveller>();
    }

    public static Traveller FindExistingTraveller(TravellerType type) {
        foreach(Traveller t in AllActiveTravellers) {
            if(t.type == type && t.gameObject.activeSelf) {
                return t;
            }
        }
        return null;
    }

    public void GivePath(List<LineVertex> path, Quest questBeingRan) {
        if(path.Count > 0) {
            if(questBeingRan == null) throw new Exception("Can't give traveller a path without a quest");
            if(questBeingRan.state != QuestState.completeable) throw new Exception("Traveller cannot run a quest that is not in state 'completeable'!");
            _QuestToComplete = questBeingRan;
            _RedLineToFollow = path;
        }
        else throw new Exception("Can't give traveller a zero-length path");
    }

    public static void SetActive(Traveller t) {
        if(t == null) throw new Exception("Cannot set Traveller as active, no traveller provided");
        Active = t;
    }

    public void RunPath() {
        if(_RedLineToFollow == null) return;
        _isRunningPath = true;
    }


    #endregion
}
