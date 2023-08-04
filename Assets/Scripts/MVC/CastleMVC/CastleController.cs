using Assets.Scripts.MVC.CastleMVC.Buildinngs;
using Assets.Scripts.MVC.CastleMVC.View;
using Assets.Scripts.MVC.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MVC.CastleMVC
{
    public class CastleController : MonoBehaviour
    {
        private CastleModel _castleModel;
        private CastleCommandsSender _castleCommandsSender;
        private ProgramState _programState;
        private BuildingsPicker _buildingsPicker;
        private HireCreatureBuildingWindow _hireCreatureBuildingWindow;
        private BuildingsListWindow _buildingsListWindow;
        private CommonData _commonData;


        public void Init(CastleModel castleModel,
            CastleCommandsSender castleCommandsSender,
            ProgramState programState,
            BuildingsPicker buildingsPicker,
            HireCreatureBuildingWindow hireCreatureBuildingWindow,
            BuildingsListWindow buildingsListWindow,
            CommonData commonData)
        {
            _castleModel = castleModel;
            _castleCommandsSender = castleCommandsSender;
            _programState = programState;
            _buildingsPicker = buildingsPicker;
            _hireCreatureBuildingWindow = hireCreatureBuildingWindow;
            _buildingsListWindow = buildingsListWindow;
            _commonData = commonData;
        }


        public void SetBuildings(List<Building> buildings)
        {
            _castleModel.SetBuildings(buildings);
        }

        private void Update()
        {
            if (_programState.StatesOfProgram != StatesOfProgram.Castle)
                return;
            if (Input.GetMouseButtonDown(0))
                TrySelectBuilding();
        }

        private void TrySelectBuilding()
        {
            if(_buildingsPicker.TryPickBuilding(out Building building))
            {
                if(building.BuildingType == BuildingType.BuyBilding)
                {
                    _buildingsListWindow.Open();
                }
                else
                {
                    if (_commonData.TryGetDicBuildingDTOByID(building.Id, out DicBuildingDTO buildingDTO))
                    {
                        if (_commonData.TryGetDicCreatureDTOByID((int)buildingDTO.creatureId, out DicCreatureDTO creature))
                        {
                            DicCreatureDTO dicCreatureDTO = creature.Clone();
                            List<DicCreatureDTO> dicCreatureDTOs = new List<DicCreatureDTO>();
                            if (dicCreatureDTO.upgradeToId == 0)
                            {
                                dicCreatureDTOs = _castleModel.CurrentDicCastleDTO.creatureSet
                                  .Select(cId => _commonData.CreaturesDictianory[cId])
                                  .Where(c => c.level == dicCreatureDTO.level).ToList();
                            }
                            else
                            {
                                dicCreatureDTOs.Add(dicCreatureDTO);
                            }
  
                            _hireCreatureBuildingWindow.Init(dicCreatureDTOs);
                            _hireCreatureBuildingWindow.Open();
                        }
                    }
                }
            }
        }

    }
}