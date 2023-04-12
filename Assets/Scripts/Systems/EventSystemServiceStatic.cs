using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBatchStatic
{
    public object target;
    public Delegate _delegate;

    public EventBatchStatic(Delegate del, object target)
    {
        this.target = target;
        this._delegate = del;
    }

    public override bool Equals(object obj)
    {
        var p = obj as EventBatchStatic;
        if(p == null)
        {
            return false;
        }

        return target == p.target && _delegate == p._delegate;
    }
}

public static class EventSystemServiceStatic
{
    //FOR DEBUG
    public static string LogTag = "EventSystemService";
    
    public static void DispatchEnum(object obj, Enum eventName, params object[] args)
    {
        Dispatch(obj, eventName.GetEventString(), args);
    }
    
    public static void AddListenerEnum(object obj, Enum eventName, Delegate action)
    {
        AddListener(obj, eventName.GetEventString(), action);
    }

    public static void DispatchAllEnum(Enum eventName, params object[] args)
    {
        DispatchAll(eventName.GetEventString(), args);
    }

    public static void RemoveListenerEnum(object obj, Enum eventName, Delegate action)
    {
        RemoveListener(obj, eventName.GetEventString(), action);
    }

    public static string GetEventString(this Enum enu)
    {
        return "E_" + enu;
    }
    
    private static Dictionary<string, List<EventBatchStatic>> eventBatchStaticDict = new Dictionary<string, List<EventBatchStatic>>();

    public static void AddListener(object obj, string eventName, Delegate action)
    {
        //if (obj == null) return;
        //EventDispatcher eventDispatcher = obj.AddMissingComponent<EventDispatcher>();
        //eventDispatcher.Add(eventName, action);
        EventBatchStatic batch = new EventBatchStatic(action, obj);
        if(!eventBatchStaticDict.TryGetValue(eventName, out var batches))
        {
            batches = new List<EventBatchStatic>();
            eventBatchStaticDict.Add(eventName, batches);
            batches.Add(batch);
        }
        else
        {
            if (batches.Contains(batch))
            {
                //if (eventName == EVENT_NAME.UPDATE_SPECIAL_CHEST)
                {
                    Debug.LogError($"Duplicate Add found event name {eventName}");
                }
                return;
            }
            batches.Add(batch);
        }
    }

    private static object cTarget;
    private static Delegate cDelegate;

    public static void RemoveListener (object obj, string eventName, Delegate action)
    {
        //if (obj == null) return;
        //EventDispatcher eventDispatcher = obj.AddMissingComponent<EventDispatcher>();
        //eventDispatcher.Remove(eventName, action);
        cTarget = obj;
        cDelegate = action;
        List<EventBatchStatic> batches;
        if (!eventBatchStaticDict.TryGetValue(eventName, out batches))
        {
            return;
        }

        var bathToDEL = batches.Find(isMatchToRemove);
        batches.Remove(bathToDEL);
    }

    public static void DispatchAll(string eventName, params object[] args)
    {
        List<EventBatchStatic> batchesOut;

        if(eventBatchStaticDict.TryGetValue(eventName, out batchesOut))
        {
            for(int i = batchesOut.Count - 1; i >= 0; i--)
            {
                if(batchesOut[i]._delegate != null)
                {
                    //batchesOut[i]._delegate.DynamicInvoke(args);
                    batchesOut[i]._delegate.Method.Invoke(batchesOut[i]._delegate.Target, args);
                }
            }
        }
    }

    public static void Dispatch(object obj, string eventName, params object[] args)
    {
        //if (obj == null) return;
        //EventDispatcher eventDispatcher = obj.AddMissingComponent<EventDispatcher>();
        //eventDispatcher.Trigger(eventName, args);
        List<EventBatchStatic> batchesOut;

        if(eventBatchStaticDict.TryGetValue(eventName, out batchesOut))
        {
            for(int i = batchesOut.Count - 1; i >= 0; i--)
            {
                var count = batchesOut.Count;
                if(i < count && batchesOut[i]._delegate != null)
                {
                    if(batchesOut[i].target == null || batchesOut[i].target == obj)
                    {
                        //batchesOut[i]._delegate.DynamicInvoke(args);
                        batchesOut[i]._delegate.Method.Invoke(batchesOut[i]._delegate.Target, args);
                    }
                        
                }
            }
        }
    }

    private static bool isMatchToRemove(EventBatchStatic b)
    {
        return cTarget == b.target && cDelegate == b._delegate;
    }

    private static bool isMatchToRemoveAll(EventBatchStatic b)
    {
        return true;
    }

    public static string GetName()
    {
        return "Event System Service";
    }

    public static string GetId()
    {
        return "namnh.service.eventsystem";
    }

    public static void RemoveAll(string[] listKeys)
    {
        for(int i = 0; i < listKeys.Length; i++)
        {
            var eventName = listKeys[i];
            List<EventBatchStatic> batches;
            if(!eventBatchStaticDict.TryGetValue(eventName, out batches))
            {
                continue;
            }

            batches.Clear();
            //batches.RemoveWhere(isMatchToRemoveAll);
        }
    }

}