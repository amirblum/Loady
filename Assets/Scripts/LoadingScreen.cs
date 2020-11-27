using System;
using System.Collections;
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
        StartCoroutine(LoadingBarCoroutine(loadingSequence.LoadTime));
        StartCoroutine(SequenceCoroutine(loadingSequence.Tips));
    }

    private IEnumerator LoadingBarCoroutine(float loadTime)
    {
        for (var time = 0f; time < loadTime; time += Time.deltaTime)
        {
            _loadingBar.fillAmount = time / loadTime;
            yield return null;
        }
    }

    private IEnumerator SequenceCoroutine(LoadingSequence.LoadingTip[] tips)
    {
        foreach (var tip in tips)
        {
            Debug.Log($"Applying tip text {tip.TipText}");
            yield return DisplayTipCoroutine(tip);
            yield return FadeInNextTipButton();
            yield return WaitUntilNextTipButtonPressed();
            _nextTipButton.gameObject.SetActive(false);
        }
    }

    private IEnumerator DisplayTipCoroutine(LoadingSequence.LoadingTip tip)
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
