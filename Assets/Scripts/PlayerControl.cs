using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)] float moveDuration = 0.2f;
    [SerializeField, Range(0.01f, 1f)] float jumpHeight = 0.5f;

    int leftBoundary;
    int rightBoundary;
    int backBoundary;

    [SerializeField] TMP_Text stepText;
    [SerializeField] int maxTravel;
    public int MaxTravel { get => maxTravel; }
    [SerializeField] int currentTravel;
    public int CurrentTravel { get => currentTravel; }

    float eagleSpawnCounter = 3f;
    public GameObject eagle;
    public Transform eagleSpawnPoint;

    public GameManager gameManager;

    public AudioClip jumpClip;
    public AudioClip gameOverClip;
    public GameObject sfxer;
    bool die = false;

    private void Update()
    {
        eagleSpawnCounter -= Time.deltaTime;
        if(eagleSpawnCounter <= 0f)
        {
            Instantiate(eagle, eagleSpawnPoint.position, eagleSpawnPoint.rotation);
            eagleSpawnCounter = 1000f;
        }

        var direction = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction += new Vector3(0,0, 1f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction += new Vector3(0, 0, -1f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction += new Vector3(-1f,0,0f);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction += new Vector3(1f, 0, 0f);
        }

        if (direction != Vector3.zero && !IsJumping())
            Jump(direction);
    }

    public void Setup(int minZPos, int extent)
    {
        backBoundary = minZPos - 1;
        leftBoundary = -(extent + 1);
        rightBoundary = extent + 1;
    }

    private void Jump(Vector3 targetDirection)
    {
        var TargetPosition = transform.position + targetDirection;
        transform.LookAt(TargetPosition);
        var moveSeq = DOTween.Sequence(transform);
        moveSeq.Append(transform.DOMoveY(jumpHeight, moveDuration / 2)); 
        moveSeq.Append(transform.DOMoveY(0f, moveDuration / 2));

        if (TargetPosition.x < backBoundary || TargetPosition.x < leftBoundary || TargetPosition.x > rightBoundary)
            return;

        if (Tree.AllPositions.Contains(TargetPosition))
            return;

        AudioSource sfxSource = Instantiate(sfxer, transform.position, transform.rotation).GetComponent<AudioSource>();
        sfxSource.clip = jumpClip;
        sfxSource.Play();

        eagleSpawnCounter = 3f;
        transform.DOMoveX(TargetPosition.x, moveDuration); 
        transform.DOMoveZ(TargetPosition.z, moveDuration)
            .OnComplete(UpdateTravel);
    } 

    void UpdateTravel()
    {
        currentTravel = (int)this.transform.position.z;

        if(currentTravel > maxTravel)
        {
            maxTravel = currentTravel;
        }

        stepText.text = "STEP: " + maxTravel.ToString();
    }
    
    private bool IsJumping()
    {
        return DOTween.IsTweening(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            gameManager.GameOver();
            if(!die)
                AnimateDie();
        }
    }

    public void AnimateDie()
    {
        AudioSource sfxSource = Instantiate(sfxer, transform.position, transform.rotation).GetComponent<AudioSource>();
        sfxSource.clip = gameOverClip;
        sfxSource.Play();

        transform.DOScaleY(0.1f, 0.2f);
        transform.DOScaleX(1.2f,0.2f);
        transform.DOScaleZ(1.1f, 0.2f);
        die = true;
        this.enabled = false;
    }
    
}
