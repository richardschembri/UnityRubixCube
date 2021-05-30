using System.Collections.Generic;
using UnityEngine;
using RSToolkit;
using System.Collections.ObjectModel;
using UnityRubixCube.Utils;
using RSToolkit.Helpers;
using System.Collections;
using UnityEngine.Events;

namespace UnityRubixCube {
    [RequireComponent(typeof(CubieSpawner))]
    public class RubixCube : RSMonoBehaviour
    {
        public enum ERubixAxis{
            X = 0, Y = 1, Z = 2
        }

        public enum ECubeState{
            IDLE = 0,
            MANUAL = 1,
            AUTO = 2
        }

        public enum CubeFace{
            UP, DOWN, LEFT, RIGHT, FRONT, BACK
        }

        public class Move{
            public int LayerIndex {get; private set;}
            public ERubixAxis MoveAxis {get; private set;}

            public bool Clockwise {get; set;}

            public bool IsShuffle {get; private set;}

            public Move(int layerIndex, ERubixAxis  moveDirection, bool clockwise, bool isShuffle){
                LayerIndex = layerIndex;
                MoveAxis  = moveDirection;
                Clockwise = clockwise;
                IsShuffle = isShuffle;
            }

            public Move(RubixSaveUtils.MoveSaveInfo moveSaveInfo){
                LayerIndex = moveSaveInfo.LayerIndex;
                MoveAxis  = (ERubixAxis)moveSaveInfo.MoveAxis;
                Clockwise = moveSaveInfo.Clockwise;
                IsShuffle = moveSaveInfo.IsShuffle;
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


            public override string ToString()
            {
                return $"Move - LayerIndex: {LayerIndex}, MoveAxis: {MoveAxis}, Clockwise: {Clockwise}, IsShuffle: {IsShuffle} ";
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

        public UnityEvent OnShuffleEnd {get; private set;} = new UnityEvent();
        public UnityEvent OnSolved {get; private set;} = new UnityEvent();

        private int _shuffles = 0;

        public bool IsShuffleing{
            get{
                return CanShuffle() && _shuffles > 0;
            }
        }


        public Vector3 CubieOffset {get { return _cubieSpawnerComponent.CubieOffset; } }
        public float CubieDistance {get { return _cubieSpawnerComponent.CubieDistance; } }

        public bool IsCubieSelected(Cubie target){
            return SelectedCubie != null && target.ParentCube == this;
        }

        public bool IsCubieSelected(){
           return IsCubieSelected(SelectedCubie);
        }

        public bool IsLayerMoveSet(){
            return _selectedLayer.IsLayerMoveSet();
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

        // TODO: refactor
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
                if(move.IsShuffle){
                    _shuffles--;
                    // ShuffleStep();
                    if(_shuffles > 0){
                        StartCoroutine(ShuffleStep());
                    }else{
                        OnShuffleEnd.Invoke(); 
                    }
                }else if(IsSolved()){
                    LogInDebugMode("Solved!");
                    OnSolved.Invoke();
                }else{
                    LogInDebugMode("Not Solved!");
                }
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

        public void ClearMoves(){
            _moves.Clear();
        }
        public void RestoreMoves(){
            _moves = RubixSaveUtils.LoadMoves();
        }
        public bool SaveCube(){
            return _cubieSpawnerComponent.SaveCube();
        }

        public void SaveMoves(){
            RubixSaveUtils.SaveMoves(_moves);
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
            return SetLayerMove(new Move(layerIndex, moveDirection,clockwise, false));
        }

        public bool CanShuffle(){
            return _moves.Last == null ||  _moves.Last.Value.IsShuffle;
        }
        public bool SetShuffleLayerMove(){
            // Cannot shuffle after a normal move
            if(!CanShuffle()){
                return false;
            }
            var newMove = new Move(RandomHelpers.RandomInt(CubiesPerSide),(ERubixAxis)RandomHelpers.RandomInt(3), RandomHelpers.RandomBool(), true);
            if(_moves.Last != null 
                && _moves.Last.Value.LayerIndex == newMove.LayerIndex
                && _moves.Last.Value.MoveAxis == newMove.MoveAxis
                && _moves.Last.Value.Clockwise != newMove.Clockwise ){
                    // Do not undo previous shuffle
                    newMove.Reverse();
                }
            return SetLayerMove(newMove);
        }

        IEnumerator ShuffleStep(){
            yield return new WaitForEndOfFrame();
            if(CanShuffle() && GetCubeState() == ECubeState.IDLE && _shuffles > 0){
                SetShuffleLayerMove();
                TriggerAutoRotate();
            }
        }

        public bool Shuffle(int shuffles){
            if(!CanShuffle() || GetCubeState() != ECubeState.IDLE){
                return false;
            }
            _shuffles = shuffles;
            // ShuffleStep();
            StartCoroutine(ShuffleStep());
            return true;
        }

        public bool TriggerAutoRotate(){
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

        public void UndoPlayerMove(){
            _selectedLayer.UndoPlayerMove();
        }

        public float GetTreshold(){
            return 1f / CubiesPerSide * 0.5f;
        }

        public Cubie.CubieIndex GetLocalPositionIndex(Vector3 localPosition){
            return _cubieSpawnerComponent.GetLocalPositionIndex(localPosition);
        }

        private bool CompareVectors(Vector3 a, Vector3 b){
            return (int)a.x == (int)b.x && (int)a.y == (int)b.y && (int)a.z == (int)b.z;
        }
        public bool IsSolved(){
            var cubies = GetCubies();
            var compare = cubies[0];
            for(int i = 1; i < cubies.Count; i++){
                if(!CompareVectors(compare.transform.localRotation.eulerAngles, cubies[i].transform.localRotation.eulerAngles)){//(compare.transform.localRotation.eulerAngles != cubies[i].transform.localRotation.eulerAngles){
                    // Debug.Log($"{compare.transform.localRotation.eulerAngles} != {cubies[i].transform.localRotation.eulerAngles}");
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