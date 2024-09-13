using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    [SerializeField] private AnimationCurve showCurve;
    [SerializeField] private AnimationCurve hideCurve;
    [SerializeField] private float animationSpeed;
    private Action _hideEndAction;
    IEnumerator ShowPanel(GameObject _gameObject)
    {
        float timer = 0f;
        while(timer <= 1)
        {
            _gameObject.transform.localScale = Vector3.one * showCurve.Evaluate(timer);
            timer += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.localScale = Vector3.one;

    }

    IEnumerator HidePanel(GameObject _gameObject)
    {
        float timer = 0f;
        while (timer <= 1)
        {
            _gameObject.transform.localScale = Vector3.one * hideCurve.Evaluate(timer);
            timer += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.localScale = Vector3.zero;
        _hideEndAction?.Invoke();
    }

    public void Show(GameObject _gameObject)
    {
        StartCoroutine(ShowPanel(_gameObject));
    }

    public void Hide(GameObject _gameObject, Action hideEndAction)
    {
        StartCoroutine(HidePanel(_gameObject));
        this._hideEndAction = hideEndAction;
    }
}
