using UnityEngine;
using RSToolkit;
using System.Collections.Generic;
using UnityRubixCube.Managers;

namespace UnityRubixCube {
    public class CubieFace : RSMonoBehaviour 
    {
        public enum EMouseDirection{
            NONE,
            X, Y
        }

        private EMouseDirection _mouseDirection = EMouseDirection.NONE;
        public Cubie ParentCubie {get; private set;}

        float _dragModifier = 1;
        private float _mouseZ;

        private Vector3? _dragStart = null;
        private Vector3? _dragStartHit = null;
        
        Cubie _behindCubie;
        Cubie _dragCubie;

        public bool IsMouseOver {get; private set;}= false;

        float _dragDistance;

        protected override void InitComponents()
        {
            base.InitComponents();
            ParentCubie = transform.parent.GetComponent<Cubie>();
        }

        private Vector3 GetDragDirection(){
            var _dragEnd3D = GetMouseHit().point;
            return (_dragEnd3D - _dragStartHit.Value).normalized * ParentCubie.ParentCube.CubieDistance;
        }

        private Vector3 GetBehindFaceDirection(){
            return (ParentCubie.transform.position - transform.position).normalized * ParentCubie.ParentCube.CubieDistance;
        }

        private Vector3 GetDragOffsetLocalPosition(bool oppsite){
            if(oppsite){
                return ParentCubie.transform.localPosition + GetDragDirection();
            }else{
                return ParentCubie.transform.localPosition - GetDragDirection();
            }
        }
        private Vector3 GetDragOffsetPosition(bool oppsite){
            if(oppsite){
                return ParentCubie.transform.position + GetDragDirection();
            }else{
                return ParentCubie.transform.position - GetDragDirection();
            }
        }

        private Cubie GetBehindFaceCubie(){
            var offsetPosition = ParentCubie.transform.localPosition + GetBehindFaceDirection();
            var offsetFarPosition = ParentCubie.transform.localPosition + (GetBehindFaceDirection() * (ParentCubie.ParentCube.CubiesPerSide - 1));

            var cubies = ParentCubie.ParentCube.GetCubies();
            Cubie oppositeCubie = null;

            for(int i = 0; i < cubies.Count; i++){
                if(cubies[i] == ParentCubie){
                    continue;
                }
                if( (Vector3.Distance(offsetPosition, cubies[i].transform.localPosition) < ParentCubie.ParentCube.CubieDistance * 0.1f)){
                    
                    return cubies[i];
                }
                else if(( Vector3.Distance(offsetFarPosition, cubies[i].transform.localPosition) < ParentCubie.ParentCube.CubieDistance * 0.01f))
                {
                    oppositeCubie = cubies[i];
                }    
            }

            return oppositeCubie;
        }

        private Cubie GetDragNeighbour(out bool isOpposite){
            var dragOffsetPosition = GetDragOffsetLocalPosition(true);  
            var dragOppositeOffsetPosition = GetDragOffsetLocalPosition(false);  
            var cubies = ParentCubie.ParentCube.GetCubies();
            isOpposite = false;
            Cubie oppositeCubie = null;
            for(int i = 0; i < cubies.Count; i++){
                if( Vector3.Distance(dragOffsetPosition , cubies[i].transform.localPosition) < 0.1f ){
                    isOpposite = false;
                    return cubies[i];
                }else if ( Vector3.Distance(dragOppositeOffsetPosition, cubies[i].transform.localPosition) < 0.1f){
                    isOpposite = true;
                    oppositeCubie = cubies[i];
                }    
            }
            return oppositeCubie;
        }


        public override void ResetValues(){
            _dragStart = null;
            _dragStartHit = null;
            ParentCubie.DeselectCubie();
        }

        private RaycastHit GetMouseHit(){
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit); 
            return hit;
        }
        private RubixCube.ERubixAxis GetCommonRubixAxis(){
            if(_dragCubie.Index.x == _behindCubie.Index.x && ParentCubie.Index.x == _behindCubie.Index.x){
                return RubixCube.ERubixAxis.X;
            }else if (_dragCubie.Index.y == _behindCubie.Index.y && ParentCubie.Index.y == _behindCubie.Index.y){
                return RubixCube.ERubixAxis.Y;
            }
            return RubixCube.ERubixAxis.Z;
        }

        // This function is an abomination that needs to be destroyed
        private void SetDragModifier(RubixCube.Move move, bool isOppositeCubie){
            var dirA = ParentCubie.transform.position - ParentCubie.ParentCube.GetSelectedLayerTransform().position;
            var dirB = _dragCubie.transform.position - ParentCubie.ParentCube.GetSelectedLayerTransform().position;

            var signedAngle = Vector3.SignedAngle(dirA, dirB, move.GetMoveVector());
            _dragModifier = 1;
            if((signedAngle < 0 && _dragDistance >= 0f) || (signedAngle >= 0 && _dragDistance < 0f))
            {
               _dragModifier = -1;
            }
            if(isOppositeCubie){

               _dragModifier = -_dragModifier;
            }

        }
        void HandleDrag(){

            switch(_mouseDirection){
                case EMouseDirection.NONE:
                _mouseDirection = EMouseDirection.Y;
                _dragDistance = Input.mousePosition.y - _dragStart.Value.y;
                if(Mathf.Abs(Input.mousePosition.x - _dragStart.Value.x) > Mathf.Abs(_dragDistance)){
                    _mouseDirection = EMouseDirection.X;
                    _dragDistance = Input.mousePosition.x - _dragStart.Value.x;
                }
                break;
                case EMouseDirection.X:
                    _dragDistance = Input.mousePosition.x - _dragStart.Value.x;
                break;
                case EMouseDirection.Y:
                    _dragDistance = Input.mousePosition.y - _dragStart.Value.y;
                break;
            }
            if(Mathf.Abs(_dragDistance) <  ParentCubie.ParentCube.DragDeadzone){
                _mouseDirection = EMouseDirection.NONE;
            }
            if(ParentCubie.ParentCube.GetCubeState() != RubixCube.ECubeState.IDLE ||
                _mouseDirection == EMouseDirection.NONE){
                if(ParentCubie.ParentCube.GetCubeState() == RubixCube.ECubeState.MANUAL){
                    ParentCubie.ParentCube.ManualRotate(_dragDistance * _dragModifier); 
                }
                return;
            }

           _dragCubie = GetDragNeighbour(out bool isOpposite);
           if(_dragCubie == null){
               return;
           }
            var commonAxis = GetCommonRubixAxis();
            int layerIndex = ParentCubie.Index.z;
            switch(commonAxis){
                case RubixCube.ERubixAxis.X:
                layerIndex = ParentCubie.Index.x;
                break;
                case RubixCube.ERubixAxis.Y:
                layerIndex = ParentCubie.Index.y;
                break;
            }
            var newMove = new RubixCube.Move(layerIndex, commonAxis, true, false);
            SetDragModifier(newMove, isOpposite);
             ParentCubie.ParentCube.SetLayerMove(newMove); //isClockwise); //zDiff > 0);
            ParentCubie.ParentCube.ManualRotate(0f);
        }
        #region Mouse Events

        void OnMouseOver(){
            IsMouseOver = true;
        }
        void OnMouseExit(){
            IsMouseOver = false;
        }

        void OnMouseDown()
        {
            if(!ParentCubie.SelectCubie() || GameManager.Instance.CurrentState != GameManager.EGameStates.IN_GAME){
                return;
            }

            _mouseZ = Camera.main.WorldToScreenPoint( gameObject.transform.position).z;
            _dragStart = Input.mousePosition;// GetMouseAsWorldPoint(Input.mousePosition); // Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _dragStartHit = GetMouseHit().point;

            _behindCubie = GetBehindFaceCubie();
        }

        void OnMouseDrag()
        {
            if(_dragStartHit == null ) {
                return;
            }

            HandleDrag();
        }

        void OnMouseUp(){
            ResetValues();
            if(ParentCubie.ParentCube.GetCubeState() == RubixCube.ECubeState.MANUAL){
                ParentCubie.ParentCube.TriggerAutoRotate();
            }
        }
        
        #endregion Mouse Events
        
    }
}