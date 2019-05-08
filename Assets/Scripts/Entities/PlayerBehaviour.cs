using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : Attackable
{
    public int Health = 1;

    public override void TakeDamage(int dmg) {
        Health -= dmg;
        if(Health <= 0) {
            GameManager.Instance.Respawn();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
