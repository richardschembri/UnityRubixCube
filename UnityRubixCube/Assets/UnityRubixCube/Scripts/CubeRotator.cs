using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRubixCube {
    public class CubeRotator : MonoBehaviour
    {
        enum SwipeDirection{
            UP, DOWN, LEFT, RIGHT, NONE
        }

        class SwipePosition{
            public Vector2 Start;
            Vector2 _end;
            public Vector2 End{
                get{
                    return _end;
                }set{
                    _end = value;
                    Travel = (new Vector2(End.x - Start.x, End.y - Start.y));
                    Travel.Normalize();
                }
            }

            public Vector2 Travel{ get; private set; }

            public SwipePosition(){
                Start = Vector3.zero;
                End = Vector3.zero;
            }
        }

        SwipePosition _cubeSwipePosition = new SwipePosition();
        [SerializeField]
        Transform target;

        void SwipeListener(){
            if(Input.GetMouseButtonDown(1)){
                _cubeSwipePosition.Start = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Debug.Log(_cubeSwipePosition.Start);
            } else if(Input.GetMouseButtonUp(1)){
                _cubeSwipePosition.End = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                switch(GetSwipeDirection()){
                    case SwipeDirection.LEFT:
                        target.Rotate(0, 90, 0, Space.World);              
                        break;
                    case SwipeDirection.RIGHT:
                        target.Rotate(0, -90, 0, Space.World);              
                        break;
                }
            }


        }

        bool IsSwipeInTresholdY(){
            return _cubeSwipePosition.Travel.y > 0.5f && _cubeSwipePosition.Travel.y > 0.5f;
        }

        SwipeDirection GetSwipeDirection(){
            if(_cubeSwipePosition.Travel.x < 0 && IsSwipeInTresholdY()){
                return SwipeDirection.LEFT;
            }else if(_cubeSwipePosition.Travel.x > 0 && IsSwipeInTresholdY()){
                return SwipeDirection.RIGHT;
            }

            return SwipeDirection.NONE;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}