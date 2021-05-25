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
        public enum ERubixAxis{
            X, Y, Z
        }

        public enum ECubeState{
            IDLE,
            MANUAL,
            AUTO
        }
        public class Move{
            public int LayerIndex {get; private set;}
            public ERubixAxis MoveAxis {get; private set;}

            public bool Clockwise {get; private set;}

            public Move(int layerIndex, ERubixAxis  moveDirection,bool clockwise){
                LayerIndex = layerIndex;
                MoveAxis  = moveDirection;
                Clockwise = clockwise;
            }

            public void Reverse(){

                Clockwise = !Clockwise;
            }

            public Vector3 GetMoveVector(bool abs = false){
                switch (MoveAxis){
                    case ERubixAxis.X:
                        return abs || Clockwise ? Vector3.right : Vector3.left;
                    case ERubixAxis.Y:
                        return abs || Clockwise ? Vector3.up : Vector3.down;
                    case ERubixAxis.Z:
                        return abs || Clockwise ? Vector3.forward : Vector3.back;
                }

                return Vector3.zero;
            }

        }

        [SerializeField]
        private int _cubiesPerSide = 3;
        public int CubiesPerSide {get {return _cubiesPerSide;} private set{_cubiesPerSide = value;}}
        private CubieSpawner _cubieSpawnerComponent;
        [SerializeField]
        private RubixLayer _selectedLayer;

        private int _layerMask = 1 << 8;

        private LinkedList<Move> _moves = new LinkedList<Move>();

        public Cubie SelectedCubie { get; private set; } = null;

        [SerializeField]
        private float _dragTreshold = 0.025f;
        public float DragTreshold { get {return _dragTreshold;} }
        [SerializeField]
        private float _dragSensitivity = 0.4f;
        public float DragSensitivity {get {return _dragSensitivity;}}

        public bool IsCubieSelected(Cubie target){
            return SelectedCubie != null && target.ParentCube == this;
        }

        public bool IsCubieSelected(){
           return IsCubieSelected(SelectedCubie);
        }
        public bool SelectCubie(Cubie target){
            if(IsCubieSelected(target)){
                return false;
            }
            SelectedCubie = target; 
            return true;
        }

        public bool DeselectCubie(Cubie target){
            if(!IsCubieSelected(target)){
                return false;
            }
            SelectedCubie = null; 
            return true;
        }
        public bool DeselectCubie(){
            return DeselectCubie(SelectedCubie);
        }
        private bool IsNeighbourIndex(int a, int b){
            return Mathf.Abs(a - b) <= 1;
        }
        public List<Cubie> GetNeighbours(Cubie target){
            var cubies = GetCubies();
            var result = new List<Cubie>();
            for(int i = 0; i < cubies.Count; i++){
                if(cubies[i] == target){
                    continue;
                }
                if(IsNeighbourIndex(target.Index.x, cubies[i].Index.x)
                    && IsNeighbourIndex(target.Index.y, cubies[i].Index.y)
                    && IsNeighbourIndex(target.Index.z, cubies[i].Index.z)){
                        result.Add(cubies[i]);
                    }
            }
            return result;
        }

        public ECubeState GetCubeState(){
            return _selectedLayer.CurrentCubeState;
        }
        #region RSMonoBehaviour Functions
        protected override void InitComponents()
        {
            base.InitComponents();
            _cubieSpawnerComponent = GetComponent<CubieSpawner>();
        }

        protected override void InitEvents()
        {
            base.InitEvents();
            _selectedLayer.OnMovePerformed.AddListener(SelectedLayerOnMovePerformed_Listener);
        }

        #endregion RSMonoBehaviour Functions

        #region Events
        private void SelectedLayerOnMovePerformed_Listener(Move move, bool isUndo){
            if(isUndo){
                _moves.RemoveLast();
            }else{
                _moves.AddLast(move);
            }
        }
        #endregion Events


        public void GenerateCube(){
            _cubieSpawnerComponent.GenerateCube();
        }
        public void GenerateCube(int cubiesPerSide){
            CubiesPerSide = cubiesPerSide;
            GenerateCube();
        }

        public void ClearCube(){
            _cubieSpawnerComponent.DestroyAllSpawns();
        }

        public bool RestoreCube(){
            if(_cubieSpawnerComponent.RestoreCube(ref _cubiesPerSide)){
                return true;
            }
            return false;
        }
        public bool SaveCube(){
            return _cubieSpawnerComponent.SaveCube();
        }

        public ReadOnlyCollection<Cubie> GetCubies(){
            return _cubieSpawnerComponent.SpawnedGameObjects;
        }

        public int CubiesCount(){
            return GetCubies().Count;
        }

        public bool SetLayerMove(Move move){
            return _selectedLayer.SetLayerMove(move);
        }
        public bool SetLayerMove(int layerIndex, ERubixAxis moveDirection,bool clockwise){
            return SetLayerMove(new Move(layerIndex, moveDirection,clockwise));
        }

        public bool TriggerAutoRotate(){
            LogInDebugMode("TriggerAutoRotate");
            return _selectedLayer.TriggerAutoRotate();
        }

        public bool ManualRotate(float by){
            return _selectedLayer.ManualRotate(by);
        }

        public bool HasMoves(){
            return _moves.Count > 0;
        }

        public Move GetLastMove(){
            return _moves.Last.Value;
        }

        public void UndoMove(){
            _selectedLayer.UndoMove();
        }

        public float GetTreshold(){
            return 1f / CubiesPerSide * 0.5f;
        }

        public Cubie.CubieIndex GetLocalPositionIndex(Vector3 localPosition){
            return _cubieSpawnerComponent.GetLocalPositionIndex(localPosition);
        }

        public bool IsSolved(){
            var cubies = GetCubies();
            var compare = cubies[0];
            for(int i = 1; i < cubies.Count; i++){
                if(compare.transform.localRotation != cubies[i].transform.localRotation){
                    return false;
                }
                compare = cubies[i];
            }
            return true;
        }

        // Update is called once per frame
        void Update()
        {
           
        }
    }
}