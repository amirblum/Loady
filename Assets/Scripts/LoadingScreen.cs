using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
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
            timeLastTipShown = Time.timeSinceLevelLoad;
            _loadBarQueue.Enqueue(sequenceElement.LoadBar);
            yield return DisplayTipCoroutine(sequenceElement.Tip);
        }
    }

    private float timeLastTipShown;

    
    private IEnumerator LoadingBarCoroutine()
    {
        _loadingBar.fillAmount = 0f;
        
        while (true)
        {
            yield return new WaitUntil(() => _loadBarQueue.Count > 0);

            var loadBarConfig = _loadBarQueue.Dequeue();

            var timeSinceLastSection = Time.timeSinceLevelLoad - timeLastTipShown;
            if (timeSinceLastSection < loadBarConfig.LoadDelay)
            {
                yield return new WaitForSeconds(loadBarConfig.LoadDelay - timeSinceLastSection);
            }

            var startingFillAmount = _loadingBar.fillAmount;
            for (var time = 0f; time < loadBarConfig.LoadTime; time += Time.deltaTime)
            {
                var additionalFillPercent = time / loadBarConfig.LoadTime;
                _loadingBar.fillAmount = startingFillAmount + additionalFillPercent * (loadBarConfig.LoadPercent - startingFillAmount);
                yield return null;
            }
            
            _loadingBar.fillAmount = loadBarConfig.LoadPercent;
        }
    }

    private IEnumerator DisplayTipCoroutine(LoadingSequence.LoadingTip tip)
    {
        yield return DisplayTipTextCoroutine(tip);
        yield return FadeInNextTipButton();
        yield return WaitUntilNextTipButtonPressed();
        _nextTipButton.gameObject.SetActive(false);
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
        
        for (var time = 0f; time < _nextTipButtonFadeInTime; time += Time.deltaTime)
        {
            canvasGroup.alpha = time / _nextTipButtonFadeInTime;
            yield return null;
        }
    }

    [Button]
    private void RunTestSequence()
    {
        StartSequence(_testSequence);
    }
}
