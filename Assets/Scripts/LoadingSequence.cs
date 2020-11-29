using System;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "LoadingSequence", menuName = "LoadingSequence", order = 1)]
public class LoadingSequence : ScriptableObject
{
    [Serializable]
    public class LoadBarConfig
    {
        public float LoadDelay;
        public float LoadPercent;
        public float LoadTime;
        public Ease LoadEase = Ease.OutQuad;
    }
    
    [Serializable]
    public class LoadingTip
    {
        public string TipText;
        public float ScrollSpeed = 50;
        public Sprite Emoji;
    }

    [Serializable]
    public class LoadingConfig
    {
        public LoadBarConfig LoadBar;
        public LoadingTip Tip;
    }

    public string SequenceName;
    public LoadingConfig[] Sequence;
    public GameObject LevelToLoad;
}