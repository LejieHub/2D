using Cinemachine;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer // �µ�ͼ��������
    {
        public Transform layerTransform;
        [Range(0f, 1f)]
        public float horizontalFactor = 0.5f;
        [Range(0f, 1f)]
        public float verticalFactor = 0f; // Ĭ�ϴ�ֱ�����ƶ�
    }

    public CinemachineVirtualCamera virtualCamera;
    public ParallaxLayer foreground; // ʹ���½ṹ����Transform
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

        // �ֱ�Ӧ���Ӳ�Ч����ÿ��ͼ��
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
    // �༭���ű��������ڱ༭���е�������
    void OnValidate()
    {
        // ȷ����ֱ����Ϊ0���̶���
        background.verticalFactor = 0;
        midground.verticalFactor = 0;
        
        // ���ֱ����ƶ�������
        //if (background.horizontalFactor > midground.horizontalFactor)
       // {
          //  background.horizontalFactor = midground.horizontalFactor * 0.5f;
       // }
    }
#endif
}