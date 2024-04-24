using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    public GameObject zombiePrefab;
    GameObject bulletPrefab;  
    Transform root;

    int zombieCount = 5;
    int bulletCount = 5;

    public Dictionary<string, Stack<GameObject>> poolDict = new Dictionary<string, Stack<GameObject>>();

    public void Init()
    {
        if(root == null)
        {
            root = new GameObject("@Poolable").transform;
        }    

        zombiePrefab = Resources.Load<GameObject>("Prefabs/Zombies/Zombie");
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        
        CreatePool(zombiePrefab, zombieCount);
        CreatePool(bulletPrefab, bulletCount);
    }

    void CreatePool(GameObject original, int count = 10)
    {
        if(original.GetComponent<IPoolable>() != null)
        {
            string poolName = original.GetComponent<IPoolable>().Type.ToString();
            //Debug.Log("poolName" + poolName);
            Stack<GameObject> newStack = new Stack<GameObject>();
            poolDict.Add(poolName, newStack);

            for (int i = 0; i < count; i ++)
            {
                GameObject go = CreatePoolable(original);
                //poolDict[poolName].Push(go);
                Push(go);
            }
        }
    }

    public void Push(GameObject poolable) 
    {
        if(poolable.GetComponent<IPoolable>() == null)
            return;

        string poolName = poolable.GetComponent<IPoolable>().Type.ToString();
        if(poolDict.ContainsKey(poolName))
        {
            //Debug.Log("Push : " + _poolable.gameObject.name);
            poolable.transform.SetParent(root);
            poolable.gameObject.SetActive(false);
            poolDict[poolName].Push(poolable);
        }        
    }

    public GameObject Pop(Define.PoolableType type)
    {
        GameObject newObject = null;
        string poolName = type.ToString();
        if (poolDict[poolName].Count > 0)
        {
            newObject = poolDict[poolName].Pop();
            //Debug.Log("Pop : " + newObject.name);
            newObject.GetComponent<Collider>().enabled = true;
        }
        else
        {
            //Debug.Log("CreatePoolable : " + _type);
            switch(type)
            {
                case Define.PoolableType.Bullet:
                    newObject = CreatePoolable(bulletPrefab);
                    break;
                case Define.PoolableType.Zombie:
                    newObject = CreatePoolable(zombiePrefab);
                    break;
            }
        }
        newObject.transform.parent = null;
        newObject.SetActive(true);
        return newObject;
    }

    public GameObject CreatePoolable(GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab, root);
        go.name = prefab.name;
        return go;
    }

    public void Clear()
    {
        poolDict.Clear();
    }

}
