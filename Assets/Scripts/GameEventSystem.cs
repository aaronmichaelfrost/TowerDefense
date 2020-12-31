using UnityEngine;
using System;
using UnityEngine.Events;

public class GameEventSystem : MonoBehaviour
{

    // Make singleton instance
    public static GameEventSystem current = null;

    private void Awake()
    {
        if (current)
            Destroy(this.gameObject);

        current = this;
    }


    public UnityEvent onPauseGame;

    public UnityEvent onUnpauseGame;

}
