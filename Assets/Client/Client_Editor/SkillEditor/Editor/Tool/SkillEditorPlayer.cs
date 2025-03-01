using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillEditorPlayer : SerializedMonoBehaviour
{
    private bool isPlaying = false;
    public bool IsPlaying => IsPlaying;

    private SkillClip skillClip;
    private int currentFrameIndex;
    private float playerTotalTime;
    private int frameRote;

    private Transform modelTransform;
    public Transform ModelTransform => modelTransform;

    private List<SkillColliderEvent> curColliderList = new List<SkillColliderEvent>();

    [SerializeField] private Dictionary<string, GameObject> weaponDic = new ();
    public Dictionary<string, GameObject> WeaponDic => weaponDic;

    private void OnDrawGizmos()
    {
        if (curColliderList.Count != 0)
        {
            for (int i = 0; i < curColliderList.Count; i++)
            {
                SkillGizmosTool.DrawDetection(curColliderList[i], this);
            }
        }
    }
}