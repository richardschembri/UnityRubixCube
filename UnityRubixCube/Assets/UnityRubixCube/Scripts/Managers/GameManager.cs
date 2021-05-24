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
        [SerializeField]
        private Slider _sizeSlider;
        private Text _sizeSliderText;

        const float ZOOM_MIN = -5f;
        const float ZOOM_MAX = -1.75f;

        [SerializeField] private RubixCube _rubixCube;

        [SerializeField] private UIPopup _mainMenu;
        [SerializeField] private StopWatch _stopWatch;

        #region RSMonoBehavior Functions
        public override bool Init(bool force = false)
        {
            return base.Init(force);
        }
        #endregion RSMonoBehavior Functions

        private void Zoom(float by){
           float newZoom = Camera.main.transform.position.z + by;
           newZoom = Mathf.Min(ZOOM_MAX, newZoom); 
           newZoom = Mathf.Max(ZOOM_MIN, newZoom); 
           Camera.main.transform.position = new Vector3(
               Camera.main.transform.position.x,
               Camera.main.transform.position.y,
               newZoom) ;
        }
        protected override void InitComponents()
        {
            base.InitComponents();
            _sizeSliderText = _sizeSlider.GetComponentInChildren<Text>();
            RefreshSizeSliderText();
        }

        private void RefreshSizeSliderText(){
            int s = (int)_sizeSlider.value;
            _sizeSliderText.text = $"{s}x{s}x{s}";
        }

        protected override void InitEvents(){
            _sizeSlider.onValueChanged.AddListener(SizeSliderOnValueChanged_Listener);
        }

        private void SizeSliderOnValueChanged_Listener(float value){
            RefreshSizeSliderText();
        }

        public void StartGame(){
            if(CurrentState != EGameStates.MAIN_MENU){
                return;
            }
            CurrentState = EGameStates.STARTING;
            _rubixCube.GenerateCube((int)_sizeSlider.value);
            _stopWatch.ResetTimer();
            _mainMenu.ClosePopup();
            _stopWatch.StartTimer();
        }
        
        // Start is called before the first frame update
        void Start()
        {
           _mainMenu.OpenPopup(); 
        }

        // Update is called once per frame
        void Update()
        {
           Zoom(Input.mouseScrollDelta.y); 
        }
    }
}
