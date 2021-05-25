using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRubixCube.Controls {
    public class StopWatch : MonoBehaviour
    {
		public enum EStopWatchState{
			STOPPED,
			PAUSED,
			RUNNING
		}
		public EStopWatchState CurrentState {get; private set;} = EStopWatchState.STOPPED;
        private Text _textComponent;
        public float ElapsedSeconds {get; private set;}

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
            if (CurrentState != EStopWatchState.RUNNING) return;

            ElapsedSeconds += Time.deltaTime;
            var timeSpan = TimeSpan.FromSeconds(ElapsedSeconds);
            _textComponent.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }

		public void SetTimer(float seconds){
			ElapsedSeconds = seconds;
		}

        public void StartTimer()
        {
            CurrentState = EStopWatchState.RUNNING;
        }
		public void PauseTimer(){
            CurrentState = EStopWatchState.PAUSED;
		}

        public void ResetTimer()
        {
            ElapsedSeconds = 0;
        }

        public void StopTimer()
        {
            CurrentState = EStopWatchState.STOPPED;
            ResetTimer();
        }
    }
}
