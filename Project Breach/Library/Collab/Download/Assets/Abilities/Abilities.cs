using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour {

    [SerializeField] AbilityConfig[] abilities;

    Character character;

	void Start ()
    {
        character = GetComponent<Character>();
        AttachInitialAbilities();
	}

    void AttachInitialAbilities()
    {
        for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
        {
            abilities[abilityIndex].AttachAbilityTo(gameObject);
        }
    }

    public void AttempAbility(int abilityIndex, GameObject target = null)
    {
        StartCoroutine(UseAbility(abilityIndex));
    }

    public int GetNumberOfAbilities()
    {
        return abilities.Length;
    }

    public AbilityConfig GetAbilityAtIndex(int indexPosition)
    {
        return abilities[indexPosition];
    }

    IEnumerator UseAbility(int abilityIndex)
    {
        bool canExit = false;
        print(gameObject.name);
        while (!canExit)
        {
            yield return new WaitForFixedUpdate();

            abilities[abilityIndex].PlotAbility();
            if(Input.GetMouseButtonDown(0))
            {
                abilities[abilityIndex].DisablePlot();
                abilities[abilityIndex].Use();
                character.UseAbilityAction();
                canExit = true;
            }
            else if(Input.GetMouseButtonDown(1))
            {
                abilities[abilityIndex].DisablePlot();
                character.TaskFinished();
                canExit = true;
            }
        }
    }
}
