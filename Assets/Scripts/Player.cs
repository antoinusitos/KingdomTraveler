using UnityEngine;

public class Player : MonoBehaviour
{
    private Transform myTransform { get; set; } = null;

    public SpriteRenderer _mySpriteRenderer = null;

    private SpriteRenderer mySpriteRenderer { get { return _mySpriteRenderer; } }

    public Sprite[] mySprites = null;

    private void Start()
    {
        myTransform = transform;
    }

    public void CalculateOrientation(Vector3 target)
    {
        Vector3 worldPos = target;
        worldPos.z = myTransform.position.z;
        Vector3 dir = worldPos - myTransform.position;

        float dot = Vector3.Dot(myTransform.up, dir.normalized);

        if (dot >= 0.75f)
        {
            mySpriteRenderer.sprite = mySprites[0];
        }
        else if (dot < 0.75f && dot >= 0.25f)
        {
            if (worldPos.x > myTransform.position.x)
                mySpriteRenderer.sprite = mySprites[1];
            else
                mySpriteRenderer.sprite = mySprites[7];
        }
        else if (dot < 0.25f && dot >= -0.25f)
        {
            if (worldPos.x > myTransform.position.x)
                mySpriteRenderer.sprite = mySprites[2];
            else
                mySpriteRenderer.sprite = mySprites[6];
        }
        else if (dot < -0.25f && dot >= -0.75f)
        {
            if (worldPos.x > myTransform.position.x)
                mySpriteRenderer.sprite = mySprites[3];
            else
                mySpriteRenderer.sprite = mySprites[5];
        }
        else
        {
            mySpriteRenderer.sprite = mySprites[4];
        }
    }
}
