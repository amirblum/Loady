using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LoadingSequence", menuName = "LoadingSequence", order = 1)]
public class LoadingSequence : ScriptableObject
{
    [Serializable]
    public class LoadingTip 
    {
        public float PercentToLoad;
        public string TipText;
        public float ScrollSpeed = 50;
        public Sprite Emoji;
    }

    public string SequenceName;
    public float LoadTime = 10;
    public LoadingTip[] Tips;
}