using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Prime31;
using UnityEngine.SceneManagement;


public class DemoScene : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	public float bouncePower = 20f;
	public float bouncePower2 = 20f;

	public Text timer;
	public Text levelScores;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
	private bool gameOver;
	private float time;
	private GameObject scoreKeep;
	private Queue positions;

	void Awake()
	{
		Cursor.visible = false;
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
		scoreKeep = GameObject.Find ("ScoreKeeper");
		if (scoreKeep.GetComponent<KeepScore> ().isReplaying) {
			this.GetComponent<ReplayBot> ().enabled = true;
			this.GetComponent<Collider2D> ().enabled = false;
			this.enabled = false;
		}
		positions = new Queue ();
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		if (hit.collider.tag == "Sink") {
			hit.collider.gameObject.GetComponent<SinkInLava> ().TriggerSink ();
			_velocity.y = bouncePower;
		}
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;
		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{
		if(col.tag == "Win")			//For win colliders
			gameEnd(true);
		if (col.tag == "Bounce")		//For anything bouncy
			_velocity.y = bouncePower;
		if (col.tag == "Bounce2")		
			_velocity.y = bouncePower2;
		//Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		//Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		positions.Enqueue (transform.position);
		time += Time.deltaTime;
		timer.text = string.Format("{0:0}:{1:00}", Mathf.Floor(time/60), time % 60);
		levelScores.text = "";
		for (int i = 0; i < SceneManager.GetActiveScene ().buildIndex; i++)
			levelScores.text += "Level " + (i + 1) + ":" + scoreKeep.GetComponent<KeepScore> ().GetLevelScore (i) + " ";

		if( _controller.isGrounded )
			_velocity.y = 0;

		if(_controller.gameObject.transform.position.y < -3 && !gameOver) {
			gameEnd(false);
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			restart ();
		}

		if (Input.GetKeyDown (KeyCode.Z)) {   //For Debugging
			gameEnd (true);
		}
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();

		if( Input.GetKey( KeyCode.RightArrow ) )
		{
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Run" ) );
		}
		else if( Input.GetKey( KeyCode.LeftArrow ) )
		{
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Run" ) );
		}
		else
		{
			normalizedHorizontalSpeed = 0;

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Idle" ) );
		}


		// we can only jump whilst grounded
		if( _controller.isGrounded && Input.GetKeyDown( KeyCode.UpArrow ) )
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
			_animator.Play( Animator.StringToHash( "Jump" ) );
		}


		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets uf jump down through one way platforms
		if( _controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) && !Input.GetKey(KeyCode.UpArrow) )
		{
			_velocity.y *= 3f;
			_controller.ignoreOneWayPlatformsThisFrame = true;
		}
		if (_controller.collisionState.right || _controller.collisionState.left)
			_velocity.y /= 1.05f;
		if (_controller.collisionState.right && Input.GetKey (KeyCode.UpArrow)) {
			_velocity.x = -10;
			_velocity.y = 10;
		}
		if (_controller.collisionState.left && Input.GetKey (KeyCode.UpArrow)) {
			_velocity.x = 10;
			_velocity.y = 10;
		}
		_controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}

	public void gameEnd(bool won) {
		Cursor.visible = true;
		if (won) {
			scoreKeep.GetComponent<KeepScore> ().AddScore (SceneManager.GetActiveScene ().buildIndex - 1, string.Format ("{0:#,###.##}", time));
			scoreKeep.GetComponent<KeepScore> ().AddReplay (SceneManager.GetActiveScene ().buildIndex - 1, positions);
		}
		GameObject end = GameObject.Find("EndGame");
		Image img = end.GetComponentInChildren<Image>();
		Text txt = end.GetComponentInChildren<Text>();
		GameObject btn = (won ? end.transform.FindChild("NextLevel").gameObject : end.transform.FindChild("Replay").gameObject);
		txt.text = (won ? "Game Over! \n You won!" : "Game Over! \n You lost!");
		img.enabled = true;
		txt.enabled = true;
		btn.SetActive(true);
		timer.enabled = false;
		gameOver = true;
	}

	public void restart() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void NextLevel() {
		int lvl = SceneManager.GetActiveScene ().buildIndex;
		SceneManager.LoadScene (lvl + 1);
	}

}
