using Cinemachine;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer // 新的图层配置类
    {
        public Transform layerTransform;
        [Range(0f, 1f)]
        public float horizontalFactor = 0.5f;
        [Range(0f, 1f)]
        public float verticalFactor = 0f; // 默认垂直方向不移动
    }

    public CinemachineVirtualCamera virtualCamera;
    public ParallaxLayer foreground; // 使用新结构代替Transform
    public ParallaxLayer midground;
    public ParallaxLayer background;

    private Vector3 lastCamPosition;

    void Start()
    {
        if (virtualCamera != null)
        {
            lastCamPosition = virtualCamera.transform.position;
        }
        else
        {
            Debug.LogError("Virtual Camera is not assigned to ParallaxManager");
            enabled = false;
        }
    }

    void LateUpdate()
    {
        Vector3 delta = virtualCamera.transform.position - lastCamPosition;

        // 分别应用视差效果到每个图层
        ApplyParallaxToLayer(ref foreground, delta);
        ApplyParallaxToLayer(ref midground, delta);
        ApplyParallaxToLayer(ref background, delta);

        lastCamPosition = virtualCamera.transform.position;
    }

    private void ApplyParallaxToLayer(ref ParallaxLayer layer, Vector3 delta)
    {
        if (layer.layerTransform != null)
        {
            layer.layerTransform.position += new Vector3(
                delta.x * layer.horizontalFactor,
                delta.y * layer.verticalFactor,
                0
            );
        }
    }

#if UNITY_EDITOR
    // 编辑器脚本，便于在编辑器中调整参数
    void OnValidate()
    {
        // 确保垂直因子为0（固定）
        background.verticalFactor = 0;
        midground.verticalFactor = 0;
        
        // 保持背景移动性最弱
        //if (background.horizontalFactor > midground.horizontalFactor)
       // {
          //  background.horizontalFactor = midground.horizontalFactor * 0.5f;
       // }
    }
#endif
}