using Assets.Scripts.GameResources.MapCreatures;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MVC.Battle.Views
{
    public class CreatureBattleIcon : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        public CreatureModelObject CreatureModelObject { get; private set; }


        public void SetCreatureModelObject(Sprite icon)
        {
            _icon.sprite = icon;
            _icon.color = Color.white;
        }

    }
}