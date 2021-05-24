using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRubixCube.Controls {
    public class StopWatch : MonoBehaviour
    {
        private Text _textComponent;
        public bool IsRunning{get; private set;} = false;
        private float _elapsedSeconds;

        [SerializeField]
        private bool _autoStart = false;

        void Awake()
        {
            _textComponent = GetComponent<Text>();
        }

        void Start()
        {
            if (_autoStart)
            {
                StartTimer();
            }
        }

        void Update()
        {
            if (!IsRunning) return;

            _elapsedSeconds += Time.deltaTime;
            var timeSpan = TimeSpan.FromSeconds(_elapsedSeconds);
            _textComponent.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }

        public void StartTimer()
        {
            IsRunning = true;
        }

        public void ResetTimer()
        {
            _elapsedSeconds = 0;
        }

        public void StopTimer()
        {
            IsRunning = false;
        }
    }
}
