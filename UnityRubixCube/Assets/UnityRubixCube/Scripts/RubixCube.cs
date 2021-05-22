using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit;
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

            public Vector3 GetMoveVector(){
                switch (MoveAxis){
                    case EMoveAxis.X:
                        return IsPositive ? Vector3.right : Vector3.left;
                    case EMoveAxis.Y:
                        return IsPositive ? Vector3.up : Vector3.down;
                    case EMoveAxis.Z:
                        return IsPositive ? Vector3.forward : Vector3.back;
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


        #region RSMonoBehaviour Functions
        protected override void InitComponents()
        {
            base.InitComponents();
            _cubieSpawnerComponent = GetComponent<CubieSpawner>();
        }

        #endregion RSMonoBehaviour Functions

        public void GenerateCube(){
            _cubieSpawnerComponent.GenerateCube(_cubiesPerSide);
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

        public void MoveLayer(Move move){
            _selectedLayer.MoveLayer(move);
        }
        public void MoveLayer(int layerIndex, Move.EMoveAxis moveDirection,bool isPositive){
            MoveLayer(new Move(layerIndex, moveDirection,isPositive));
        }
        // Start is called before the first frame update
        void Start()
        {
            if(!Initialized){
                return;
            }
            GenerateCube();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

}