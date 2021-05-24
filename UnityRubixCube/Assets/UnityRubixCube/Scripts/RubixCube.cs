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
            CAMERA,
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

        public ECubeState GetRotationState(){
            return _selectedLayer.CurrentRotationState;
        }
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
        public void GenerateCube(int cubiesPerSide){
            CubiesPerSide = cubiesPerSide;
            GenerateCube();
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

        RaycastHit _mouseRaycastHit;
        Ray _mouseRay;
        Cubie _hitCubie = null;
    
        // Update is called once per frame
        void Update()
        {
            /*
           // LogInDebugMode($"Mouse Input: {Input.mousePosition}");
           if(!Input.GetMouseButtonDown(0)
                || _selectedLayer.CurrentRotationState == RubixLayer.ERotationState.AUTO){
               return;
           } 

            LogInDebugMode($"click");
            _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            // LogInDebugMode($"Mouse Down Input: {Input.mousePosition}");
            if(Physics.Raycast(_mouseRay, out _mouseRaycastHit, 500.0f, _layerMask)){
                var hitParent = _mouseRaycastHit.collider.transform.parent; 
            LogInDebugMode($"Hit object: {_mouseRaycastHit.collider.transform.name }");
                if(hitParent == null ){
                    return;
                }
                _hitCubie = hitParent.GetComponent<Cubie>();
                if(_hitCubie == null){
                    return;
                }
            LogInDebugMode($"Hit cubie: {_hitCubie.gameObject.name }");
            }
            */
           
        }
    }
}