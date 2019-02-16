﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Skill_01 : SkillController
{

  // Use this for initialization
  public float damage = 3f;
  public BoxCollider2D boxCollider;
  public int lane;
  private GameObject ex;

  public override void OnStart ()
  {
    spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
    boxCollider = gameObject.GetComponent<BoxCollider2D> ();
    boxCollider.enabled = false;
    Master.PlayAnimation (spriteRenderer, "Textures/Skills/Skill_01/SkillAnimation", 0, 19, 0.06f, true, null, this);
    damage = Master.SkillData.skill_01_data.Damage;
    ex = Resources.Load<GameObject> ("Prefabs/explosion1");
  }

  bool isBuildInPosition = false;

  public override bool Set ()
  {

    foreach (GameObject go in Master.Touch.listGameObjectsAtMousePosition) {
      if (go.tag == "UnitPosition") {
        isBuildInPosition = true;
        Master.Lane.SetUnitAtPosition (go, gameObject);
        gameObject.transform.DOMove (go.transform.position, 0.3f);
        lane = go.GetComponent<BuildPosition> ().lane;
        Master.QuestData.IncreaseProgressValue ("03");
        boxCollider.enabled = true;
      }
    }

    Master.Lane.HideUnitPosition ();

    if (!isBuildInPosition) {
      Destroy (transform.parent.gameObject);
      return false;
    }

    return true;
  }

  public override void OnChoosingPosition ()
  {
    gameObject.transform.parent.transform.position = Master.Touch.mousePositionGameplay;
    Master.Lane.ChangeColorPosition ();
  }

  public override void CollisionController (GameObject obj)
  {
    if (obj.tag == "Enemy") {
      if (isBuildInPosition) {
        if (obj.GetComponent<EnemyController> ().status.CurrentLane == lane) {
          Master.Lane.RemoveObjectAtPosition (gameObject);
          Master.Audio.PlaySound ("snd_explosion");
          Destroy (GetComponent<BoxCollider2D> ());
          obj.GetComponent<EnemyController> ().GetHit (Master.SkillData.skill_01_data.Damage);

          Master.Effect.ShakeCamera (5);
          GameObject clone = Instantiate (ex, transform.position, Quaternion.identity);
          clone.name = "explosion1";
          Destroy (transform.parent.gameObject, 0.5f);
        }
      }
    }
  }
}
