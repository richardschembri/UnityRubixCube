using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace UnityRubixCUbe{
    public class CubeRotator : MonoBehaviour
    {
        enum SwipeDirection{
            UP, DOWN, LEFT, RIGHT
        }
        struct SwipePosition{
            public Vector2 Start;
            public Vector2 End;
            public Vector2 Current;
        }

        SwipePosition _cubeSwipePosition;

        void SwipeListener(){
            if(Input.GetMouseButtonDown(1)){
                _cubeSwipePosition.Start = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Debug.Log(_cubeSwipePosition.Start);
            }
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