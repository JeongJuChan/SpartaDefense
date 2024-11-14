# DefenseSparta

# 각 클래스와 역할
## 캐슬
* Castle : 성의 처음 위치를 세팅하고, CastleClan의 초기 세팅 관리
* CastleClan : 히어로들이 있는 리스트를 관리하고, 각 히어로의 초기 세팅을 함
## 캐릭터
* Monster : 스테이지에 존재하는 몬스터
* Hero : 성 위에 위치하는 히어로, 주로 원거리 공격에 대한 메서드들이 존재
## 전투
* ActiveTargetHandler : 필드 위의 몬스터 리스트 관리
* BattleController : 전투 전 이벤트 세팅과 전투에서의 명령 지시
* FieldMonsterController : 필드 몬스터의 생성과 죽음 이벤트 등록과 해제
* IDamagable : 피해를 입을 수 있는 개체에 대한 인터페이스
* TargetInRangeTrigger : 타겟이 필드에 들어왔는지 체크하는 트리거
* IShootParabola : 포물선 공격 인터페이스
* IShootStraight : 직선 공격 인터페이스
* ShootingAnimationEventHandler : 애니메이션의 Shooting 이벤트를 받아 Hero에 전달
* IMoveParabola : 포물선 투사체 인터페이스
* IMoveStraight : 직선 투사체 인터페이스
* Projectile : 투사체가 어떻게 움직이는지에 대한 메서드들이 존재
## 데이터
* CastleBaseData : 성의 기본 데이터
* HeroBaseData : 히어로의 기본 데이터
* HeroStatInfo : 히어로의 스텟 데이터
* MonsterBaseData : 몬스터의 기본 데이터
* MonsterSpawnData : 몬스터 스폰 데이터
* PoolInfoSO : 풀링 오브젝트의 풀링 세팅 값
* HeroBaseDataSO : 히어로의 기본 데이터 딕셔너리, 랭크 데이터 관련 딕셔너리 SO
* MonsterBaseDataSO : 몬스터의 기본 데이터 SO
## 스폰
* MonsterSpawnDataHandler : 몬스터 스폰 데이터 세팅
* MonsterSpawner : 몬스터 스폰을 관리
* IPoolable : 풀링 될 개체에 대한 인터페이스
* IPooler : 풀링할 개체에 대한 인터페이스, 각자의 ObjectPool을 관리
* ObjectPool : Queue를 사용하는 오브젝트 풀, IPoolable<T> 개체의 실제 풀링을 담당
* MonsterObjectPooler : 몬스터 오브젝트 풀링을 관리
* HeroProjectileObjectPooler : 히어로 투사체 오브젝트 풀링을 관리
* HeroProjectileSpawner : 히어로 투사체 스폰을 관리
## 유틸리티
* Consts : 자주 쓰이는 const 값들 모음
* EnumUtility : CSV 파싱에서 쓰이는 메서드들 
* Enums : enum 모음
* MonoBehaviorSingleton : MonoBehavior를 상속받는 abstract 싱글턴 클래스
* Sigleton : 일반 abstract 싱글턴 클래스