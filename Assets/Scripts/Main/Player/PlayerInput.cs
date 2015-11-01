﻿using UnityEngine;
using System.Collections;

public class PlayerInput {
	private PlayerInput(){ }
	private static PlayerInput _instance;
	/// <summary>
	/// 唯一のインスタンスを返す
	/// </summary>
	public static PlayerInput instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new PlayerInput();
			}

			return _instance;
		}
	}


	public float horizontal
	{
		get { return Input.GetAxis(InputName.Horizontal); }
	}

	public float vertical {
		get { return Input.GetAxis(InputName.Vertical); }
	}

	/// <summary>
	/// カメラ回転入力
	/// </summary>
    public Vector2 cameraRotation
	{
		get
		{
			Vector2 rotationInput = Vector2.zero;

			rotationInput.x = Input.GetAxisRaw(InputName.CameraX);
			rotationInput.y = Input.GetAxisRaw(InputName.CameraY);
			if (Input.GetMouseButton(1))
			{
				rotationInput = Camera.main.ScreenToViewportPoint(Input.mousePosition);
				// 中央を(0, 0)にする
				rotationInput.x -= 0.5f;
				rotationInput.y -= 0.5f;
			}
			
			return rotationInput;
		}
	}

	/// <summary>
	/// ハイジャンプ入力には二つのキーの同時押しが必要
	/// そのためのキーそれぞれを列挙体で表す。
	/// 現在はハイジャンプ限定の仕組みだが、いずれ汎用的な処理にするべき
	/// </summary>
	public enum HighJumpKey
	{
		None,
		Main,
		Sub,
		All = Main | Sub,
	}

	private HighJumpKey highJumpKey = HighJumpKey.None;
	private float highJumpInputTime = 0.5f;
	private float highJumpInputCount = 0.0f;

	public bool jump
	{
		get { return Input.GetButtonDown(InputName.Jump); }
	}

	public bool highJump { get; private set; }
	public bool atack { get; private set; }

	/// <summary>
	/// プレイヤー入力情報更新
	/// </summary>
	public void Update(float elapsedTime)
	{
		// 入力情報を追加
		highJumpKey = HighJumpKey.None;
		if(Input.GetButton(InputName.Jump))
		{
			highJumpKey |= HighJumpKey.Main;
		}

		if(Input.GetButton(InputName.HighJump))
		{
			highJumpKey |= HighJumpKey.Sub;
		}

		Debug.Log(highJumpKey);

		// 受付時間のうちに両方のボタンが入力されている
		if (highJumpInputCount < highJumpInputTime && !highJump)
		{
			if (highJumpKey == HighJumpKey.All)
			{
				highJump = true;
			}
			else
			{
				highJump = false;
			}
		}
		else
		{
			highJump = false;
		}

		if (highJumpKey == HighJumpKey.None)
		{
			highJumpInputCount = 0.0f;
		}
		else if (highJumpKey != HighJumpKey.None)
		{
			// ハイジャンプボタンのどちらかが押されていればカウントしていく
			highJumpInputCount += elapsedTime;
		}
		else
		{
			highJumpInputCount = 0.0f;
		}
	}
}
