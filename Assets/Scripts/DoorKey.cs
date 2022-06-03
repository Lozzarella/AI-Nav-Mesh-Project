using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : MonoBehaviour
{
    [SerializeField] SlidingDoors _doorToUnlock;

    public void UnlockDoor()
    {
        if (_doorToUnlock == null) return;

        _doorToUnlock.IsLocked = false;
    }

    public void LockDoor()
    {
        if (_doorToUnlock == null) return;

        _doorToUnlock.IsLocked = true;
    }
}
