using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Character
{
    public class CharacterController : MonoBehaviour
    {
        #region Properties

        //Movement

        [SerializeField] float _walkSpeed = 5f;
        [SerializeField] float _runSpeed = 8f;
        private float speed;
        [SerializeField] public bool _canMove;
        [SerializeField] private bool _isRunning;

        [SerializeField] private float horizontalInput;
        [SerializeField] private float verticalInput;
        [SerializeField] private Vector2 movementDirection;

        //Anim

        private float _facing;
        [SerializeField] private Animator _animator;

        //RayCast

        [SerializeField] private Transform _originRay;
        [SerializeField] private float _rayOffsetX;
        [SerializeField] private float _rayOffsetY;
        [SerializeField] private float _distanceOfCast;
        [SerializeField] private GameObject _previousTarget;
        [SerializeField] private GameObject _target;
        [SerializeField] private GameObject _lastWaterUsed;

        //Actions

        [SerializeField] private bool _doAction;
        [SerializeField] private GameObject _selectAnchor;
        [SerializeField] private GameObject _statePlayerIndic;
        [SerializeField] private SpriteRenderer _statePlayerSprite;
        [SerializeField] public SpriteRenderer _statePlayerBackground;
        [SerializeField] private List<Sprite> _stateSprites;


        //Resources

        [SerializeField] private int _wood;
        [SerializeField] private int _carot;
        [SerializeField] public bool _haveWater;

        [SerializeField] private TextMeshProUGUI _woodVisual;
        [SerializeField] private TextMeshProUGUI _carotVisual;
        [SerializeField] private TextMeshProUGUI _waterVisual;

        //Audio
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private List<AudioClip> _audioClips;

        #endregion

        #region Methods

        private void Start()
        {
            _canMove = true;
            speed = _walkSpeed;
            _facing = 1;
            _audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            #region Movement

            //Running
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _isRunning = true;
                speed = _runSpeed;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _isRunning = false;
                speed = _walkSpeed;
            }

            //Input Movement
            if (_canMove)
            {
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");

                movementDirection = new Vector2(horizontalInput, verticalInput).normalized;

                transform.position += (Vector3)movementDirection * speed * Time.deltaTime;
            }

            #endregion

            #region Animations

            //Facing
            if (horizontalInput < 0)
            {
                _facing = -1;
                transform.localScale = new Vector3(-1, 1, 1);
            }

            if (horizontalInput > 0)
            {
                _facing = 1;
                transform.localScale = new Vector3(1, 1, 1);
            }

            //Movement
            if (movementDirection != Vector2.zero)
            {
                if (_isRunning)
                {
                    _animator.SetBool("IsRunning", true);
                    _animator.SetBool("IsWalking", false);
                }
                else
                {
                    _animator.SetBool("IsWalking", true);
                    _animator.SetBool("IsRunning", false);
                }
            }
            else
            {
                _animator.SetBool("IsWalking", false);
                _animator.SetBool("IsRunning", false);
            }

            //Action

            // Little arrow follow player above him
            _statePlayerIndic.transform.position =
                new Vector3(transform.position.x, transform.position.y + 0.1f, 0);

            // Do an action ( _canPlay --> tuto finished )
            if (Input.GetKeyDown(KeyCode.T) && _target.GetComponent<Hittable>().CanBeUse)
            {
                _doAction = true;
                movementDirection = Vector2.zero;
                _canMove = false;
                _target.GetComponent<Hittable>().CanBeUse = false;

                //Lumbering
                if (_target.GetComponent<Hittable>().type == Hittable.Type.Tree && _target.GetComponent<Hittable>().life > 0)
                {
                    _wood++;
                    _animator.SetBool("IsLumbering", true);
                }

                //Attack
                if (_target.GetComponent<Hittable>().type == Hittable.Type.Skeleton)
                {
                    _target.GetComponent<Hittable>().life--;
                    _animator.SetBool("IsAttacking", true);
                }

                //Recovering Water
                if (_target.GetComponent<Hittable>().type == Hittable.Type.Water && !_haveWater)
                {
                    _audioSource.clip = _audioClips[3];
                    _audioSource.Play();
                    _haveWater = true;
                    _lastWaterUsed = _target;
                }

                //Watering and Recovering Carrots
                if (_target.GetComponent<Hittable>().type == Hittable.Type.CarrotHole)
                {
                    if (!_target.GetComponent<Hittable>()._haveCarrot)
                    {
                        if (_haveWater)
                        {
                            _animator.SetBool("IsWatering", true);
                            _audioSource.clip = _audioClips[2];
                            _audioSource.Play();
                            _haveWater = false;
                            _lastWaterUsed.GetComponent<Hittable>().life = 1;
                            _target.GetComponent<Hittable>().CanBeUse = false;
                        }
                        else
                            _target.GetComponent<Hittable>().CanBeUse = true;
                    }
                    else
                    {
                        _audioSource.clip = _audioClips[4];
                        _audioSource.Play();
                        _carot++;
                        _target.GetComponent<Hittable>()._haveCarrot = false;
                    }
                }

                StartCoroutine(DoAction());
                //Visual

                //UI
                _woodVisual.SetText(_wood.ToString());
                _carotVisual.SetText(_carot.ToString());
                _waterVisual.SetText(_haveWater ? "Charge" : "Empty");
            }

            #endregion

            #region Raycast

            //Raycast Creation
            RaycastHit2D hit = Physics2D.Raycast(
                new Vector2(_originRay.position.x + _rayOffsetX * _facing, _originRay.position.y + _rayOffsetY),
                new Vector3(_facing, 0, 0), _distanceOfCast);


            //Raycast Detection
            if (hit.collider.GetComponent<Hittable>() != null)
            {
                _target = hit.collider.gameObject;

                if (hit.collider.GetComponent<Hittable>().type == Hittable.Type.EndRay)
                {
                    _statePlayerBackground.transform.parent.gameObject.SetActive(false);
                    _selectAnchor.SetActive(false);
                    _previousTarget.GetComponent<Hittable>().IsSelected = false;
                    _statePlayerSprite.sprite = null;
                }
                else
                {
                    _statePlayerBackground.transform.parent.gameObject.SetActive(true);
                    _selectAnchor.SetActive(true);
                    _selectAnchor.transform.position = _target.GetComponent<Hittable>().Center.position;
                    _previousTarget.GetComponent<Hittable>().IsSelected = false;
                    _target.GetComponent<Hittable>().IsSelected = true;
                    _previousTarget = _target;

                    if (!_doAction)
                    {
                        if (hit.collider.GetComponent<Hittable>().life > 0)
                        {
                            if (hit.collider.GetComponent<Hittable>().type == Hittable.Type.Tree)
                            {
                                _statePlayerBackground.color = new Color(0.109f, 0.529f, 0);
                                _statePlayerSprite.sprite = _stateSprites[1];
                            }
                            else if (hit.collider.GetComponent<Hittable>().type == Hittable.Type.Skeleton)
                            {
                                _statePlayerSprite.sprite = _stateSprites[2];
                                _statePlayerBackground.color = Color.red;
                            }
                            else if (hit.collider.GetComponent<Hittable>().type == Hittable.Type.CarrotHole)
                            {
                                _statePlayerBackground.color = new Color(0.109f, 0.529f, 0);
                                if (hit.collider.GetComponent<Hittable>()._haveCarrot)
                                {
                                    _statePlayerBackground.color = new Color(0.109f, 0.529f, 0);
                                    _statePlayerSprite.sprite = _stateSprites[4];
                                }
                                else
                                {
                                    if (!_haveWater)
                                        _statePlayerBackground.transform.parent.gameObject.SetActive(false);
                                    else
                                    {
                                        _statePlayerBackground.color = new Color(0.109f, 0.529f, 0);
                                        _statePlayerSprite.sprite = _stateSprites[3];
                                    }
                                }
                            }
                            else if (hit.collider.GetComponent<Hittable>().type == Hittable.Type.Water && !_haveWater)
                            {
                                _statePlayerBackground.color = new Color(0.109f, 0.529f, 0);
                                _statePlayerSprite.sprite = _stateSprites[5];
                            }
                        }
                        else
                        {
                            _statePlayerBackground.transform.parent.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                _statePlayerBackground.transform.parent.gameObject.SetActive(false);
            }
        }

        #endregion
        
        //Coroutine for all actions
        IEnumerator DoAction()
        {
            _statePlayerSprite.sprite = _stateSprites[0];
            yield return new WaitForSeconds(0.5f);
            switch (_target.GetComponent<Hittable>().type)
            {
                case Hittable.Type.Tree:
                    _target.GetComponent<Hittable>().life--;
                    _animator.SetBool("IsLumbering", false);
                    _audioSource.clip = _audioClips[0];
                    _audioSource.Play();
                    yield return new WaitForSeconds(0.7f);
                    _target.GetComponent<Hittable>().CanBeUse = true;
                    break;

                case Hittable.Type.Skeleton:
                    _target.GetComponent<Animator>()
                        .SetBool(_target.GetComponent<Hittable>().life > 0 ? "IsHurt" : "Dying", true);

                    yield return new WaitForSeconds(0.05f);
                    _animator.SetBool("IsAttacking", false);
                    _audioSource.clip = _audioClips[1];
                    _audioSource.Play();
                    _target.GetComponent<Animator>().SetBool("IsHurt", false);
                    _target.GetComponent<Animator>().SetBool("Dying", false);

                    if (_target.GetComponent<Hittable>().life <= 0)
                    {
                        _target.GetComponent<Animator>().SetBool("IsDead", true);
                        _target.GetComponent<Hittable>().SkeletonDead();
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.7f);
                        _target.GetComponent<Hittable>().CanBeUse = true;
                    }

                    break;

                case Hittable.Type.Water:
                    _target.GetComponent<Hittable>().life = 0;
                    break;

                case Hittable.Type.CarrotHole:
                    if (_animator.GetBool("IsWatering"))
                    {
                        _animator.SetBool("IsWatering", false);
                        yield return new WaitForSeconds(0.5f);
                        _target.GetComponent<Hittable>()._haveCarrot = true;
                    }

                    break;
            }

            _canMove = true;
            _doAction = false;
            _statePlayerSprite.sprite = null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(new Vector2(_originRay.position.x + _rayOffsetX * _facing, _originRay.position.y + _rayOffsetY),
                new Vector3(_facing, 0, 0));
        }

        #endregion
    }
}