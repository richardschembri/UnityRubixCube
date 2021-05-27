using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit;
using　UnityRubixCube.Managers;
using UnityEngine.Events;

namespace UnityRubixCube.Controllers{
    public class CameraController : RSMonoBehaviour
    {
        Camera _cameraComponent;

        private Vector3 _initialCameraPos;
        private Quaternion _initialCameraRot;
        const float ZOOM_MIN = 40;
        const float ZOOM_MAX = 100;

        [SerializeField] float _cameraRotationSpeed = 1f;
        [SerializeField] float _cameraAnimSpeed = 2f;
        [SerializeField] float _zoomSpeed = 1f;

        [SerializeField] private int _animSpins = 2;
        private int _spinCount = 0;
        private float _spinDegree = 0f;
        public bool IsAnimating {get; private set;} = false;

        public UnityEvent OnAnimationComplete {get; private set;} = new UnityEvent();

        public void ResetCamera(){
            Camera.main.transform.position = _initialCameraPos;
            Camera.main.transform.rotation = _initialCameraRot;
            ResetAnimValues();
        }

        protected override void InitComponents()
        {
            base.InitComponents();
            _cameraComponent = this.GetComponent<Camera>();
        }

        private void Zoom(float by){
           Camera.main.fieldOfView = Mathf.Clamp(_cameraComponent.fieldOfView - (by * _zoomSpeed),
                                                        ZOOM_MIN, ZOOM_MAX);
        }

        private bool CanAnimate(){
            return _spinCount < _animSpins;
        }
        private void ResetAnimValues(){
            _spinCount = 0;
            _spinDegree = 0f;
            IsAnimating = false;
        }
        public void TriggerAnimateCelebration(bool fromStart){
            if(fromStart){
                ResetAnimValues();
            }
            IsAnimating = true;
        }

        private void AnimateCelebrationStep(){
            if(!CanAnimate() && IsAnimating){
                IsAnimating = false;
                OnAnimationComplete.Invoke();
            }
            if(!IsAnimating){
                return;
            }
            _spinDegree += Mathf.Abs(_cameraAnimSpeed);
            if(_spinDegree >= 360f){
                _spinCount++;
                _spinDegree = 0f;
            }
            transform.RotateAround(GameManager.Instance.MainRubixCube.transform.position,
                                            transform.up,
                                            _cameraAnimSpeed);

            transform.RotateAround(GameManager.Instance.MainRubixCube.transform.position,
                                            transform.right,
                                            _cameraAnimSpeed);
        }


        public override bool Init(bool force = false)
        {
            if(!base.Init(force)){
                return false;
            }
            _initialCameraPos = Camera.main.transform.position;
            _initialCameraRot = Camera.main.transform.rotation;

            return true;
        }
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            AnimateCelebrationStep();
            if (GameManager.Instance.CurrentState != GameManager.EGameStates.IN_GAME
                    || GameManager.Instance.MainRubixCube.GetCubeState() != RubixCube.ECubeState.IDLE
                    || GameManager.Instance.MainRubixCube.IsCubieSelected()
                    || IsAnimating){
                return;
            }    

            if(Input.mouseScrollDelta.y != 0){
                Zoom(Input.mouseScrollDelta.y); 
            }

            if (Input.GetMouseButton(0))
            {
                transform.RotateAround(GameManager.Instance.MainRubixCube.transform.position,
                                                transform.up,
                                                Input.GetAxis("Mouse X") * _cameraRotationSpeed);

                transform.RotateAround(GameManager.Instance.MainRubixCube.transform.position,
                                                transform.right,
                                                -Input.GetAxis("Mouse Y") * _cameraRotationSpeed);
            }
            return;
        }
    }
}
