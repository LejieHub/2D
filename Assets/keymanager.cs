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
                // �Զ���������������
                GameObject obj = new GameObject("KeyManager");
                _instance = obj.AddComponent<KeyManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    // ���Կ�׵����ÿ��
    public void AddKey(string keyID)
    {
        if (!_collectedKeys.Contains(keyID))
        {
            _collectedKeys.Add(keyID);
            Debug.Log($"���Կ�ף�{keyID} (��ǰԿ������{_collectedKeys.Count})");
        }
    }

    // ����Ƿ����ָ��Կ��
    public bool CheckKey(string keyID)
    {
        bool hasKey = _collectedKeys.Contains(keyID);
        Debug.Log($"���Կ�ף�{keyID} => {hasKey}");
        return hasKey;
    }

    // �������Կ�ף��������¿�ʼ��Ϸ��
    public void ResetKeys()
    {
        _collectedKeys.Clear();
    }
}