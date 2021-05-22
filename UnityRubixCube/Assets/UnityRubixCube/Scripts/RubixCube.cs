using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit;
using RSToolkit.Collections;
using System.Collections.ObjectModel;

namespace UnityRubixCube {
    [RequireComponent(typeof(CubieSpawner))]
    public class RubixCube : RSMonoBehaviour
    {
        public class Move{
            public enum EMoveAxis{
                X, Y, Z
            }
            public int LayerIndex {get; private set;}
            public EMoveAxis MoveAxis {get; private set;}

            public bool IsPositive {get; private set;}

            public Move(int layerIndex, EMoveAxis moveDirection,bool isPositive){
                LayerIndex = layerIndex;
                MoveAxis  = moveDirection;
                IsPositive = isPositive;
            }

            public Vector3 GetMoveVector(bool abs = false){
                switch (MoveAxis){
                    case EMoveAxis.X:
                        return abs || IsPositive ? Vector3.right : Vector3.left;
                    case EMoveAxis.Y:
                        return abs || IsPositive ? Vector3.up : Vector3.down;
                    case EMoveAxis.Z:
                        return abs || IsPositive ? Vector3.forward : Vector3.back;
                }

                return Vector3.zero;
            }

        }

        [SerializeField]
        private int _cubiesPerSide = 3;
        public int CubiesPerSide {get {return _cubiesPerSide;}}
        private CubieSpawner _cubieSpawnerComponent;
        [SerializeField]
        private RubixLayer _selectedLayer;

        private int _layerMask = 1 << 8;

        private LinkedList<Move> _moves = new LinkedList<Move>();


        #region RSMonoBehaviour Functions
        protected override void InitComponents()
        {
            base.InitComponents();
            _cubieSpawnerComponent = GetComponent<CubieSpawner>();
        }

        #endregion RSMonoBehaviour Functions

        public void GenerateCube(){
            _cubieSpawnerComponent.GenerateCube();
        }

        public void ClearCube(){
            _cubieSpawnerComponent.DestroyAllSpawns();
        }

        public ReadOnlyCollection<Cubie> GetCubies(){
            return _cubieSpawnerComponent.SpawnedGameObjects;
        }

        public int CubiesCount(){
            return GetCubies().Count;
        }

        public void MoveLayer(Move move, bool isManual){
            _selectedLayer.MoveLayer(move, isManual);
        }
        public void MoveLayer(int layerIndex, Move.EMoveAxis moveDirection,bool isPositive, bool isManual){
            MoveLayer(new Move(layerIndex, moveDirection,isPositive), isManual);
        }

        public float GetTreshold(){
            return 1f / CubiesPerSide * 0.5f;
        }

        public Cubie.CubieIndex GetLocalPositionIndex(Vector3 localPosition){
            return _cubieSpawnerComponent.GetLocalPositionIndex(localPosition);
        }
        // Start is called before the first frame update
        void Start()
        {
            if(!Initialized){
                return;
            }
            GenerateCube();
        }

        RaycastHit _mouseRaycastHit;
        Ray _mouseRay;
        Cubie _hitCubie = null;
    
        // Update is called once per frame
        void Update()
        {
           // LogInDebugMode($"Mouse Input: {Input.mousePosition}");
           if(!Input.GetMouseButtonDown(0)
                || _selectedLayer.CurrentRotationState == RubixLayer.ERotationState.AUTO){
               return;
           } 

            _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // LogInDebugMode($"Mouse Down Input: {Input.mousePosition}");
            if(Physics.Raycast(_mouseRay, out _mouseRaycastHit, 100.0f, _layerMask)){
                var hitParent = _mouseRaycastHit.collider.transform.parent; 
                if(hitParent == null ){
                    return;
                }
                _hitCubie = hitParent.GetComponent<Cubie>();
                if(_hitCubie == null){
                    return;
                }
            }
           
        }
    }
}