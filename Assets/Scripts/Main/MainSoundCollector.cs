﻿using UnityEngine;

namespace Kuvo
{
	/// <summary>
	/// 特定の名前から
	/// 対応するAudioClipを取得する
	/// (Mainシーンで使用されるAudioClipを保持している)
	/// </summary>
	public class MainSoundCollector : BaseSoundCollector<MainSoundCollector.SoundName>
	{
		public enum SoundName
		{
			NONE,
			// EnemySounds
			EnemySAttack,       // 近距離攻撃
			EnemyLAttack,       // 遠距離攻撃
			EnemyDamage,        // ダメージ
		}

		protected override void Awake()
		{
			sounds.Add(SoundName.NONE, null);
			sounds.Add(SoundName.EnemySAttack, Resources.Load<AudioClip>("Sounds/SoundEffects/EnemyVoice") as AudioClip);
			sounds.Add(SoundName.EnemyLAttack, Resources.Load<AudioClip>("Sounds/SoundEffects/EnemyVoice") as AudioClip);
			sounds.Add(SoundName.EnemyDamage, Resources.Load<AudioClip>("Sounds/SoundEffects/EnemyDeathSE") as AudioClip);
		}
	}
}
