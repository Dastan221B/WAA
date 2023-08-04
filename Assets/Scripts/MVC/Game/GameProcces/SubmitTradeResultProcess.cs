using Assets.Scripts.MVC.TradeMVC;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.MVC.Game.GameProcces
{
    public class SubmitTradeResultProcess
    {
        private GameModel _gameModel;
        private TradeController _tradeController;

        public SubmitTradeResultProcess(GameModel gameModel , TradeController tradeController)
        {
            _gameModel = gameModel;
            _tradeController = tradeController;
        }

        public void SubmitTradeResultHandler(MessageInput message)
        {
            SubmitTradeResult submitTradeResult = Newtonsoft.Json.JsonConvert.DeserializeObject<SubmitTradeResult>(message.body);
            if (submitTradeResult.result)
            {
                _tradeController.ExitFromTradePanel();
                if (_gameModel.TryGetHeroModelObject(submitTradeResult.requesterHeroObjectId,out HeroModelObject heroModelObject1))
                {
                    Debug.Log("Requester!!!");
                    heroModelObject1.SetArmySlots(submitTradeResult.receiverArmy);
                }
                if (_gameModel.TryGetHeroModelObject(submitTradeResult.receiverHeroObjectId, out HeroModelObject heroModelObject2))
                {
                    Debug.Log("Receiver!!!");
                    heroModelObject2.SetArmySlots(submitTradeResult.requesterArmy);
                }
            }

        }
    }
}