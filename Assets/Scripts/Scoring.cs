using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class Scoring : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI rankDisplay;
    public TextMeshProUGUI comboDisplay;
    public GameObject endUI;
    public GameObject restartUI;
    public GameObject floatingRankPrefab;
    private float frpOffset = 0.3f;

    public GameObject partSysPrefab;
    private float partSysHeightOffset = 0.1f; 

    private int score;
    private int combo = 0;
    private int maxCombo = 0;

    private float endHeight;

    public struct Score
    {
        public string name;
        public int range;
        public int points;
        public Color color;
        public int count;

        public Score(string n, int r, int p, Color c, int co)
        {
            name = n;
            range = r;
            points = p;
            color = c;
            count = co;
        }
    }

    Score Perfect = new Score("PERFECT", 30, 1000, Color.orange, 0);
    Score Great = new Score("GREAT", 70, 700, Color.cyan, 0);
    Score Good = new Score("GOOD", 140, 500, Color.green, 0);
    Score Ok = new Score("OK", 190, 240, Color.yellow, 0);
    Score Bad = new Score("BAD", 250, 100, Color.red, 0);
    Score Miss = new Score("MISS", 250, 0, Color.gray, 0);

    public void inputScore(double timeDifference, int lane)
    {
        Score scoreToAdd = Miss;

        // if the note is in range of being hit (not too high)
        // when a note gets too low, it will delete automatically, so we don't count that as an actual miss
        if (timeDifference < (double)Perfect.range / 1000)
        {
            scoreToAdd = Perfect;
            Perfect.count++;
            combo++;

        }
        else if (timeDifference < (double)Great.range / 1000)
        {
            scoreToAdd = Great;
            Great.count++;
            combo++;

        }
        else if (timeDifference < (double)Good.range / 1000)
        {
            scoreToAdd = Good;
            Good.count++;
            combo++;

        }
        else if (timeDifference < (double)Ok.range / 1000)
        {
            scoreToAdd = Ok;
            Ok.count++;
            combo++;

        }
        else if (timeDifference < (double)Bad.range / 1000)
        {
            scoreToAdd = Bad;
            Bad.count++;
            combo = 0;
        }

        if (combo > maxCombo)
        {
            maxCombo = combo;
        }

        score += scoreToAdd.points;
        scoreDisplay.text = score.ToString();
        rankDisplay.text = scoreToAdd.name;
        comboDisplay.text = "Combo: " + combo.ToString();
        spawnRank(scoreToAdd, lane);

        if (scoreToAdd.points != Miss.points)
        {
            spawnParticle(scoreToAdd, lane);
        }
    }

    public void inputScore(Score scoreToAdd, int lane)
    {
        if (scoreToAdd.name == "BAD")
        {
            combo = 0;
        }
        else if (scoreToAdd.name == "MISS")
        {
            combo = 0;
            Miss.count++;
        }

        score += scoreToAdd.points;
        scoreDisplay.text = score.ToString();
        rankDisplay.text = scoreToAdd.name;
        comboDisplay.text = "Combo: " + combo.ToString();
        spawnRank(scoreToAdd, lane);
    }

    public void spawnRank(Score score, int lane)
    {
        var frp = GameManager.Instance.spawnAtLocation(floatingRankPrefab, new Vector3(lane - 4, -1, 0));

        frp.GetComponent<FloatAndDisappear>().text = score.name;
        frp.GetComponent<FloatAndDisappear>().color = score.color;
    }

    public void spawnParticle(Score score, int lane)
    {
        var partSys = GameManager.Instance.spawnAtLocation(partSysPrefab, new Vector3(lane - 4, -1, 0));

        var mainMod = partSys.GetComponent<ParticleSystem>().main;

        mainMod.startColor = score.color;
    }
    public int getMissRange() { return Miss.range; }

    public Score getMiss() { return Miss; }

    public int getScore() { return score; }

    public void setEndHeight(float height) { endHeight = height; }

    public void endGame()
    {
        Debug.Log("ending");
        StartCoroutine(endGameProcedure());
    }
    private IEnumerator endGameProcedure()
    {
        scoreDisplay.gameObject.SetActive(false);
        rankDisplay.gameObject.SetActive(false);
        comboDisplay.gameObject.SetActive(false);
        endUI.SetActive(true);
        WaitForSeconds betweenScore = new WaitForSeconds(0.2f);

        yield return new WaitForSeconds(1);

        // lol
        TextMeshProUGUI rankName = endUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI rankNum = endUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        // extra lol
        rankName.text += Perfect.name + "\n";
        rankNum.text += Perfect.count + "\n";
        yield return betweenScore;

        rankName.text += Great.name + "\n";
        rankNum.text += Great.count + "\n";
        yield return betweenScore;

        rankName.text += Good.name + "\n";
        rankNum.text += Good.count + "\n";
        yield return betweenScore;

        rankName.text += Ok.name + "\n";
        rankNum.text += Ok.count + "\n";
        yield return betweenScore;

        rankName.text += Bad.name + "\n";
        rankNum.text += Bad.count + "\n";
        yield return betweenScore;

        rankName.text += Miss.name + "\n";
        rankNum.text += Miss.count + "\n";
        yield return betweenScore;

        rankName.text += "\nMax Combo" + "\n";
        rankNum.text += "\n" + maxCombo + "\n";
        yield return betweenScore;

        rankName.text += "Score" + "\n";
        rankNum.text += score + "\n";

        yield return new WaitForSeconds(1);
        restartUI.SetActive(true);
    }


    // will implement if we need a high score function, but as of now it's useless
    public void resetScore()
    {
        score = 0;
        combo = 0;
        maxCombo = 0;
    }
}
