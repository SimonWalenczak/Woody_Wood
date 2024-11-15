using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] private GameObject _player;
    [SerializeField] private TextMeshProUGUI _tutoTextVisual;
    [SerializeField] private List<String> _tutoText;
    [SerializeField] private GameObject _tutoPanel;
    [SerializeField] private int _index;

    [Header("Pause Menu")] 
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private bool _IsPaused;
    private bool _canBePaused;
    
    private void Start()
    {
        _index = 1;
        _tutoTextVisual.SetText(_tutoText[0]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_index < _tutoText.Count)
                _tutoTextVisual.SetText(_tutoText[_index++]);
            else
            {
                _tutoPanel.SetActive(false);
                _player.SetActive(true);
                _canBePaused = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _canBePaused)
        {
            if (!_IsPaused)
            {
                Time.timeScale = 0;
                _pausePanel.SetActive(true);
                _IsPaused = true;
            }
            else
            {
                Time.timeScale = 1;
                _pausePanel.SetActive(false);
                _IsPaused = false;
            }
        }
    }
}
