using System;
using UnityEngine;

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
        public bool WaitForNextPressed = true;
    }

    public string SequenceName;
    public LoadingTip[] TipSequence;
}