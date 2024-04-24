using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : SingletoneBehaviour<PoolManager>
{
    public class Pool
    {
        // 생성할 오리지널 객체
        GameObject original;
        // Poolable 객체를 저장할 스택
        Stack<Poolable> stack = new Stack<Poolable>();
        public Transform root;

        public Pool(GameObject original, int count = 10)
        {
            this.original = original;

            root = new GameObject($"@Root_{original.name}").transform;

            for(int i = 0; i < count; i++)
            {
                Push(Create());
            }
        }

        // 객체 생성 
        Poolable Create()
        {
            GameObject go = Instantiate(original);
            go.name = original.name;
            go.transform.parent = root;
            return go.GetOrAddComponent<Poolable>();
        }

        // 객체 팝
        public Poolable Pop()
        {
            Poolable poolable; 
            if ( stack.Count > 0)
                poolable = stack.Pop();
            else
                poolable = Create();
        
            poolable.gameObject.SetActive(true);
            return poolable;
        }

        // 객체 푸쉬
        public void Push(Poolable poolable)
        {
            poolable.gameObject.SetActive(false);
            stack.Push(poolable);
        }

        public void Clear()
        {
            stack.Clear();
        }
    }

    Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

    public void DisableAllPool()
    {
        foreach(var pool in pools)
        {
            Poolable[] poolables = pool.Value.root.GetComponentsInChildren<Poolable>();
            foreach(Poolable p in poolables)
            {
                if(p.gameObject.activeSelf)
                    Push(p);
            }
        }
    }

    public void EnableAllPool()
    {
        foreach(var p in pools)
        {
            p.Value.root.gameObject.SetActive(true); 
        }
    }
    
    // 풀 생성(오리지널 객체, 초기 생성개수)
    public void CreatePool(GameObject original, int count = 10)
    {
        if(pools.ContainsKey(original.name))
            return;

        Pool pool = new Pool(original, count);
        pools.Add(original.name, pool);
    }

    public Poolable Pop(GameObject original)
    {
        if(!pools.ContainsKey(original.name))
            CreatePool(original);
        
        return pools[original.name].Pop();
    }

    public void Push(Poolable poolable)
    {
        if(!pools.ContainsKey(poolable.gameObject.name))
            CreatePool(poolable.gameObject);
        
        pools[poolable.gameObject.name].Push(poolable);   
    }

    public void Clear()
    {
        foreach(var p in pools)
            p.Value.Clear();
        
        pools.Clear();
    }
    
}

