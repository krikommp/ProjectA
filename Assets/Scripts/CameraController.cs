using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Client
{
    public class CameraController : MonoBehaviour
    {
        public Transform target; // 玩家的Transform组件
        public float smoothing = 5f; // 相机跟随的平滑系数
        [FormerlySerializedAs("YOffset")] public float yOffset = 0.0f;

        private Vector3 offset; // 相机与玩家的距离


        void Start()
        {
            offset = transform.position - target.position;
            offset.y += yOffset;
        }

        void FixedUpdate()
        {
            Vector3 targetCamPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }
}