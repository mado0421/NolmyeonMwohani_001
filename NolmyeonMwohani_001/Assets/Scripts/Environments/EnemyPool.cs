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
    [SerializeField] private float _numBurst = 3;   // �� ���� �� ���� ��ȯ�� ���ΰ�
    [SerializeField] private float _spawnDistance = 40;

    private Vector3 _spawnDistanceVector;

    public void Start()
    {
        // �÷��̾� ĳ���͸� ã�Ƶд�. �̰� ���� �ν��Ͻ��� �ٲٴ°� ���ƺ��̴µ�
        _player = GameObject.FindGameObjectWithTag("Player");
        _spawnDistanceVector = new Vector3(0, 0, _spawnDistance);

        // �� ĳ���͵��� �̸� �����Ѵ�
        for (uint i = 0; i < _numMaxEnemies; i++)
        {
            GameObject temp = Instantiate(_enemy);
            temp.transform.SetParent(transform);
            temp.SetActive(false);
            _pool.Add(temp);
        }
        StartCoroutine(SpawnDelay());
    }

    // ���� �ð����� ȭ�� �ۿ� ��ȯ�Ѵ�.
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
            // ��ȯ�� ��ġ�� ���Ѵ�.
            // �÷��̾� �������κ��� �����Ÿ� �̻� ������ ���� �����ϸ� �� �� ����.
            // �÷��̾� ĳ���� �����ǿ� (���⺤�� * distance)���͸� 360�� ������
            // ���Ƿ� ȸ����Ų �� ���ϸ� �� �� ����.
            InitializeEnemy(FindInactiveEnemy());
        }
    }

}
