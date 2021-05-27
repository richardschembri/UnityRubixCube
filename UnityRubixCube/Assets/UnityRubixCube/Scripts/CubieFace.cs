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
        private Vector3 _offset;
        private List<Cubie> _cubieNeighbours = null;
        
        Cubie _closestCubie;
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

        public override void ResetValues(){
            _dragStart = null;
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

        void OnMouseDown()
        {
            if(!ParentCubie.SelectCubie() || GameManager.Instance.CurrentState != GameManager.EGameStates.IN_GAME){
                return;
            }
            _mouseZ = Camera.main.WorldToScreenPoint( gameObject.transform.position).z;
            _dragStart = Input.mousePosition;
            _offset = gameObject.transform.position - GetMouseAsWorldPoint(_dragStart.Value);


            _cubieNeighbours = ParentCubie.GetNeighbours();
        }
        void OnMouseDrag()
        {
            if(_dragStart == null){
                return;
            }

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
            if( ParentCubie.ParentCube.GetCubeState() != RubixCube.ECubeState.IDLE
                || _mouseDirection == EMouseDirection.NONE
                || _cubieNeighbours == null
                || _cubieNeighbours.Count <= 0){
                    if(ParentCubie.ParentCube.GetCubeState() == RubixCube.ECubeState.MANUAL){
                        ParentCubie.ParentCube.ManualRotate(_dragDistance ); 
                    }
                    return;
            }

            /*
            _closestCubie = _cubieNeighbours[0];
            _mouseWorldPoint = GetMouseAsWorldPoint(Input.mousePosition) + _offset;
            for(int i = 1; i < _cubieNeighbours.Count; i++){
               if(Vector3.Distance(_cubieNeighbours[i].transform.position, _mouseWorldPoint) < 
                   Vector3.Distance(_closestCubie.transform.position, _mouseWorldPoint))
                {
                    _closestCubie = _cubieNeighbours[i];
                    break;
                }
            }
            */
            _closestCubie = null;
            for(int i = 0; i < _cubieNeighbours.Count; i++){
                if(_cubieNeighbours[i].IsMouseOver())
                {
                    _closestCubie = _cubieNeighbours[i];
                    break;
                }
            }
            if(_closestCubie == null){
                return;
            }

            int xDiff = _closestCubie.Index.x - ParentCubie.Index.x;
            int yDiff = _closestCubie.Index.y - ParentCubie.Index.y;
            int zDiff = _closestCubie.Index.z - ParentCubie.Index.z;
            /*
            if(!ParentCubie.IsEdge(RubixCube.ERubixAxis.X) && _closestCubie.Index.x == ParentCubie.Index.x){
                if(_closestCubie.Index.y == ParentCubie.Index.y){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.x, RubixCube.ERubixAxis.X, zDiff > 0);
                    ParentCubie.ParentCube.ManualRotate(0f);
                    Debug.Log("Set　Layer Move");
                }
            }else if(!ParentCubie.IsEdge(RubixCube.ERubixAxis.Y) && _closestCubie.Index.y == ParentCubie.Index.y){
                if(_closestCubie.Index.x == ParentCubie.Index.x){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.y, RubixCube.ERubixAxis.Y, zDiff > 0);
                    ParentCubie.ParentCube.ManualRotate(0f);
                    Debug.Log("Set　Layer Move");
                }
            }else if(!ParentCubie.IsEdge(RubixCube.ERubixAxis.Z) && _closestCubie.Index.z == ParentCubie.Index.z){
                if(_closestCubie.Index.y == ParentCubie.Index.y){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.z, RubixCube.ERubixAxis.Z, xDiff > 0);
                    ParentCubie.ParentCube.ManualRotate(0f);
                    Debug.Log("Set　Layer Move");
                }
            }
            */
            if(_closestCubie.Index.x == ParentCubie.Index.x && _closestCubie.Index.y == ParentCubie.Index.y){
                if(ParentCubie.IsEdge(RubixCube.ERubixAxis.Y)){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.x, RubixCube.ERubixAxis.X, zDiff > 0);
                }else{
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.y, RubixCube.ERubixAxis.Y, zDiff > 0);
                }
            }else if(_closestCubie.Index.y == ParentCubie.Index.y && _closestCubie.Index.z == ParentCubie.Index.z){
                if(ParentCubie.IsEdge(RubixCube.ERubixAxis.Y)){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.z, RubixCube.ERubixAxis.Z, -xDiff > 0);
                }else{
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.y, RubixCube.ERubixAxis.Y, xDiff > 0);
                }
            }else if(_closestCubie.Index.z == ParentCubie.Index.z && _closestCubie.Index.x == ParentCubie.Index.x){
                if(ParentCubie.IsEdge(RubixCube.ERubixAxis.X)){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.z, RubixCube.ERubixAxis.Z, -yDiff  > 0);
                }else if(ParentCubie.IsEdge(RubixCube.ERubixAxis.Z)){
                    ParentCubie.ParentCube.SetLayerMove(ParentCubie.Index.x, RubixCube.ERubixAxis.X, (ParentCubie.Index.z == 0 ? -yDiff : yDiff) > 0);
                }
            }
                ParentCubie.ParentCube.ManualRotate(0f);
            Debug.Log($"Closest Cubie {_closestCubie.transform.name} to {ParentCubie.gameObject.name}");

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