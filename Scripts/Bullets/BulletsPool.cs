using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletsPool : MonoBehaviour
{
    public static BulletsPool instance;
    public Dictionary<BulletTypes, List<GameObject>> PooledObjects = new Dictionary<BulletTypes, List<GameObject>>(); 
    public ObjectPool[] objectPools;

    [SerializeField] Transform myTransform;

    private void Awake()
    {
        instance = this;

        foreach (ObjectPool _Pool in objectPools)
        {
            List<GameObject> _PooledObj = new List<GameObject>();
            GameObject tmp;
            for (int i = 0; i < _Pool.amountToPool; i++)
            {
                tmp = Instantiate(_Pool.objectPb, myTransform);
                tmp.SetActive(false);
                _PooledObj.Add(tmp);
            }
            PooledObjects.Add(_Pool.bulletType, _PooledObj);
        }
    }

    public GameObject GetPooledGameObject(BulletTypes _bulletTypes)
    {
        List<GameObject> _ObjToGet = PooledObjects[_bulletTypes];
        for(int i = 0; i < _ObjToGet.Count; i++)
        {
            if (!_ObjToGet[i].activeInHierarchy)
            {
                return _ObjToGet[i];
            }
        }
        return null;
    }

    public void ReturnToPool(GameObject _gameObject)
    {
        
        _gameObject.transform.SetParent(myTransform);
        _gameObject.SetActive(false);
    }
}

[Serializable]
public class ObjectPool
{
    public GameObject objectPb;
    public int amountToPool;
    public BulletTypes bulletType;
}

