using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRubixCube.Utils{
    public class JsonUtils
    {
        public static T[] FromJson<T>(string json)
        {
            JsonArrayWrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<JsonArrayWrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            JsonArrayWrapper<T> wrapper = new JsonArrayWrapper<T>();
            wrapper.Items = array;
            return UnityEngine.JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class JsonArrayWrapper<T>
        {
            public T[] Items;
        }
    }
}
