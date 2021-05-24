using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit;
using RSToolkit.UI.Controls;
using UnityRubixCube.Controls;

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

        const float ZOOM_MIN = 40;
        const float ZOOM_MAX = 100;


        [SerializeField] float _cameraRotationSpeed = 1f;
        [SerializeField] float _zoomSpeed = 1f;
        [SerializeField] private RubixCube _rubixCube;

        [SerializeField] private UIPopup _mainMenu;
        [SerializeField] private UIPopup _pauseMenu;
        [SerializeField] private UIPopup _congratsMenu;
        [SerializeField] private StopWatch _stopWatch;
        private CanvasGroup _stopWatchCanvasGroup;
        #region RSMonoBehavior Functions
        public override bool Init(bool force = false)
        {
            return base.Init(force);
        }

        protected override void InitComponents()
        {
            base.InitComponents();
            _sizeSliderText = _sizeSlider.GetComponentInChildren<Text>();
            RefreshSizeSliderText();
            _stopWatchCanvasGroup = _stopWatch.transform.parent.GetComponent<CanvasGroup>();
            TimerToggleonValueChanged_Listener(_timerToggle.isOn);
        }
        protected override void InitEvents(){
            _sizeSlider.onValueChanged.AddListener(SizeSliderOnValueChanged_Listener);
            _pauseMenu.OnOpenPopup.AddListener(PauseMenuOnOpenPopup_Listener);
            _timerToggle.onValueChanged.AddListener(TimerToggleonValueChanged_Listener);
        }

        #endregion RSMonoBehavior Functions

/*
        private void Zoom(float by){
           float newZoom = Camera.main.transform.position.z + by;
           newZoom = Mathf.Min(ZOOM_MAX, newZoom); 
           newZoom = Mathf.Max(ZOOM_MIN, newZoom); 
           Camera.main.transform.position = new Vector3(
               Camera.main.transform.position.x,
               Camera.main.transform.position.y,
               newZoom) ;
        }
        */
        private void Zoom(float by){
           float newZoom = Camera.main.fieldOfView - (by * _zoomSpeed);
           newZoom = Mathf.Min(ZOOM_MAX, newZoom); 
           newZoom = Mathf.Max(ZOOM_MIN, newZoom); 
           Camera.main.fieldOfView = newZoom;
        }

        private void RefreshSizeSliderText(){
            int s = (int)_sizeSlider.value;
            _sizeSliderText.text = $"{s}x{s}x{s}";
        }
        #region Events
        private void SizeSliderOnValueChanged_Listener(float value){
            RefreshSizeSliderText();
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
        private void ResetGame(){
            _stopWatch.StopTimer();
            _rubixCube.ClearCube();
            CloseAllPopups();
        }
        public void QuitGame(){
            CurrentState = EGameStates.END;
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
        }
        // Update is called once per frame
        void Update()
        {
            if(CurrentState != EGameStates.IN_GAME){
                return;
            }
            if(Input.mouseScrollDelta.y != 0){
                Zoom(Input.mouseScrollDelta.y); 
            }

            if (Input.GetMouseButton(0))
            {
                Camera.main.transform.RotateAround(_rubixCube.transform.parent.position,
                                                Camera.main.transform.up,
                                                -Input.GetAxis("Mouse X") * _cameraRotationSpeed);

                Camera.main.transform.RotateAround(_rubixCube.transform.parent.position,
                                                Camera.main.transform.right,
                                                -Input.GetAxis("Mouse Y") * _cameraRotationSpeed);
            }
        }
        #endregion MonoBehavior Functions
    }
}
