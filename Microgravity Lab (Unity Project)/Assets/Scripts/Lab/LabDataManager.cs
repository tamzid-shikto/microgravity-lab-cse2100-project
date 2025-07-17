using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabDataManager : MonoBehaviour
{
    public IChangableData<bool> _simulationRunningState = new IChangableData<bool>(true);

    public IChangableData<int> _currentTool = new IChangableData<int>(0);
    public IChangableData<int> _currentToolUI = new IChangableData<int>(0);
    public Dictionary<string, LabData.LabObjectPoolMember> _labObjectPool = new Dictionary<string, LabData.LabObjectPoolMember>();
    public IChangableData<int> _activeExperimenterIndex = new IChangableData<int>();
    public IChangableData<bool> isSelfHost = new IChangableData<bool>();

    public IChangableData<int> modifiedTick = new IChangableData<int>();
    public Action<string> OnBasicNotify;

    public Action OnRoomJoinDone;

    public List<SO_LabObject> so_LabObjects = new List<SO_LabObject>();
    Dictionary<string, SO_LabObject> dict_so_objects = new Dictionary<string, SO_LabObject>();

    public IChangableData<KeyCode> DEVTOOLS_KEYDOWN = new IChangableData<KeyCode>();

    private void Awake()
    {
        LabHost.labDataManager = this;
    }

    private void Start()
    {
        ScheduleCallback();

        foreach(var so in so_LabObjects) dict_so_objects.Add(so.objectNID, so);
    }

    void ScheduleCallback()
    {
        /*LabHost.labInputManager.OnToolSwitch += (int index) =>
        {
            currentTool.SetData(index);
        };
        simulationRunningState.OnChange += (bool _) =>
        {
            currentTool.TriggerCallback();
        };*/
    }

    public SO_LabObject GetObjectSOByNID(string nid)
    {
        SO_LabObject so;
        if(dict_so_objects.TryGetValue(nid, out so)) return so;
        return null;
    }
}

namespace LabData
{
    public class LabObjectPoolMember
    {
        public string objectID;
        public string prefabNID;
        public Rigidbody rb;
    }
}
