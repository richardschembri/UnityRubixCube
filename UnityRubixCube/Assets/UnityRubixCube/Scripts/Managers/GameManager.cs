using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit;
using RSToolkit.UI.Controls;
using UnityRubixCube.Controls;
using UnityRubixCube.Controllers;
using UnityRubixCube.Utils;

namespace UnityRubixCube.Managers{

    public class GameManager : RSSingletonMonoBehaviour<GameManager>
    {

        public enum EGameStates{
            MAIN_MENU,
            STARTING,
            SHUFFLE,
            IN_GAME,
            PAUSE,
            END
        }

        public EGameStates CurrentState {get; private set;} = EGameStates.MAIN_MENU;
        
        [SerializeField] private Slider _sizeSlider;
        private Text _sizeSliderText;

        [SerializeField] private Toggle _timerToggle;

        [SerializeField] private RubixCube _rubixCube;
        public RubixCube MainRubixCube {get{return _rubixCube; }}

        [SerializeField] private UIPopup _mainMenu;
        [SerializeField] private UIPopup _pauseMenu;
        [SerializeField] private UIPopup _congratsMenu;
        [SerializeField] private StopWatch _stopWatch;
        [SerializeField] private CameraController _cameraController;
        private CanvasGroup _stopWatchCanvasGroup;
        [SerializeField] private Button _continueButton;

        #region RSMonoBehavior Functions

        protected override void InitComponents()
        {
            base.InitComponents();
            _sizeSliderText = _sizeSlider.GetComponentInChildren<Text>();
            RefreshSizeSliderText();
            _stopWatchCanvasGroup = _stopWatch.transform.parent.GetComponent<CanvasGroup>();
            TimerToggleonValueChanged_Listener(_timerToggle.isOn);
        }
        protected override void InitEvents(){
            _mainMenu.OnOpenPopup.AddListener(MainMenuOnOpenPopup_Listener);
            _sizeSlider.onValueChanged.AddListener(SizeSliderOnValueChanged_Listener);
            _pauseMenu.OnOpenPopup.AddListener(PauseMenuOnOpenPopup_Listener);
            _timerToggle.onValueChanged.AddListener(TimerToggleonValueChanged_Listener);
        }

        #endregion RSMonoBehavior Functions

        private void RefreshSizeSliderText(){
            int s = (int)_sizeSlider.value;
            _sizeSliderText.text = $"{s}x{s}x{s}";
        }
        #region Events

        private void SizeSliderOnValueChanged_Listener(float value){
            RefreshSizeSliderText();
        }

        private void MainMenuOnOpenPopup_Listener(UIPopup popup, bool keepCache){
            _continueButton.interactable = RubixSaveUtils.HasCubies();
        }
        private void PauseMenuOnOpenPopup_Listener(UIPopup popup, bool keepCache){
            CurrentState = EGameStates.PAUSE;
            _stopWatch.PauseTimer();
        }

        private void TimerToggleonValueChanged_Listener(bool on){
            _stopWatchCanvasGroup.alpha = on ? 1f : 0f;
        }
        #endregion Events
        public void StartGame(){
            if(CurrentState != EGameStates.MAIN_MENU && CurrentState != EGameStates.END){
                return;
            }
            CurrentState = EGameStates.STARTING;
            _rubixCube.GenerateCube((int)_sizeSlider.value);
            _stopWatch.ResetTimer();
            _mainMenu.ClosePopup();
            _stopWatch.StartTimer();

            CurrentState = EGameStates.IN_GAME;

        }
        public void ContinueGame(){
            if(CurrentState != EGameStates.MAIN_MENU && CurrentState != EGameStates.END){
                return;
            }
            CurrentState = EGameStates.STARTING;
            _rubixCube.RestoreCube();
            _mainMenu.ClosePopup();
            _stopWatch.SetTimer(RubixSaveUtils.LoadElapsedTime());
            _stopWatch.StartTimer();

            CurrentState = EGameStates.IN_GAME;

        }
        private void ResetGame(){
            _cameraController.ResetCamera();
            _stopWatch.StopTimer();
            _rubixCube.ClearCube();
            CloseAllPopups();
        }
        public void QuitGame(){
            CurrentState = EGameStates.END;
            RubixSaveUtils.SaveElapsedTime(_stopWatch.ElapsedSeconds);
            _rubixCube.SaveCube();
            ResetGame();
            _mainMenu.OpenPopup();
        }

        public void RestartGame(){
            CurrentState = EGameStates.END;
            ResetGame();
            StartGame();
        }

        public void BackPauseMenu(){
            _pauseMenu.ClosePopup();
            _stopWatch.StartTimer();

            CurrentState = EGameStates.IN_GAME;
        }

        public void SaveCubeState(){

        }

        private void CloseAllPopups(){
            _mainMenu.ClosePopup();
            _pauseMenu.ClosePopup();
            _congratsMenu.ClosePopup();
        }

        #region MonoBehavior Functions
        // Start is called before the first frame update
        void Start()
        {
           _mainMenu.OpenPopup(); 
           Debug.Log(PlayerPrefs.GetString("cubies"));
        }
        // Update is called once per frame
        void Update()
        {
        }
        #endregion MonoBehavior Functions
    }
}
