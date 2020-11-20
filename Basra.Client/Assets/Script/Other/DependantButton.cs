using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public abstract class DependantButton : DependantElemenet
{
    protected abstract bool ActivateCondition { get; }

    Coroutine ActivateCoroutine;

    [SerializeField] GameObject LoadingIcon;
    GameObject LoadingIconInstance;

    void OnEnable()
    {
        if (!ActivateCondition)
            ActivateCoroutine = StartCoroutine(Activate());
    }

    void OnDisable()
    {
        if (LoadingIconInstance != null)
            Destroy(LoadingIconInstance);
        if (ActivateCoroutine != null)
            StopCoroutine(ActivateCoroutine);
    }

    IEnumerator Activate()
    {
        LoadingIconInstance = Instantiate(LoadingIcon, transform);
        LoadingIconInstance.transform.DOBlendableLocalRotateBy(Vector3.forward * 180, 1f).SetLoops(int.MaxValue);
        GetComponent<Button>().interactable = false;

        while (!ActivateCondition) yield return new WaitForFixedUpdate();

        Destroy(LoadingIconInstance);
        GetComponent<Button>().interactable = true;
    }

}
