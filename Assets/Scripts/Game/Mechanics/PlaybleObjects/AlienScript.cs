using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class AlienScript : ShootableObject
{
    // Start is called before the first frame update
    [SerializeField] private Vector3 GrowingSpeed = ((Vector3.up + Vector3.right) / 1000); //�������� �����
    [SerializeField] private static float MaxGrowSize = 0.45f; //�� ������ ������� ����� ������� �������� (���������� ����� ����� �� �������)
    [SerializeField] public int Damage = -1; //���� ��������� ��������� ��� ������
    [SerializeField] private GameObject teleportPS;
    [SerializeField] private GameObject backTeleportPS;
    [SerializeField] private GameObject smokePS;
    [SerializeField] private GameObject firePS;
    public bool dead = false; // ����� �� ��������
    private Rigidbody2D rigidBody2;
    private Collider2D collider2;

    public event Action<GameObject> ImDestroidEvent;
    public event Action<GameObject> ImBangedSelfEvent;
    public event Action<GameObject> ImGetShotedEvent;
    /*��� ����� �� ��������� ��� �������� ������ � �������� � ��������� �������, �� ��������� ���������
      �������� ���������� �������
      ���������� ���� �� ���������*/
    public override int GetShoted()
    {
        if (!dead)
        {
            ImGetShotedEvent?.Invoke(gameObject);
            collider2.isTrigger = true;
            rigidBody2.drag = 0;
            rigidBody2.angularDrag = 0.0005f;
            rigidBody2.velocity = Vector2.down * 4;
            rigidBody2.angularVelocity = 720 * UnityEngine.Random.Range(-2f, 2f);
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
        ImBangedSelfEvent.Invoke(gameObject);
        //gameMaster.ChangeHealth(Damage);
        Instantiate(firePS, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 4), Quaternion.identity);
        Destroy(transform.gameObject);

    }
    public void PlayTeleportBack()
    {
        Instantiate(backTeleportPS, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 4), Quaternion.identity);
    }
    /*����������� ������������ �������
      � Rigidbody2D, ���������� ���������� ���������� ��� ��������� */
    void Start()
    {
        rigidBody2 = GetComponent<Rigidbody2D>();
        //gameMaster = FindObjectOfType<GameMaster>();
        //gameMaster.totalAlienNumber += 1;
        Instantiate(teleportPS, new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 4), Quaternion.identity);
        collider2 = GetComponent<Collider2D>();
    }

    //��� ����������� ����������� ����� ����������
    private void OnDestroy()
    {
        ImDestroidEvent?.Invoke(gameObject);
        //gameMaster.totalAlienNumber -= 1;
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

