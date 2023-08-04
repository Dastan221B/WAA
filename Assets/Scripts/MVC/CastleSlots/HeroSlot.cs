using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MVC.CastleSlots
{
    public enum SlotType { CastleSlot , GarissonSlot}

    public class HeroSlot : MonoBehaviour
    {
        [SerializeField] private SlotType _slotType;
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _baseSprite;
        public bool IsHaveHero { get; private set; }

        public SlotType SlotType => _slotType;

        public void EnterHero(Sprite sprite)
        {
            _icon.sprite = sprite;
            IsHaveHero = true;
        }

        

        public void ExitHero()
        {
            _icon.sprite = _baseSprite;
            IsHaveHero = false;
        }
        
    }
}