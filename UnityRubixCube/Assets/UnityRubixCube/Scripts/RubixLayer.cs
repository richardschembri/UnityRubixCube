using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit;
using UnityEngine.Events;
using UnityRubixCube.Managers;

namespace UnityRubixCube {
    public class RubixLayer : RSMonoBehaviour
    {

        public RubixCube.ECubeState CurrentCubeState{get; private set;} = RubixCube.ECubeState.IDLE;
        public RubixCube ParentCube {get; private set;}
        Quaternion _targetRotation;
        RubixCube.Move _targetMove = null;

        public bool? IsUndo(){
            if(_targetMove == null){
                return null;
            }
            return ParentCube.HasMoves() && _targetMove == ParentCube.GetLastMove();
        }
        [SerializeField]
        Transform Visualizer;

        public class MovePerformedEvent : UnityEvent<RubixCube.Move, bool> { }
        public MovePerformedEvent OnMovePerformed {get; private set;} = new MovePerformedEvent();

        [SerializeField]
        private float _speed = 300f;

        #region RSMonoBehaviour Functions
        protected override void InitComponents()
        {
            base.InitComponents();
            ParentCube = GetComponentInParent<RubixCube>();
        }
        #endregion RSMonoBehaviour Functions

        public bool ReleaseCubies(){
          var cubies = GetComponentsInChildren<Cubie>();  
          if(cubies.Length <= 0){
              return false;
          }
          for(int i = 0; i < cubies.Length; i++){
              cubies[i].transform.parent = ParentCube.transform;
              cubies[i].RefreshIndex();
          }
          return true;
        }
        private void CollectCubies(){
            float treshhold = ParentCube.GetTreshold();
            var allCubies = ParentCube.GetCubies();
            float distance = Mathf.Infinity;
            for(int i = 0; i < allCubies.Count; i++){
                switch(_targetMove.MoveAxis){
                    case RubixCube.ERubixAxis.X:
                    distance = allCubies[i].transform.localPosition.x - transform.localPosition.x;
                    break;
                    case RubixCube.ERubixAxis.Y:
                    distance = allCubies[i].transform.localPosition.y - transform.localPosition.y;
                    break;
                    case RubixCube.ERubixAxis.Z:
                    distance = allCubies[i].transform.localPosition.z - transform.localPosition.z;
                    break;
                }
                distance = Mathf.Abs(distance);
                if(distance < treshhold){
                    allCubies[i].transform.parent = transform;
                }
            }
        }

        public bool TriggerAutoRotate(){
            if ( CurrentCubeState == RubixCube.ECubeState.AUTO){
                return false;
            }
            if(Mathf.Abs(Quaternion.Angle(transform.localRotation, _targetRotation)) > 90){
                _targetMove.Reverse();
            }
            CurrentCubeState = RubixCube.ECubeState.AUTO;
            return true;
        }

        float _step;
        public bool SetLayerMove(RubixCube.Move move){
            if(CurrentCubeState != RubixCube.ECubeState.IDLE){
                return false;
            }
            _targetMove = move;
            _targetRotation = Quaternion.Euler(_targetMove.GetMoveVector() * (IsUndo().Value ? -90 : 90));

            float scale = 1f / ParentCube.CubiesPerSide;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = move.GetMoveVector(true) * (scale * move.LayerIndex - 0.5f + (scale / 2f));
            switch(move.MoveAxis){
                case RubixCube.ERubixAxis.X:
                    Visualizer.localScale = new Vector3(0.01f, 1.25f, 1.25f);
                    break;
                case RubixCube.ERubixAxis.Z:
                    Visualizer.localScale = new Vector3(1.25f, 1.25f, 0.01f);
                    break;
                case RubixCube.ERubixAxis.Y:
                    Visualizer.localScale = new Vector3(1.25f, 0.01f, 1.25f);
                    break;
            }
            Visualizer.gameObject.SetActive(true);
            CollectCubies();
            return true;
        }

        public bool CanUndoPlayerMove(){
            return ParentCube.HasMoves() &&  !ParentCube.GetLastMove().IsShuffle;
        }

        public bool UndoPlayerMove(){
            return CanUndoPlayerMove() && SetLayerMove(ParentCube.GetLastMove()) && TriggerAutoRotate(); 
        }
        public bool ManualRotate(float by){
            if(_targetMove == null || GameManager.Instance.CurrentState != GameManager.EGameStates.IN_GAME){
                return false;
            }
            CurrentCubeState = RubixCube.ECubeState.MANUAL;
            _targetRotation = Quaternion.Euler(_targetMove.GetMoveVector() * (IsUndo().Value ? -90 : 90));

            if(Mathf.Abs(Quaternion.Angle(transform.localRotation, _targetRotation)) > 1){
                transform.localRotation =  Quaternion.Euler(_targetMove.GetMoveVector() * by * ParentCube.DragSensitivity);
            }
            return true;
        }

        private void AutoRotate(){
            _step = _speed * Time.deltaTime;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _targetRotation, _step);

            if(Quaternion.Angle(transform.localRotation, _targetRotation) <= 1){
                transform.localRotation = _targetRotation;
                CurrentCubeState = RubixCube.ECubeState.IDLE;

                Visualizer.gameObject.SetActive(false);
                ReleaseCubies();
                if(transform.localRotation != Quaternion.Euler(Vector3.zero)){
                    
                    OnMovePerformed.Invoke(_targetMove, IsUndo().Value);
                }
                _targetMove = null;
            }
        }
        
        #region MonoBehavior Functions
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(CurrentCubeState == RubixCube.ECubeState.AUTO){
                AutoRotate();
            }
        }
        #endregion MonoBehavior Functions
    }
}