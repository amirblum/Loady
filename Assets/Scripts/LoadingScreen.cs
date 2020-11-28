using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] LoadingSequence _testSequence;

    [Header("Config")] 
    [SerializeField] float _nextTipButtonFadeInTime;

    [Header("Elements")] 
    [SerializeField] Image _loadingBar;
    [SerializeField] TMP_Text _loadingTip;
    [SerializeField] Button _nextTipButton;

    private float _timeLastTipShown;
    private readonly Queue<LoadingSequence.LoadBarConfig> _loadBarQueue = new Queue<LoadingSequence.LoadBarConfig>();

    protected void Start()
    {
        _loadingBar.fillAmount = 0;
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _nextTipButton.onClick.Invoke();
        }
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

            var fillTween = 
                _loadingBar.DOFillAmount(
                loadBarConfig.LoadPercent, 
                loadBarConfig.LoadTime)
                    .SetEase(loadBarConfig.LoadEase);
            
            yield return fillTween.WaitForCompletion();
        }
    }

    private IEnumerator DisplayTipCoroutine(LoadingSequence.LoadingTip tip)
    {
        _nextTipButton.gameObject.SetActive(false);
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
        StartSequence(_testSequence);
    }
}
