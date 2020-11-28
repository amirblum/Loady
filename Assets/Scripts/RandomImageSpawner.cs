using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RandomImageSpawner : MonoBehaviour
{
    [SerializeField] Sprite _testImage;
    [SerializeField] Image _imagePrefab;
    
    [SerializeField] int _numOfSpawns;
    [SerializeField] float _allowedSpawnDistance;
    [SerializeField] float _timeBetweenSpawns;
    [SerializeField] float _spawnFadeTime;

    private float _halfCanvasWidth;
    private float _halfCanvasHeight;

    private int _maxRandomTries = 10;

    protected void Awake()
    {
        var canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        var rect = canvasRect.rect;
        // _canvasWidth = rect.width;
        // _canvasHeight = rect.height;
        
        _halfCanvasWidth = 1920f / 2f;
        _halfCanvasHeight = 1080f / 2f;
    }

    public void SpawnImages(Sprite imageToSpawn)
    {
        StartCoroutine(SpawnImagesCoroutine(imageToSpawn));
    }

    private IEnumerator SpawnImagesCoroutine(Sprite imageToSpawn)
    {
        var lastSpawnedPosition = Vector2.zero;
        
        for (var i = 0; i < _numOfSpawns; i++)
        {
            SpawnImage(i, imageToSpawn, GetNextRandomPosition(lastSpawnedPosition));
            yield return new WaitForSeconds(_timeBetweenSpawns);
        }
    }

    private void SpawnImage(int spawnNum, Sprite imageToSpawn, Vector2 spawnPosition)
    {
        var spawn = Instantiate(_imagePrefab, transform);
        spawn.name = $"SpawnedImage{spawnNum + 1}";
        
        spawn.GetComponent<RectTransform>().anchoredPosition = spawnPosition;
        
        spawn.sprite = imageToSpawn;
            
        var fadeInTween = spawn.DOFade(0f, _spawnFadeTime)
            .From()
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => Destroy(spawn.gameObject));
    }

    private Vector2 GetNextRandomPosition(Vector2 lastSpawnedPosition)
    {
        var newPosition = GetRandomPosition();
        var tries = 1;

        while (Vector2.Distance(newPosition, lastSpawnedPosition) < _allowedSpawnDistance)
        {
            newPosition = GetRandomPosition();
            if (++tries >= _maxRandomTries)
            {
                break;
            }
        }

        return newPosition;
    }

    private Vector2 GetRandomPosition()
    {
        return new Vector2(
            Random.Range(-_halfCanvasWidth, _halfCanvasWidth),
            Random.Range(-_halfCanvasHeight, _halfCanvasHeight)
            );
    }

    [Button]
    private void TestRun()
    {
        SpawnImages(_testImage);
    }
}
