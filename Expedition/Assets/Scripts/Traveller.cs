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
    [SerializeField] private bool isReal = true;

    [SerializeField] private TravellerType type;
    [SerializeField] private float _reachDestinationRadius;
    public Quest QuestToComplete { get; private set; }
    #endregion

    #region [Private]
    private NavMeshAgent _Agent;
    private GameObject _Model;
    private List<Vector3> _Path;
    private Vector3 _lastReachedPosition;
    private bool _isRunningPath;
    private int _pathIndex;
    private bool _isLost;
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
        // if within radius of destination.
        if( _isRunningPath && Vector3.Distance(transform.position, _Path[_pathIndex]) < _reachDestinationRadius) {
            if(_pathIndex < _Path.Count-1) {
                _lastReachedPosition = _Path[_pathIndex];
                _pathIndex++;
                ValidateAndSetDestination(_Path[_pathIndex]);
            }
            else _isRunningPath = false;
        }
        Debug.DrawLine(transform.position, _Agent.destination, Color.yellow, 0.1f);

        // if is lost
            // do animation / UI/UX stuff
    }
    #endregion

    #region [Methods]
    // Check if the traveller can figure out how to reach a given spot.
    public void ValidateAndSetDestination(Vector3 pos) {
        _isLost = false;
        GetComponent<Collider>().enabled = false;
        Vector3 preCheckPos = transform.position;
        Vector3 travellerHeight = new Vector3(0, Expedition.travellerLOSHeight, 0);

        // The traveller turns off their collider and instantly runs over to the destination, if possible.
        if (_Agent.Warp(pos)) {
            _Agent.Warp(preCheckPos);
            GetComponent<Collider>().enabled = true;
            Debug.Log("trav passed pathing check");
        }
        // if not possible;
        else {
            _isLost = true;
            _isRunningPath = false;
            Debug.Log("trav failed pathing check");
            return;
        }

        // if the warp was successful, the traveller checks for any obstructions between the
        if ( !Physics.Linecast(_lastReachedPosition + travellerHeight, pos + travellerHeight, out RaycastHit hit, Expedition.travellerLOSMask) ) {
            Debug.DrawLine(_lastReachedPosition + travellerHeight, pos + travellerHeight, Color.white, 30f, false);
            Debug.Log("trav passed LOS check");
        }
        else {
            Debug.DrawLine(_lastReachedPosition + travellerHeight, hit.point, Color.red, 30f, false);
            _isLost = true;
            _isRunningPath = false;
            Debug.Log("trav failed LOS check");
            return;
        }
        _Agent.SetDestination(pos);
    }

    // Find a traveller of the given type and instantiate a random one of its variants at given position.
    public static Traveller InstantiateTraveller(TravellerType type, Vector3 pos) {
        foreach(TravellerPrefab pref in Expedition.Travellers) {
            if(pref.type == type && pref.Variants != null && pref.Variants.Count > 0) {
                return GameObject.Instantiate(pref.Variants[UnityEngine.Random.Range(0, pref.Variants.Count - 1)], pos, Quaternion.identity).GetComponent<Traveller>();
            }
        }
        throw new Exception("Could not find traveller of that type. Check to make sure one is assigned");
    }
    /*
    public static void DestroyAllTravellers() {
        foreach(Traveller t in AllActiveTravellers) {
            Destroy(t.gameObject);
        }
        AllActiveTravellers = new List<Traveller>();
    }
    */
    public static void DestroyActive() {
        if(Active == null) return;
        Destroy(Active.gameObject);
        Active = null;
    }
    /*
    public static Traveller FindExistingTraveller(TravellerType type) {
        foreach(Traveller t in AllActiveTravellers) {
            if(t.type == type && t.gameObject.activeSelf) {
                return t;
            }
        }
        return null;
    }
    */
    public void GivePath(List<LineVertex> path, Quest questBeingRan) {
        if(path.Count > 0) {
            if(questBeingRan == null) throw new Exception("Can't give traveller a path without a quest");
            if(questBeingRan.state != QuestState.completeable) throw new Exception("Traveller cannot run a quest that is not in state 'completeable'!");
            QuestToComplete = questBeingRan;

            // convert vertex list into a list of basic vec3's
            var convertedPath = new List<Vector3>();
            foreach(LineVertex v in path) {
                convertedPath.Add(v.transform.position);
            }
            _Path = convertedPath;
        }
        else throw new Exception("Can't give traveller a zero-length path");
    }

    public static void SetActive(Traveller t) {
        if(t == null) throw new Exception("Cannot set Traveller as active, no traveller provided");
        Active = t;
    }

    public void RunPath() {
        if(_Path == null) return;
        _lastReachedPosition = transform.position;
        _isRunningPath = true;
        _pathIndex = 1; // start at one because he's already at the starting point, which is always where vert 0 is.
        ValidateAndSetDestination(_Path[_pathIndex]);
    }


    #endregion
}
