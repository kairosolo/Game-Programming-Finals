using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public override void Awake()
    {
        base.Awake();
        SetCharacter();
    }

    public override void ThisTurn()
    {
        base.ThisTurn();
        SetMoveset();
        BattleManager.Instance.PlayerTurn(characterName, characterSO.characterPortraitSprite);
    }

    public override void ExecuteMove()
    {
        UpdateMana(-preselectedMove.manaCost);
        BattleManager battleManager = BattleManager.Instance;
        target = battleManager.GetTarget();
        battleManager.FocusMove(this, target);

        CameraManager.Instance.TargetTakingAction(target, preselectedMove, isEnemy);
        Vector3 offset = Vector3.zero;
        if (target != preselectedMove.moveOwner) //If move does not targets the self
        {
            offset = new Vector3(2f, 0, 0);
        }
        transform.position = target.transform.position - offset;

        MovesetManager.Instance.DisableMoveset();
        MovesetManager.Instance.SetCharacterAction(characterName + " uses " + preselectedMove.moveName + " to " + target.characterName);
        Invoke(nameof(ExecuteMoveDelay), 1f);
    }

    private void ExecuteMoveDelay()
    {
        preselectedMove.Execute(this, target);
    }
}