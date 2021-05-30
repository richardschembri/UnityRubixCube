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

        private float _mouseZ;

        private Vector3? _dragStart = null;
        private Vector3? _dragStartHit = null;
        private Vector3 _offset;
        private List<Cubie> _cubieNeighbours = null;
        
        Cubie _closestCubie;
        Cubie _behindCubie;
        Cubie _dragCubie;
        Vector3 _mouseWorldPoint;

        public bool IsMouseOver {get; private set;}= false;

        float _dragDistance;

        protected override void InitComponents()
        {
            base.InitComponents();
            ParentCubie = transform.parent.GetComponent<Cubie>();
        }

        private Vector3 GetMouseAsWorldPoint(Vector3 mousePosition)
        {

            Vector3 mousePosition3D = mousePosition;

            mousePosition3D.z = _mouseZ;

            return Camera.main.ScreenToWorldPoint(mousePosition3D);

        }

        private Vector3 GetDragDirection(){
            var _dragEnd3D = GetMouseHitPosition();
            return (_dragEnd3D - _dragStartHit.Value).normalized * ParentCubie.ParentCube.CubieDistance;
        }

        private Vector3 GetBehindFaceDirection(){
            return (ParentCubie.transform.position - transform.position).normalized * ParentCubie.ParentCube.CubieDistance;
        }

        private Vector3 GetDragOffsetPosition(bool oppsite){
            if(oppsite){
                return ParentCubie.transform.localPosition - GetDragDirection();
            }else{
                return ParentCubie.transform.localPosition + GetDragDirection();
            }
        }

/*
        private Cubie GetBehindFaceNeighbour(){
            var offsetPosition = ParentCubie.transform.localPosition + GetBehindFaceDirection();

            for(int i = 0; i < _cubieNeighbours.Count; i++){
                if( Vector3.Distance(offsetPosition, _cubieNeighbours[i].transform.localPosition) < ParentCubie.ParentCube.CubieDistance * 1.01f){
                    return _cubieNeighbours[i];
                }    
            }
            return null;
        }
*/
        private Cubie GetBehindFaceCubie(){
            var offsetPosition = ParentCubie.transform.localPosition + GetBehindFaceDirection();
            var offsetFarPosition = ParentCubie.transform.localPosition + (GetBehindFaceDirection() * (ParentCubie.ParentCube.CubiesPerSide - 1));

            var cubies = ParentCubie.ParentCube.GetCubies();
            Cubie oppositeCubie = null;

            // Debug.Log($"Cubie Pos: {ParentCubie.transform.position} - Offset Pos:{offsetPosition}");
            for(int i = 0; i < cubies.Count; i++){
                if(cubies[i] == ParentCubie){
                    continue;
                }
                if( (Vector3.Distance(offsetPosition, cubies[i].transform.localPosition) < ParentCubie.ParentCube.CubieDistance * 0.1f)){
                    
                    // Debug.Log($"Neighbour Pos: {cubies[i].transform.localPosition} {Vector3.Distance(offsetPosition, cubies[i].transform.localPosition)}");
                    return cubies[i];
                }
                else if(( Vector3.Distance(offsetFarPosition, cubies[i].transform.localPosition) < ParentCubie.ParentCube.CubieDistance * 0.01f))
                {
                    oppositeCubie = cubies[i];
                }    
            }

            // Debug.Log($"{ParentCubie.transform.localPosition} -> {offsetFarPosition} {(GetBehindFaceDirection() * ParentCubie.ParentCube.CubieDistance * ParentCubie.ParentCube.CubiesPerSide)}");
            return oppositeCubie;
        }
        /*
        private Cubie GetDragNeighbour(bool opposite){
            var dragOffsetPosition = GetDragOffsetPosition(opposite);  

            for(int i = 1; i < _cubieNeighbours.Count; i++){
                if( Vector3.Distance(dragOffsetPosition , _cubieNeighbours[i].transform.localPosition) < 0.1f ){
                    return _cubieNeighbours[i];
                }    
            }
            return null;
        }
        */

        private Cubie GetDragNeighbour(out bool isOpposite){
            var dragOffsetPosition = GetDragOffsetPosition(true);  
            var dragOppositeOffsetPosition = GetDragOffsetPosition(false);  
            var cubies = ParentCubie.ParentCube.GetCubies();
            isOpposite = false;
            for(int i = 0; i < cubies.Count; i++){
                if( Vector3.Distance(dragOffsetPosition , cubies[i].transform.localPosition) < 0.1f ){
                    return cubies[i];
                }else if ( Vector3.Distance(dragOppositeOffsetPosition, cubies[i].transform.localPosition) < 0.1f){
                    isOpposite = true;
                    return cubies[i];
                }    
            }
            return null;
        }


        public override void ResetValues(){
            _dragStart = null;
            _dragStartHit = null;
            _cubieNeighbours = null;
            ParentCubie.DeselectCubie();
        }

        #region Mouse Events

        void OnMouseOver(){
            IsMouseOver = true;
        }
        void OnMouseExit(){
            IsMouseOver = false;
        }

        public Vector3? GetNormal(){
            var ray = new Ray(Camera.main.transform.position, this.transform.position - Camera.main.transform.position); // Camera.main.ScreenPointToRay(this.transform.position);
            if (!Physics.Raycast(ray, out RaycastHit hit) || hit.transform != this)
                return null;
            return hit.normal;
        }

        private Vector3 GetMouseHitPosition(){
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit); 
            return hit.point;
        }

        void OnMouseDown()
        {
            if(!ParentCubie.SelectCubie() || GameManager.Instance.CurrentState != GameManager.EGameStates.IN_GAME){
                return;
            }

            _mouseZ = Camera.main.WorldToScreenPoint( gameObject.transform.position).z;
            _dragStart = Input.mousePosition;// GetMouseAsWorldPoint(Input.mousePosition); // Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _dragStartHit = GetMouseHitPosition();
            _offset = gameObject.transform.position - GetMouseAsWorldPoint(_dragStart.Value);

            _cubieNeighbours = ParentCubie.GetNeighbours();
            _behindCubie = GetBehindFaceCubie();
            Debug.Log($"{ParentCubie.name}/{GetBehindFaceCubie().name}");
        }
        float _dragModifier = 1;

        private RubixCube.ERubixAxis GetCommonRubixAxis(){
            if(_dragCubie.Index.x == _behindCubie.Index.x && ParentCubie.Index.x == _behindCubie.Index.x){
                return RubixCube.ERubixAxis.X;
            }else if (_dragCubie.Index.y == _behindCubie.Index.y && ParentCubie.Index.y == _behindCubie.Index.y){
                return RubixCube.ERubixAxis.Y;
            }
            return RubixCube.ERubixAxis.Z;
        }
        private RubixCube.ERubixAxis GetRotationRubixAxis(){
            var commonAxis = GetCommonRubixAxis();
            if(_dragCubie.Index.x == ParentCubie.Index.x && commonAxis != RubixCube.ERubixAxis.X){
                return RubixCube.ERubixAxis.X;
            }else if (_dragCubie.Index.y == ParentCubie.Index.y &&  commonAxis != RubixCube.ERubixAxis.Y){
                return RubixCube.ERubixAxis.Y;
            }
            return RubixCube.ERubixAxis.Z;
        }

        void HandleDrag(){

            switch(_mouseDirection){
                case EMouseDirection.NONE:
                _mouseDirection = EMouseDirection.Y;
                _dragDistance = Input.mousePosition.y - _dragStart.Value.y;
                if(Mathf.Abs(Input.mousePosition.x - _dragStart.Value.x) > _dragDistance){
                    _mouseDirection = EMouseDirection.X;
                    _dragDistance = Input.mousePosition.x - _dragStart.Value.x;
                }

                if(Mathf.Abs(_dragDistance) < ParentCubie.ParentCube.DragTreshold){
                    _mouseDirection = EMouseDirection.NONE;
                }
                break;
                case EMouseDirection.X:
                    _dragDistance = Input.mousePosition.x - _dragStart.Value.x;
                break;
                case EMouseDirection.Y:
                    _dragDistance = Input.mousePosition.y - _dragStart.Value.y;
                break;
            }

            if(ParentCubie.ParentCube.GetCubeState() != RubixCube.ECubeState.IDLE){
                if(ParentCubie.ParentCube.GetCubeState() == RubixCube.ECubeState.MANUAL){
                    ParentCubie.ParentCube.ManualRotate(_dragDistance * _dragModifier); 
                }
                return;
            }

            if(Mathf.Abs(_dragDistance) < ParentCubie.ParentCube.CubieDistance / 3) {
                return;
            }

           _dragCubie = GetDragNeighbour(out bool isOpposite);
           if(_dragCubie == null){
               return;
           }
            Debug.Log($"{ParentCubie.ParentCube.GetCubeState()} {ParentCubie.name} -> {_dragCubie.name}");
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
            // Debug.Log($"commonAxis: {commonAxis} directionRubixAxis: {directionRubixAxis} ");
            ParentCubie.ParentCube.SetLayerMove(layerIndex, commonAxis, true); //zDiff > 0);
            ParentCubie.ParentCube.ManualRotate(0f);
        }
        void OnMouseDrag()
        {

            if(_dragStartHit == null ) {
                return;
            }

            HandleDrag();
        }
        private bool IsClockwise(RubixCube.ERubixAxis axis, int selfIndex, int neighbourIndex, RubixCube.ERubixAxis edgeAxis){
            bool result = true;
            switch(edgeAxis){
                case RubixCube.ERubixAxis.X:
                    if(axis == RubixCube.ERubixAxis.Y){
                        // y:1 -> y:2  (clockwise)
                        result = neighbourIndex - selfIndex > 0;
                    }else if (axis == RubixCube.ERubixAxis.Z){
                        // z:1 -> z:2  (anticlockwise)
                        result = neighbourIndex - selfIndex < 0;
                    }
                    break;
                case RubixCube.ERubixAxis.Y:
                    if(axis == RubixCube.ERubixAxis.X){
                        // x:1 -> x:2  (anticlockwise)
                        result = neighbourIndex - selfIndex < 0;
                    }else if (axis == RubixCube.ERubixAxis.Z){
                        // z:1 -> z:2  (clockwise)
                        result = neighbourIndex - selfIndex > 0;
                    }
                    break;
                case RubixCube.ERubixAxis.Z:
                    if(axis == RubixCube.ERubixAxis.X){
                        // x:1 -> x:2  (clockwise)
                        result = neighbourIndex - selfIndex > 0;
                    }else if (axis == RubixCube.ERubixAxis.Y){
                        // y:1 -> y:2  (anticlockwise)
                        result = neighbourIndex - selfIndex < 0;
                    }
                    break;

            }
            if(ParentCubie.IsTop(edgeAxis)){
                return result;
            }

            return !result;
        }
        private bool IsClockwise(RubixCube.ERubixAxis axis, RubixCube.ERubixAxis edgeAxis){
            return IsClockwise(axis, ParentCubie.Index.x, _closestCubie.Index.x, edgeAxis);
        }

        void OnMouseUp(){
            ResetValues();
            if(ParentCubie.ParentCube.GetCubeState() == RubixCube.ECubeState.MANUAL){
                ParentCubie.ParentCube.TriggerAutoRotate();
            }
        }

        Vector2 ScreenDirection(Vector2 screenPoint, Vector3 worldPoint, Vector3 worldDirection) {
            Vector2 shifted = Camera.main.WorldToScreenPoint(worldPoint + worldDirection);

            return (shifted - screenPoint).normalized;
        }

        float GetDragDistance(Vector2 dragStart, Vector2 dragCurrent, Vector2 dragDirection) {
            return Vector2.Dot(dragCurrent - dragStart, dragDirection);
        }

        /*
        public RubixCube.ECubeFace GetCubeFace(){

        }
        */
        
        #endregion Mouse Events
        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if(!ParentCubie.IsCubieSelected() || _dragStart == null){
                return;
            }

            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawLine(Camera.main.ScreenToWorldPoint(_dragStart.Value),
                                            Camera.main.ScreenToWorldPoint(Input.mousePosition));
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}