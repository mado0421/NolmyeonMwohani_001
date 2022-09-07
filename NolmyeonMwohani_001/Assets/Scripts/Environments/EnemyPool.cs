using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _enemy;
    [SerializeField] private List<GameObject> _pool;

    [SerializeField] private uint _numMaxEnemies = 100;
    [SerializeField] private float _spawnCooltime = 5;
    [SerializeField] private float _numBurst = 3;   // 한 번에 몇 개를 소환할 것인가
    [SerializeField] private float _spawnDistance = 40;

    private Vector3 _spawnDistanceVector;

    public void Start()
    {
        // 플레이어 캐릭터를 찾아둔다. 이거 전역 인스턴스로 바꾸는게 나아보이는데
        _player = GameObject.FindGameObjectWithTag("Player");
        _spawnDistanceVector = new Vector3(0, 0, _spawnDistance);

        // 적 캐릭터들을 미리 생성한다
        for (uint i = 0; i < _numMaxEnemies; i++)
        {
            GameObject temp = Instantiate(_enemy);
            temp.transform.SetParent(transform);
            temp.SetActive(false);
            _pool.Add(temp);
        }
        StartCoroutine(SpawnDelay());
    }

    // 일정 시간마다 화면 밖에 소환한다.
    private IEnumerator SpawnDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnCooltime);
            SpawnPrefabs();
        }
    }
    private GameObject FindInactiveEnemy()
    {
        foreach (GameObject temp in _pool)
            if (!temp.activeSelf) return temp;
        return null;
    }
    private Vector3 GetSpawnPosition()
    {
        return _player.transform.position + Quaternion.Euler(0, Random.value * 360, 0) * _spawnDistanceVector;
    }
    private void InitializeEnemy(GameObject obj)
    {
        if (obj == null) return;
        obj.transform.position = GetSpawnPosition();
        obj.SetActive(true);
    }
    private void SpawnPrefabs()
    {
        for(uint i = 0; i < _numBurst; i++)
        {
            // 소환할 위치를 구한다.
            // 플레이어 방향으로부터 일정거리 이상 떨어진 곳에 생성하면 될 것 같다.
            // 플레이어 캐릭터 포지션에 (방향벡터 * distance)벡터를 360도 내에서
            // 임의로 회전시킨 후 더하면 될 것 같다.
            InitializeEnemy(FindInactiveEnemy());
        }
    }

}
