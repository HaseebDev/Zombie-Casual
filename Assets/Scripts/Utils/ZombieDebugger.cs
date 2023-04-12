using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDebugger : MonoBehaviour
{
    public List<Zombie> _listZombie;

    private void Start()
    {
        for (int i = _listZombie.Count - 1; i >= 0; i--)
        {
            var zomDes = DesignHelper.GetZombieDesign(_listZombie[i].gameObject.name.Trim());
            _listZombie[i].Initialize(zomDes, 1.0f, 1.0f, 1, 1, 0);
        }
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        for (int i = _listZombie.Count - 1; i >= 0; i--)
        {
            _listZombie[i].UpdateZombie(deltaTime);
        }
    }
}
