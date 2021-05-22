using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit;

namespace UnityRubixCube {
    public class RubixLayer : RSMonoBehaviour
    {
        public enum ERotationState{
            IDLE,
            MANUAL,
            AUTO
        }

        public ERotationState CurrentRotationState{get; private set;} = ERotationState.IDLE;
        public RubixCube ParentCube {get; private set;}
        Quaternion _targetRotation;

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
        private void CollectCubies(RubixCube.Move move){
            float treshhold = ParentCube.GetTreshold();
            var allCubies = ParentCube.GetCubies();
            float distance = Mathf.Infinity;
            for(int i = 0; i < allCubies.Count; i++){
                switch(move.MoveAxis){
                    case RubixCube.Move.EMoveAxis.X:
                    distance = allCubies[i].transform.localPosition.x - transform.localPosition.x;
                    break;
                    case RubixCube.Move.EMoveAxis.Y:
                    distance = allCubies[i].transform.localPosition.y - transform.localPosition.y;
                    break;
                    case RubixCube.Move.EMoveAxis.Z:
                    distance = allCubies[i].transform.localPosition.z - transform.localPosition.z;
                    break;
                }
                distance = Mathf.Abs(distance);
                if(distance < treshhold){
                    allCubies[i].transform.parent = transform;
                }
            }
        }

        public void TriggerAutoRotate(RubixCube.Move move){
            _targetRotation = Quaternion.Euler(move.GetMoveVector() * 90);
            CurrentRotationState = ERotationState.AUTO;
        }

        float _step;
        public bool MoveLayer(RubixCube.Move move, bool isManual){
            if(CurrentRotationState == ERotationState.MANUAL && isManual){
                // ManualTurn
                return true;
            }
            if(CurrentRotationState != ERotationState.IDLE){
                return false;
            }
            float scale = 1f / ParentCube.CubiesPerSide;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = move.GetMoveVector(true) * (scale * move.LayerIndex - 0.5f + (scale / 2f));
            CollectCubies(move);
            if(!isManual){
                TriggerAutoRotate(move);
            }
            return true;
        }

        private void AutoRotate(){
            _step = _speed * Time.deltaTime;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _targetRotation, _step);

            if(Quaternion.Angle(transform.localRotation, _targetRotation) <= 1){
                transform.localRotation = _targetRotation;
                CurrentRotationState = ERotationState.IDLE;
                ReleaseCubies();
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
            if(CurrentRotationState == ERotationState.AUTO){
                AutoRotate();
            }
        }
        #endregion MonoBehavior Functions
    }
}