using Assets.Scripts.MVC.CastleSlots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MVC.CastleMVC.View
{
    public class CastleView : MonoBehaviour
    {
        private StrategyCamera _strategyCamera;
        private GameObject _castle;
        private CastleCommandsSender _castleCommandsSender;
        private CastleModel _castleModel;
        private SlotsModel _slotsModel;
        private Transform _terrainObjectsParent;
        private GameObject _buildings;
        private GameObject _playerPanel;
        private BuildingsListWindow _buildingsListWindow;
        private HeroSlot _heroGarrisonSlot;
        private HeroSlot _heroCasstleSlot;
        private Heroes _heroes;

        public void Init(Heroes heroes,BuildingsListWindow buildingsListWindow,CastleCommandsSender castleCommandsSender, CastleModel castleModel, SlotsModel slotsModel)
        {
            _heroes = heroes;
            _buildingsListWindow = buildingsListWindow;
            _castleCommandsSender = castleCommandsSender;
            _castleModel = castleModel;
            _slotsModel = slotsModel;
        }

        public void Init(HeroSlot garrisonSlot , HeroSlot castleSlot,StrategyCamera strategyCamera,Button exitButton,Button openCouncilButton, GameObject castle, Transform terrainObjectsParent, GameObject buildings ,  GameObject playerPanel)
        {
            _heroCasstleSlot = castleSlot;
            _heroGarrisonSlot = garrisonSlot;
            _strategyCamera = strategyCamera;
            _terrainObjectsParent = terrainObjectsParent;
            _castle = castle;
            _buildings = buildings;
            _playerPanel = playerPanel;
            exitButton.onClick.AddListener(() => ExitFromCastle());
            openCouncilButton.onClick.AddListener(() => OpenCouncil());
        }

        public void HandleHeroInGarrison(CastleObjectFullInfo castleObjectFullInfo)
        {
            if(castleObjectFullInfo.heroInCastle == null)
            {
                _heroCasstleSlot.ExitHero();
            }
            else
            {
                _heroCasstleSlot.EnterHero(_heroes.GetHeroByID(castleObjectFullInfo.heroInCastle.dicHeroId).Icon);
            }

            if (castleObjectFullInfo.heroInGarrison == null)
            {
                _heroGarrisonSlot.ExitHero();
            }
            else
            {
                _heroGarrisonSlot.EnterHero(_heroes.GetHeroByID(castleObjectFullInfo.heroInGarrison.dicHeroId).Icon);
            }
        }

        public void Open()
        {
            _strategyCamera.enabled = false;
            _terrainObjectsParent.gameObject.SetActive(false);
            _castle.SetActive(true);
            _buildings.SetActive(true); 
            _playerPanel.SetActive(false);
        }

        public void Close()
        {
            _strategyCamera.enabled = true;
            _terrainObjectsParent.gameObject.SetActive(true);
            _castle.SetActive(false);
            _buildings.SetActive(false);
            _playerPanel.SetActive(true);
        }

        public void OpenCouncil()
        {
            _buildingsListWindow.Open();
        }

        public void ExitFromCastle()
        {
            var garrison = new List<ArmySlotInfo>(_slotsModel.GarrisonArmy).ExcludeNull();
            _castleCommandsSender.SendLeaveCastleRequest(_castleModel.CurrentCastleID, _castleModel.GarrisonID,
                new List<ArmySlotInfo>(_slotsModel.CastleArmy).ExcludeNull(), garrison);
        }

    }
}