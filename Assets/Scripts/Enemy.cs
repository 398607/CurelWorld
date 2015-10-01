using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Enemy : MovingObject {

	public int playerDamage;

	private Animator animator;
	private Transform target;
	private bool skipMove;

	private int[] dx;
	private int[] dy;

	// Use this for initialization
	protected override void Start() {
		GameManager.instance.AddEnemyToList(this);
		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;

		dx = new int[] { 0, 0, 1, -1 };
		dy = new int[] { 1, -1, 0, 0 };

		base.Start();
	}

	protected override void AttemptMove<T>(int xDir, int yDir) {
		if (skipMove) {
			skipMove = false;
			return;
		}
		base.AttemptMove<T>(xDir, yDir);

		skipMove = true;
	}

	public void MoveEnemy() {
		Vector3 tmpPosition = transform.position;
		float minDistance = (target.position - tmpPosition).sqrMagnitude;
		int minIndex = -1; // do not move
		
		for (int dir = 0; dir < 4; dir ++) {
			tmpPosition.x += dx[dir];
			tmpPosition.y += dy[dir];
			float distance = (target.position - tmpPosition).sqrMagnitude;
			if (distance < minDistance) {
				minDistance = distance;
				minIndex = dir;
			}
			tmpPosition.x -= dx[dir];
			tmpPosition.y -= dy[dir];
		}

		AttemptMove<Player>(dx[minIndex], dy[minIndex]);
	}

	protected override void OnCantMove<T>(T component) {
		Player hitPlayer = component as Player;

		animator.SetTrigger("enemyAttack");

		hitPlayer.LoseFood(playerDamage + Random.Range(-5, 5));
	}
}
