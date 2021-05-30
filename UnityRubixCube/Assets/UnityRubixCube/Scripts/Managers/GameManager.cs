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
            WIN,
            END
        }

        public EGameStates CurrentState {get; private set;} = EGameStates.MAIN_MENU;
        
        [SerializeField] private Slider _sizeSlider;
        private Text _sizeSliderText;

        [SerializeField] private Toggle _timerToggle;

        [SerializeField] private RubixCube _rubixCube;
        public RubixCube MainRubixCube {get{return _rubixCube; }}

        [SerializeField] private StopWatch _stopWatch;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private Text _congratsTimeTakenText;
        private CanvasGroup _stopWatchCanvasGroup;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _undoButton;
        [SerializeField] private Button _resetCameraButton;
        [SerializeField] private int _shuffles = 10;

        [Header("Popups")]
        [SerializeField] private UIPopup _mainMenu;
        [SerializeField] private UIPopup _pauseMenu;
        [SerializeField] private UIPopup _congratsMenu;
        [SerializeField] private UIPopup _confirmDialog;
        private bool _confirmRestart = false;

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

            _rubixCube.OnShuffleEnd.AddListener(OnShuffleEnd_Listener);
            _rubixCube.OnSolved.AddListener(RubixCubeOnSolved_Listner);

            _cameraController.OnAnimationComplete.AddListener(CameraControllerOnAnimationComplete_Listner);
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
            _confirmDialog.ClosePopup();
            CurrentState = EGameStates.PAUSE;
            _stopWatch.PauseTimer();
        }

        private void TimerToggleonValueChanged_Listener(bool on){
            _stopWatchCanvasGroup.alpha = on ? 1f : 0f;
        }

        private void OnShuffleEnd_Listener(){
            CurrentState = EGameStates.IN_GAME;
            _stopWatch.StartTimer();
        }
        private void RubixCubeOnSolved_Listner(){
            _stopWatch.PauseTimer();
            ToggleInGameButtons(false);
            CurrentState = EGameStates.WIN;
            _cameraController.TriggerAnimateCelebration(true);
        }
        private void CameraControllerOnAnimationComplete_Listner(){
            _congratsTimeTakenText.text = _stopWatch.GetFormatTime();
            _congratsMenu.OpenPopup();
        }
        #endregion Events
        public void StartGame(){
            if(CurrentState != EGameStates.MAIN_MENU && CurrentState != EGameStates.END){
                return;
            }
            CurrentState = EGameStates.STARTING;
            _rubixCube.GenerateCube((int)_sizeSlider.value);
            _stopWatch.ResetTimer();
            ToggleInGameButtons(true);
            _mainMenu.ClosePopup();

            Shuffle();
        }

        private void ToggleInGameButtons(bool on){
            _menuButton.gameObject.SetActive(on);
            _undoButton.gameObject.SetActive(on);
            _resetCameraButton.gameObject.SetActive(on);
        }

        private void Shuffle(){
            if(_rubixCube.Shuffle(_shuffles)){
                CurrentState = EGameStates.SHUFFLE;
            }
        }

        public void ContinueGame(){
            if(CurrentState != EGameStates.MAIN_MENU && CurrentState != EGameStates.END){
                return;
            }
            CurrentState = EGameStates.STARTING;
            _rubixCube.RestoreCube();
            _rubixCube.RestoreMoves();
            _mainMenu.ClosePopup();
            _stopWatch.SetTimer(RubixSaveUtils.LoadElapsedTime());
            _stopWatch.StartTimer();

            CurrentState = EGameStates.IN_GAME;

        }
        private void ResetGame(){
            _confirmDialog.ClosePopup();
            _cameraController.ResetCamera();
            _stopWatch.StopTimer();
            _rubixCube.ClearCube();
            _rubixCube.ClearMoves();
            CloseAllPopups();
        }

        public void QuitGame(){
            _confirmDialog.ClosePopup();
            CurrentState = EGameStates.END;
            RubixSaveUtils.SaveElapsedTime(_stopWatch.ElapsedSeconds);
            _rubixCube.SaveCube();
            _rubixCube.SaveMoves();
            ResetGame();
            _mainMenu.OpenPopup();
        }
        public void PromptRestart(){
            _confirmRestart = true;
            _confirmDialog.OpenPopup();
        }
        public void PromptQuit(){
            _confirmRestart = false;
            _confirmDialog.OpenPopup();
        }

        public void PauseConfirm(){
            if(_confirmRestart){
                RestartGame();
            }else{
                QuitGame();
            }
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
        }
        #endregion MonoBehavior Functions
    }
}
