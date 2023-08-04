using System;
using System.Collections.Generic;

[Serializable]
public class HeroObjectFullInfo
{
    public int level { get; set; }
    public int attack { get; set; }
    public int defence { get; set; }
    public int power { get; set; }
    public int knowledge { get; set; }
    public int movePoints { get; set; }
    public int dicHeroId { get; set; }
    public string mapObjectId { get; set; }
    public List<ArmySlotInfo> army { get; set; }

    public HeroObjectFullInfo Clone()
    {
        HeroObjectFullInfo newHeroObject = (HeroObjectFullInfo)MemberwiseClone();
        newHeroObject.army = army;
        newHeroObject.mapObjectId = mapObjectId;
        return newHeroObject;
    }
}
