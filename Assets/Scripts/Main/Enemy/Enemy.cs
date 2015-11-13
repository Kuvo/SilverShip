﻿using UnityEngine;
using System.Collections;
using Uxtuno;

namespace Kuvo
{
	/// <summary>
	/// 敵の共通動作を規定した抽象クラス
	/// </summary>
	abstract public class Enemy : Actor
	{
		public enum ActionState
		{
			None,
			Bone,
			Move,
			Attack,
			Death,
		}

		public ActionState currentState { get; protected set; }

		/// <summary>
		/// 地面の上に立っているかどうか
		/// </summary>
		public bool haveGround { get; protected set; }

		private CameraController cameraController;

		/// <summary>
		/// エネミーを目視することができる最も近い距離
		/// (エネミーの大きさに応じて変更する必要がある)
		/// </summary>
		abstract protected float sight { get; set; }

		private GameObject _shortRangeAttackAreaObject;   // shortRangeAttackAreaObjectプロパティの実体

		/// <summary>
		/// 攻撃判定用のゲームオブジェクトを取得する
		/// </summary>
		protected GameObject shortRangeAttackAreaObject
		{
			get
			{
				if (!_shortRangeAttackAreaObject)
				{
					foreach (GameObject child in gameObject.GetChildren(true))
					{
						if (child.tag == TagName.AttackArea)
						{
							_shortRangeAttackAreaObject = child;
						}
					}

					_shortRangeAttackAreaObject.GetSafeComponent<AttackArea>();

					if (!_shortRangeAttackAreaObject)
					{
						Debug.LogError("攻撃判定用のゲームオブジェクトが見つかりませんでした。");
					}
				}

				return _shortRangeAttackAreaObject;
			}
		}

		protected virtual void Awake()
		{
			currentState = ActionState.Bone;
			haveGround = false;
		}

		protected virtual void Start()
		{
			if (currentState != ActionState.Bone)
			{
				currentState = ActionState.Bone;
			}

			cameraController = GameObject.FindGameObjectWithTag(TagName.CameraController).GetComponent<CameraController>();
		}

		protected virtual void Update()
		{
			// カメラとの距離に応じて描画状態を切り替える
			Vector3 cameraToVector = cameraController.cameraTransform.position - transform.position;
			if (cameraToVector.magnitude < sight)
			{
				isShow = false;
			}
			else
			{
				isShow = true;
			}

		}

		protected virtual void LateUpdate()
		{
			if (hp <= 0 && currentState != ActionState.Death)
			{
				currentState = ActionState.Death;
				StopAllCoroutines();
				StartCoroutine(OnDie(2));
			}
		}

		protected virtual void OnCollisionExit(Collision collision)
		{
			if (collision.transform.tag == TagName.Scaffold)
			{
				haveGround = false;
			}
		}

		public void OnCollisionEnter(Collision collision)
		{
			if (collision.transform.tag == TagName.Scaffold)
			{
				haveGround = true;
			}
		}

		public override void Damage(int attackPower, float magnification)
		{
			base.Damage(attackPower, magnification);
			if (haveGround)
			{
				StartCoroutine(GroundStagger());
			}
			else
			{
				StartCoroutine(AirStagger());
			}
		}

		/// <summary>
		/// 指定秒をかけて死亡する
		/// </summary>
		/// <param name="second"> インスタンスが消滅するまでの時間</param>
		/// <returns></returns>
		protected virtual IEnumerator OnDie(float second)
		{
			float time = 0.0f;
			while (time < second)
			{
				time += Time.deltaTime;

				yield return new WaitForEndOfFrame();
			}
			Debug.Log("死んだー", gameObject);
			Destroy(gameObject);
		}

		/// <summary>
		/// 空中でよろける
		/// </summary>
		abstract protected IEnumerator AirStagger();

		/// <summary>
		/// 地上でよろける
		/// </summary>
		abstract protected IEnumerator GroundStagger();

		/// <summary>
		/// 近接攻撃
		/// </summary>
		abstract public IEnumerator ShortRangeAttack();
	}
}