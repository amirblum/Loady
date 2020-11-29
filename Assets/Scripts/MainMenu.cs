using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public event Action OnPlayClicked; 
    
    protected void Awake()
    {
        var playButton = GetComponentInChildren<Button>();
        playButton.onClick.AddListener(() => OnPlayClicked?.Invoke());
    }
}