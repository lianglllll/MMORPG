using System.Collections.Generic;
using TrailsFX;
using UnityEngine;

public class UnitEffectManager : MonoBehaviour
{
    public TrailEffectProfile UnitEffectProfile;
    public TrailEffectProfile flyEffectProfile;

    public List<SkinnedMeshRenderer> skinnedMeshRenderers;
    private List<TrailEffect> trailEffects = new();

    public MeshRenderer flyMeshRenderer;
    private TrailEffect flyTrailEffect;


    public void Init()
    {
        foreach(var smr in skinnedMeshRenderers)
        {
            var te = smr.GetComponent<TrailEffect>();
            trailEffects.Add(te);
            te.enabled = false;
        }
        flyTrailEffect = flyMeshRenderer.GetComponent<TrailEffect>();
        flyTrailEffect.enabled = false;
    }

    public void StartCloneTrailFX()
    {
        foreach (var te in trailEffects)
        {
            te.enabled = true;
            te.SetProfile(UnitEffectProfile);
        }
    }
    public void StopCloneTrailFX()
    {
        foreach (var te in trailEffects)
        {
            te.enabled = false;
        }
    }

    public void StartFlyTrailFX()
    {
        flyTrailEffect.enabled = true;
        flyTrailEffect.SetProfile(flyEffectProfile);
    }
    public void StopFlyTrailFX()
    {
        flyTrailEffect.enabled = false;
    }

}
