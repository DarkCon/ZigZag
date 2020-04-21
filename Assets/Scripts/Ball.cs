using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour {
    [SerializeField] private float _fallHeight = 5f;
    
    public float GetSize() {
        return transform.localScale.z;
    }

    public void AnimateFall(Vector3 moveVector, float speed) {
        StartCoroutine(FallingAnimation(moveVector, speed));
    }

    private IEnumerator FallingAnimation(Vector3 moveVector, float speed) {
        var startPos = transform.position;
        var fallSpeed = speed * moveVector;
        
        do {
            fallSpeed += Time.deltaTime * Physics.gravity;
            transform.position += Time.deltaTime * fallSpeed;

            yield return null;
        } while (Vector3.Project(startPos - transform.position, Physics.gravity).magnitude < _fallHeight);
    }
}