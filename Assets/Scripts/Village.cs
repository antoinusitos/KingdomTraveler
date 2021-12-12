using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Village : MonoBehaviour
{
    public Player player = null;
    public float speed = 2.0f;

    private Coroutine goTo = null;

    public NavPath navPath = null;

    private int currentMaxDangerosity = 0;

    private bool conflict = false;

    public GameObject conflictPanel = null;
    public Text conflictText = null;
    public GameObject buttonsParent = null;

    public GameObject diceParent = null;
    public Text diceText = null;
    public Text resultText = null;

    public Text turnText = null;

    private Enemy currentEnemy = null;
    private bool enemyTurn = false;
    private bool attack = false;
    private bool escape = false;

    public GameObject panelEnemy = null;

    public Slider enemyLifeSlider = null;
    public Text enemyLifeSliderText = null;

    public Slider playerLifeSlider = null;
    public Text playerLifeSliderText = null;

    private Transform localCamera = null;
    private Vector3 movePos = Vector3.zero;
    public float cameraMoveSpeed = 5;

    private const float borderSize = 40;

    public GameObject revealer = null;
    public Location[] startLocation = null;

    private void Start()
    {
        localCamera = transform;
    }

    private void Update()
    {
        if (GameManager.instance.gameWaiting)
            return;

        if(Input.GetMouseButtonDown(0) && !GameManager.instance.isInMenu && !GameManager.instance.isMoving)
        {
            if (Input.mousePosition.x < borderSize || Input.mousePosition.x > Screen.width - borderSize ||
               Input.mousePosition.y < borderSize || Input.mousePosition.y > Screen.height - borderSize)
                return;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Location location = hit.transform.GetComponent<Location>();
                if (location != null && location.isReachable)
                {
                    if(navPath.FindPathTo(location.point))
                    {
                        List<Node> path = navPath.path;
                        localCamera.localPosition = Vector3.forward * -10;
                        if (goTo != null)
                        {
                            StopCoroutine(goTo);
                        }
                        currentMaxDangerosity = 0;
                        UIManager.instance.BlockTown(true);
                        goTo = StartCoroutine(GoTo(path));
                    }
                }
            }
        }
        else if(Input.GetMouseButtonDown(1) && !GameManager.instance.isInMenu && !GameManager.instance.isMoving)
        {
            movePos = Input.mousePosition;
        }
        else if(Input.GetMouseButton(1) && !GameManager.instance.isInMenu && !GameManager.instance.isMoving)
        {
            localCamera.localPosition += (movePos - Input.mousePosition) * Time.deltaTime * cameraMoveSpeed;
            movePos = Input.mousePosition;
        }
        else if(Input.mouseScrollDelta.y != 0 && !GameManager.instance.isInMenu)
        {
            if(Input.mouseScrollDelta.y > 0 && Camera.main.orthographicSize > 10)
            {
                Camera.main.orthographicSize--;
            }
            else if (Input.mouseScrollDelta.y < 0 && Camera.main.orthographicSize < 40)
            {
                Camera.main.orthographicSize++;
            }
        }
    }

    public void StartGame()
    {
        StartCoroutine(ShowStartLocations());
    }

    private IEnumerator ShowStartLocations()
    {
        Node startNode = navPath.currentNode;
        for (int j = 0; j < startLocation.Length; j++)
        {
            revealer.transform.position = player.transform.position;
            navPath.currentNode = startNode;
            if (navPath.FindPathTo(startLocation[j].point))
            {
                List<Node> path = navPath.path;
                for (int i = 0; i < path.Count; i++)
                {
                    Vector3 end = path[i].transform.position;
                    end.z = revealer.transform.position.z;
                    float dist = Vector3.Distance(revealer.transform.position, end);

                    while (dist > 0.1f)
                    {
                        revealer.transform.position = Vector3.MoveTowards(revealer.transform.position, end, Time.deltaTime * speed * 10);
                        dist = Vector3.Distance(revealer.transform.position, end);
                        yield return null;
                    }
                }
            }
        }
        revealer.SetActive(false);
        navPath.currentNode = startNode;
        GameManager.instance.gameWaiting = false;
    }

    private IEnumerator GoTo(List<Node> pos)
    {
        GameManager.instance.isMoving = true;
        GameManager.instance.isInTown = false;
        for (int i = 0; i < pos.Count; i++)
        {
            Vector3 end = pos[i].transform.position;
            end.z = player.transform.position.z;
            player.CalculateOrientation(end);
            float dist = Vector3.Distance(player.transform.position, end);

            if (pos[i].dangerosity > currentMaxDangerosity)
            {
                if(true)//Random.Range(0, 10) <= 1 - pos[i].dangerosity)
                {
                    currentMaxDangerosity = pos[i].dangerosity;
                    Conflict();
                }
            }

            while(conflict)
            {
                
                yield return null;
            }

            while (dist > 0.1f)
            {
                GameManager.instance.SetCurrentLinePos(player.transform.position);
                player.transform.position = Vector3.MoveTowards(player.transform.position, end, Time.deltaTime * speed * InventoryManager.instance.horseSpeed);
                dist = Vector3.Distance(player.transform.position, end);
                yield return null;
            }
            if(i > 0)
                GameManager.instance.RemoveOneLine();
        }
        UIManager.instance.BlockTown(false);
        GameManager.instance.isInTown = true;
        GameManager.instance.isMoving = false;
    }

    private void Conflict()
    {
        conflict = true;
        conflictPanel.SetActive(true);
        conflictText.text = "A Bandit, hidden is a bush, is attacking you.";
        currentEnemy = new Enemy();
        panelEnemy.SetActive(true);
        enemyLifeSlider.value = currentEnemy.life / currentEnemy.maxLife;
        enemyLifeSliderText.text = "Life : " + currentEnemy.life.ToString("F2") + " / " + currentEnemy.maxLife;
        playerLifeSlider.value = InventoryManager.instance.life / 100.0f;
        playerLifeSliderText.text = "Life : " + InventoryManager.instance.life.ToString("F2") + " / 100";
        StartCoroutine(TurnByTurn());
    }

    public void Attack()
    {
        buttonsParent.SetActive(false);
        diceParent.SetActive(true);
        attack = true;
    }

    public void Escape()
    {
        buttonsParent.SetActive(false);
        diceParent.SetActive(true);
        escape = false;
    }

    private IEnumerator TurnByTurn()
    {
        while(conflict)
        {
            if(enemyTurn)
            {
                buttonsParent.SetActive(false);
                int rand = Random.Range(1, 13);
                diceParent.SetActive(true);
                diceText.text = rand.ToString();
                if(rand == 1)
                {
                    resultText.text = "Critical fail, change turn";
                }
                else if (rand == 12)
                {
                    resultText.text = "Critical Success, twice damage !";
                    float damage = (currentEnemy.damage * 2) - InventoryManager.instance.defense;
                    if (damage < 0) damage = 0;
                    InventoryManager.instance.life -= damage;
                    playerLifeSlider.value = InventoryManager.instance.life / 100.0f;
                    playerLifeSliderText.text = "Life : " + InventoryManager.instance.life.ToString("F2") + " / 100";
                }
                else
                {
                    rand -= 1; // put rand between 1 and 10
                    float randTemp = rand / 10.0f; // convert rand for percent
                    float damage = (currentEnemy.damage * randTemp) - InventoryManager.instance.defense;
                    if (damage < 0) damage = 0;
                    InventoryManager.instance.life -= damage;
                    resultText.text = "You take " + (damage).ToString("F2") + " damage";
                    playerLifeSlider.value = InventoryManager.instance.life / 100.0f;
                    playerLifeSliderText.text = "Life : " + InventoryManager.instance.life.ToString("F2") + " / 100";

                }
                yield return new WaitForSeconds(2);
                diceParent.SetActive(false);
                enemyTurn = false;
                buttonsParent.SetActive(true);
            }
            else
            {
                if(attack)
                {
                    attack = false;
                    int rand = Random.Range(1, 13);
                    diceText.text = rand.ToString();
                    if (rand == 1)
                    {
                        resultText.text = "Critical fail, change turn";
                        yield return new WaitForSeconds(2);
                    }
                    else if (rand == 12)
                    {
                        resultText.text = "Critical Success, twice damage";
                        currentEnemy.life -= InventoryManager.instance.damage * 2;
                        if (currentEnemy.life < 0) currentEnemy.life = 0;
                        enemyLifeSlider.value = currentEnemy.life / currentEnemy.maxLife;
                        enemyLifeSliderText.text = "Life : " + currentEnemy.life.ToString("F2") + " / " + currentEnemy.maxLife;
                        yield return new WaitForSeconds(2);
                    }
                    else
                    {
                        rand -= 1; // put rand between 1 and 10
                        float randTemp = rand / 10.0f; // convert rand for percent
                        float damage = InventoryManager.instance.damage * randTemp;
                        currentEnemy.life -= damage;
                        if (currentEnemy.life < 0) currentEnemy.life = 0;
                        enemyLifeSlider.value = currentEnemy.life / currentEnemy.maxLife;
                        enemyLifeSliderText.text = "Life : " + currentEnemy.life.ToString("F2") + " / " + currentEnemy.maxLife;
                        resultText.text = "Bandit take " + (damage).ToString("F2") + " damage";
                        yield return new WaitForSeconds(2);
                    }

                    if(currentEnemy.life <= 0)
                    {
                        diceParent.SetActive(false);
                        int beforeLevel = InventoryManager.instance.level;
                        InventoryManager.instance.AddXP(currentEnemy.xpGiven);
                        turnText.gameObject.SetActive(true);
                        turnText.text = "You earn " + currentEnemy.xpGiven + " XP";
                        yield return new WaitForSeconds(2);
                        if (InventoryManager.instance.level != beforeLevel)
                        {
                            turnText.text = "Level UP !";
                            yield return new WaitForSeconds(2);
                        }
                        StopConflict();
                    }
                    else
                    {
                        turnText.gameObject.SetActive(true);
                        turnText.text = "Enemy Turn";
                        diceParent.SetActive(false);
                        yield return new WaitForSeconds(2);
                        turnText.gameObject.SetActive(false);
                        enemyTurn = true;
                    }
                }
                else if(escape)
                {
                    diceParent.SetActive(true);
                    escape = false;
                    int rand = Random.Range(1, 7);
                    diceText.text = rand.ToString();
                    if (rand == 1)
                    {
                        resultText.text = "Critical fail, you cannot escape !";
                        InventoryManager.instance.life -= currentEnemy.damage;
                        playerLifeSlider.value = InventoryManager.instance.life / InventoryManager.instance.maxLife;
                        playerLifeSliderText.text = "Life : " + InventoryManager.instance.life.ToString("F2") + " / " + InventoryManager.instance.maxLife;
                        yield return new WaitForSeconds(1);
                        resultText.text = "You take a damage from the bandit.";
                        yield return new WaitForSeconds(1);
                        buttonsParent.SetActive(false);
                        diceParent.SetActive(false); 
                        yield return new WaitForSeconds(2);
                        enemyTurn = true;
                        diceParent.SetActive(false);
                    }
                    else
                    {
                        resultText.text = "You Succeed to escape !";
                        yield return new WaitForSeconds(1);
                        buttonsParent.SetActive(true);
                        diceParent.SetActive(false);
                        StopConflict();
                    }
                    enemyTurn = false;
                }
            }

            yield return null;
        }
    }

    public void StopConflict()
    {
        enemyTurn = false;
        buttonsParent.SetActive(true);
        turnText.gameObject.SetActive(false);
        conflict = false;
        conflictPanel.SetActive(false);
    }
}
