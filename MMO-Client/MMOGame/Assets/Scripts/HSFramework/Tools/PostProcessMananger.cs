using BaseSystem.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessMananger : Singleton<PostProcessMananger>
{
    public PostProcessVolume volume;
    private ChromaticAberration chromaticAberration;
    private float value;
    private float speed = 4f;

    private void Start()
    {
        chromaticAberration = volume.profile.GetSetting<ChromaticAberration>();
    }


    /// <summary>
    /// 色差效果
    /// </summary>
    /// <param name="value"></param>
    public void ChromaticAberrationEF(float value)
    {
        StopAllCoroutines();//防止多次触发
        this.value = value;
        StartCoroutine(StartChromaticAberrationEF());
    }

    private IEnumerator StartChromaticAberrationEF()
    {
        //递增到value
        while(chromaticAberration.intensity < value)
        {
            yield return null;
            chromaticAberration.intensity.value += Time.deltaTime * speed;
        }

        //递减到0
        while (chromaticAberration.intensity > 0)
        {
            yield return null;
            chromaticAberration.intensity.value -= Time.deltaTime * speed;
        }

    }

}
