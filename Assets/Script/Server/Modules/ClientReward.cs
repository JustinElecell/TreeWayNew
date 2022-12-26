using UnityEngine;
using System.Collections;
using EleCellLogin;

public struct ClientReward { // reward processed on client side
	
	public RewardType type;
	public int rewardEnum;
	public string rewardString;

	public double amount;
	
	public ClientReward( RewardType t, int e, double a){
		type = t; rewardEnum = e; amount = a;rewardString = "";
	}
	
	public void collect(){
		if (type == RewardType.currency){
			//MjSave.earnCurrency( (InGameCurrency) rewardEnum, amount);
		}
	}
}

public enum RewardType{
	currency,
	item,
	pet
}

public enum InGameCurrency{
	coin,
	credit,
	gem,
	tickC,
	tickR,
	tickG,
    checkVail,
    Null
}

public struct ItemCost{
	public InGameCurrency currency;
	public double amount;
	
	public ItemCost(InGameCurrency c, double a){
		currency = c;
		amount = a;
	}
}