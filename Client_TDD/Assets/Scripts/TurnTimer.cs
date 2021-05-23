using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Zenject;

//you can extract more generic form of this class for monobehaviour timer (move it outside room then)  //timer is a progress bar
public class TurnTimer : MonoBehaviour
{
    private const int HandTime = 8000;
    private const int HandTimeStep = 100;

    [SerializeField] private Image timerImage;

    public UniTaskTimer uniTaskTimer { get; private set; }

    private void Start()
    {
        uniTaskTimer = new UniTaskTimer(HandTime, HandTimeStep, ticked: UpdateRemainingTimeText);
    }

    private void UpdateRemainingTimeText(float progress)
    {
        timerImage.fillAmount = progress;
    }
}