using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.Placeable.Component;

namespace Breach.UI.ToolTip
{
    public abstract class AbilityToolTip : MonoBehaviour
    {
        [Header("General Information")]
        [SerializeField] Text nameField;
        [SerializeField] Text descriptionField;
        [SerializeField] Text cooldownField;
        [SerializeField] string description;

        public virtual void Init(AbilityConfig abilityConfig)
        {
            nameField.text = abilityConfig.GetAbilityName();
            descriptionField.text = description;
            cooldownField.text = abilityConfig.GetMaxCoolDown().ToString();
        }

        protected IEnumerator StartMouseTracking()
        {
            while(true)
            {
                TrackMouse();
                yield return new WaitForFixedUpdate();
            }
        }

        private void TrackMouse()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            gameObject.transform.position = mousePosition;
        }
    }
}