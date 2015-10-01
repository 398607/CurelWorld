using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 0.5f;
	public Text foodText;

	private Animator animator;
	private int food;

	// Use this for initialization
	protected override void Start () {
		animator = GetComponent<Animator>();

		food = GameManager.instance.playerFoodPoints;
		foodText.text = "Food: " + food;

		base.Start();
	}

	private void OnDisable() {
		GameManager.instance.playerFoodPoints = food;
	}
	
	// Update is called once per frame
	private void Update () {
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0, vertical = 0;

		horizontal = (int)(Input.GetAxisRaw("Horizontal"));
		vertical = (int)(Input.GetAxisRaw("Vertical"));
		

		if (horizontal != 0)
			vertical = 0;

		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall>(horizontal, vertical);
		}
	}

	protected override void AttemptMove<T>(int xDir, int yDir) {
		food--;
		foodText.text = "Food: " + food;

		base.AttemptMove<T>(xDir, yDir);

		// RaycastHit2D hit;

		// Move(xDir, yDir, out hit);

		CheckIfGameOver();

		Debug.Log("Player`s turn ends, food " + food.ToString());

		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Exit") {
			Invoke("Restart", restartLevelDelay);
			enabled = false;
        }
		else if (other.tag == "Food") {
			int add = pointsPerFood + Random.Range(-5, 5);
			food += add;
			foodText.text = "+ " + add + " Food: " + food;
			other.gameObject.SetActive(false);
		}
		else if (other.tag == "Soda") {
			int add = pointsPerSoda + Random.Range(-10, 10);
			food += add;
			foodText.text = "+ " + add + " Food: " + food;

			other.gameObject.SetActive(false);
		}
	}

	protected override void OnCantMove<T>(T component) {
		Wall hitWall = component as Wall;
		hitWall.DamageWall(wallDamage);
		Debug.Log("Wall hit");
		animator.SetTrigger("playerChop");
	}

	private void Restart() {
		Application.LoadLevel(Application.loadedLevel);
	}

	public void LoseFood(int foodLoss) {
		animator.SetTrigger("playerHit");
		food -= foodLoss;
		foodText.text = "- " + foodLoss + " Food: " + food;
		CheckIfGameOver();
	}

	private void CheckIfGameOver() {
		if (food <= 0)
			GameManager.instance.GameOver();
	}
}
