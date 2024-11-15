using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = Character.CharacterController;

public class Hittable : MonoBehaviour
{
    public enum Type
    {
        EndRay,
        Tree,
        Skeleton,
        CarrotHole,
        Water
    }

    #region Init Variables

    [Header("For all objects")] 
    public Type type;
    public bool IsSelected; // For visual
    public bool CanBeUse; // For no spam actions
    public int life;
    public Transform Center; // The point that is surrounded by anchors


    [Header("For Skeletons")] 
    public CharacterController _CharacterController; // Used by Skeletons and Carrots
    public Sprite SpriteDead;
    public GameObject nervousSprite; // Stress sprite for skeletons

    [Header("For Trees")] public List<GameObject> SpritesGroups;

    [Header("For Carrots")] 
    public bool _haveCarrot;
    public List<Sprite> CarotSprites;

    #endregion

    private void Start()
    {
        CanBeUse = true;

        if (type == Type.Tree)
            life = 3;
        else if (type == Type.Skeleton)
            life = 2;
        else
            life = 1;
    }

    private void Update()
    {
        switch (type)
        {
            case Type.EndRay:
                break;
            
            case Type.Tree:
                switch (life)
                {
                    case 2:
                        SpritesGroups[0].SetActive(false);
                        break;
                    case 0:
                        SpritesGroups[1].SetActive(false);
                        CanBeUse = false;
                        break;
                }
                break;
            
            case Type.Skeleton:
                GetComponent<SpriteRenderer>().sortingOrder = _CharacterController.transform.position.y > transform.position.y ? 2 : 0;

                transform.localScale = _CharacterController.transform.position.x > transform.position.x ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

                if (IsSelected && life > 0)
                    nervousSprite.SetActive(true);
                else
                    nervousSprite.SetActive(false);
                break;
            
            case Type.CarrotHole:
                GetComponent<SpriteRenderer>().sprite = _haveCarrot ? CarotSprites[1] : CarotSprites[0];
                if (!_CharacterController._haveWater && !_haveCarrot)
                    CanBeUse = false;
                else
                    CanBeUse = true;
                break;
            
            case Type.Water:
                if (type == Type.Water)
                {
                    if (life <= 0)
                        _CharacterController._statePlayerBackground.transform.parent.gameObject.SetActive(false);
                    else
                    {
                        _CharacterController._statePlayerBackground.transform.parent.gameObject.SetActive(true);
                        CanBeUse = true;
                    }
                }
                break;
        }
    }

    public void SkeletonDead()
    {
        CanBeUse = false;
        GetComponent<SpriteRenderer>().sprite = SpriteDead;
        StartCoroutine(SkeletonRespawn());
    }

    IEnumerator SkeletonRespawn()
    {
        yield return new WaitForSeconds(5);
        life = 2;
        CanBeUse = true;
        GetComponent<Animator>().SetBool("IsDead", false);
    }
}