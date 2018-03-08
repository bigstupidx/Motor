using UnityEngine;
using System.Collections;
using Game;
using GameUI;
using UnityEngine.EventSystems;

public class BtnNitrogen : MonoBehaviour, IPointerDownHandler
{

	public UIEffect_LoadOnStart Effect;
	private FadeEffectGroup _effect;

	public void OnPointerDown(PointerEventData eventData)
	{
		//游戏中才响应按钮事件
		if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming)
		{
			return;
		}

		BikeManager.Ins.CurrentBike.bikeInput.OnBoost();
	}

	private void Start()
	{
		_effect=Effect.ins.GetComponent<FadeEffectGroup>();
		((EmitableParticle)_effect.Effects[0]).Particle.Clear(true);
	}

	private void Update()
	{
		if (_effect == null) return;
		var bike = BikeManager.Ins.CurrentBike;
		if (bike  == null) return;
		if (bike.bikeControl.BoostingEnergy == bike.bikeControl.BoostingEnergyMax)
		{
			_effect.FadeIn(0f);
		}
		else
		{
			_effect.FadeOut(0f);
		}
	}
}
