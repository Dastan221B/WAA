using Assets.Scripts.MVC.CastleMVC;
using Assets.Scripts.MVC.CastleMVC.View;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MVC.CastleSlots
{
    public class SlotsController : MonoBehaviour
    {
        [SerializeField] private HeroSlot _heroSlotCastle;
        [SerializeField] private HeroSlot _heroSlotInGarnison;
        private CreatureStatsInfoWindow _creatureStatsInfoWindow; 
        private ProgramState _programState;
        private SlotPicker _slotPicker;
        private SlotsModel _slotsModel;
        private CommonData _commonData;
        private ModelCreatures _modelCreatures;

        private CastleCommandsSender _castleCommandsSender;

        private CreatureSlot _currentCreatureSlot;
        private HeroSlot _currentHeroSlot;
        private CastleModel _castleModel;
        private Heroes _heroes;

        public void Init(CastleCommandsSender castleCommandsSender,ModelCreatures modelCreatures ,CommonData commonData ,ProgramState programState, SlotPicker slotPicker, SlotsModel slotsModel,
            Heroes heroes , CastleModel castleModel)
        {
            _castleCommandsSender = castleCommandsSender;
            _castleModel = castleModel;
            _heroes = heroes;
            _modelCreatures = modelCreatures;
            _commonData = commonData;
            _programState = programState;
            _slotPicker = slotPicker;
            _slotsModel = slotsModel;
        }

        public void Init(HeroSlot heroSlotInGarisson , HeroSlot heroSlotInCastle)
        {
            _heroSlotCastle = heroSlotInCastle;
            _heroSlotInGarnison = heroSlotInGarisson;
        }

        public void SetCreatureStatsInfoWindow(CreatureStatsInfoWindow creatureStatsInfoWindow)
        {
            _creatureStatsInfoWindow = creatureStatsInfoWindow;
            _creatureStatsInfoWindow.Init(_programState);
        }

        public void DisplayGarnison(Sprite hero)
        {
            _heroSlotInGarnison.EnterHero(hero);
        }
        public void DisplayCastle(Sprite hero)
        {
            _heroSlotCastle.EnterHero(hero);
        }

        private void Update()
        {
            if (_programState.StatesOfProgram != StatesOfProgram.Castle)
                return;
            PickCreatureSlot();
            PickHeroInGarrisonSlot();
        }

        private void PickHeroInGarrisonSlot()
        {
            if (Input.GetMouseButtonDown(0) && _slotPicker.TryPickSlot(out HeroSlot heroSlot))
            {
                if (_currentHeroSlot == null && heroSlot.IsHaveHero)
                {
                    Debug.Log("first pick");
                    _currentHeroSlot = heroSlot;
                }
                else if (_currentHeroSlot != null && _currentHeroSlot.IsHaveHero && !heroSlot.IsHaveHero)
                {
                    Debug.Log("Second pick");
                    var hero = _castleModel.HeroInGarrison;
                    if (_castleModel.HeroInGarrison != null)
                    {
                        hero = _castleModel.HeroInGarrison;
                    } 
                    else if(_castleModel.HeroInCastle != null)
                    {
                        hero = _castleModel.HeroInCastle;
                    }
                    Sprite heroIcon = _heroes.GetHeroByID(hero.dicHeroId).Icon;
                    if (heroSlot.SlotType == SlotType.CastleSlot)
                    {
                        //Debug.Log(_castleModel);
                        //Debug.Log(_castleModel.CurrentCastleFullOjbectInfo);
                        //Debug.Log(_castleModel.CurrentCastleFullOjbectInfo.heroInCastle);
                        //Debug.Log(_castleModel.CurrentCastleFullOjbectInfo.heroInCastle.mapObjectId);
                        Debug.Log(_slotsModel.CastleArmy);
                        Debug.Log(_slotsModel.GarrisonArmy);
                        Debug.Log(_slotsModel);
                        
                        _castleCommandsSender.SendHeroMoveToCastleRequest(hero.mapObjectId,
                            new List<ArmySlotInfo>(_slotsModel.CastleArmy.Where(item => item != null)), new List<ArmySlotInfo>(_slotsModel.GarrisonArmy.Where(item => item != null)));
                        heroSlot.EnterHero(_heroes.GetHeroByID(hero.dicHeroId).Icon);
                        _currentHeroSlot.ExitHero();
                        _currentHeroSlot = null;
                        Debug.Log("_castleModel.HeroInGarrison.dicHeroId " + _heroes.GetHeroByID(_castleModel.HeroInGarrison.dicHeroId).Name);
                    }
                    else if (heroSlot.SlotType == SlotType.GarissonSlot)
                    {
                        _castleCommandsSender.SendHeroMoveToGarissonRequest(hero.mapObjectId,
                            new List<ArmySlotInfo>(_slotsModel.CastleArmy.Where(item => item != null)), new List<ArmySlotInfo>(_slotsModel.GarrisonArmy.Where(item => item != null)));
                        heroSlot.EnterHero(_heroes.GetHeroByID(hero.dicHeroId).Icon);
                        _currentHeroSlot.ExitHero();
                        _currentHeroSlot = null;
                    }
                    
                    
                }

            }
        }

        private void PickCreatureSlot()
        {
            if (Input.GetMouseButtonDown(0) && _slotPicker.TryPickSlot(out CreatureSlot creatureSlot))
            {
                if (_currentCreatureSlot == null)
                {
                    if (creatureSlot.ArmySlotInfo != null)
                    {
                        _currentCreatureSlot = creatureSlot;
                    }
                }
                else
                {
                    if (creatureSlot == _currentCreatureSlot)
                    {
                        if (_commonData.TryGetDicCreatureDTOByID((int)creatureSlot.ArmySlotInfo.dicCreatureId, out DicCreatureDTO dicCreatureDTO))
                            _creatureStatsInfoWindow.Open(dicCreatureDTO, _modelCreatures.GetIconById((int)creatureSlot.ArmySlotInfo.dicCreatureId - 1));
                        {
                            _currentCreatureSlot = null;
                            return;
                        }
                    }

                    if (creatureSlot.SlotTypes == SlotTypes.Castle && _slotsModel.GarrisonArmyCount - 1 <= 0)
                        return;

                    if (creatureSlot.ArmySlotInfo != null)
                        return;

                    if (creatureSlot.SlotTypes == SlotTypes.Castle)
                    {
                        _slotsModel.TrySetArmySlotInCastleSlotIcon(_currentCreatureSlot.ArmySlotInfo, creatureSlot.SlotID, _currentCreatureSlot.SlotID, _currentCreatureSlot.SlotTypes);
                        _currentCreatureSlot.ResetSlot();
                        _currentCreatureSlot = null;
                    }
                    else
                    {
                        _slotsModel.TrySetArmySlotInGarissonSlotIcon(_currentCreatureSlot.ArmySlotInfo, creatureSlot.SlotID, _currentCreatureSlot.SlotID, _currentCreatureSlot.SlotTypes);
                        _currentCreatureSlot.ResetSlot();
                        _currentCreatureSlot = null;
                    }
                }
            }
        }

    }
}