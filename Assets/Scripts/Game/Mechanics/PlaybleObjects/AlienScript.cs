using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class AlienScript : ShootableObject
{
    // Start is called before the first frame update
    [SerializeField] private Vector3 GrowingSpeed = ((Vector3.up + Vector3.right) / 1000); //Скорость роста
    [SerializeField] private static float MaxGrowSize = 0.45f; //До какого размера может вырасти пришелец (одинаковый чтобы игрок не путался)
    [SerializeField] public int Damage = -1; //Урон наносимый пришелцем при взрыве
    [SerializeField] private GameObject teleportPS;
    [SerializeField] private GameObject backTeleportPS;
    [SerializeField] private GameObject smokePS;
    [SerializeField] private GameObject firePS;
    public bool dead = false; // Мертв ли пришелец
    private Rigidbody2D rigidBody2;
    private Collider2D collider2;

    public event Action<GameObject> ImDestroidEvent;
    public event Action<GameObject> ImBangedSelfEvent;
    public event Action<GameObject> ImGetShotedEvent;
    /*При клике на пришельца тот начинает падать и вращатся в случайную сторону, со случайной скоростью
      Пришелец становится мертвым
      Возвращает очки за пришельца*/
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
    //Функция роста пришельца на скорость роста
    private void Grow()
    {
        transform.localScale = transform.localScale + GrowingSpeed;
    }
    //Функция Взрыва с нанесением урона и самоуничтожением
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
    /*Подключение управляющего скрипта
      и Rigidbody2D, увеличение количества пришельцев при появлении */
    void Start()
    {
        rigidBody2 = GetComponent<Rigidbody2D>();
        //gameMaster = FindObjectOfType<GameMaster>();
        //gameMaster.totalAlienNumber += 1;
        Instantiate(teleportPS, new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 4), Quaternion.identity);
        collider2 = GetComponent<Collider2D>();
    }

    //При уничтожение уменьшается число пришельцев
    private void OnDestroy()
    {
        ImDestroidEvent?.Invoke(gameObject);
        //gameMaster.totalAlienNumber -= 1;
    }


    private void FixedUpdate()
    {

        if (transform.localScale.x < MaxGrowSize && transform.localScale.y < MaxGrowSize) //Растет пока не достиг максимального роста, иначе взрывается
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

