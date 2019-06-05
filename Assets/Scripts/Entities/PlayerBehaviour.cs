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

        if (blockMovement)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        if (movement.sqrMagnitude > 0.05 * 0.05f) {
            transform.position += movement * Time.deltaTime * Speed;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
        myAnimator.SetFloat("Speed", movement.magnitude);
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

    private void UnblockMovement() {
        blockMovement = false;
    }
}
