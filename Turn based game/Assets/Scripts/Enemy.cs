using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public override void ThisTurn()
    {
        base.ThisTurn();
        BattleManager.Instance.AITurn();
        MovesetManager.Instance.SetCharacterAction(characterName + "'s Turn");
        Invoke(nameof(MoveInvoked), 1f);
    }

    private void MoveInvoked()
    {
        int randomMove = Random.Range(0, moves.Count);
        preselectedMove = moves[randomMove];

        Character target;
        BattleManager battleManager = BattleManager.Instance;
        if (preselectedMove.isTargetSelf)
        {
            target = this;
        }
        else
        {
            if (preselectedMove.isTargetAlly) //Ally of the Enemy
            {
                target = battleManager.PickRandomEnemy(); // It picks an enemy of the player (ally of the enemy)
                transform.position = target.gameObject.transform.position + new Vector3(2f, 0, 0);
            }
            else
            {
                target = battleManager.PickRandomAlly(); // Attacks you/your ally
                transform.position = target.gameObject.transform.position + new Vector3(2f, 0, 0);
            }
        }

        this.target = target;
        battleManager.AITurn();
        MovesetManager.Instance.SetCharacterAction(characterName + " uses " + preselectedMove.moveName + " to " + target.characterName);

        battleManager.FocusMove(this, target);
        ExecuteMove(preselectedMove);
        CameraManager.Instance.TargetTakingAction(target, preselectedMove, isEnemy);
    }

    public override void ExecuteMove(Move move)
    {
        StartCoroutine(ExecuteAfterDelay(move, this.target));
    }

    private IEnumerator ExecuteAfterDelay(Move move, Character target)
    {
        yield return new WaitForSeconds(1);
        move.Execute(this, target);
        if (thisTurn && move.isTargetAlly)
        {
            GameObject vfxClone = Instantiate(move.vfx, transform.position, Quaternion.identity);
            Destroy(vfxClone, .25f);
            BattleManager.Instance.DefaultCameraView();

            Invoke(nameof(NextTurn), .5f);
        }
    }
}