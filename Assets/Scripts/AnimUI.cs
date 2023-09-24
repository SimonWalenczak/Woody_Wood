using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimUI : MonoBehaviour
{
    public enum Type
    {
        indicPlayer,
        indicSelection,
        indicStressSkeleton
    }

    public Type type;

    [SerializeField] private Transform _characterTransform;

    private void Update()
    {
        switch (type)
        {
            case Type.indicPlayer:
                Vector3 pos1 = new Vector3(transform.position.x, _characterTransform.position.y + 1, 0);
                Vector3 pos2 = new Vector3(transform.position.x, _characterTransform.position.y + 0.85f, 0);
                transform.position = Vector3.Lerp(pos1, pos2, Mathf.PingPong(Time.time * 2, 1.0f));
                break;

            case Type.indicSelection:
                Vector2 scale1 = new Vector2(4, 4);
                Vector2 scale2 = new Vector2(6, 6);
                transform.localScale = Vector3.Lerp(scale1, scale2, Mathf.PingPong(Time.time * 2, 1.0f));
                break;
            
            case Type.indicStressSkeleton:
                Vector2 scale3 = new Vector2(4, 4);
                Vector2 scale4 = new Vector2(6, 6);
                transform.localScale = Vector3.Lerp(scale3, scale4, Mathf.PingPong(Time.time * 5, 1.0f));
                break;
        }
    }
}