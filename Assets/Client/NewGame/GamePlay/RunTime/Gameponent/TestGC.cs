using System.Collections;
using System.Collections.Generic;
using Game.RunTime;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestGC : GameComponent
{
    public override void Tick()
    {
        this.gameObject.transform.position += Vector3.up * Time.deltaTime;
    }

    [Button]
    private void ContinueOrPause()
    {
        Debug.Log(GameComponentSystem.Instance.IsPaused);
        if (GameComponentSystem.Instance.IsPaused)
        {
            GameComponentSystem.Instance.UnPause();
        }
        else
        {
            GameComponentSystem.Instance.Pause();
        }
    }
}
