﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("Config")] 
    [SerializeField] LoadingSequence _startingSequence;
    [SerializeField] float _nextTipButtonFadeInTime;

    [Header("Elements")]
    [SerializeField] Image _loadingBar;
    [SerializeField] TMP_Text _loadingTip;
    [SerializeField] Button _nextTipButton;

    [Header("References")] 
    [SerializeField] MainMenu _mainMenu;
    [SerializeField] RandomImageSpawner _emojiSpawner;

    private float _timeLastTipShown;
    private readonly Queue<LoadingSequence.LoadBarConfig> _loadBarQueue = new Queue<LoadingSequence.LoadBarConfig>();
    private TweenerCore<float, float, FloatOptions> _fillTween;

    protected void Start()
    {
        _loadingBar.fillAmount = 0;

        _mainMenu.gameObject.SetActive(true);
        _mainMenu.OnPlayClicked += () =>
        {
            _mainMenu.gameObject.SetActive(false);
            StartSequence(_startingSequence);
        };
    }

    public void OnNextTip()
    {
        _nextTipButton.onClick.Invoke();
    }

    public void StartSequence(LoadingSequence loadingSequence)
    {
        Debug.Log($"Starting sequence: {loadingSequence.SequenceName}");
        StartCoroutine(SequenceCoroutine(loadingSequence));
    }

    private IEnumerator SequenceCoroutine(LoadingSequence loadingSequence)
    {
        StartCoroutine(LoadingBarCoroutine());
        
        foreach (var sequenceElement in loadingSequence.Sequence)
        {
            _timeLastTipShown = Time.timeSinceLevelLoad;
            _loadBarQueue.Enqueue(sequenceElement.LoadBar);
            yield return DisplayTipCoroutine(sequenceElement.Tip);
        }

        if (loadingSequence.LevelToLoad == null) yield break;
        
        var level = Instantiate(loadingSequence.LevelToLoad);
        var door = level.GetComponentInChildren<DoorScript>();
        door.OnDoorOpen += (sequence) =>
        {
            Destroy(level);
            StartSequence(sequence);
        };
    }
    

    private IEnumerator LoadingBarCoroutine()
    {
        _loadingBar.fillAmount = 0f;
        
        while (true)
        {
            yield return new WaitUntil(() => _loadBarQueue.Count > 0);

            var loadBarConfig = _loadBarQueue.Dequeue();

            var timeSinceLastSection = Time.timeSinceLevelLoad - _timeLastTipShown;
            if (timeSinceLastSection < loadBarConfig.LoadDelay)
            {
                yield return new WaitForSeconds(loadBarConfig.LoadDelay - timeSinceLastSection);
            }

            _fillTween?.Kill();

            _fillTween = _loadingBar.DOFillAmount(
                    loadBarConfig.LoadPercent, 
                    loadBarConfig.LoadTime)
                .SetEase(loadBarConfig.LoadEase);
            
            yield return _fillTween.WaitForCompletion();
        }
    }

    private IEnumerator DisplayTipCoroutine(LoadingSequence.LoadingTip tip)
    {
        _nextTipButton.gameObject.SetActive(false);

        if (tip.VoiceOver != null)
        {
            Camera.main.GetComponent<AudioSource>().PlayOneShot(tip.VoiceOver);
        }
        
        if (tip.Emoji != null)
        {
            _emojiSpawner.SpawnImages(tip.Emoji);
        }
        
        yield return DisplayTipTextCoroutine(tip);
        yield return FadeInNextTipButton();
        yield return WaitUntilNextTipButtonPressed();
    }

    private IEnumerator DisplayTipTextCoroutine(LoadingSequence.LoadingTip tip)
    {
        var waitBetweenLetters = 1f / tip.ScrollSpeed;
        
        for (var i = 0; i < tip.TipText.Length; i++)
        {
            var substring = tip.TipText.Substring(0, i + 1);
            _loadingTip.text = substring;
            yield return new WaitForSeconds(waitBetweenLetters); 
        }
    }

    private IEnumerator WaitUntilNextTipButtonPressed()
    {
        var nextPressed = false;
        Action waitForNextPressed = () => nextPressed = true;
        _nextTipButton.onClick.AddListener(waitForNextPressed.Invoke);
        yield return new WaitUntil(() => nextPressed);
        _nextTipButton.onClick.RemoveListener(waitForNextPressed.Invoke);
    }

    private IEnumerator FadeInNextTipButton()
    {
        _nextTipButton.gameObject.SetActive(true);
        var canvasGroup = _nextTipButton.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        var fadeInTween = canvasGroup.DOFade(1f, _nextTipButtonFadeInTime);
        yield return fadeInTween.WaitForCompletion();
    }

    [Button]
    private void RunTestSequence()
    {
        StartSequence(_startingSequence);
    }
}
