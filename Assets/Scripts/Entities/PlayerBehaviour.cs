using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : Attackable {
    public int Health = 1;
    public float Speed = 3.0f;
    public float soundReactionTime = 30.0f;

    private Rigidbody myRigidbody;
    private Animator myAnimator;
    private bool blockMovement;
    private float soundTimer = 0f;

    public override void TakeDamage(int dmg) {
        Health -= dmg;
        if (Health <= 0) {
            myAnimator.SetTrigger("Dead");
            GameManager.Instance.Respawn();
        }
    }

    // Start is called before the first frame update
    void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();

        EnemyManager.soundUnityEvent.AddListener(OnSound);
    }

    // Update is called once per frame
    void Update() {
        if (soundTimer > 0f)
            soundTimer -= Time.deltaTime;

        if (blockMovement) {
            myAnimator.SetFloat("Speed", 0f);
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float combination = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

        if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) > 1.0f) {
            horizontal = horizontal / combination;
            vertical = vertical / combination;
            combination = 1f;
        }
        Vector3 xaxis = horizontal * new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;
        Vector3 yaxis = vertical * new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
        Vector3 movement = xaxis + yaxis;
        if (combination > 0.05f) {
            transform.position += movement * Time.deltaTime * Speed;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
        myAnimator.SetFloat("Speed", combination);
    }

    private void OnSound(float range, float loudness, Vector3 pos) {
        if(soundTimer <= 0f) {

            myAnimator.SetTrigger("Scared");

            //CODE TO BLOCK MOVEMENT DURING SCARE ANIMATION
            /*
            blockMovement = true;
            myAnimator.SetFloat("Speed", 0f);
            Invoke("UnblockMovement", 4f);*/
        }
    }

    public void BlockMovement() {
        blockMovement = true;
    }

    public void UnblockMovement() {
        blockMovement = false;
    }
}
