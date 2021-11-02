using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AlienScript : ShootableObject
{
    // Start is called before the first frame update
    [SerializeField] private Vector3 GrowingSpeed = ((Vector3.up + Vector3.right) / 1000); //�������� �����
    [SerializeField] private static float MaxGrowSize = 0.45f; //�� ������ ������� ����� ������� �������� (���������� ����� ����� �� �������)
    [SerializeField] private int Damage = -1; //���� ��������� ��������� ��� ������
    [SerializeField] private GameObject teleportPS;
    [SerializeField] private GameObject backTeleportPS;
    [SerializeField] private GameObject smokePS;
    [SerializeField] private GameObject firePS;
    public bool dead = false; // ����� �� ��������
    private Rigidbody2D rigidBody2;
    
    /*��� ����� �� ��������� ��� �������� ������ � �������� � ��������� �������, �� ��������� ���������
      �������� ���������� �������
      ���������� ���� �� ���������*/
    public override int GetShoted()
    {
        if (!dead)
        {
            rigidBody2.velocity = Vector2.down * 4;
            rigidBody2.angularVelocity = 720 * Random.Range(-2f, 2f);
            dead = true;
            Instantiate(smokePS, transform);
            return base.GetShoted();
        }
        return 0;
    }
    //������� ����� ��������� �� �������� �����
    private void Grow()
    {
        transform.localScale = transform.localScale + GrowingSpeed;
    }
    //������� ������ � ���������� ����� � ����������������
    private void BangSelf()
    {
        gameMaster.ChangeHealth(Damage);
        Instantiate(firePS, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 4), Quaternion.identity);
        Destroy(transform.gameObject);

    }
    /*����������� ������������ �������
      � Rigidbody2D, ���������� ���������� ���������� ��� ��������� */
    void Start()
    {
        rigidBody2 = GetComponent<Rigidbody2D>();
        gameMaster = FindObjectOfType<GameMaster>();
        gameMaster.totalAlienNumber += 1;
        Instantiate(teleportPS, new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 4), Quaternion.identity);
    }

    //��� ����������� ����������� ����� ����������
    private void OnDestroy()
    {
        gameMaster.totalAlienNumber -= 1;
        if (dead)
        {
            Instantiate( backTeleportPS, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 4), Quaternion.identity);
        }
    }


    private void FixedUpdate()
    {

        if (transform.localScale.x < MaxGrowSize && transform.localScale.y < MaxGrowSize) //������ ���� �� ������ ������������� �����, ����� ����������
        {
            if (!dead)
            {
                Grow();
            }
        }
        else
        {
            BangSelf();
        }


    }

}

