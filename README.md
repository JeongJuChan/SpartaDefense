# DefenseSparta

# �� Ŭ������ ����
## ĳ��
* Castle : ���� ó�� ��ġ�� �����ϰ�, CastleClan�� �ʱ� ���� ����
* CastleClan : ����ε��� �ִ� ����Ʈ�� �����ϰ�, �� ������� �ʱ� ������ ��
## ĳ����
* Monster : ���������� �����ϴ� ����
* Hero : �� ���� ��ġ�ϴ� �����, �ַ� ���Ÿ� ���ݿ� ���� �޼������ ����
## ����
* ActiveTargetHandler : �ʵ� ���� ���� ����Ʈ ����
* BattleController : ���� �� �̺�Ʈ ���ð� ���������� ��� ����
* FieldMonsterController : �ʵ� ������ ������ ���� �̺�Ʈ ��ϰ� ����
* IDamagable : ���ظ� ���� �� �ִ� ��ü�� ���� �������̽�
* TargetInRangeTrigger : Ÿ���� �ʵ忡 ���Դ��� üũ�ϴ� Ʈ����
* IShootParabola : ������ ���� �������̽�
* IShootStraight : ���� ���� �������̽�
* ShootingAnimationEventHandler : �ִϸ��̼��� Shooting �̺�Ʈ�� �޾� Hero�� ����
* IMoveParabola : ������ ����ü �������̽�
* IMoveStraight : ���� ����ü �������̽�
* Projectile : ����ü�� ��� �����̴����� ���� �޼������ ����
## ������
* CastleBaseData : ���� �⺻ ������
* HeroBaseData : ������� �⺻ ������
* HeroStatInfo : ������� ���� ������
* MonsterBaseData : ������ �⺻ ������
* MonsterSpawnData : ���� ���� ������
* PoolInfoSO : Ǯ�� ������Ʈ�� Ǯ�� ���� ��
* HeroBaseDataSO : ������� �⺻ ������ ��ųʸ�, ��ũ ������ ���� ��ųʸ� SO
* MonsterBaseDataSO : ������ �⺻ ������ SO
## ����
* MonsterSpawnDataHandler : ���� ���� ������ ����
* MonsterSpawner : ���� ������ ����
* IPoolable : Ǯ�� �� ��ü�� ���� �������̽�
* IPooler : Ǯ���� ��ü�� ���� �������̽�, ������ ObjectPool�� ����
* ObjectPool : Queue�� ����ϴ� ������Ʈ Ǯ, IPoolable<T> ��ü�� ���� Ǯ���� ���
* MonsterObjectPooler : ���� ������Ʈ Ǯ���� ����
* HeroProjectileObjectPooler : ����� ����ü ������Ʈ Ǯ���� ����
* HeroProjectileSpawner : ����� ����ü ������ ����
## ��ƿ��Ƽ
* Consts : ���� ���̴� const ���� ����
* EnumUtility : CSV �Ľ̿��� ���̴� �޼���� 
* Enums : enum ����
* MonoBehaviorSingleton : MonoBehavior�� ��ӹ޴� abstract �̱��� Ŭ����
* Sigleton : �Ϲ� abstract �̱��� Ŭ����