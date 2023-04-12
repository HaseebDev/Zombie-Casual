using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuickEngine.Common;
using UnityEngine;

public class BackButtonManager : Singleton<BackButtonManager>
{
    private Stack<BackButtonListener> _Listeners = new Stack<BackButtonListener>();

    public void Push(BackButtonListener listener)
    {
        _Listeners.Push(listener);
        // Debug.LogError("Push " + listener.gameObject.name);
    }

    public void Pop(BackButtonListener listener)
    {
        if (_Listeners.Count > 0)
        {
            var peek = _Listeners.Peek();
            if (peek == listener)
            {
                _Listeners.Pop();
                // Debug.LogError("Pop " + listener.gameObject.name);
            }
            else
            {
                _Listeners.Remove(listener);
            }
        }
    }

    public bool HasPopup()
    {
        return _Listeners.Count > 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Debug.LogError("ON BACK");
            if (_Listeners.Count > 0)
            {
                var peek = _Listeners.Peek();
                // Debug.LogError("Trigger " + peek.gameObject.name);
                peek.OnBack();
            }
        }
    }

    public void ShowCanGoBackText(bool useTopcanvas = false)
    {
        string notifyText = "You can't go back from this screen";
        if (useTopcanvas)
        {
            TopLayerCanvas.instance.ShowFloatingTextNotify(notifyText, toUpper: false);
        }
        else
        {
            MasterCanvas.CurrentMasterCanvas?.ShowFloatingTextNotify(notifyText, toUpper: false);
        }
    }

    public void ClearAll()
    {
        _Listeners.Clear();
    }
}