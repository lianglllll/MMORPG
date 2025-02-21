using System.Collections.Generic;
using TrailsFX;
using UnityEngine;

public class UnitEffectManager : MonoBehaviour
{
    public List<SkinnedMeshRenderer> skinnedMeshRenderers;
    private List<TrailEffect> trailEffects = new();
    public TrailEffectProfile effectProfile;
    public void Init()
    {
        foreach(var smr in skinnedMeshRenderers)
        {
            var te = smr.GetComponent<TrailEffect>();
            trailEffects.Add(te);
            te.enabled = false;
        }
    }

    public void StartCloneTrailFX()
    {
        foreach (var te in trailEffects)
        {
            te.enabled = true;
            te.SetProfile(effectProfile);
        }
    }

    public void StopCloneTrailFX()
    {
        foreach (var te in trailEffects)
        {
            te.enabled = false;
        }
    }

}
