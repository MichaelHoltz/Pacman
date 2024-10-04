using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodes : MonoBehaviour
{

    private int _numToSpawn = 28;
    [SerializeField] private float _spawnOffset = 0.3f;
    [SerializeField] private float _currentSpawnOffset ;
    // Start is called before the first frame update
    void Start()
    {
        //trick to spawn multiple nodes



        if (gameObject.name == "Node" && gameObject.transform.childCount == 0)
        {
            Destroy(gameObject);
        }
        return;
        //    _currentSpawnOffset = _spawnOffset;
        //    for (int i = 0; i < _numToSpawn; i++)
        //    {
        //        //Clone a new node
        //        //GameObject node = Instantiate(gameObject, new Vector3(transform.position.x + Random.Range(-_spawnOffset, _spawnOffset), transform.position.y + Random.Range(-_spawnOffset, _spawnOffset), transform.position.z + Random.Range(-_spawnOffset, _spawnOffset)), Quaternion.identity);
        //        GameObject node = Instantiate(gameObject, new Vector3(transform.position.x , transform.position.y + _currentSpawnOffset, 0), Quaternion.identity);
        //        _currentSpawnOffset += _spawnOffset;
        //        //node.transform.parent = transform.parent;
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
