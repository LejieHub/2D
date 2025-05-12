// KeyManager.cs
using UnityEngine;
using System.Collections.Generic;

public class KeyManager : MonoBehaviour
{
    private static KeyManager _instance;
    private HashSet<string> _collectedKeys = new HashSet<string>();

    public static KeyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 自动创建管理器对象
                GameObject obj = new GameObject("KeyManager");
                _instance = obj.AddComponent<KeyManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    // 添加钥匙到永久库存
    public void AddKey(string keyID)
    {
        if (!_collectedKeys.Contains(keyID))
        {
            _collectedKeys.Add(keyID);
            Debug.Log($"获得钥匙：{keyID} (当前钥匙数：{_collectedKeys.Count})");
        }
    }

    // 检查是否存在指定钥匙
    public bool CheckKey(string keyID)
    {
        bool hasKey = _collectedKeys.Contains(keyID);
        Debug.Log($"检查钥匙：{keyID} => {hasKey}");
        return hasKey;
    }

    // 清空所有钥匙（用于重新开始游戏）
    public void ResetKeys()
    {
        _collectedKeys.Clear();
    }
}