# NolmyeonMwohani_001
### NolmyeonMwohani_001 / 쿼터뷰 로그라이크 슈팅게임

#### 2022-09-05
게임 기획하기
- 쿼터뷰 슈팅 로그라이크
- 플레이어는 보는 방향 회전을 통한 캐릭터의 이동 방향 전환과 순간 가속, 커서 방향으로 공격, 이 세가지 조작만 가능하다.
- 적은 화면 바깥에서 끊임없이 생성되며 플레이어 캐릭터를 향해 이동, 공격 등을 한다.
- 적을 처치하면 **골드**와 **경험치**, **아이템**을 얻을 수 있다.
	- **골드**는 메인게임 **밖**에서 플레이어 캐릭터의 **영구적 성장**을 위해 사용된다.
	- **경험치**는 메인게임 **안**에서 플레이어 캐릭터의 **일시적 성장**을 위해 사용된다.
	- **아이템**은 메인게임 **안**에서 플레이어 캐릭터에게 **일시적으로 도움을 주는 컴패니언을 선택**할 때 사용된다.
- 미니게임인 만큼, 메인게임에 시간제한을 두고 버티거나 시간 내에 최종보스를 처치하는 것을 목표로 한다.
	- 또는 제한 시간에 도달할 경우, 무조건 최종보스전으로 이동하게 하는 기믹도 괜찮을 것 같다.
- 플레이어 캐릭터의 플레이 컨셉을 다양하게 주고 싶으나 일단 간단하게 하나만 우선 구현하는 것을 목표로 한다.
	- 가장 중점이 되는 것은 데리고 다니는 **컴패니언의 강화**.

지금 당장 필요한 것들을 생각해보자.
	
	 - 플레이어 캐릭터를 이동, 공격, 조종해야 한다.
	 - 적 캐릭터도 이동, 공격 등을 해야 한다.
		 - 캐릭터 클래스를 두고 상속받게 하자. 이동(), 공격(), 체력, 이동속도, 공격력(플레이어 캐릭터는 무기같은걸 추가할 수도 있겠다).
	 - 공격 방식은 근접해서 공격하는 것과 투사체를 발사하는 것, 크게 두 가지로 나뉜다.
		 - 투사체를 발사하는 방식은 투사체와 비슷하게 생긴 충돌체를 추가해서 충돌검사를 하면 된다.
		 - 근접해서 공격하는 방식은 공격하는 주체 앞에 히트박스 역할을 할 충돌체를 두고, 껐다 켰다 하며 충돌검사를 하면 어떨까?

씬은 다음과 같이 둔다.

	  크게 Title, MainGame, Lobby, Result, 이렇게 4개의 씬을 준비한다.
	Title -> Lobby -> MainGame -> Result -> Lobby

입력은 다음처럼 받는다.

	플레이어 캐릭터의 회전: A, D
	플레이어 캐릭터의 가속: Space(또는 Mouse Right Button)
	플레이어 캐릭터의 공격: Mouse Left Button
	
[[Unity] 키 입력 받는 방법 세 가지 비교 (tistory.com)](https://unity-programmer.tistory.com/9) 이 곳의 세 번째 방법을 참고하여 작성하되, GetKeyDown()을 사용하면 눌린 프레임에만 함수 호출이 되고 그 뒤로는 안돼서 GetKey()로 변경하였다. 
```C#
public new void Start()
{
    base.Start();
    _keyDic = new Dictionary<KeyCode, Action>
    {
        { KeyCode.A, RotateLeft },
        { KeyCode.D, RotateRight },
        { KeyCode.Mouse0, Attack },
        { KeyCode.Mouse1, Accelerate },
    }; 
}

private new void Update()
{
    base.Update();
       
    if (Input.anyKey)
        foreach(var dic in _keyDic)
            if(Input.GetKey(dic.Key)) dic.Value();
}

private void RotateLeft() { transform.Rotate(0, Time.deltaTime * -_rotateSpd, 0); }
private void RotateRight() { transform.Rotate(0, Time.deltaTime * _rotateSpd, 0); }
private void Accelerate() { Debug.Log("Accelerate"); }

private Dictionary<KeyCode, Action> _keyDic;
private float _rotateSpd;
```


카메라가 캐릭터를 따라가야 계속 볼 수 있다.
플레이어 캐릭터의 위치를 받되, 회전값은 받지 않도록 스크립트를 만들고 카메라나 플레이어 캐릭터에 붙여준다. 
실제로 이동하고 회전하는지, 카메라가 잘 따라가는지 등을 확인하기 위해 플레인 오브젝트 하나를 깔아준다.
```C#
class CameraManager : MonoBehaviour
{
    private void Start()
    {
        _followOffset = new Vector3(0, 13.3f, -8.3f);
    }

    void Update()
    {
        Follow();
    }

    private void Follow()
    {	// 부드럽게 따라가지 말고 즉각적으로 따라가도록 만들었다.
	    // 추후에 slerp 등으로 살짝만 텀을 주면 괜찮을 것.
        transform.position = _followTarget.position + _followOffset;
    }

    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector3 _followOffset;
}
```

플레이어 캐릭터와 적 캐릭터의 공격을 만들어야 한다.

공격은 얼마만큼의 피해를 얼마나 자주, 얼마나 멀리서 가할 수 있는지로 정해진다. 무기 추상 클래스를 만들고 Fire() 함수를 만들어주자.
```C#
public void Attack() 
{
    if (_firable)
    {
        // Pull bullet object or Check Collisions for melee.
        Fire();
        Debug.Log("Fire");
        _firable = false;
        StartCoroutine(Delay());
    }
}
abstract protected void Fire();

IEnumerator Delay()
{
    yield return new WaitForSeconds(_rps);
    _firable = true;
}
```
플레이어 캐릭터는 짧은 사거리의 투사체 발사 공격을, 적 캐릭터는 가까이 접근한 뒤,  근접 공격하는 스크립트를 작성해야 한다.
투사체 공격은 직선으로 날아가는 방식과 투포환같이 포물선으로 날아가는 방식이 있다.
우선 직선 공격을 만들도록 하자.

## 겪은 문제
	- 추상클래스를 상속받은 클래스가 '일관성 없는 액세스 가능성: 'XXX' 기본 클래스가 'YYY' 클래스보다 액세스하기 어렵습니다.' 라는 오류를 띄웠다.
	[컴파일러 오류 CS0051 | Microsoft Docs](https://docs.microsoft.com/ko-kr/previous-versions/4sscdk02(v=vs.120)?redirectedfrom=MSDN)
	기본 클래스들을 정의할 때, 따로 public을 붙여주지 않았는데 해당 기본 클래스를 상속받은 클래스를 정의할 때 public을 붙여줘서 생긴 문제였다. 
	public을 지워서 해결하였다.
	
	- 상속받은 클래스에서 기본 클래스의 가상/추상 함수를 재정의한 함수가 의도대로 호출되지 않는 문제가 있었다.
	[Override 및 New 키워드를 사용하여 버전 관리 - C# 프로그래밍 가이드 | Microsoft Docs](https://docs.microsoft.com/ko-kr/dotnet/csharp/programming-guide/classes-and-structs/versioning-with-the-override-and-new-keywords)
	New와 Override 키워드의 차이점을 모른채로 New를 사용해서 생긴 문제였다.
	Override로 키워드를 변경하여 해결하였다.
	
	- class 앞에 붙는 public과 private가 정확히 어떤 역할을 맡는지 헷갈린다.
	[액세스 한정자 - C# 프로그래밍 가이드 | Microsoft Docs](https://docs.microsoft.com/ko-kr/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers)

#### 2022-09-06
총알을 관리하기 위해 오브젝트 풀링을 구현하려 했는데 찾아보니 해당 기능을 엔진에서 공식적으로 지원하는 듯하다. 새로 추가되는 기능들을 잘 찾아보는 것도 중요해 보인다.
다만 현재 사용하고 있는 엔진의 버전을 올려야 한다는 문제가 있다.
완전 개발초기기도 하고 여러 기능 써보면 좋을 것 같아 2020.3.32f1에서 2021.3.9f1로 업데이트했다.

이 기능을 써도 여러 프리팹을 사용하려면 여러가지 만져줘야 할 것 같다.
각 원거리 무기 별로 각자의 총알 리스트를 갖게 하면 어떨까?
동시에 화면에 존재할 수 있는 총알의 개수를 구한 뒤에 해당 개수만큼만 풀을 만들어주면 될 것 같다.
총알은 _lifeTime동안 존재할 수 있다. RPS에 따라 _lifeTime 동안 생성할 수 있는 개수가 정해질 것이다.

따라서 Weapon 클래스를 상속받은 RangeWeapon 클래스의 내부를 다음처럼 작성해줬다.
```C#
//=============================================================================
// Range Weapon
//=============================================================================
public abstract class RangeWeapon : Weapon
{
    override public void Start()
    {
        base.Start();

        // 한순간에 존재할 수 있는 최대 총알 개수는 다음과 같다.
        // (총알의 지속시간 / 발사까지 걸리는 시간) * (최대 발사속도 가속보너스) + 여유 한 칸
        int numBullets = (Mathf.CeilToInt(_bullet.GetComponent<Bullet>()._lifeTime / _rps) * 2) + 1;

        for(int i = 0; i < numBullets; i++)
        {
            GameObject temp = Instantiate(_bullet);
            // 리스트는 여기 있지만 계층구조는 전역 인스턴스의 자식으로 넣어 관리할 것이다.
            temp.transform.SetParent(BulletPool.Instance.transform); 
            temp.SetActive(false);
            _bulletPool.Add(temp);
        }
    }
    override protected void Fire()
    {
        InitializeBullet(FindInactiveBullet());
    }
    private GameObject FindInactiveBullet()
    {
        foreach (GameObject temp in _bulletPool)
            if (!temp.activeSelf) return temp;
        return null;
    }
    private void InitializeBullet(GameObject obj)
    {
        if (obj == null) return;
        if (_muzzle)
        {
            obj.transform.position = _muzzle.position;
            obj.transform.rotation = _muzzle.rotation;
        }
        else
        {
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
        }
        obj.SetActive(true);    
    }

    [SerializeField] private Transform _muzzle;
    [SerializeField] private GameObject _bullet;
    private List<GameObject> _bulletPool = new List<GameObject>();
}
```
머즐은 단순히 탄이 발사될 곳의 위치정보를 가질 빈 오브젝트이다.
원거리 무기는 공격할 위치를 어떻게 정하는가?
- 오브젝트가 이동하는 방향으로 발사
- 오브젝트가 보는 방향으로 발사
- 마우스 커서가 위치한 곳을 향해 발사

특정 포지션을 주면 해당 위치를 향하게끔 발사하게 하였다.
내가 원하는 것은 전차처럼 이동방향과 포신의 방향이 달라질 수 있는 이동형 포탑 컴패니언들과 커서가 위치한  방향으로 공격하는 플레이어 캐릭터이다. 포신의 역할을 머즐이 해줄 것이다.
화면상에서 현재 클릭한 곳의 위치를 구하는 함수, 포신을 해당 방향으로 돌려줄 함수를 만들어야 한다.
```C#
//=========================================================================
// 발사할 방향을 구한다.
//=========================================================================
private Vector3 GetTargetPosition()
{
    Vector3 result = Input.mousePosition;
    Ray ray = Camera.main.ScreenPointToRay(result);
    RaycastHit hit;
    LayerMask layerMask = LayerMask.GetMask("Ground");
    if (Physics.Raycast(ray, out hit, layerMask))
    {
        result = hit.point;
    }
    else result = transform.position;

    return result;
}
```
커서 위치를 받아 발사 방향을 구하고 웨폰을 해당방향을 보도록 회전시킨다.
머즐은 웨폰의 자식 오브젝트이므로 머즐을 회전시켜선 의도와 달라진다.
프로젝트 세팅에서 피직스의 레이어 충돌관계를 설정해주기 위해 Player, Enemy, PlayerBullet, EnemyBullet 레이어를 추가해준다.
Ground와 충돌했을 때, Character를 찾으려 하는 일이 있어서 그 부분을 처리해줬다.

이제 플레이어 캐릭터가 마우스 좌클릭을 하면, 커서 위치로 공격하고, 적 캐릭터에 총알이 충돌하면 피해를 입게 하였다. 체력이 0 이하가 되면 비활성화 시켰다.

이제 적 캐릭터도 풀을 만들어서 관리하고 화면 밖에서 계속 생성해주면 된다. 적 캐릭터는 플레이어 캐릭터를 향해 쫓아간다. 충분히 근접하면 근접 공격으로 피해를 줄 수 있어야 한다. 내일 마저 구현하자.

## 겪은 문제
	- 총알을 위해 오브젝트 풀링을 하려 했는데 의도대로 작동하지 않는 문제가 있었다.
	[Compiler Error CS1657 | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs1657?f1url=%3FappId%3Droslyn%26k%3Dk(CS1657))
	C/C++을 할 때 썼던 오브젝트 풀링 방식을 그대로 써보려고 했는데 다른 스크립트에서 생성한 오브젝트 리스트의 요소의 값을 참조해서 변경하려는게 안됐다. foreach에서는 안된다고 한다.
	for로 바꿔서 사용해봤는데도 오류를 뱉는다.
	오브젝트 풀을 만드는 곳을 불렛풀 오브젝트 하나에서 각 원거리 무기 컴포넌트로 변경하여 해결하였다.
	

> Written with [StackEdit](https://stackedit.io/).