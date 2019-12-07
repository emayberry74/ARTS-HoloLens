using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(IBzThirdPerson))]
	public sealed class BzThirdPersonControl : MonoBehaviour
	{
		private IBzThirdPerson _character;
		private IBzRagdoll _ragdoll;
		private IBzDamageable _health;
		private Transform _camTransform;
		private bool _jumpPressed;
		private bool _fire;
		private bool _crouch;
        public float forwardVar;
        public double sidways;
        public GameObject holder;
        public GameObject square;
        public Transform backpack;

		private void Start()
		{
			if (Camera.main == null)
				Debug.LogError("Error: no main camera found.");
			else
				_camTransform = Camera.main.transform;

			_character = GetComponent<IBzThirdPerson>();
			_health = GetComponent<IBzDamageable>();
			_ragdoll = GetComponent<IBzRagdoll>();
		}
        private bool inventoryFull = false;
        public Transform guide;
        private bool onlyOnce = true;
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            GameObject box = hit.gameObject;
            if (hit.transform.tag == "box")
            {

                _character.Move(Vector3.zero, true, _jumpPressed);
                box.transform.parent = holder.transform;
                box.transform.position = backpack.position;
                inventoryFull = true;
            }

            if(hit.transform.tag == "trig")
            {
                inventoryFull = false;
                //HA DO MATH
                if(onlyOnce)
                {
                    
                    _character.Move(Vector3.zero, _crouch, true);
                    
                    onlyOnce = false;
                }
                square.transform.position = guide.position;
               

            }
        }
        void Update()
		{
			// read user input: jump, fire and crouch

            if(inventoryFull)
            {
                square.transform.position = backpack.position;
            }
			if (!_jumpPressed)
				_jumpPressed = Input.GetButtonDown("Jump");
			if (!_fire)
				_fire = Input.GetMouseButtonDown(0);

			_crouch = Input.GetKey(KeyCode.C);
		}

		private void FixedUpdate()
		{
            if (!Move.stop)
            {
                // read user input: movement
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");

                // calculate move direction and magnitude to pass to character

                Vector3 camForward = new Vector3(_camTransform.forward.x, 0, _camTransform.forward.z).normalized;
                Vector3 move = forwardVar * camForward + h * _camTransform.right;
                if (move.magnitude > 1)
                    move.Normalize();



                ProcessDamage();

                // pass all parameters to the character control script
                _character.Move(move, _crouch, _jumpPressed);
                _jumpPressed = false;

                // if ragdolled, add a little move
                if (_ragdoll != null && _ragdoll.IsRagdolled)
                    _ragdoll.AddExtraMove(move * 100 * Time.deltaTime);
            }
            else
            {
                _character.Move(Vector3.zero, _crouch, _jumpPressed);
                _jumpPressed = false;
            }
		}

        /// <summary>
        /// if health script attached, shot the character
        /// </summary>
        private void ProcessDamage()
		{
			if (_health == null)
				return;
            
			if (_fire)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                _health.Shot(ray, 0.40f, 10000f);
				_fire = false;
			}

			if (_jumpPressed && _health.IsDead())
			{
				_health.Health = 1f;
				_jumpPressed = false;
			}
		}
	}
}